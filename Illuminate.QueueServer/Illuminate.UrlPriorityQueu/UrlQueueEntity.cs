using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.PriorityQueue;
using Illuminate.Interfaces.IlluminateQueue;


namespace Illuminate.UrlPriorityQueue
{
    public class UrlQueueEntity : IPriorityQueueEntity<string>, ICloneable
    {
        protected string _data;
        protected DateTime _date;
        protected int _priority;

        protected string _uniqueProductIdentifier;
        protected int _siteId;

        public string Data
        {
            get { return _data; }
        }

        public DateTime Date
        {
            get { return _date; }
			set { _date = value; }
        }

        public int Priority
        {
            get { return _priority; }
        }

        public int SiteId
        {
            get { return _siteId; }
        }

        public string UniqueProductIdentifier
        {
            get { return _uniqueProductIdentifier; }
        }

        #region Constructors

        public UrlQueueEntity(string data, DateTime date, int priority)
        {
            _data = data;
            _date = date;
            _priority = priority;
        }

        public UrlQueueEntity(string data, int priority, int siteId)
        {
            _data = data;
            _priority = priority;
            _siteId = siteId;
        }

        public UrlQueueEntity(string data, int priority, int siteId, string uniqueProductIdentifier)
        {
            _data = data;
            _priority = priority;
            _siteId = siteId;
            _uniqueProductIdentifier = uniqueProductIdentifier;
        }

        public UrlQueueEntity(string data, DateTime date, int priority, int siteId, string uniqueProductIdentifier)
        {
            _data = data;
            _date = date;
            _priority = priority;
            _siteId = siteId;
            _uniqueProductIdentifier = uniqueProductIdentifier;
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            UrlQueueEntity b = (UrlQueueEntity)obj;

            if (this.Date != b.Date)
            {
                return this.Date.CompareTo(b.Date);
            }
            else
            {
                return this.Priority.CompareTo(b.Priority);
            }
        }

        #endregion

        #region IPriorityQueueEntity<string> Members

        public string Key
        {
            get
            {
                if ((_uniqueProductIdentifier == null) || (_uniqueProductIdentifier == string.Empty))
                {
                    return _data;
                }
                else
                {
                    return _uniqueProductIdentifier + "|" + _siteId.ToString();
                }
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new UrlQueueEntity(_data, _date, _priority, _siteId, _uniqueProductIdentifier);
        }

        #endregion
    }
}
