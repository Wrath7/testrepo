using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.PriorityQueue;
using Gazaro;
using System.Threading;
using Illuminate.Interfaces.IlluminateQueue;

namespace Illuminate.UrlPriorityQueue
{
    public class UrlPriorityQueue : PriorityQueue<string>, Illuminate.Interfaces.IlluminateQueue.IIlluminateQueue
    {
        protected Dictionary<int, DateTime> _siteAvailableCrawlTime;
        protected Dictionary<string, int> _siteEntryPoints;
		protected Dictionary<string, byte> _activityLog;
		private DateTime _activityLogTruncateDate = new DateTime();
		private Thread _saveThread = null;
		private bool _isSaving = false;
		private bool _quit = false;
		private DateTime _nextSaveDate = new DateTime();
		private Gazaro.Interfaces.IDataService _dataService;

        private string LOGID = "QUEUE_SERVER";

		public DateTime NextSaveDate
		{
			get
			{
				return _nextSaveDate;
			}
		}

        public int ActvityLogCount
        {
            get
            {
                return _activityLog.Count;
            }
        }

        /// <summary>
        /// Constructor Create a new UrlPriorityQueue
        /// </summary>
        /// <param name="dataService"></param>
        public UrlPriorityQueue(Gazaro.Interfaces.IDataService dataService) : base()
		{
			_siteAvailableCrawlTime = new Dictionary<int, DateTime>();
			
			_siteEntryPoints = new Dictionary<string, int>();

			_dataService = dataService;

            _activityLog = new Dictionary<string, byte>();
			ResetActivityLog();

			_saveThread = new Thread(new ThreadStart(SaveEvent));
			_saveThread.Start();
        }

        /// <summary>
        /// Insert a ndw Url Entity Into the queue.
        /// </summary>
        /// <param name="newEnt"></param>
        /// <returns></returns>
        public new bool Insert(IPriorityQueueEntity<string> newEnt)
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
            Illuminate.Tools.Logger.WriteLine("Checking Activity Log", Illuminate.Tools.Logger.Severity.Debug, LOGID);
			bool insert = true;

			#region Check Activity Log

			lock (_activityLog)
			{
                Illuminate.Tools.Logger.WriteLine("Activity Log Lock Aquired", Illuminate.Tools.Logger.Severity.Debug, LOGID);
				#region Check if activity log needs to be reset

				TimeSpan ts = DateTime.Now.Subtract(_activityLogTruncateDate);

				if (ts.TotalSeconds >= 30)
				{
                    Illuminate.Tools.Logger.WriteLine("Resetting Activity Log", Illuminate.Tools.Logger.Severity.Debug, LOGID);
                    ResetActivityLog();
                    Illuminate.Tools.Logger.WriteLine("Reset Activity Log Complete", Illuminate.Tools.Logger.Severity.Debug, LOGID);
				}

				#endregion


				if (_activityLog.ContainsKey(newEnt.Key)) insert = false;
                Illuminate.Tools.Logger.WriteLine("Activity Log Lock checked. Insert is: " + insert, Illuminate.Tools.Logger.Severity.Debug, LOGID);
                //if (newEnt.UniqueProductIdentifier != null)
                //    if (newEnt.UniqueProductIdentifier.Length > 0 && insert) 
                //        if (_activityLog.ContainsKey(newEnt.UniqueProductIdentifier))
                //            insert = false;
			}

			#endregion

            if ((insert) && (this.Count > 100000))
            {
                Illuminate.Tools.Logger.WriteLine("Queue OverCapacity Not Inserting this URL.", Illuminate.Tools.Logger.Severity.Debug, LOGID);
                insert = false;
            }

