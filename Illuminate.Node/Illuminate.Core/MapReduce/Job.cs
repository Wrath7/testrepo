using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Illuminate.MapReduce
{
	public sealed class Job : Illuminate.MapReduce.IJob
	{
		#region Class Members

		/// <summary>
		/// Timeout interval to wait for all Map functions to finish
		/// </summary>
		public int TimeoutInterval
		{
			get
			{
				return _timeoutInterval;
			}
			set
			{
				_timeoutInterval = value;
			}
		}

		public List<string> FunctionalKeys
		{
			get
			{
				return _functionalKeys;
			}
		}



		/// <summary>
		/// Timeout interval to wait for all Map functions to finish
		/// </summary>
		private int _timeoutInterval = 5000;
        private Dictionary<object, IInputData> _intermediateResults = new Dictionary<object, IInputData>();
		private object _results = null;
		private int _threadCount = 0;
		private int _doneCnt = 0;
		private string LOGNAME = string.Empty;
		private bool _mapTimedOut = false;
		private List<string> _functionalKeys = new List<string>();
		private List<IInputData> _unfunctionalData = new List<IInputData>();

		#endregion

		#region MapReduce Delegates

		/// <summary>
		/// Map Function
		/// </summary>
		private Map _map;

		/// <summary>
		/// Reduce Function
		/// </summary>
		private Reduce _reduce;

		/// <summary>
		/// Failure Function
		/// </summary>
		private Failure _failure;

		/// <summary>
		/// Map Delegate
		/// </summary>
		/// <param name="data">Input Data</param>
		public delegate IInputData Map(IInputData input);

		/// <summary>
		/// Reduce Delegate
		/// </summary>
		/// <param name="data"></param>
		public delegate object Reduce(object data);

		/// <summary>
		/// Failure Delegate
		/// </summary>
		/// <param name="input"></param>
		public delegate List<object> Failure(List<IInputData> input);

		#endregion

		public Job(Map map, Reduce reduce, Failure failure, string logName)
		{
			_map = map;
			_reduce = reduce;
            _failure = failure;
			LOGNAME = logName;
		}

		public object Execute(Dictionary<string, object> input)
		{
			foreach (KeyValuePair<string, object> inputPair in input)
			{
				IInputData id = new InputData(inputPair.Key, inputPair.Value);

                lock (_intermediateResults)
                {
                    _intermediateResults.Add(id.Key, id);
                }

				System.Threading.ThreadPool.QueueUserWorkItem(MapWaitCallBack, id);

				_threadCount++;
			}

			//Launch Synchonization Timer
			DateTime startTime = DateTime.Now;

			//Wait for all intermediate results to return
			while (_doneCnt != _threadCount)
			{
				TimeSpan ts = DateTime.Now.Subtract(startTime);

				if (ts.TotalMilliseconds >= _timeoutInterval)
				{
					lock (_intermediateResults)
					{
						_mapTimedOut = true;
					}

					break;
				}
				//throw new Illuminate.Exceptions.MapTimeoutException("Not all map tasks have completed in the desired amount of time - Done: " + _doneCnt.ToString() + " - Total: " + _threadCount);
				Thread.Sleep(1);
			}

			//Generate Intermediate Results
			#region Create Intermediate Result List

			List<object> intermediateResults = new List<object>();
            lock (_intermediateResults)
            {
                foreach (IInputData idata in _intermediateResults.Values)
                {
                    if (idata.Done && idata.ErrorCode == 0)
                    {
                        _functionalKeys.Add(idata.Key.ToString());
                        intermediateResults.Add(idata.IntermediateResult);
                    }
                    else
                    {
                        _unfunctionalData.Add(idata);
                    }
                }
            }

			#endregion

			#region Failure Handler

			if (_unfunctionalData.Count > 0)
			{
				List<object> returnData = _failure(_unfunctionalData);

				foreach (object intermediateResult in returnData)
				{
					intermediateResults.Add(intermediateResult);
				}
			}

			#endregion

			_results = _reduce(intermediateResults);

			return _results;
		}

		private void MapWaitCallBack(object data)
		{
			IInputData id = (IInputData)data;

			id = _map(id);

			lock (_intermediateResults)
			{
				if (!_mapTimedOut) id.Done = true;
				_doneCnt++;
				_intermediateResults[id.Key] = id;
			}
		}

	}
}
