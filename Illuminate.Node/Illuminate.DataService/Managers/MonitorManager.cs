using System;
using Illuminate.Tools;
using Illuminate.Tools.Data.MySql;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace Illuminate.Node.Managers
{
	public class Monitor : Manager, INodeManagerMonitor
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Service">Reference to the Service object</param>
		public Monitor(Illuminate.Node.Service service)
		{
			GS = service;
		}

		#endregion

		#region Communication Methods

		private void SendNodeCommand(string start, DateTime statusDate, string nodeCommand, string destination, string parameters, string queryName)
		{
			Query q = new Query("insert into NodeCommand (Start, StatusDate, NodeCommand, Destination, Parameters) values (?Start, ?StatusDate, ?NodeCommand, ?Destination, ?Parameters)", queryName, GS.MonitorConnection);
			q.Parameters.Add("?Start", start, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?StatusDate", statusDate, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?NodeCommand", nodeCommand, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Destination", destination, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Parameters", parameters, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public void SendReady(string Start)
		{
			Logger.WriteLine("Inserting Ready...", Logger.Severity.Debug);
			SendNodeCommand(Start, DateTime.Now, "READY", "Manager", "", "SendReady");
			Logger.WriteLine("Ready Inserted...", Logger.Severity.Debug);
		}

		public void SendReadyConfirm(string Destination, string currentManager)
		{
			SendNodeCommand("Manager", DateTime.Now, "READYCONFIRM", Destination, currentManager, "SendReadyConfirm");
		}

		public string GetCommand(string Destination)
		{
			Query q = new Query("select * from NodeCommand where destination = ?Destination", "SeleteGetCommand", GS.MonitorConnection);
			q.Parameters.Add("?Destination", Destination, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return "";
			}
			else
			{
				q = new Query("delete from NodeCommand where destination = ?Destination", "DeleteGetCommand", GS.MonitorConnection);
				q.Parameters.Add("?Destination", Destination, ParameterCollection.FieldType.Text);
				q.RunQueryNoResult();

				StringBuilder Sb = new StringBuilder();

				foreach (DataRow Dr in Dt.Rows)
				{
					Sb.Append(Dr["nodecommand"].ToString() + " " + Dr["Start"].ToString() + " " + Dr["Destination"].ToString() + " " + Dr["parameters"].ToString() + "\n");
				}

				return Sb.ToString();
			}
		}

		public void DeleteCommandsForNode(string nodeId)
		{
			Query q = new Query("delete from NodeCommand where Start = ?NodeId or Destination = ?NodeId", "DeleteCommandsForNode", GS.MonitorConnection);
			q.Parameters.Add("?NodeId", nodeId, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public int GetAgentTypeCount(string agentType)
		{
			Query q = new Query("select count(*) from NodeAgent, Node where Node.NodeName = NodeAgent.NodeId AND Node.Status = 'ALIVE' AND Agent = ?Agent", "GetExtractorCount", GS.MonitorConnection);
			q.Parameters.Add("?Agent", agentType, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return -1;
			}

			return int.Parse(Dt.Rows[0][0].ToString());
		}

		public Collections.IAgent GetAgents(string NodeName)
		{
			Query q = new Query("select * from NodeAgent where NodeId = ?NodeName", "GetAllNodeAgent", GS.MonitorConnection);
			q.Parameters.Add("?NodeName", NodeName, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			return Entities.Agent.Bind(Dt, GS);
		}

		public void SendAddAgent(string Destination, string agentType, int entryId)
		{
			SendAddAgent(Destination, agentType + " " + entryId);
		}

		public void SendAddAgent(string Destination, string Parameters)
		{
			SendNodeCommand("Manager", DateTime.Now, "ADD", Destination, Parameters, "SendAddAgent");
		}

		public void SendRemoveAgent(string Destination, string Parameters)
		{
			SendNodeCommand("Manager", DateTime.Now, "REMOVE", Destination, Parameters, "SendAddAgent");
		}

		public void ActivateAgents(string NodeId)
		{
			Query q = new Query("select * from NodeAgent where nodeid = ?NodeId", "ActivateAgents", GS.MonitorConnection);
			q.Parameters.Add("?NodeId", NodeId, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			foreach (DataRow Dr in Dt.Rows)
			{
				SendAddAgent(NodeId, Dr["Agent"].ToString() + " " + Dr["entryid"].ToString());
			}
		}

		public void SendAddAgentConfirm(string From, string Parameters)
		{
			SendNodeCommand(From, DateTime.Now, "ADDED", "Manager", Parameters, "SendAddAgentConfirm");
		}

		public void SendRemoveAgentConfirm(string From, string Parameters)
		{
			SendNodeCommand(From, DateTime.Now, "REMOVED", "Manager", Parameters, "SendAddAgentConfirm");
		}

		public string GetSetting(Settings setting)
		{
			return GetSetting(setting.ToString());
		}

		//Overload to allow applications running on Illuminate to access their own settings.
		//Core Illuminate functions should use the version that takes the Settings enumeration.
		public string GetSetting(string setting)
		{
			Query q = new Query("select SettingValue from Settings where SettingName = ?SettingName", "GetSetting", GS.MonitorConnection);
			q.Parameters.Add("?SettingName", setting, ParameterCollection.FieldType.Text);
			DataTable dt = q.RunQuery();

			if (dt.Rows.Count == 0)
			{
				return null;
			}

			return (string)dt.Rows[0][0];
		}

		public Dictionary<string, int> GetTotalAgentLimits(string minMax)
		{
			Query q = new Query("select AgentName, Count from TotalAgentLimits where MinMax = ?MinMax", "GetTotalAgentLimits", GS.MonitorConnection);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);
			DataTable dt = q.RunQuery();

			Dictionary<string, int> agentLimits = new Dictionary<string, int>();
			foreach (DataRow dr in dt.Rows)
			{
				agentLimits.Add(dr[0].ToString(), int.Parse(dr[1].ToString()));
			}

			return agentLimits;
		}

		public Dictionary<string, int> GetNodeAgentLimits(string minMax)
		{
			Query q = new Query("select AgentName, Count from NodeAgentLimits where MinMax = ?MinMax", "GetNodeAgentLimits", GS.MonitorConnection);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);
			DataTable dt = q.RunQuery();

			Dictionary<string, int> agentLimits = new Dictionary<string, int>();
			foreach (DataRow dr in dt.Rows)
			{
				agentLimits.Add(dr[0].ToString(), int.Parse(dr[1].ToString()));
			}

			return agentLimits;
		}

		public bool GetManagerRecoveryControl()
		{
			//attempt to get a user lock on the string ManagerRecoveryControl, 10 second timeout
			Query q = new Query("select GET_LOCK('ManagerRecoveryControl', 10)", "GetManagerRecoveryControl", GS.MonitorConnection);
			DataTable dt = q.RunQuery();

			if (dt.Rows.Count == 0)
			{
				return false;
			}

			return (int.Parse(dt.Rows[0][0].ToString()) == 1);
		}

		public void ReleaseManagerRecoveryControl()
		{
			Query q = new Query("select RELEASE_LOCK('ManagerRecoveryControl')", "ReleaseManagerRecoveryControl", GS.MonitorConnection);
			q.RunQueryNoResult();
		}

		public void NotifyNewManagerStarted(string newManager)
		{
			Query q = new Query("insert into NodeCommand (Start, StatusDate, NodeCommand, Destination, Parameters) select 'Manager', ?StatusDate, ?NodeCommand, NodeName, ?Parameters from Node where Status = 'ALIVE'", "NotifyNewManagerStarted", GS.MonitorConnection);
			q.Parameters.Add("?StatusDate", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?NodeCommand", "NEWMANAGER", ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Parameters", newManager, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		/// <summary>
		/// Gets the NodeID for the next manager node if the current manager stops responding.
		/// The current manager's status should already be updated in the database as not ALIVE.
		/// </summary>
		/// <returns>NodeID of the next manager node</returns>
		public string GetNextManagerNode()
		{
			Query q = new Query("select NodeName from Node where status = 'ALIVE' having min(EntryId);", "GetNextManagerNode", GS.MonitorConnection);
			DataTable dt = q.RunQuery();

			if (dt.Rows.Count == 0)
			{
				return null;
			}

			return (string)dt.Rows[0][0];
		}

		public void UpdateManagerNode(string newNodeId)
		{
			Query q = new Query("update Settings set SettingValue = ?NodeId where SettingName = 'Manager'", "UpdateManagerNode", GS.MonitorConnection);
			q.Parameters.Add("?NodeId", newNodeId, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public void SendStatus(string From, string Parameters)
		{
			SendNodeCommand(From, DateTime.Now, "ALIVE", "Manager", Parameters, "SendStatus");
		}

		public void SendStatusConfirm(string Destination)
		{
			SendNodeCommand("Manager", DateTime.Now, "ALIVECONFIRM", Destination, "", "SendStatusConfirm");
		}

		public void UpdateStatus(string Status, int EntryId, string NodeId)
		{
			Query q = new Query("update NodeAgent set status = ?Status, statusdate = ?Date where EntryId = ?EntryId", "UpdateNodeAgentStatus", GS.MonitorConnection);
			q.Parameters.Add("?Status", Status, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?EntryId", EntryId, ParameterCollection.FieldType.Numeric);
			q.Parameters.Add("?Date", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.RunQueryNoResult();

			//update the status of the node as well
			UpdateStatus(Status, NodeId);
		}

		public void UpdateStatus(string Status, string NodeId)
		{
			Query q = new Query("update Node set status = ?Status, statusdate = ?Date where nodename = ?NodeName", "UpdateNodeStatus", GS.MonitorConnection);
			q.Parameters.Add("?Status", Status, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?NodeName", NodeId, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Date", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.RunQueryNoResult();
		}

		public int AddCrawler(string NodeId)
		{
			int entryId = InsertAgent(NodeId, "Gazaro.Services.Crawler.dll");
			return entryId;
		}

		public int AddExtractor(string NodeId)
		{
			int entryId = InsertAgent(NodeId, "Gazaro.Services.Extraction.dll");
			return entryId;
		}

		public int AddDataCompiler(string NodeId)
		{
			int entryId = InsertAgent(NodeId, "Gazaro.Services.DataCompiler.dll");
			return entryId;
		}

        public int AddAffiliateLinkGenerator(string NodeId)
		{
            int entryId = InsertAgent(NodeId, "Gazaro.Services.AffiliateLinkGenerator.dll");
			return entryId;
		}

        public int AddEmailDiscovery(string NodeId)
        {
            int entryId = InsertAgent(NodeId, "Gazaro.Services.EmailDiscovery.dll");
			return entryId;
        }

        public int AddEmailPublisher(string NodeId)
        {
            int entryId = InsertAgent(NodeId, "Gazaro.Services.EmailPublisher.dll");
			return entryId;
        }

		public int InsertAgent(string NodeId, string Agent)
		{
			Query q = new Query("insert into NodeAgent (nodeid, agent, statusdate, status, errorinformation, agentid) values (?NodeId_, ?Agent_, ?StatusDate_, ?Status_, ?ErrorInformation_, ?AgentId_)", "InsertAgent", GS.MonitorConnection);
			q.Parameters.Add("?NodeId_", NodeId, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Agent_", Agent, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?StatusDate_", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?Status_", "NEW", ParameterCollection.FieldType.Text);
			q.Parameters.Add("?ErrorInformation_", "", ParameterCollection.FieldType.Text);
			q.Parameters.Add("?AgentId_", 0, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();

			q = new Query("select max(entryid) from NodeAgent", "InserAgentGetMax", GS.MonitorConnection);
			return int.Parse(q.RunQuery().Rows[0][0].ToString());
		}

		public List<string> DetectDeadNodes(int maxMinutesAlive)
		{
			List<string> deadNodes = new List<string>();

			Query q = new Query("select NodeName from Node where Status = 'ALIVE' and DATE_ADD(StatusDate, INTERVAL " + maxMinutesAlive + " MINUTE) < SYSDATE()", "GetDeadNodes", GS.MonitorConnection);
			DataTable dt = q.RunQuery();
			if (dt.Rows.Count == 0)
			{
				return deadNodes;
			}

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string nodeName = dt.Rows[i][0].ToString();
				deadNodes.Add(nodeName);

				q = new Query("update Node set Status = 'DEAD' where NodeName = ?NodeName", "UpdateStatusForDeadNodes", GS.MonitorConnection);
				q.Parameters.Add("?NodeName", nodeName, ParameterCollection.FieldType.Text);
				q.RunQueryNoResult();
			}
			return deadNodes;
		}

		#endregion
	}
}
