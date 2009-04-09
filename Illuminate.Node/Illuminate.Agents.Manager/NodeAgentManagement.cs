using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Tools;

namespace Illuminate.Agents
{
	internal class NodeAgentManagement : Illuminate.Core.IlluminateObject
	{
		#region Events

		public delegate void OnSendWarningDelegate(LoadBalancingWarning warning);
		public event OnSendWarningDelegate OnSendWarning;

		private void RaiseSendWarning(LoadBalancingWarning warning)
		{
			if (OnSendWarning != null)
				OnSendWarning(warning);
		}

		#endregion

		#region Members

		private Illuminate.Node.Service _dataService;

		private Dictionary<string, int> _minimumTotalAgentLimits;
		private Dictionary<string, int> _maximumTotalAgentLimits;
		private Dictionary<string, int> _minimumNodeAgentLimits;
		private Dictionary<string, int> _maximumNodeAgentLimits;

		private Dictionary<string, Node.Collections.IAgent> _nodeAgents;
		private Dictionary<string, Dictionary<string, int>> _nodeAgentTypeCount;
		private Dictionary<string, List<string>> _agentNodes;

		private Dictionary<string, List<string>> _nodeAgentsToAdd;
		private Dictionary<string, List<string>> _nodeAgentsToRemove;

		#endregion

		#region Constructor

		public NodeAgentManagement(Illuminate.Node.Service dataService, string logName)
		{
			_dataService = dataService;
			LOGNAME = logName;

			_nodeAgents = new Dictionary<string, Node.Collections.IAgent>();
			_nodeAgentTypeCount = new Dictionary<string, Dictionary<string, int>>();
			_agentNodes = new Dictionary<string, List<string>>();
			_nodeAgentsToAdd = new Dictionary<string, List<string>>();
			_nodeAgentsToRemove = new Dictionary<string, List<string>>();

			_minimumTotalAgentLimits = _dataService.Monitor.GetTotalAgentLimits("MIN");
			_maximumTotalAgentLimits = _dataService.Monitor.GetTotalAgentLimits("MAX");
			_minimumNodeAgentLimits = _dataService.Monitor.GetNodeAgentLimits("MIN");
			_maximumNodeAgentLimits = _dataService.Monitor.GetNodeAgentLimits("MAX");
		}

		#endregion

		/// <summary>
		/// Gets the node from the given list that has the highest load average.
		/// </summary>
		/// <param name="nodes">List of nodes to check</param>
		/// <param name="systemLoads">Dictionary containing the system loads for all nodes</param>
		/// <returns>Node name of the node with the highest load average</returns>
		private string GetNodeHighestLoad(List<string> nodes, Dictionary<string, Node.Entities.ISystemLoad> systemLoads)
		{
			string highestLoadNode = null;
			double highestLoad = double.MinValue;
			foreach (string nodeName in nodes)
			{
				if (systemLoads.ContainsKey(nodeName) && systemLoads[nodeName].Load5Min > highestLoad)
				{
					highestLoad = systemLoads[nodeName].Load5Min;
					highestLoadNode = nodeName;
				}
			}
			return highestLoadNode;
		}

		/// <summary>
		/// Gets the node that has the lowest load average.
		/// </summary>
		/// <param name="nodes">List of nodes to check</param>
		/// <param name="systemLoads">Dictionary containing the system loads for all nodes</param>
		/// <returns>Node name of the node with the lowest load average</returns>
		private string GetNodeLowestLoad(List<string> nodes, Dictionary<string, Node.Entities.ISystemLoad> systemLoads)
		{
			string lowestLoadNode = null;
			double lowestLoad = double.MaxValue;
			foreach (string nodeName in nodes)
			{
				if (systemLoads.ContainsKey(nodeName) && systemLoads[nodeName].Load5Min < lowestLoad)
				{
					lowestLoad = systemLoads[nodeName].Load5Min;
					lowestLoadNode = nodeName;
				}
			}
			return lowestLoadNode;
		}

		/// <summary>
		/// Removes the given node from all internal collections.
		/// </summary>
		/// <param name="nodeName">Node name of the node to remove</param>
		public void RemoveNode(string nodeName)
		{
			_nodeAgents.Remove(nodeName);
			foreach (KeyValuePair<string, List<string>> kvp in _agentNodes)
				kvp.Value.RemoveAll(delegate(string s) { return s.Equals(nodeName); });
		}

