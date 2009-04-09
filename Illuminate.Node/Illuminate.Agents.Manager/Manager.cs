using System;
using Illuminate.Tools;
using System.Text;

namespace Illuminate.Agents
{
	public class Manager : Illuminate.Core.Node.AgentStandard, Illuminate.Interfaces.IAgent
	{
		//the node id this manager is running on (_context.NodeId is overwritten on start with 'MGR', which is useless)
		//used to inform new nodes what node has the manager
		private string _currentManagerNodeName = null;

		private bool _enableLoadBalancing;
		private LoadBalancer _loadBalancer;
		private int _numMinutesLoadBalanceNodeInterval = 5;
		private DateTime _lastLoadBalance = DateTime.Now;

		private Illuminate.Node.Service _dataService;

		public new void InitializeAgent(Illuminate.Contexts.AgentContext context)
		{
			_dataService = context.NodeDataService;

			base.InitializeAgent(context);

			_currentManagerNodeName = _context.NodeId;
			_context.NodeId = Illuminate.Communication.Communicator.MANAGERNAME;

			Logger.WriteLine("Getting AgentManagerInterval...", Logger.Severity.Debug, LOGNAME);
			int agentManagerInterval = 0;
			if (!int.TryParse(_dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.AgentManagerInterval), out agentManagerInterval))
				throw new Illuminate.Exceptions.ErrorException("AgentManagerInterval either not defined or invalid.");
			_context.Interval = agentManagerInterval;
			Logger.WriteLine("AgentManagerInterval: " + _context.Interval.ToString() + "...", Logger.Severity.Debug, LOGNAME);

			Logger.WriteLine("Getting EnableLoadBalancing...", Logger.Severity.Debug, LOGNAME);
			_enableLoadBalancing = false;
			if (_dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.EnableLoadBalancing) == "True")
			{
				_enableLoadBalancing = true;
				_loadBalancer = new LoadBalancer(context, LOGNAME);
			}
			else
			{
				Logger.WriteLine("LoadBalancing is disabled.", Logger.Severity.Information, LOGNAME);
			}
			Logger.WriteLine("EnableLoadBalancing: " + _enableLoadBalancing.ToString() + "...", Logger.Severity.Debug, LOGNAME);

			_context.Communicator.OnDataIn += new Illuminate.Communication.Communicator.OnDataInDelegate(com_OnDataIn);

			_dataService.Monitor.NotifyNewManagerStarted(_currentManagerNodeName);
		}

		public new void Run()
		{
			Logger.WriteLine("Manager has executed", Logger.Severity.Debug, _context.AgentId);

			PollCommand();

			if (_enableLoadBalancing && DateTime.Now.Subtract(_lastLoadBalance).TotalMinutes > _numMinutesLoadBalanceNodeInterval)
			{
				Logger.WriteLine("Executing Load Balancer...", Logger.Severity.Debug, LOGNAME);
				_loadBalancer.LoadBalance();

				_lastLoadBalance = DateTime.Now;

				Logger.WriteLine("Load Balancer complete.", Logger.Severity.Debug, LOGNAME);
			}
		}

		void com_OnDataIn(Illuminate.Communication.Command Command)
		{

		}

		#region Communication

		private void PollCommand()
		{
			Logger.WriteLine("Poll for command", Logger.Severity.Debug, _context.NodeId);

			string commands = _dataService.Monitor.GetCommand("Manager");

			if (commands.Length > 0)
			{
				string[] cmdLines = commands.Split('\n');

				for (int i = 0; i < cmdLines.Length; i++)
				{
					if (cmdLines[i].Length > 0)
					{
						string Command = cmdLines[i];

						Logger.WriteLine("Received Command: " + Command, Logger.Severity.Debug, _context.NodeId);

						Illuminate.Communication.Command data = Illuminate.Communication.Command.Deserialize(Command);

						switch (data.Action)
						{
							case "READY":
								_dataService.Monitor.SendReadyConfirm(data.From, _currentManagerNodeName);
								_dataService.Monitor.ActivateAgents(data.From);
								break;

							case "ADDED":
								_dataService.Monitor.UpdateStatus("ALIVE", int.Parse(data.Parameters[0]), data.From);
								break;
							case "ALIVE":
								//if the first parameter is an "O" (letter O), this is from the monitor so it applies to the entire node
								if (data.Parameters[0] == "O")
									_dataService.Monitor.UpdateStatus("ALIVE", data.From);
								else
									_dataService.Monitor.UpdateStatus("ALIVE", int.Parse(data.Parameters[0]), data.From);

								//send a confirmation back so the monitor knows the manager is still alive
								_dataService.Monitor.SendStatusConfirm(data.From);
								break;
							case "REMOVED":
								_dataService.Monitor.UpdateStatus("ALIVE", int.Parse(data.Parameters[0]), data.From);
								break;
						}
					}
				}
			}
		}

		#endregion
	}
}