            Illuminate.Tools.Logger.WriteLine("Activity Log Lock released. Insert is: " + insert, Illuminate.Tools.Logger.Severity.Debug, LOGID);
            if (insert)
            {
                DateTime crawlTime;
                lock (_siteAvailableCrawlTime)
                {
                    if (_siteAvailableCrawlTime.TryGetValue(newEnt.SiteId, out crawlTime))
                    {
                        if (crawlTime <= DateTime.Now)
                        {
                            crawlTime = DateTime.Now; 
                        }
                         
                        _siteAvailableCrawlTime[newEnt.SiteId] = crawlTime.AddSeconds(1);
                    }
                    else
                    {
                        crawlTime = DateTime.Now;
                        _siteAvailableCrawlTime.Add(newEnt.SiteId, crawlTime.AddSeconds(1));
                    }
                }
                newEnt.Date = crawlTime;
                if (base.Insert(newEnt))
                {
                    Illuminate.Tools.Logger.WriteLine("Successfully insert with CrawlTime: " + crawlTime.ToString(), Illuminate.Tools.Logger.Severity.Debug, LOGID);
                    return true;
                }
                else
                {
                    Illuminate.Tools.Logger.WriteLine("Url Already Existed in the Queue", Illuminate.Tools.Logger.Severity.Debug, LOGID);
                    lock (_siteAvailableCrawlTime)
                    {
                        _siteAvailableCrawlTime[newEnt.SiteId] = _siteAvailableCrawlTime[newEnt.SiteId].AddSeconds(-1);
                    }
                }
            }

			return false;
        }

        /// <summary>
        /// Pops a URL from the priority queue if it's date time is earlier than now.
        /// </summary>
        /// <returns></returns>
        public new UrlQueueEntity Pop()
        {
            UrlQueueEntity peeked = Peek();
            if ((peeked != null) && (peeked.Date <= DateTime.Now))
            {
                UrlQueueEntity uqe = (UrlQueueEntity)base.Pop();
                bool isEntryPoint = false;

                if (uqe == null)
                {
                    Illuminate.Tools.Logger.WriteLine("Popped Null Out of the Queue Returning null", Illuminate.Tools.Logger.Severity.Debug, LOGID);
                    return uqe;
                }

                if (uqe.Date > DateTime.Now)
                {
                    Illuminate.Tools.Logger.WriteLine("Popped a URL with a future crawl time. Reinserting the URL and retuning null", Illuminate.Tools.Logger.Severity.Debug, LOGID);
                    base.Insert(uqe);
                    return null;
                }

                if (_siteEntryPoints.ContainsKey((string)uqe.Data))
                {
                    uqe.Date = DateTime.Now.AddHours(24);
                    Illuminate.Tools.Logger.WriteLine("URL is an EntryPoint. Reinserting the URL in the Queue with Crawltime: " + uqe.Date.ToString(), Illuminate.Tools.Logger.Severity.Debug, LOGID);
                    base.Insert(uqe);
                    isEntryPoint = true;
                }

                #region Add Url to Activity Log
                lock (_activityLog)
                {
                    if ((!isEntryPoint) || (!_activityLog.ContainsKey(uqe.Key)))
                    {
                        if (_activityLog.Count > 1000)
                        {
                            _activityLog.Clear();
                        }
                        _activityLog.Add(uqe.Key, 1);
                        

                        //if (uqe.UniqueProductIdentifier != null)
                        //    if (uqe.UniqueProductIdentifier.Length > 0)
                        //        _activityLog.Add(uqe.UniqueProductIdentifier, uqe);
                    }

                }
                #endregion

                Illuminate.Tools.Logger.WriteLine("URL Inserted into Activity Log Returning " + uqe.Data.ToString(), Illuminate.Tools.Logger.Severity.Debug, LOGID);
                    
                return uqe;
            }
            else
            {
                //nothing in the queue has a crawl date before now.
                Illuminate.Tools.Logger.WriteLine("Nothing in the queue has a crawl date before now.", Illuminate.Tools.Logger.Severity.Debug, LOGID);
                return null;
            }
        }

        /// <summary>
        /// Return the first Url on the queue but don't remove it.
        /// </summary>
        /// <returns></returns>
        public new UrlQueueEntity Peek()
        {
            return (UrlQueueEntity)base.Peek();
        }