		/// <summary>
		/// Makes sure all minimum total limits are not violated.
		/// Sorts the nodes based on increasing load, then assigns agents through the list until all critical agents are started.
		/// Nodes that are overloaded will still get agents added, if necessary.
		/// Operations are queued until ApplyAgentChanges is called.
		/// </summary>
		/// <param name="systemLoads">Dictionary containing the system loads for all nodes</param>
		/// <returns>Boolean indicating if a recovery took place</returns>
		public bool RecoverCriticalAgents(Dictionary<string, Node.Entities.ISystemLoad> systemLoads)
		{
			if (_minimumTotalAgentLimits.Count == 0)
				return false;

			bool recoveryPerformed = false;

			//sortedNodes is only sorted if we actually need to recover a critical agent
			List<string> sortedNodes = null;
			int i = 0;
			foreach (string minAgent in _minimumTotalAgentLimits.Keys)
			{
				if (!_agentNodes.ContainsKey(minAgent) || _agentNodes[minAgent].Count == 0)
				{
					if (sortedNodes == null)
						sortedNodes = LoadBalancer.SortNodesIncreasingLoad(systemLoads.Keys, systemLoads);

					recoveryPerformed = true;
					string nodeName = sortedNodes[i++];
					bool added = AddNodeAgent(nodeName, minAgent);
					if (!added)
						RaiseSendWarning(new LoadBalancingWarning(LoadBalancingWarning.WarningTypeEnum.AddCriticalAgentFailure, nodeName));

					if (i >= sortedNodes.Count)
						i = 0; //use sorted list as roundrobin
				}
			}

			//also check minimum node agent limits
			foreach (string minAgent in _minimumNodeAgentLimits.Keys)
			{
				foreach (string nodeName in _nodeAgentTypeCount.Keys)
				{
					if (_nodeAgentTypeCount[nodeName].ContainsKey(minAgent) && _nodeAgentTypeCount[nodeName][minAgent] < _minimumNodeAgentLimits[minAgent])
					{
						recoveryPerformed = true;
						int numNeedAdded = _minimumNodeAgentLimits[minAgent] - _nodeAgentTypeCount[nodeName][minAgent];
						bool added = true;
						for (int j = 0; j < numNeedAdded; j++)
							added &= AddNodeAgent(nodeName, minAgent);

						if (!added)
							RaiseSendWarning(new LoadBalancingWarning(LoadBalancingWarning.WarningTypeEnum.AddCriticalAgentFailure, nodeName));
					}
				}
			}

			return recoveryPerformed;
		}

		/// <summary>
		/// Removes extra agents that violate total and node maximum limits.
		/// Operations are queued until ApplyAgentChanges is called.
		/// </summary>
		/// <param name="systemLoads">Dictionary containing the system loads for all nodes</param>
		/// <returns>Boolean indicating success</returns>
		public bool RemoveLimitViolatingAgents(Dictionary<string, Illuminate.Node.Entities.ISystemLoad> systemLoads)
		{
			bool removalPerformed = false;
			foreach (string maxAgent in _maximumTotalAgentLimits.Keys)
			{
				if (_agentNodes.ContainsKey(maxAgent) && _agentNodes[maxAgent].Count > _maximumTotalAgentLimits[maxAgent])
				{
					removalPerformed = true;
					string highestLoadNode = GetNodeHighestLoad(_agentNodes[maxAgent], systemLoads);
					bool removed = RemoveNodeAgent(highestLoadNode, maxAgent);
					if (!removed)
						RaiseSendWarning(new LoadBalancingWarning(LoadBalancingWarning.WarningTypeEnum.RemoveExtraAgentFailure, highestLoadNode));
				}
			}
			foreach (string maxAgent in _maximumNodeAgentLimits.Keys)
			{
				foreach (string nodeName in _nodeAgentTypeCount.Keys)
				{
					if (_nodeAgentTypeCount[nodeName].ContainsKey(maxAgent) && _nodeAgentTypeCount[nodeName][maxAgent] > _maximumNodeAgentLimits[maxAgent])
					{
						removalPerformed = true;
						int numNeedRemoved = _nodeAgentTypeCount[nodeName][maxAgent] - _maximumNodeAgentLimits[maxAgent];
						bool removed = true;
						for (int i = 0; i < numNeedRemoved; i++)
							removed &= RemoveNodeAgent(nodeName, maxAgent);

						if (!removed)
							RaiseSendWarning(new LoadBalancingWarning(LoadBalancingWarning.WarningTypeEnum.RemoveExtraAgentFailure, nodeName));
					}
				}
			}

			return removalPerformed;
		}

