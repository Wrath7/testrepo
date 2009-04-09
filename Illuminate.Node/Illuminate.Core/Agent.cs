using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Threading;
using Illuminate.Interfaces;
using Illuminate.Tools;
using System.IO;
using System.Diagnostics;

namespace Illuminate.Core.Node
{
	/// <summary>
	/// This class represents a Agent from a Agent collection of a node cluster.
	/// </summary>
	[Serializable]
	public class Agent
	{
		public enum AgentStatus
		{
			NotInit = 0,
			Init = 4,
			ErrorInit = 10,
			Executing = 1,
			Sleeping = 2,
			Error = 3,
			Starting = 5,
			Started = 6,
			Stopping = 7,
			Stopped = 8,
			Killed = 9
		}

		#region Class Members

		private string _agentId = string.Empty;
        private string _sectionName = string.Empty;
		private Illuminate.Communication.Communicator _com;
		private string _nodeId = string.Empty;
		private AgentStatus _status = AgentStatus.NotInit;
		private int _runCount = 0;
		private Thread _agentThread;
		private bool _stopAgent = false;
		private Interfaces.IAgent _agent;
		private AgentStandard _standard;
		private string LOGNAME = string.Empty;
		private string _agentPath = string.Empty;
		private string _agentType = string.Empty;
		private object _writeLock = new object();
		private string _errorMessage = string.Empty;
		private DateTime _nextExecutionTime = new DateTime();
		private DateTime _lastExecutionTime = new DateTime();
		private DateTime _startExecutionTime = new DateTime();
		private int _nativeThreadId = 0;

		#endregion

		#region Public Properties

		public Illuminate.Communication.Communicator Communicator
		{
			get
			{
				return _com;
			}
		}

		public DateTime NextExecutionTime
		{
			get
			{
				return _nextExecutionTime;
			}
		}

		public DateTime LastExecutionTime
		{
			get
			{
				return _lastExecutionTime;
			}
		}

		public DateTime StartExecutionTime
		{
			get
			{
				return _startExecutionTime;
			}
		}

		/// <summary>
		/// Returns the Agent Id in GUID String format
		/// </summary>
		public string AgentId
		{
			get
			{
				return _agentId;
			}
		}

        public string SectionName
        {
            get
			{ 
				return _sectionName; 
			}
        }

		public string AgentType
		{
			get
			{
				return _agentType;
			}
		}

		public AgentStatus Status
        {
            get
            {
				return (AgentStatus)_status;
            }
        }

		/// <summary>
		/// The amount times the task/crawler has been initiated
		/// </summary>
		public int RunCount
		{
			get
			{
				return _runCount;
			}
			set
			{

				_runCount++;
			}
		}

		public Interfaces.IAgent AgentObject
		{
			get
			{
				return _agent;
			}
		}

		public string LogName
		{
			get
			{
				return LOGNAME;
			}
		}

		public string InternalStatus
		{
			get
			{
				return _agent.GetStatus();
			}
		}

		public Thread AgentThread
		{
			get
			{
				return _agentThread;
			}
		}

		public int NativeThreadId
		{
			get
			{
				return _nativeThreadId;
			}
		}

		#endregion

		#region Constructor

		public Agent(string NodeId, string AgentId, string AgentPath, Illuminate.Communication.Communicator Com)
		{
			_com = Com;
			_nodeId = NodeId;
            _agentId = _nodeId + "_" + AgentId;
			_agentPath = AgentPath;
            _sectionName = AgentId;

			#region Get Agent Filename

			string tempPath = _agentPath;
			tempPath = tempPath.Replace("\\", "/");
			string[] pathParts = tempPath.Split('/');
			_agentType = pathParts[pathParts.Length - 1];

			#endregion

			#region Get Log Name

			if (_agentId.Contains("Monitor") || _agentId.Contains("Manager"))
			{
				LOGNAME = _agentId;
			}
			else
			{
				LOGNAME = _agentId + "_" + _agentType;
			}

			#endregion
		}

		#endregion

		/// <summary>
		/// Starts the timer if the Agent is enabled.
		/// </summary>
		public void Start()
		{
			Logger.WriteLine("Starting Agent...", Logger.Severity.Debug, LOGNAME);

			Logger.WriteLine("Status = " + _status.ToString() + "...", Logger.Severity.Debug, LOGNAME);

			Logger.WriteLine("Continuing Initialization", Logger.Severity.Debug, LOGNAME);

			_status = AgentStatus.Starting;
			
			Initialize();

			//Initialize the timer
			bool timerStarted = InitializeTimer();

			//If the timer was initialized, enable the timer.
			if (timerStarted)
			{
				_stopAgent = false;

				_status = AgentStatus.Started;
			}
			else
			{
				_status = AgentStatus.Error;
			}

		}

