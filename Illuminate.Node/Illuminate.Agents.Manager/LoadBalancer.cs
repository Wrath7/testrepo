using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Node;
using Illuminate.Tools;
using Illuminate.Interfaces;

namespace Illuminate.Agents
{
	internal class LoadBalancer : Illuminate.Core.IlluminateObject
	{
		#region Members

		private Illuminate.Node.Service _dataService;

		private Dictionary<string, Node.Entities.ISystemLoad> _systemLoads;
		private NodeAgentManagement _nodeAgentManagement;

		private ILoadBalancer _extendedLoadBalancer;

		#region Warning Messages

		private const int NODE_WARNING_DELAY_HOURS = 6;
		private List<LoadBalancingWarning> _warningMessages;
		private Dictionary<string, DateTime> _nodeWarningSendTime;
		private Dictionary<string, List<LoadBalancingWarning.WarningTypeEnum>> _sentWarningMessages;

		#endregion

		#region Settings

		private string _warningDisplayName = "Illuminate Load Balancer";
		private string _warningFromAddress;
		private string _warningToAddress;

		private int _maxWaitBeforeDead;
		private double _optimalSystemLoad;
		private double _optimalLoadWindowSize;
		private double _optimalSystemLoadLowerBound;
		private double _optimalSystemLoadUpperBound;

		#endregion

		#endregion

		#region Constructor

		public LoadBalancer(Illuminate.Contexts.AgentContext context, string logName)
		{
			_dataService = context.NodeDataService;
			LOGNAME = logName;
			_sentWarningMessages = new Dictionary<string, List<LoadBalancingWarning.WarningTypeEnum>>();
			_systemLoads = new Dictionary<string, Illuminate.Node.Entities.ISystemLoad>();
			_nodeAgentManagement = new NodeAgentManagement(_dataService, LOGNAME);
			_nodeAgentManagement.OnSendWarning += new NodeAgentManagement.OnSendWarningDelegate(QueueWarningEmail);
			_warningMessages = new List<LoadBalancingWarning>();
			_nodeWarningSendTime = new Dictionary<string, DateTime>();

			string warningDisplayName = _dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.LoadBalancerDisplayName);
			if (!string.IsNullOrEmpty(warningDisplayName))
				_warningDisplayName = warningDisplayName;
			Logger.WriteLine("LoadBalancerDisplayName: " + _warningDisplayName, Logger.Severity.Debug, LOGNAME);

			_warningFromAddress = _dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.LoadBalancerFromAddress);
			if (string.IsNullOrEmpty(_warningFromAddress))
				throw new Illuminate.Exceptions.ErrorException("LoadBalancerFromAddress either not defined or invalid.");
			Logger.WriteLine("LoadBalancerFromAddress: " + _warningFromAddress, Logger.Severity.Debug, LOGNAME);

