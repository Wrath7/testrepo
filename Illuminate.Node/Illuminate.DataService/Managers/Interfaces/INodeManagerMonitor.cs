using System;
using System.Collections.Generic;

namespace Illuminate.Node.Managers
{
	/// <summary>
	/// Settings that can be accessed using this manager.
	/// </summary>
	public enum Settings
	{
		Manager,
		SystemLoadRefreshInterval,
		AgentMonitorInterval,
		AgentManagerInterval,
		AliveMessageRate,
		ManagerResponseThreshold,
		ManagerRecoveryWaitTime,
		MaxWaitBeforeDead,
		OptimalSystemLoad,
		OptimalLoadWindowSize,
		EnableLoadBalancing,
		LoadBalancerDisplayName,
		LoadBalancerFromAddress,
		LoadBalancerWarningDest,
		ExtendedLoadBalancer
	}

	public interface INodeManagerMonitor
	{
		void ActivateAgents(string NodeId);
		int AddCrawler(string NodeId);
		int AddExtractor(string NodeId);
		int AddDataCompiler(string NodeId);
		int AddAffiliateLinkGenerator(string NodeId);
		int AddEmailDiscovery(string NodeId);
		int AddEmailPublisher(string NodeId);
		string GetCommand(string Destination);
		void DeleteCommandsForNode(string nodeId);
		string GetSetting(Settings setting);
		string GetSetting(string setting);
		Dictionary<string, int> GetTotalAgentLimits(string minMax);
		Dictionary<string, int> GetNodeAgentLimits(string minMax);
		string GetNextManagerNode();
		bool GetManagerRecoveryControl();
		void ReleaseManagerRecoveryControl();
		void NotifyNewManagerStarted(string newManager);
		void UpdateManagerNode(string newNodeId);
		int InsertAgent(string NodeId, string Agent);
		int GetAgentTypeCount(string agentType);
		Collections.IAgent GetAgents(string NodeName);
		void SendAddAgent(string Destination, string agentType, int entryId);
		void SendAddAgent(string Destination, string Parameters);
		void SendAddAgentConfirm(string From, string Parameters);
		void SendRemoveAgent(string Destination, string Parameters);
		void SendRemoveAgentConfirm(string From, string Parameters);
		void SendReady(string Start);
		void SendReadyConfirm(string Destination, string currentManager);
		void SendStatus(string From, string Parameters);
		void SendStatusConfirm(string Destination);
		void UpdateStatus(string Status, int EntryId, string NodeId);
		void UpdateStatus(string Status, string NodeId);

		List<string> DetectDeadNodes(int maxMinutesAlive);
	}
}