		/// <summary>
		/// Determines if additional agents for each agent type can be added to a node.
		/// </summary>
		/// <param name="agentRemovalOrder">List of agent types in the order that an agent should be removed</param>
		/// <returns>Boolean indicating if additional agents can be added</returns>
		public bool CanAddMoreAgents(List<string> agentRemovalOrder)
		{
			foreach (string agentType in agentRemovalOrder)
			{
				bool canAddAgentType = false;
				foreach (string nodeName in _nodeAgents.Keys)
				{
					if (VerifyAgentLimits(nodeName, agentType, false))
						canAddAgentType = true;
				}
				if (!canAddAgentType)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Verifies that adding or removing the given agent on the given node will not violate the system limits.
		/// </summary>
		/// <param name="nodeName">Node name of the node</param>
		/// <param name="agentType">Agent that needs to be tested</param>
		/// <param name="remove">True == removing the agent, False == adding</param>
		/// <returns>Boolean indicating if this action is allowed</returns>
		private bool VerifyAgentLimits(string nodeName, string agentType, bool remove)
		{
			//remove==false ===> adding
			if (remove)
			{
				//count the number of this agent type that is scheduled to be removed
				int numToBeRemoved = 1; //count the one being removed right now
				foreach (KeyValuePair<string, List<string>> kvp in _nodeAgentsToRemove)
				{
					foreach (string nodeAgentType in kvp.Value)
					{
						if (nodeAgentType == agentType)
							numToBeRemoved++;
					}
				}

				//make sure this does not violate total minimum limits
				if (_minimumTotalAgentLimits.ContainsKey(agentType) && _agentNodes.ContainsKey(agentType)
					&& _agentNodes[agentType].Count - numToBeRemoved < _minimumTotalAgentLimits[agentType])
				{
					return false;
				}
				//make sure this does not violate node minimum limits
				if (_minimumNodeAgentLimits.ContainsKey(agentType) && _agentNodes.ContainsKey(agentType)
					&& _agentNodes[agentType].Count - numToBeRemoved < _minimumNodeAgentLimits[agentType])
				{
					return false;
				}
			}
			else
			{
				int currentNumber = 1; //count the one being added right now
				for (int i = 0; i < _nodeAgents[nodeName].Count; i++)
				{
					Node.Entities.IAgent agent = _nodeAgents[nodeName][i];
					if (agent.AgentName == agentType)
						currentNumber++;
				}
				//also count the number scheduled to be added
				foreach (KeyValuePair<string, List<string>> kvp in _nodeAgentsToAdd)
				{
					foreach (string nodeAgentType in kvp.Value)
					{
						if (nodeAgentType == agentType)
							currentNumber++;
					}
				}

				//make sure this does not violate total maximum limits
				if (_maximumTotalAgentLimits.ContainsKey(agentType)
					&& currentNumber > _maximumTotalAgentLimits[agentType])
				{
					Logger.WriteLine("Attempt to add agent of type " + agentType + " to node " + nodeName + ".  Unable to add due to total agent limits.", Logger.Severity.Error, LOGNAME);
					return false;
				}
				//make sure this does not violate node maximum limits
				if (_maximumNodeAgentLimits.ContainsKey(agentType)
					&& currentNumber > _maximumNodeAgentLimits[agentType])
				{
					Logger.WriteLine("Attempt to add agent of type " + agentType + " to node " + nodeName + ".  Unable to add due to node agent limits.", Logger.Severity.Error, LOGNAME);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Updates all internal collections storing agents for the given node.
		/// </summary>
		/// <param name="nodeName">Node name of the node to update</param>
		public void UpdateNodeAgents(string nodeName)
		{
			_nodeAgents[nodeName] = _dataService.Monitor.GetAgents(nodeName);

			//calculate how many of each agent type this node has
			if (_nodeAgentTypeCount.ContainsKey(nodeName))
				_nodeAgentTypeCount[nodeName].Clear();
			else
				_nodeAgentTypeCount.Add(nodeName, new Dictionary<string, int>());
				
			Dictionary<string, int> nodeAgentTypeCounts = _nodeAgentTypeCount[nodeName];
			for (int i = 0; i < _nodeAgents[nodeName].Count; i++)
			{
				Node.Entities.IAgent agent = _nodeAgents[nodeName][i];
				if (nodeAgentTypeCounts.ContainsKey(agent.AgentName))
					nodeAgentTypeCounts[agent.AgentName] += 1;
				else
					nodeAgentTypeCounts.Add(agent.AgentName, 1);
			}

			//keep track of which nodes have different types of agents
			foreach (KeyValuePair<string, List<string>> kvp in _agentNodes)
				kvp.Value.RemoveAll(delegate(string n) { return n == nodeName; });

			for (int i = 0; i < _nodeAgents[nodeName].Count; i++)
			{
				Node.Entities.IAgent agent = _nodeAgents[nodeName][i];
				if (!_agentNodes.ContainsKey(agent.AgentName))
					_agentNodes.Add(agent.AgentName, new List<string>());
				_agentNodes[agent.AgentName].Add(nodeName);
			}
		}

		/// <summary>
		/// Adds an agent of the given type to the given node.
		/// Operations are queued until ApplyAgentChanges is called.
		/// </summary>
		/// <param name="nodeName">Node name of the node</param>
		/// <param name="agentType">Agent to add to the node</param>
		/// <returns>Bool indicating success (if false, this agent cannot be added to this node)</returns>
		public bool AddNodeAgent(string nodeName, string agentType)
		{
			//enforce agent limits
			if (!VerifyAgentLimits(nodeName, agentType, false))
				return false;

			if (!_nodeAgentsToAdd.ContainsKey(nodeName))
				_nodeAgentsToAdd.Add(nodeName, new List<string>());
			_nodeAgentsToAdd[nodeName].Add(agentType);

			return true;
		}

		/// <summary>
		/// Adds an agent of the given type to the nodes in the provided collection.
		/// If an agent is assigned to a node, that node is removed from the nodesToUse collection.
		/// Otherwise, the node is added back into the collection for further load balancing operations.
		/// Agents will be added until either numToAdd is reached, or there are no nodes left in the collection.
		/// </summary>
		/// <param name="agentType">Agent to add</param>
		/// <param name="numToAdd">Number of the specified agent type to add</param>
		/// <param name="nodesToUse">Names of the nodes to add the agent to</param>
		/// <returns>Number of agents actually assigned to nodes</returns>
		public int AddAgent(string agentType, int numToAdd, List<string> nodesToUse)
		{
			//Take the first node off the list and try to add the agent to it.
			//If the agent could not be added, put the node back into the list so it can be used for other agent types.
			int numAdded = 0;
			for (int i = 0; i < nodesToUse.Count; i++)
			{
				string nodeName = nodesToUse[0]; //do not use i as an index into collection (treat it as a queue)
				nodesToUse.RemoveAt(0);
				if (AddNodeAgent(nodeName, agentType))
					numAdded++;
				else
					nodesToUse.Add(nodeName);
			}
			return numAdded;
		}

		/// <summary>
		/// Removes an agent from the given node.  The type of agent removed is not taken into consideration.
		/// This should only be used when there is no specified order for agent removal.
		/// Operations are queued until ApplyAgentChanges is called.
		/// </summary>
		/// <param name="nodeName">Node name of the node</param>
		/// <returns>Bool indicating success (if false, an agent cannot be removed from this node)</returns>
		public bool RemoveNodeAgent(string nodeName)
		{
			Node.Collections.IAgent nodeAgents = _nodeAgents[nodeName];
			for (int i = 0; i < nodeAgents.Count; i++)
			{
				if (RemoveNodeAgent(nodeName, nodeAgents[i].AgentName))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Removes an agent of the given type from the given node.
		/// Operations are queued until ApplyAgentChanges is called.
		/// </summary>
		/// <param name="nodeName">Node name of the node</param>
		/// <param name="agentType">Agent to remove from the node</param>
		/// <returns>Bool indicating success (if false, this agent cannot be removed from this node)</returns>
		public bool RemoveNodeAgent(string nodeName, string agentType)
		{
			//enforce agent limits
			if (!VerifyAgentLimits(nodeName, agentType, true))
				return false;

			if (!_nodeAgentsToRemove.ContainsKey(nodeName))
				_nodeAgentsToRemove.Add(nodeName, new List<string>());
			_nodeAgentsToRemove[nodeName].Add(agentType);

			return true;
		}

		/// <summary>
		/// Removes an agent of the given type from the swarm.
		/// The node with the highest load will be picked.
		/// Operations are queued until ApplyAgentChanges is called.
		/// </summary>
		/// <param name="agentType">Agent to remove</param>
		/// <param name="numToRemove">Number of agents to remove</param>
		/// <param name="systemLoads">Dictionary containing the system loads for all nodes</param>
		/// <returns>Bool indicating success (if false, this agent cannot be removed from this node)</returns>
		public int RemoveAgent(string agentType, int numToRemove, Dictionary<string, Illuminate.Node.Entities.ISystemLoad> systemLoads)
		{
			int numRemoved = 0;
			if (_agentNodes.ContainsKey(agentType))
			{
				List<string> sortedNodes = LoadBalancer.SortNodesIncreasingLoad(_agentNodes[agentType], systemLoads);
				for (int i = sortedNodes.Count - 1; i >= 0; i--) //going through collection backwards results in decreasing loads
				{
					if (RemoveNodeAgent(sortedNodes[i], agentType))
						numRemoved++;

					if (numRemoved >= numToRemove)
						break;
				}
			}
			return numRemoved;
		}

		/// <summary>
		/// Removes an agent from the specified node based on the order in the agentRemovalOrder parameter.
		/// Operations are queued until ApplyAgentChanges is called.
		/// </summary>
		/// <param name="nodeName">Name of node to remove an agent from</param>
		/// <param name="agentRemovalOrder">List of agent types in the order that an agent should be removed</param>
		/// <returns>Bool indicating success (if false, an agent cannot be removed from this node)</returns>
		public bool RemoveAgent(string nodeName, List<string> agentRemovalOrder)
		{
			Illuminate.Node.Collections.IAgent nodeAgents = _nodeAgents[nodeName];

			//agentRemovalOrder should be sorted based on the order to remove agents to reduce load
			foreach (string agentType in agentRemovalOrder)
			{
				if (nodeAgents.Contains(agentType) && RemoveNodeAgent(nodeName, agentType))
					return true;
			}
			
			//If we got this far, an agent couldn't be removed
			return false;
		}

		#region Database Modification Methods

		/// <summary>
		/// Applies all pending changes.
		/// Removals are processed before additions.
		/// </summary>
		public void ApplyAgentChanges()
		{
			foreach (KeyValuePair<string, List<string>> kvp in _nodeAgentsToRemove)
			{
				foreach (string agentType in kvp.Value)
				{
					Logger.WriteLine("Removing agent " + agentType + " from node " + kvp.Key, Illuminate.Tools.Logger.Severity.Information, LOGNAME);
					RemoveAgent(kvp.Key, agentType);
				}
				UpdateNodeAgents(kvp.Key);
			}
			_nodeAgentsToRemove.Clear();

			foreach (KeyValuePair<string, List<string>> kvp in _nodeAgentsToAdd)
			{
				foreach (string agentType in kvp.Value)
				{
					Logger.WriteLine("Adding agent " + agentType + " to node " + kvp.Key, Illuminate.Tools.Logger.Severity.Information, LOGNAME);
					AddAgent(kvp.Key, agentType);
				}
				UpdateNodeAgents(kvp.Key);
			}
			_nodeAgentsToAdd.Clear();
		}

		/// <summary>
		/// Immediately adds an agent of the given type to the given node.
		/// A Node Command will be sent notifying the node.
		/// </summary>
		/// <param name="nodeName">Node name of the node</param>
		/// <param name="agentType">Agent to add</param>
		private void AddAgent(string nodeName, string agentType)
		{
			int entryId = _dataService.Monitor.InsertAgent(nodeName, agentType);
			_dataService.Monitor.SendAddAgent(nodeName, agentType, entryId);
		}

		/// <summary>
		/// Immediately removes an agent of the given type from the given node.
		/// A Node Command will be sent notifying the node.
		/// </summary>
		/// <param name="nodeName">Node name of the node</param>
		/// <param name="agentType">Agent to remove</param>
		private void RemoveAgent(string nodeName, string agentType)
		{
			//need to find one to get the entry id
			for (int i = 0; i < _nodeAgents[nodeName].Count; i++)
			{
				Node.Entities.IAgent agent = _nodeAgents[nodeName][i];
				if (agent.AgentName == agentType)
				{
					_dataService.Admin.DeleteAgent(agent.EntryId);
					_dataService.Monitor.SendRemoveAgent(nodeName, agent.EntryId.ToString());
					_nodeAgents[nodeName].Remove(agent); //keep up to date if another agent is removed from this node without refreshing
					break;
				}
			}
		}

		#endregion
	}
}
