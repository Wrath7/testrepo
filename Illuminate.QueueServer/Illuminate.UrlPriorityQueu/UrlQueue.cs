using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.PriorityQueue;
using Gazaro;
using System.Threading;
using Illuminate.Interfaces.IlluminateQueue;
using System.Configuration;

namespace Illuminate.UrlPriorityQueue
{
    public class UrlQueue : IIlluminateQueue
    {
        protected Dictionary<int, Queue<UrlQueueEntity>> _siteQueues;
        protected List<Queue<UrlQueueEntity>> _roundRobinQueues;
        protected object _wholeQueueLock;
        protected object _roundRobinLock;
        protected DateTime _nextCrawlTime;
        protected int _roundRobinId;

        protected int _numberOfQueueServers;
        protected int _millisecondsPerCrawl;

        private Thread _saveThread = null;
        private bool _isSaving = false;
        private bool _quit = false;
        private DateTime _nextSaveDate = new DateTime();
        private Gazaro.Interfaces.IDataService _dataService;

        private string LOGNAME = "QUEUE_SERVER";

        public DateTime NextCrawlTime
        {
            get { return _nextCrawlTime; }
        }

        public DateTime NextSaveDate
        {
            get
            {
                return _nextSaveDate;
            }
        }

        public int Count
        {
            get
            {
                int sum = 0;
                foreach (Queue<UrlQueueEntity> siteQueue in _siteQueues.Values)
                {
                    sum += siteQueue.Count;
                }
                return sum;
            }
        }

        public string Status
        {
            get
            {
                string status = LOGNAME + ": Status\n";
                status += "Next Crawl Time: " + _nextCrawlTime + "\n";
                foreach (KeyValuePair<int, Queue<UrlQueueEntity>> site in _siteQueues)
                {
                    status += site.Key + "\t " + site.Value.Count + "\n";
                }
                return status;
            }
        }

        /// <summary>
        /// Constructor Create a new UrlPriorityQueue
        /// </summary>
        /// <param name="dataService"></param>
        public UrlQueue(Gazaro.Interfaces.IDataService dataService, string logName)
            : base()
        {
            _siteQueues = new Dictionary<int,Queue<UrlQueueEntity>>();
            _roundRobinQueues = new List<Queue<UrlQueueEntity>>();
            _roundRobinId = 0;
            _nextCrawlTime = DateTime.Now;

            //create the locks
            _wholeQueueLock = new object();
            _roundRobinLock = new object();

			_numberOfQueueServers = ConfigurationManager.AppSettings["QueueServer"].ToString().Split(',').Length;
			_millisecondsPerCrawl = int.Parse(ConfigurationManager.AppSettings["MillisecondsPerCrawl"].ToString());

            _dataService = dataService;

            _saveThread = new Thread(new ThreadStart(SaveEvent));
            _saveThread.Start();

			LOGNAME = logName;
        }

        /// <summary>
        /// Insert a new Url Entity Into the queue.
        /// </summary>
        /// <param name="newEnt"></param>
        /// <returns></returns>
        public bool Insert(IPriorityQueueEntity<string> newEnt)
        {
            return this.Insert((UrlQueueEntity)newEnt);
        }

        /// <summary>
        /// Insert a new UrlEntity Into the queue.
        /// </summary>
        /// <param name="newEnt"></param>
        /// <returns></returns>
        public bool Insert(UrlQueueEntity newEnt)
        {
            Queue<UrlQueueEntity> siteQueue;
            if (!_siteQueues.TryGetValue(newEnt.SiteId, out siteQueue))
            {
                lock (_wholeQueueLock)
                {
                    if (!_siteQueues.TryGetValue(newEnt.SiteId, out siteQueue))
                    {
                        Dictionary<int, Queue<UrlQueueEntity>> newSiteQueues = new Dictionary<int,Queue<UrlQueueEntity>>();
                        foreach (KeyValuePair<int, Queue<UrlQueueEntity>> kvp in _siteQueues)
                        {
                            newSiteQueues.Add(kvp.Key, kvp.Value);
                        }
                        siteQueue = new Queue<UrlQueueEntity>();
                        newSiteQueues.Add(newEnt.SiteId, siteQueue);

                        _roundRobinQueues.Add(siteQueue);
                        _siteQueues = newSiteQueues;
                    }
                }

            }
            lock (siteQueue)
            {
                siteQueue.Enqueue(newEnt);
            }
            return true;
        }