        /// <summary>
        /// Initializes the Agent
        /// </summary>
        public void Initialize()
        {
			//try
			//{
				_status = AgentStatus.Init;

				Logger.WriteLine("Initializing Agent...", Logger.Severity.Debug, LOGNAME);

				//Get the Agent from the event reference
				Illuminate.Tools.Invoker inv = new Illuminate.Tools.Invoker();
				_agent = (Illuminate.Interfaces.IAgent)inv.Invoke(_agentPath, typeof(Illuminate.Interfaces.IAgent));

				Logger.WriteLine("Agent Invoked...", Logger.Severity.Debug, LOGNAME);

				if (_agent != null)
				{
					Logger.WriteLine("Initializing Invoked Plugin...", Logger.Severity.Debug, LOGNAME);

					Logger.WriteLine("Starting Agent...", Logger.Severity.Debug);

					Contexts.AgentContext context = new Contexts.AgentContext(_nodeId, _sectionName, AgentId, _com, this);
					context.LogName = LOGNAME;

					_standard = (AgentStandard)_agent;

					_standard.InitializeAgent(context);
					_agent.InitializeAgent(context);

					Logger.WriteLine("Invoked Plugin Initialized...", Logger.Severity.Debug, LOGNAME);
				}
				else
				{
					Logger.WriteLine("Invoked Agent is NULL...", Logger.Severity.Debug, LOGNAME);
				}
			//}
			//catch (Exception e)
			//{
			//	_status = AgentStatus.ErrorInitializing;

			//	string ErrorMsg = "Error trying to initialize agent: " + _agentPath + " " + e.Message.Replace("\r", "").Replace("\n", "");

			//	throw new Illuminate.Exceptions.ErrorException(ErrorMsg);
			//}
        }

		/// <summary>
		/// Stops the timer
		/// </summary>
		public void Stop()
		{
			lock (this)
			{
				//if this agent did not initialize, it is already stopped (thread was never started)
				//ErrorInitializing is different from Error (With Error, the thread was started)
				if (_status == AgentStatus.ErrorInit || _status == AgentStatus.NotInit)
					_status = AgentStatus.Stopped;
				else
					_status = AgentStatus.Stopping;

				_stopAgent = true;
			}
		}

		public void StopTimer()
		{
			_stopAgent = true;
		}

		/// <summary>
		/// Initializes the timer object
		/// </summary>
		/// <returns></returns>
		private bool InitializeTimer()
		{
			_agentThread = new Thread(new ThreadStart(timer_Elapsed));
			_agentThread.Name = _agentId;
			_agentThread.Start();

			//Return True if the timer was initialized
			return true;
		}

		/// <summary>
		/// Event fired when the timer interval has completed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// object sender, System.Timers.ElapsedEventArgs e
		void timer_Elapsed()
		{
			_nativeThreadId = AppDomain.GetCurrentThreadId();

			while(!_stopAgent)
			{
				try
				{
					_status = AgentStatus.Executing;

					_startExecutionTime = DateTime.Now;

					//Run standard Run first
					_standard.Run();

					//Run the event task
					_agent.Run();

					_lastExecutionTime = DateTime.Now;

					_runCount++;

					//WriteStatusToFile();
				}
				catch (Illuminate.Exceptions.CriticalException er)
				{
				    _status = AgentStatus.Error;

				    _errorMessage = er.Message + "\n" + er.StackTrace;

				    _stopAgent = true;

				    Tools.Logger.WriteLine("Critical Error: " + er.Message, Illuminate.Tools.Logger.Severity.Fatal, LOGNAME);
				}
				catch (Illuminate.Exceptions.ErrorException er)
				{
				    _status = AgentStatus.Error;

				    Tools.Logger.WriteLine("Error: " + er.Message, Illuminate.Tools.Logger.Severity.Error, LOGNAME);
				}
				catch (Exception er)
				{
				    _status = AgentStatus.Error;

				    Tools.Logger.WriteLine("Error: " + er.Message + " " + er.StackTrace.ToString(), Illuminate.Tools.Logger.Severity.Error, _nodeId);
				}
				finally
				{
					if (!_stopAgent)
					{
						_nextExecutionTime = DateTime.Now.AddMilliseconds(_agent.GetInterval());

						_status = AgentStatus.Sleeping;

						//Set the next run time and the last run time for the event
						Logger.WriteLine("Waiting: " + _agent.GetInterval().ToString(), Logger.Severity.Debug, LOGNAME);

						Wait();
					}
                }
			} //while

			if (_status != AgentStatus.Error || _status != AgentStatus.Killed)
			{
				try
				{
					//agent is stopping, perform any necessary cleanup
					_agent.Cleanup();
					_standard.Cleanup();
				}
				catch (Exception er)
				{
					if (_agentThread.ThreadState != System.Threading.ThreadState.Stopped && _agentThread.ThreadState != System.Threading.ThreadState.Aborted && _agentThread.ThreadState != System.Threading.ThreadState.AbortRequested)
					{
						Logger.WriteLine("Error occurred during agent cleanup: " + er.Message + " " + er.StackTrace, Logger.Severity.Error, _nodeId);
					}
				}

				_status = AgentStatus.Stopped;
			}			
		}

		public void Wait()
		{
			int counter = 0;
			int intervalCount = _agent.GetInterval();

			while (counter < intervalCount && !_stopAgent)
			{
				Thread.Sleep(500);
				counter += 500;
			}
		}

		public void KillAgent()
		{
			if (_agentThread != null)
			{
				_stopAgent = true;
				_agentThread.Abort();

				while (true)
				{
					Thread.Sleep(1);

					if (_agentThread.ThreadState == System.Threading.ThreadState.Aborted) break;
					if (_agentThread.ThreadState == System.Threading.ThreadState.Stopped) break;
				}

				_status = AgentStatus.Killed;
			}
		}
	}
}
