using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Interfaces.IlluminateQueue
{
    public interface IIlluminateQueue
        //: Illuminate.Interfaces.IIlluminateObject
    {


        DateTime NextSaveDate
        { get; }

        int Count
        { get;  }

        /// <summary>
        /// Insert a new Url Entity Into the queue.
        /// </summary>
        /// <param name="newEnt"></param>
        /// <returns></returns>
        bool Insert(IPriorityQueueEntity<string> newEnt);


        /// <summary>
        /// Pops a URL from the priority queue if it's date time is earlier than now.
        /// </summary>
        /// <returns></returns>
        IPriorityQueueEntity<string> Pop();
        

        /// <summary>
        /// Return the first Url on the queue but don't remove it.
        /// </summary>
        /// <returns></returns>
        IPriorityQueueEntity<string> Peek();

        /// <summary>
        /// Save the Url queue to the database
        /// </summary>
        void Save();

        void Load();

        void Clear();

        void Quit();
    }
}
