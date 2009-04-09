using System;

namespace Illuminate.Node.Managers
{
	public interface IAdmin
	{
		void DeleteAgent(int EntryId);
		void DeleteNode(int EntryId);
		Illuminate.Node.Entities.IAgent GetAgent(int EntryId);
		Illuminate.Node.Collections.IAgent GetAgents(string NodeName);
		Illuminate.Node.Entities.INode GetNode(int EntryId);
		void InsertNode(string NodeName);
		Illuminate.Node.Collections.INode SearchNodeByKeyword(string Keyword);
		Illuminate.Node.Collections.INode SearchNodeByLetter(string Letter);
		void UpdateAgent(int EntryId, string NodeId, string Agent);
		void UpdateNode(int EntryId, string NodeName);

		void InsertSetting(string settingName, string settingValue);
		void DeleteSetting(string settingName);
		void UpdateSetting(string settingName, string settingValue);
		Entities.ISetting GetSetting(string settingName);
		Collections.ISetting GetAllSettings();

		void DeleteNode(string NodeId);
		void DeleteAgent(string NodeId, string AgentId);
		Entities.INode GetNode(string NodeId);

		void InsertTotalAgentLimit(string agentName, string minMax, int count);
		void DeleteTotalAgentLimit(string agentName, string minMax);
		void UpdateTotalAgentLimit(string agentName, string minMax, int count);
		Entities.IAgentLimit GetTotalAgentLimit(string agentName, string minMax);
		Collections.IAgentLimit GetAllTotalAgentLimits();
		void InsertNodeAgentLimit(string agentName, string minMax, int count);
		void DeleteNodeAgentLimit(string agentName, string minMax);
		void UpdateNodeAgentLimit(string agentName, string minMax, int count);
		Entities.IAgentLimit GetNodeAgentLimit(string agentName, string minMax);
		Collections.IAgentLimit GetAllNodeAgentLimits();
	}
}