        /// <summary>
        /// Return the first i Urls on the queue but don't remove them.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public new UrlQueueEntity[] Peek(int i)
        {
            IPriorityQueueEntity<string>[] iEntities = base.Peek(i);
            UrlQueueEntity[] entities = new UrlQueueEntity[iEntities.Length];

            for (int j = 0; j < iEntities.Length; j++)
            {
                entities[j] = (UrlQueueEntity)iEntities[j];

				if (j == 9) break;
            }
            return entities;
        }

        /// <summary>
        /// Save the Url queue to the database
        /// </summary>
        public void Save()
        {
            Illuminate.Tools.Logger.WriteLine("Saving", Illuminate.Tools.Logger.Severity.Debug, LOGID);
            lock (_siteAvailableCrawlTime)
            {
                foreach (KeyValuePair<int, DateTime> sitePair in _siteAvailableCrawlTime)
                {
                    _dataService.Crawler.Site.SetProcessDate(sitePair.Key, sitePair.Value);
                }
            }
            Illuminate.Tools.Logger.WriteLine("Saved Sites ProcessDate", Illuminate.Tools.Logger.Severity.Debug, LOGID);
			PriorityQueue<string> saveState = (PriorityQueue<string>)base.Clone();
			UrlQueueEntity uqe = (UrlQueueEntity)saveState.Pop();
			
			_dataService.Crawler.UrlQueue.ClearUrlQueue();

			while (uqe != null)
			{
			    _dataService.Crawler.UrlQueue.InsertIntoUrlQueue(uqe.Data, uqe.UniqueProductIdentifier, uqe.Date, uqe.Priority, 0, uqe.SiteId);
			    uqe = (UrlQueueEntity)saveState.Pop();
			}
            Illuminate.Tools.Logger.WriteLine("Save Complete", Illuminate.Tools.Logger.Severity.Debug, LOGID);
            GC.Collect();
        }

		private void SaveEvent()
		{
            while (!_quit)
            {
                int delayToSave = 120000;
                _nextSaveDate = DateTime.Now.AddMilliseconds(delayToSave);
                Thread.Sleep(delayToSave);

                _isSaving = true;
                Save();
                _isSaving = false;
            }
		}

        public void Load()
        {
            //Load Sites and Crawler times
            Gazaro.Crawler.Collections.ISite sites = _dataService.Crawler.Site.GetSites();
            foreach (Gazaro.Crawler.Entities.ISite site in sites)
            {
                _siteEntryPoints.Add(site.EntryPoint, 0);
                if (site.ProcessDate.Year == 1900)
                {
                    _siteAvailableCrawlTime.Add(site.SiteId, site.ProcessDate);
                }
            }

            //Load Url Queue
            Gazaro.Crawler.Collections.IUrlQueue savedUrlQueue = _dataService.Crawler.UrlQueue.GetUrlQueue();
            foreach (Gazaro.Crawler.Entities.IUrlQueue savedUrl in savedUrlQueue)
            {
                UrlQueueEntity uqe = new UrlQueueEntity(savedUrl.Url, savedUrl.QueueDate, (int)savedUrl.Priority, savedUrl.SiteId, savedUrl.Key);
                base.Insert(uqe);
            }
        }

        public new void Clear()
        {
			ResetActivityLog();

            _siteAvailableCrawlTime.Clear();
            _siteEntryPoints.Clear();
            base.Clear();
        }

		public void SaveActivityLog()
		{
			//_dataService.Crawler.UrlQueue.TruncateActivityLog();

			//foreach (KeyValuePair<string, UrlQueueEntity> kvp in _activityLog)
			//{
			//    if (kvp.Key.StartsWith("http://")) _dataService.Crawler.UrlQueue.InsertIntoActivityLog(kvp.Value.Data, kvp.Value.SiteId, kvp.Value.UniqueProductIdentifier, kvp.Value.Date);
			//}
		}

		public void ResetActivityLog()
		{
			#region Set Activity Log

			_activityLog.Clear();
			DateTime tomorrow = DateTime.Now.AddDays(1);
			_activityLogTruncateDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 12, 0, 0);

			#endregion
		}

		public void Quit()
		{
			_quit = true;

			if (!_isSaving) _saveThread.Abort();
		}
    }
}
