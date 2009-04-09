using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.PriorityQueue;

namespace Illuminate.Queue.QueueData
{
	public class Data
	{
		private static Gazaro.Interfaces.IDataService _ds;
		private static Illuminate.UrlPriorityQueue.UrlQueue _queue;
		private static string logName = string.Empty;

		public static string LOGNAME
		{
			get
			{
				return logName;
			}
			set
			{
				logName = value;
			}
		}

		public static Gazaro.Interfaces.IDataService DataService
		{
			get
			{
				if (_ds == null)
				{
					_ds = new Gazaro.DataService();
				}

				return _ds;
			}
		}

		public static Illuminate.UrlPriorityQueue.UrlQueue Queue
		{
			get
			{
				if (_queue == null)
				{
					_queue = new Illuminate.UrlPriorityQueue.UrlQueue(DataService, LOGNAME);
				}

				return _queue;
			}
		}
	}
}
