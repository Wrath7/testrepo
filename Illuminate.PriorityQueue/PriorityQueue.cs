using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Interfaces.IlluminateQueue;

namespace Illuminate.PriorityQueue
{
    /// <summary>
    /// Priority Queue stores IPriorityQueueEntity, and returns them in order of priority. 
    /// This queue is thread safe.
    /// </summary>
    /// <typeparam name="K">Key K used to uniquely identify a element in the queue</typeparam>
    public class PriorityQueue <K> :ICloneable
    {
        LinkedListEntity<IPriorityQueueEntity<K>> _first;
        LinkedListEntity<IPriorityQueueEntity<K>> _last;

        Dictionary<K, int> _entities;

        object _dictionaryLock = new object();
        object _queueHeadLock = new object();

        public int Count
        {
            get { return _entities.Count; }
        }

        /// <summary>
        /// Create a new Priority Queue.
        /// </summary>
        public PriorityQueue()
        {
            _first = null;
            _last = null;

            _entities = new Dictionary<K, int>();
        }

        /// <summary>
        /// Inserts a new entity into the queue.  Uses the entities Key K to determine if it has been already added.
        /// </summary>
        /// <param name="newEnt"></param>
        /// <returns>True if the entity is successfully added, False if the entity is already in the queue.</returns>
        public bool Insert(IPriorityQueueEntity<K> newEnt)
        {
            //Check if the entity to be added is already in the queue
            bool alreadyExists;
            lock (_dictionaryLock)
            {
                alreadyExists = _entities.ContainsKey(newEnt.Key);
                if (!alreadyExists)
                {
                    _entities.Add(newEnt.Key, 0);
                }
            }

            if (!alreadyExists)
            {
                //This is a new entity to the queue add it.
                InsertNewEntity(newEnt);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Insert the entity into the queue.  
        /// </summary>
        /// <param name="newEnt"></param>
        private void InsertNewEntity(IPriorityQueueEntity<K> newEnt)
        {
            LinkedListEntity<IPriorityQueueEntity<K>> newNode = new LinkedListEntity<IPriorityQueueEntity<K>>(newEnt);
                            
            bool insertCompleted = false;
            while (!insertCompleted)
            {
                LinkedListEntity<IPriorityQueueEntity<K>> lockedLast = _last;
                if (lockedLast != null)
                {
                    if (newEnt.CompareTo(lockedLast.Value) >= 0)
                    {
                        //New entity should appear after the last value. Attempt to add it.
                        lock (lockedLast)
                        {
                            if ((lockedLast != null) && (lockedLast == _last))
                            {
                                //We have the last Node locked. 
                                if (newEnt.CompareTo(_last.Value) >= 0)
                                {
                                    //the new entity should still appear after the last value.
                                    _last.Next = newNode;
                                    _last = newNode;
                                    insertCompleted = true;
                                }
                            }
                            else
                            {
                                //Loop again the state has changed while we tried to obtain the lock.
                            }
                        }

                    }
                    else
                    {
                        //New entity should appear before the last value.  Check the first.
                        LinkedListEntity<IPriorityQueueEntity<K>> grabbedFirst = _first;
                        if ((grabbedFirst != null) && (newEnt.CompareTo(grabbedFirst.Value) <= 0))
                        {
                            //New entity should appear before the first.  Attempt to add it.
                            insertCompleted = InsertInFirst(newEnt, newNode);
                        }
                        else
                        {
                            if (grabbedFirst != null)
                            {
                                //New entity should appear in between the middle node and last node.
                                LinkedListEntity<IPriorityQueueEntity<K>> node = grabbedFirst.Next;
                                LinkedListEntity<IPriorityQueueEntity<K>> previousNode = grabbedFirst;
                                insertCompleted = InsertInMiddle(newEnt, newNode, ref node, ref previousNode);
                            }
                            else
                            {
                                //Loop again the state has changed
                            }
                        }
                    }

                }
                else
                {
                    if (_first != null)
                    {
                        //Loop again the state has changed.
                    }
                    else
                    {
                        lock (_queueHeadLock)
                        {
                            //Locked the queue head verify that that other entities were not added while waiting for the lock.
                            if ((_first != null) || (_last != null))
                            {
                                //Loop again the state has changed
                            }
                            else
                            {
                                //Queue remains empty add the entity.
                                _first = newNode;
                                _last = newNode;
                                insertCompleted = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Entity should be added into the first possition attempt to do so.
        /// </summary>
        /// <param name="newEnt"></param>
        /// <param name="newNode"></param>
        /// <returns></returns>
        private bool InsertInFirst(IPriorityQueueEntity<K> newEnt, LinkedListEntity<IPriorityQueueEntity<K>> newNode)
        {
            bool insertCompleted = false;
            bool stillFirst = true;
            
            while ((!insertCompleted) && stillFirst)
            {
                //loop until the node is inserted or the queue changes and it is determined the entity should no longer appear at the front.   
                LinkedListEntity<IPriorityQueueEntity<K>> lockedFirst = _first;
                if (lockedFirst != null)
                {
                    //Lock the first node and attempt to add before it.
                    lock (lockedFirst)
                    {
                        if ((lockedFirst == _first) && (!_first.IsRemoved))
                        {
                            //The node we locked is the first node. Confirm entity to be added before the first node.
                            if (newEnt.CompareTo(_first.Value) <= 0)
                            {
                                //The node goes before the first node.  Add it.
                                newNode.Next = _first;
                                _first = newNode;
                                insertCompleted = true;
                            }
                            else
                            {
                                //We have locked the first node but a new entity has been inserted that this entity should appear after.
                                //Break the loop and exit the method.
                                stillFirst = false;
                            }
                        }
                        else
                        {
                            //The node we locked is no longer first. The queue state has changed. Loop again to attempt to lock it.
                        }
                    }
                }
                else
                {
                    stillFirst = false;
                }
            }
            return insertCompleted;
        }

        /// <summary>
        /// Entity should be added in the middle somewhere after previous node. find the location and attempt to add it.
        /// </summary>
        /// <param name="newEnt"></param>
        /// <param name="newNode"></param>
        /// <param name="node"></param>
        /// <param name="previousNode"></param>
        /// <returns></returns>
        private bool InsertInMiddle(IPriorityQueueEntity<K> newEnt, LinkedListEntity<IPriorityQueueEntity<K>> newNode, ref LinkedListEntity<IPriorityQueueEntity<K>> node, ref LinkedListEntity<IPriorityQueueEntity<K>> previousNode)
        {
            bool insertCompleted = false;
            //Loop until inserted.
            while (!insertCompleted)
            {
                while ((node != null) && (newEnt.CompareTo(node.Value) > 0))
                {
                    //Loop through the nodes until we find the location in the queue this node should be inserted into.
                    previousNode = node;
                    node = node.Next;
                }

                //place to insert found.  Lock the node to be modified.
                lock (previousNode)
                {
                    //Confirm that the new entity still belongs after the locked node.
                    if ((previousNode.Next != null) && (newEnt.CompareTo(previousNode.Next.Value) <= 0))
                    {
                        if (previousNode.IsRemoved)
                        {
                            //The previous node we were to add the new entity after is no longer in the queue. 
                            //The new entity should appear at the front of the queue.  Break the loop and exit method.
                            break;
                        }
                        else
                        {
                            //New entity belongs here add it.  
                            newNode.Next = previousNode.Next; //use previousNode.Next instead of node since PreviousNode.Next may have changed.
                            previousNode.Next = newNode;
                            insertCompleted = true;
                        }
                    }
                    else
                    {
                        //Queue state has changed a new entity has been added after this node that should appear before the new entity.
                        previousNode = previousNode.Next;
                        node = previousNode.Next;
                    }
                }
            }
            return insertCompleted;
        }

        /// <summary>
        /// Remove and item from the head of the queue.
        /// </summary>
        /// <returns></returns>
        public IPriorityQueueEntity<K> Pop()
        {
            if (_first == null)
            {
                //Queue is empty return nothing
                return null;
            }
            else
            {
                // loop attempting to lock the first node.
                while (true)
                {
                    IPriorityQueueEntity<K> ent = null;
                    bool poppedWorked = false;
                    LinkedListEntity<IPriorityQueueEntity<K>> lockedFirst = _first;
                    if (lockedFirst == null)
                    {
                        //Queue is empty return nothing.
                        return null;
                    }
                    else
                    {
                        lock (lockedFirst)
                        {
                            //Confirm node we have locked is still first. Try again if it is not.
                            if (lockedFirst == _first)
                            {
                                //We have locked the first node.
                                _first.IsRemoved = true; //Set the removed flag.
                                ent = _first.Value;
                                if (_first != _last)
                                {
                                    //There are more entities in the queue, move the first to point to the next node.
                                    _first = _first.Next;
                                }
                                else
                                {
                                    //We are removing the last entity in the queue.  Set the first and last pointers to indecate an empty queue.
                                    _first = null;
                                    _last = null;
                                }
                                poppedWorked = true;
                            }
                        }

                        if (poppedWorked)
                        {
                            //We have successfully obtained the first element in the queue remove it from the contains check entities dictionary.
                            lock (_dictionaryLock)
                            {
                                _entities.Remove(ent.Key);
                            }
                            return ent;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return the top entity from the queue but don't remove it.
        /// </summary>
        /// <returns></returns>
        public IPriorityQueueEntity<K> Peek()
        {
            IPriorityQueueEntity<K>[] peeks = Peek(1);
            if (peeks.Length == 0)
                return null;
            else
                return peeks[0];
        }

        /// <summary>
        /// Return the top i entities from the queue but don't remove them.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public IPriorityQueueEntity<K>[] Peek(int i)
        {
            List<IPriorityQueueEntity<K>> topI = new List<IPriorityQueueEntity<K>>(i);
            LinkedListEntity<IPriorityQueueEntity<K>> node = _first;

            int j=0;
            while (node != null && j < i)
            {
                topI.Add(node.Value);
                node = node.Next;
                j++;
            }

            return topI.ToArray();
        }

        /// <summary>
        /// Clear the Queue
        /// </summary>
        protected void Clear()
        {
            //obtain the dictionary lock
            lock (_dictionaryLock)
            {
                //obtain the queue head lock
                lock (_queueHeadLock)
                {
                    //loop until we have locked the first node in the queue.
                    bool lockFirstWorked = false;
                    while (!lockFirstWorked)
                    {

                        LinkedListEntity<IPriorityQueueEntity<K>> lockedFirst = _first;
                        if (lockedFirst != null)
                        {
                            //Queue has an element in it. lock the first element.
                            lock (lockedFirst)
                            {
                                if (lockedFirst == _first)
                                {
                                    //Locked the first queue element successfully
                                    lockFirstWorked = true;

                                    if (_last != _first)
                                    {
                                        //There is more than one element in the queue lock the last element.
                                        bool lockLastWorked = false;
                                        while (!lockLastWorked)
                                        {
                                            //Loop until the last element is successfully locked.
                                            LinkedListEntity<IPriorityQueueEntity<K>> lockedLast = _last;
                                            if (lockedLast != null)
                                            {
                                                lock (lockedLast)
                                                {
                                                    if (lockedLast == _last)
                                                    {
                                                        //We have successfully locked the last element. Clear the queues data.
                                                        lockLastWorked = true;
                                                        ClearData();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //There is only one element in the queue and we have already locked it.  Clear the queue data.
                                        ClearData();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (_last == null)
                            {
                                //The queue is empty. Clear data anyway.
                                lockFirstWorked = true;
                                ClearData();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Wipes the Data from the queue.  This method should only be called if all appropriate locks have been obtained 
        /// or the queue is running in a single thread.
        /// </summary>
        private void ClearData()
        {
            _entities.Clear();
            _first = null;
            _last = null;
        }


        #region ICloneable Members

        public object Clone()
        {
            PriorityQueue<K> clone = new PriorityQueue<K>();
            LinkedListEntity<IPriorityQueueEntity<K>> lockedFirst = _first;
            if (lockedFirst != null)
            {
                LinkedListEntity<IPriorityQueueEntity<K>> origNode;
                LinkedListEntity<IPriorityQueueEntity<K>> cloneNode;
                LinkedListEntity<IPriorityQueueEntity<K>> first;

                first = new LinkedListEntity<IPriorityQueueEntity<K>>((IPriorityQueueEntity<K>)lockedFirst.Value.Clone());

                origNode = lockedFirst;
                cloneNode = first;

                Dictionary<K, int> cloneDic = new Dictionary<K, int>();
                cloneDic.Add(cloneNode.Value.Key, 0);

                while (origNode.Next != null)
                {
                    origNode = origNode.Next;
                    if (!cloneDic.ContainsKey(origNode.Value.Key))
                    {
                        LinkedListEntity<IPriorityQueueEntity<K>> nextCloneNode = new LinkedListEntity<IPriorityQueueEntity<K>>((IPriorityQueueEntity<K>)origNode.Value.Clone());
                        cloneNode.Next = nextCloneNode;
						
						cloneNode = nextCloneNode;

                        cloneDic.Add(cloneNode.Value.Key, 0);
                        
                    }
                }

                clone._first = first;
                clone._last = cloneNode;
                clone._entities = cloneDic;
            }
            return clone;
        }

        #endregion
    }
}