        /// <summary>
        /// Pops a URL from the priority queue if it's date time is earlier than now.
        /// </summary>
        /// <returns></returns>
        public IPriorityQueueEntity<string> Pop()
        {
            if (_nextCrawlTime <= System.DateTime.Now)
            {
                int roundRobinId = GetNextRoundRobinId();
                if (roundRobinId >= 0)
                {
                    Queue<UrlQueueEntity> siteQueue = _roundRobinQueues[roundRobinId];
                    lock (siteQueue)
                    {
                        UrlQueueEntity toReturn = siteQueue.Dequeue();
                        return toReturn;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        protected int GetNextRoundRobinId()
        {
            int queueIdToReturn = -1;
            lock (_roundRobinLock)
            {
                int stop = _roundRobinQueues.Count;

                while ((_nextCrawlTime <= DateTime.Now) && (queueIdToReturn == -1) && (stop > 0))
                {
                    queueIdToReturn = _roundRobinId;
                    _roundRobinId = (_roundRobinId + 1) % _roundRobinQueues.Count;
                    if (_roundRobinId == 0)
                    {
                        _nextCrawlTime = _nextCrawlTime.AddMilliseconds(_numberOfQueueServers*_millisecondsPerCrawl);
                    }
                    if (_roundRobinQueues[queueIdToReturn].Count == 0)
                    {
                        queueIdToReturn = -1;
                        stop--;
                    }
                }
            }
            return queueIdToReturn;
        }

        /// <summary>
        /// Return the first Url on the queue but don't remove it.
        /// </summary>
        /// <returns></returns>
        public IPriorityQueueEntity<string> Peek()
        {
            if (_roundRobinQueues.Count > 0)
            {
                if (_roundRobinQueues[_roundRobinId].Count > 0)
                {
                    IPriorityQueueEntity<string> toReturn = null;
                    try
                    {
                        toReturn = _roundRobinQueues[_roundRobinId].Peek();
                    }
                    catch
                    {
                        //Statechanged while attempting to peek the queue.  Causing an error return null
                    }
                    return toReturn;
                }
                else
                {
                    if (this.Count > 0)
                    {
                        int stopPoint = _roundRobinId;
                        int point = (_roundRobinId + 1) % _roundRobinQueues.Count;

                        while ((point != stopPoint))
                        {
                            if (_roundRobinQueues[point].Count > 0)
                            {
                                IPriorityQueueEntity<string> toReturn = null;
                                try
                                {
                                    toReturn = _roundRobinQueues[point].Peek();
                                }
                                catch
                                {
                                    //Statechanged while attempting to peek the queue.  Causing an error return null
                                }
                                return toReturn;
                            }
                            else
                            {
                                point = (point + 1) % _roundRobinQueues.Count;
                            }
                        }

                        //State has changed while attempting to peek and queues are empty or close to it return null
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return the first i Urls on the queue but don't remove them.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public new UrlQueueEntity[] Peek(int i)
        {
            UrlQueueEntity[] toReturn;
            if (this.Count > i)
            {
                toReturn = new UrlQueueEntity[i];
            }
            else
            {
                if (this.Count > 0)
                {
                    toReturn = new UrlQueueEntity[this.Count];
                }
                else
                {
                    return new UrlQueueEntity[0];
                }
            }
            
            int rrPoint = _roundRobinId;
            int rrStop = rrPoint;
            bool first = true;

            int j = 0;
            while ((first || (rrPoint != rrStop)) && j < toReturn.Length)
            {
                first = false;
                if (_roundRobinQueues[rrPoint].Count > 0)
                {
                    try
                    {
                        toReturn[j] = _roundRobinQueues[rrPoint].Peek();
                        j++;
                    }
                    catch
                    {
                        //State changed between when we checked and when we peeked.
                    }
                }
                rrPoint = (rrPoint + 1) % _roundRobinQueues.Count;
            }

            while (j < toReturn.Length)
            {
                UrlQueueEntity uqe = new UrlQueueEntity("Value Hidden In queue", new DateTime(), 1);
                toReturn[j] = uqe;
            }
            return toReturn;
        }

        /// <summary>
        /// Save the Url queue to the database
        /// </summary>
        public void Save()
        {
            throw new NotImplementedException();
        }

        private void SaveEvent()
        {
            while (!_quit)
            {
                int delayToSave = 3600000;
                _nextSaveDate = DateTime.Now.AddMilliseconds(delayToSave);
                Thread.Sleep(delayToSave);

                _isSaving = true;
                //Save();
                _isSaving = false;
            }
        }

        public void Load()
        {
			//if (Count == 0)
			//{
			//    UrlQueueEntity qe = new UrlQueueEntity("http://www.circuitcity.com", 0, 200, "");
			//    Insert(qe);

			//    qe = new UrlQueueEntity("http://www.bestbuy.com/site/index.jsp", 0, 201, "");
			//    Insert(qe);

			//    qe = new UrlQueueEntity("http://www.newegg.com", 0, 202, "");
			//    Insert(qe);

			//    qe = new UrlQueueEntity("http://www.compusa.com", 0, 203, "");
			//    Insert(qe);

				
			//}
        }

        public new void Clear()
        {
            lock (_wholeQueueLock)
            {
                lock (_roundRobinLock)
                {
                    _roundRobinId = 0;
                    foreach (Queue<UrlQueueEntity> siteQueue in _roundRobinQueues)
                    {
                        lock (siteQueue)
                        {
                            siteQueue.Clear();
                        }
                    }
                    _roundRobinQueues.Clear();
                    _siteQueues.Clear();
                }
            }
        }

        public void Quit()
        {
            _quit = true;

            if (!_isSaving) _saveThread.Abort();
        }
    }
}