			_warningToAddress = _dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.LoadBalancerWarningDest);
			if (string.IsNullOrEmpty(_warningToAddress))
				throw new Illuminate.Exceptions.ErrorException("LoadBalancerWarningTo either not defined or invalid.");
			Logger.WriteLine("LoadBalancerWarningTo: " + _warningToAddress, Logger.Severity.Debug, LOGNAME);

			if (!int.TryParse(_dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.MaxWaitBeforeDead), out _maxWaitBeforeDead))
				throw new Illuminate.Exceptions.ErrorException("MaxWaitBeforeDead either not defined or invalid.");
			Logger.WriteLine("MaxWaitBeforeDead: " + _maxWaitBeforeDead.ToString(), Logger.Severity.Debug, LOGNAME);

			if (!double.TryParse(_dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.OptimalSystemLoad), out _optimalSystemLoad))
				throw new Illuminate.Exceptions.ErrorException("OptimalSystemLoad either not defined or invalid.");
			Logger.WriteLine("OptimalSystemLoad: " + _optimalSystemLoad.ToString(), Logger.Severity.Debug, LOGNAME);

			if (!double.TryParse(_dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.OptimalLoadWindowSize), out _optimalLoadWindowSize))
				throw new Illuminate.Exceptions.ErrorException("OptimalLoadWindowSize either not defined or invalid.");
			Logger.WriteLine("OptimalLoadWindowSize: " + _optimalLoadWindowSize.ToString(), Logger.Severity.Debug, LOGNAME);
			_optimalSystemLoadLowerBound = _optimalSystemLoad - _optimalLoadWindowSize;
			_optimalSystemLoadUpperBound = _optimalSystemLoad + _optimalLoadWindowSize;

			string extendedLoadBalancer = _dataService.Monitor.GetSetting(Illuminate.Node.Managers.Settings.ExtendedLoadBalancer);
			if (!string.IsNullOrEmpty(extendedLoadBalancer))
			{
				Logger.WriteLine("ExtendedLoadBalancer: " + extendedLoadBalancer, Logger.Severity.Information, LOGNAME);
				try
				{
					Invoker inv = new Invoker();
					_extendedLoadBalancer = (ILoadBalancer)inv.Invoke(context.AgentPath + extendedLoadBalancer, typeof(ILoadBalancer));
					_extendedLoadBalancer.InitializePlugin(_dataService, LOGNAME);
				}
				catch (Exception ex)
				{
					Logger.WriteLine("Unable to load extended load balancer: " + ex.Message, Logger.Severity.Error, LOGNAME);
					_extendedLoadBalancer = null;
				}
			}

			//load initial data
			UpdateSystemLoadCache();
		}

		#endregion

		/// <summary>
		/// Updates the internal cache of the system loads and creates a collection of nodes to be load balanced.
		/// </summary>
		/// <returns>List of the node names that should be load balanced</returns>
		private List<string> UpdateSystemLoadCache()
		{
			Node.Collections.ISystemLoad systemLoads = _dataService.SystemLoad.GetSystemLoadAliveNodes();
			if (systemLoads == null)
				return null;

			//combine these values in the local cache
			List<string> nodesToLoadBalance = new List<string>();
			for (int i = 0; i < systemLoads.Count; i++)
			{
				Node.Entities.ISystemLoad systemLoad = systemLoads[i];

				_systemLoads[systemLoad.NodeName] = systemLoad;
				_nodeAgentManagement.UpdateNodeAgents(systemLoad.NodeName);

				if (systemLoad.Load5Min < _optimalSystemLoadLowerBound)
					nodesToLoadBalance.Add(systemLoad.NodeName);
			}

			Logger.WriteLine(nodesToLoadBalance.Count + " nodes are eligible for load balancing.", Logger.Severity.Debug, LOGNAME);
			return nodesToLoadBalance;
		}

		/// <summary>
		/// Removes the given node from all internal collections.
		/// </summary>
		/// <param name="nodeName">Node name of the node to remove</param>
		private void RemoveNode(string nodeName)
		{
			_systemLoads.Remove(nodeName);
			_nodeAgentManagement.RemoveNode(nodeName);
		}

		/// <summary>
		/// Load balances all eligible nodes.
		/// This is the main method that performs all the work of the load balancer.
		/// </summary>
		public void LoadBalance()
		{
			Logger.WriteLine("Detecting DEAD nodes...", Logger.Severity.Debug, LOGNAME);
			List<string> deadNodes = _dataService.Monitor.DetectDeadNodes(_maxWaitBeforeDead);
			foreach (string deadNode in deadNodes)
			{
				Logger.WriteLine("Node " + deadNode + " has failed.", Logger.Severity.Error, LOGNAME);
				RemoveNode(deadNode);
				QueueWarningEmail(new LoadBalancingWarning(LoadBalancingWarning.WarningTypeEnum.NodeFailure, deadNode));
			}

			List<string> nodesToLoadBalance = UpdateSystemLoadCache();
			if (nodesToLoadBalance == null)
			{
				Logger.WriteLine("No nodes were alive.  Unable to load balance.", Logger.Severity.Error, LOGNAME);
				return;
			}

			bool recoveryPerformed = _nodeAgentManagement.RecoverCriticalAgents(_systemLoads);
			recoveryPerformed |= _nodeAgentManagement.RemoveLimitViolatingAgents(_systemLoads);
			if (recoveryPerformed)
			{
				Logger.WriteLine("Applying changes from critical agent recovery and limit violating agent removal...", Logger.Severity.Debug, LOGNAME);
				_nodeAgentManagement.ApplyAgentChanges();
				Logger.WriteLine("Applying critical agent recovery and limit violating agent removal complete.", Logger.Severity.Debug, LOGNAME);
			}

			if (_extendedLoadBalancer != null)
				_extendedLoadBalancer.RefreshData();

			bool insufficientCapacity = DetectInsufficientCapacity();
			if (insufficientCapacity)
			{
				Logger.WriteLine("Insufficient capacity in swarm.  Unable to add additional agents due to all nodes at optimal load or agent limits.", Logger.Severity.Error, LOGNAME);
				QueueWarningEmail(new LoadBalancingWarning(LoadBalancingWarning.WarningTypeEnum.InsufficientCapacity));
			}

			//no nodes to load balance
			if (nodesToLoadBalance.Count == 0)
				return;

			//remove agents from nodes that are overloaded
			List<string> nodesToContinueLoadBalancing = LoadBalanceOverloadedNodes(nodesToLoadBalance);

			//if we have an extended load balancer, ask it for agents to add or remove
			if (_extendedLoadBalancer != null)
			{
				Dictionary<string, int> agentsToRemove = null, agentsToAdd = null;
				_extendedLoadBalancer.GetAgentChangeList(out agentsToRemove, out agentsToAdd);

				Logger.WriteLine("Applying agent change list...", Logger.Severity.Debug, LOGNAME);
				foreach (KeyValuePair<string, int> kvp in agentsToRemove)
				{
					_nodeAgentManagement.RemoveAgent(kvp.Key, kvp.Value, _systemLoads);
				}
				_nodeAgentManagement.ApplyAgentChanges();

				List<string> sortedNodesToContinueLoadBalancing = LoadBalancer.SortNodesIncreasingLoad(nodesToContinueLoadBalancing, _systemLoads);
				foreach (KeyValuePair<string, int> kvp in agentsToAdd)
				{
					int numAdded = _nodeAgentManagement.AddAgent(kvp.Key, kvp.Value, sortedNodesToContinueLoadBalancing);
					if (numAdded == sortedNodesToContinueLoadBalancing.Count)
						break; //no more nodes available
				}
				_nodeAgentManagement.ApplyAgentChanges();

				Logger.WriteLine("Agent change list applied.", Logger.Severity.Debug, LOGNAME);
			}

			//load balancing is complete, can send warning emails now
			SendWarningEmails();
		}

		/// <summary>
		/// Insufficient Capacity is when all nodes are at optimal load and additional agents are needed.
		/// </summary>
		private bool DetectInsufficientCapacity()
		{
			bool needMoreAgents = false;
			bool canAddMoreAgents = true;
			if (_extendedLoadBalancer != null)
			{
				needMoreAgents = _extendedLoadBalancer.NeedAdditionalAgents();
				canAddMoreAgents = _nodeAgentManagement.CanAddMoreAgents(_extendedLoadBalancer.AgentRemovalOrder);
			}

			//are all nodes at optimal load?
			bool freeSpaceOnNodes = false;
			foreach (Node.Entities.ISystemLoad sysLoad in _systemLoads.Values)
			{
				if (sysLoad.Load5Min < _optimalSystemLoadLowerBound)
					freeSpaceOnNodes = true;
			}

			return needMoreAgents && !(canAddMoreAgents && freeSpaceOnNodes);
		}

		/// <summary>
		/// Removes agents from nodes that are above the optimal system load.
		/// </summary>
		/// <param name="nodesToLoadBalance">Nodes that should be checked</param>
		/// <returns>List of nodes that were not affected by this step</returns>
		private List<string> LoadBalanceOverloadedNodes(List<string> nodesToLoadBalance)
		{
			List<string> nodesToContinueLoadBalancing = new List<string>();
			foreach (string nodeName in nodesToLoadBalance)
			{
				if (_systemLoads[nodeName].Load5Min > _optimalSystemLoadUpperBound)
				{
					Logger.WriteLine("Node " + nodeName + "'s load is " + _systemLoads[nodeName].Load5Min + ". Agents will be removed.", Logger.Severity.Information, LOGNAME);
					bool removed = true;
					if (_extendedLoadBalancer != null)
						removed = _nodeAgentManagement.RemoveAgent(nodeName, _extendedLoadBalancer.AgentRemovalOrder);
					else
						removed = _nodeAgentManagement.RemoveNodeAgent(nodeName); //don't have priorities for agent removal, just remove any agent we can

					if (!removed)
					{
						//send warning, cannot lower load
						Logger.WriteLine("Unable to remove agents from node " + nodeName + " to reduce load. Load on " + nodeName + " is too high.", Logger.Severity.Error, LOGNAME);
						QueueWarningEmail(new LoadBalancingWarning(LoadBalancingWarning.WarningTypeEnum.RemoveAgentFailure, nodeName));
					}
				}
				else
				{
					Logger.WriteLine("Load on " + nodeName + " is within acceptable limit (" + _systemLoads[nodeName].Load5Min + ").", Logger.Severity.Debug, LOGNAME);
					nodesToContinueLoadBalancing.Add(nodeName);
				}
			}
			_nodeAgentManagement.ApplyAgentChanges();
			return nodesToContinueLoadBalancing;
		}

		/// <summary>
		/// Sorts the given list of nodes based on increasing load.
		/// </summary>
		/// <param name="nodes">List of nodes to sort</param>
		/// <param name="systemLoads">Dictionary containing all system load information for nodes</param>
		/// <returns>List of NodeNames</returns>
		internal static List<string> SortNodesIncreasingLoad(ICollection<string> nodes, Dictionary<string, Node.Entities.ISystemLoad> systemLoads)
		{
			List<string> sortedNodes = new List<string>(nodes.Count);
			double previousLoadAdded = double.MinValue;
			for (int i = 0; i < nodes.Count; i++)
			{
				//find the node with the next highest load
				double lowestLoad = double.MaxValue;
				Node.Entities.ISystemLoad lowestSystemLoad = null;
				foreach (string node in nodes)
				{
					Node.Entities.ISystemLoad sysLoad = systemLoads[node];
					if (sysLoad.Load5Min <= lowestLoad && sysLoad.Load5Min > previousLoadAdded)
					{
						lowestLoad = sysLoad.Load5Min;
						lowestSystemLoad = sysLoad;
					}
				}

				if (lowestSystemLoad != null)
				{
					sortedNodes.Add(lowestSystemLoad.NodeName);
					previousLoadAdded = lowestLoad;
				}
			}

			return sortedNodes;
		}

		#region Warning Emails

		/// <summary>
		/// Queues a load balancing warning for future sending.
		/// Verifies that a message of the specified type has not been sent within NODE_WARNING_DELAY_HOURS.
		/// </summary>
		/// <param name="warning">Warning to be sent</param>
		protected void QueueWarningEmail(LoadBalancingWarning warning)
		{
			if (WarningShouldSendTime(warning) || WarningShouldSendType(warning))
			{
				_warningMessages.Add(warning);

				_nodeWarningSendTime[warning.NodeName] = DateTime.Now;
				if (!_sentWarningMessages.ContainsKey(warning.NodeName))
					_sentWarningMessages.Add(warning.NodeName, new List<LoadBalancingWarning.WarningTypeEnum>());
				_sentWarningMessages[warning.NodeName].Add(warning.WarningType);
			}
		}

		/// <summary>
		/// Determines if a warning should be sent based on the last time a warning was sent.
		/// </summary>
		/// <param name="warning">Warning to be sent</param>
		/// <returns>Boolean indicating if a warning should be sent</returns>
		private bool WarningShouldSendTime(LoadBalancingWarning warning)
		{
			bool shouldSendTime = true;
			if (_nodeWarningSendTime.ContainsKey(warning.NodeName))
			{
				TimeSpan lastEmailInterval = DateTime.Now.Subtract(_nodeWarningSendTime[warning.NodeName]);
				if (lastEmailInterval.TotalHours < NODE_WARNING_DELAY_HOURS)
					shouldSendTime = false;
				else
					_sentWarningMessages.Remove(warning.NodeName); //greater than NODE_WARNING_DELAY_HOURS, reset the sent warning messages
			}
			return shouldSendTime;
		}

		/// <summary>
		/// Determines if a warning should be sent based on if a warning of the same type has been sent.
		/// </summary>
		/// <param name="warning">Warning to be sent</param>
		/// <returns>Boolean indicating if a warning should be sent</returns>
		private bool WarningShouldSendType(LoadBalancingWarning warning)
		{
			return !(_sentWarningMessages.ContainsKey(warning.NodeName) 
				&& _sentWarningMessages[warning.NodeName].Contains(warning.WarningType));
		}

		/// <summary>
		/// Sends all queued warning emails.
		/// </summary>
		private void SendWarningEmails()
		{
			if (_warningMessages.Count == 0)
				return;

			string warningMessage = "The following warnings occurred during load balancing operations at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ":<br><ul>";
			foreach (LoadBalancingWarning warning in _warningMessages)
			{
				string message = string.Empty;
				switch (warning.WarningType)
				{
					case LoadBalancingWarning.WarningTypeEnum.NodeFailure:
						message = "Node " + warning.NodeName + " has failed!";
						break;
					case LoadBalancingWarning.WarningTypeEnum.InsufficientCapacity:
						message = "Insufficient capacity in swarm.  Unable to add additional agents due to all nodes at optimal load or agent limits.";
						break;
					case LoadBalancingWarning.WarningTypeEnum.RemoveAgentFailure:
						message = "Unable to remove agents from node " + warning.NodeName + " to reduce load. Load on " + warning.NodeName + " is too high.";
						break;
					case LoadBalancingWarning.WarningTypeEnum.AddCriticalAgentFailure:
						message = "Unable to add a critical agent to Node " + warning.NodeName + ".  Agents defined with minimum limits may not be running.";
						break;
					case LoadBalancingWarning.WarningTypeEnum.RemoveExtraAgentFailure:
						message = "Unable to remove extra agents from Node " + warning.NodeName + ".  Agents defined with a maximum limit may have too many instances running";
						break;
					default:
						message = "Warning occurred of type " + warning.WarningType.ToString() + " on Node " + warning.NodeName + ".";
						break;
				}
				warningMessage += "<li>" + message + "</li>";
			}
			warningMessage += "</ul>";
			
			EmailService emailService = new EmailService();
			emailService.SendEmail(_warningFromAddress, _warningDisplayName, _warningToAddress, "Illuminate Load Balancer Warning", warningMessage);

			_warningMessages.Clear();
		}

		#endregion
	}

	internal class LoadBalancingWarning
	{
		public enum WarningTypeEnum
		{
			NodeFailure,
			InsufficientCapacity,
			RemoveAgentFailure,
			AddCriticalAgentFailure,
			RemoveExtraAgentFailure
		}

		#region Members

		public WarningTypeEnum WarningType;
		public string NodeName = "General";

		#endregion

		public LoadBalancingWarning(WarningTypeEnum warningType)
		{
			WarningType = warningType;
		}

		public LoadBalancingWarning(WarningTypeEnum warningType, string nodeName)
		{
			WarningType = warningType;
			NodeName = nodeName;
		}
	}
}
