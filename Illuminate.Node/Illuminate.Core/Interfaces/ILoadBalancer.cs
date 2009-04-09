using System;
using System.Collections.Generic;

namespace Illuminate.Interfaces
{
	/// <summary>
	/// Interface determining required operations for an extended load balancer within Illuminate.
	/// To enable an extended load balancer, create a class implementing this interface, compile it to a DLL,
	/// and set the 'ExtendedLoadBalancer' setting to the name of the DLL.
	/// </summary>
	public interface ILoadBalancer : Illuminate.Interfaces.IIlluminateObject
	{
		/// <summary>
		/// Initializes the plugin and creates all necessary helper objects.
		/// Must be called before other operations are used.
		/// </summary>
		/// <param name="nodeDataService">Node Data service to use</param>
		/// <param name="logName">Logging name to use</param>
		void InitializePlugin(Illuminate.Node.INodeService nodeDataService, string logName);

		/// <summary>
		/// List of agent types in the order that an agent should be removed.
		/// </summary>
		List<string> AgentRemovalOrder { get; }

		/// <summary>
		/// Gets the number of each type of agent to add and remove from the swarm.
		/// </summary>
		/// <param name="agentsToRemove">Out parameter containing the number of each type of agent to remove</param>
		/// <param name="agentsToAdd">Out parameter containing the number of each type of agent to add</param>
		void GetAgentChangeList(out Dictionary<string, int> agentsToRemove, out Dictionary<string, int> agentsToAdd);

		/// <summary>
		/// Determines if additional agents are needed to properly load balance the swarm.
		/// </summary>
		/// <returns></returns>
		bool NeedAdditionalAgents();

		/// <summary>
		/// Calculates the number of agents of the given type to add to the swarm.
		/// </summary>
		/// <param name="agentType">Type of agent</param>
		/// <returns>Number of agents to add</returns>
		int NumAgentsToAdd(string agentType);

		/// <summary>
		/// Refreshes all internal data to ensure proper load balancing decisions.
		/// </summary>
		void RefreshData();
	}
}
