using System;
using Illuminate.Tools.Data.MySql;
using System.Text;
using System.Data;

namespace Illuminate.Node.Managers
{
	public class Admin : Manager, Illuminate.Node.Managers.IAdmin
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Service">Reference to the Service object</param>
		public Admin(Illuminate.Node.Service Service)
		{
			GS = Service;
		}

		#endregion

		#region Administration Methods

		#region Nodes

		public Collections.INode SearchNodeByLetter(string Letter)
		{
			Query q;
			DataTable Dt;

			if (Letter.Length == 0)
			{
				q = new Query("select * from Node order by nodename asc", "aaa", GS.MonitorConnection);
				Dt = q.RunQuery();
			}
			else
			{
				q = new Query("select * from Node where nodename like ?Letter order by nodename asc", "aaa", GS.MonitorConnection);
				q.Parameters.Add("?Letter", Letter + "%", ParameterCollection.FieldType.Text);
				Dt = q.RunQuery();
			}

			Collections.INode Nodes = Entities.Node.Bind(Dt, GS);

			return Nodes;
		}

		public Collections.INode SearchNodeByKeyword(string Keyword)
		{
			Query q;
			DataTable Dt;

			if (Keyword.Length == 0)
			{
				q = new Query("select * from Node order by nodename asc", "aaa", GS.MonitorConnection);
				Dt = q.RunQuery();
			}
			else
			{
				q = new Query("select * from Node where nodename like ?Keyword order by nodename asc", "aaa", GS.MonitorConnection);
				q.Parameters.Add("?Keyword", "%" + Keyword + "%", ParameterCollection.FieldType.Text);
				Dt = q.RunQuery();
			}

			Collections.INode Nodes = Entities.Node.Bind(Dt, GS);

			return Nodes;
		}

		public void InsertNode(string NodeName)
		{
			Query q = new Query("insert into Node (nodename, status, statusdate) values (?NodeName, ?Status, ?StatudDate)", "aaa", GS.MonitorConnection);
			q.Parameters.Add("?NodeName", NodeName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Status", "NEW", ParameterCollection.FieldType.Text);
			q.Parameters.Add("?StatudDate", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.RunQueryNoResult();
		}

		public void UpdateNode(int EntryId, string NodeName)
		{
			Query q = new Query("update Node set nodename = ?NodeName, status = ?Status, statusdate = ?StatudDate where entryid = ?EntryId", "aaa", GS.MonitorConnection);
			q.Parameters.Add("?NodeName", NodeName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Status", "UPDATE", ParameterCollection.FieldType.Text);
			q.Parameters.Add("?StatudDate", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?EntryId", EntryId, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}

		public Entities.INode GetNode(int EntryId)
		{
			Query q = new Query("select * from Node where entryid = ?EntryId", "aaa", GS.MonitorConnection);
			q.Parameters.Add("?EntryId", EntryId, ParameterCollection.FieldType.Numeric);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			Entities.INode Node = Entities.Node.Bind(Dt.Rows[0], GS);

			return Node;

		}

		public Entities.INode GetNode(string NodeId)
		{
			Query q = new Query("select * from Node where NodeName = ?NodeId", "aaa", GS.MonitorConnection);
			q.Parameters.Add("?NodeId", NodeId, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			Entities.INode Node = Entities.Node.Bind(Dt.Rows[0], GS);

			return Node;

		}

		public void DeleteNode(int EntryId)
		{
			Query q = new Query("delete from Node  where entryid = ?EntryId", "aaa", GS.MonitorConnection);
			q.Parameters.Add("?EntryId", EntryId, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}

		public void DeleteNode(string NodeId)
		{
			Query q = new Query("select * from Node  where NodeName = ?NodeId", "aaa", GS.MonitorConnection);
			q.Parameters.Add("?NodeId", NodeId, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return;
			}

			DeleteNode(int.Parse(Dt.Rows[0]["EntryId"].ToString()));

			q = new Query("delete from NodeAgent where NodeId = ?NodeId", "aaa", GS.MonitorConnection);
			q.Parameters.Add("?NodeId", NodeId, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		#endregion

		#region Agents

		public void DeleteAgent(int EntryId)
		{
			Query q = new Query("delete from NodeAgent where EntryId = ?EntryId", "DeleteNodeAgent", GS.MonitorConnection);
			q.Parameters.Add("?EntryId", EntryId, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}

		public void DeleteAgent(string NodeId, string AgentId)
		{
			Query q = new Query("select * from NodeAgent where NodeId  = ?NodeId and Agent = ?AgentId limit 1", "Get", GS.MonitorConnection);
			q.Parameters.Add("?NodeId", NodeId, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?AgentId", AgentId, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count != 0)
			{
				DeleteAgent(int.Parse(Dt.Rows[0]["EntryId"].ToString()));
			}
		}

		public void UpdateAgent(int EntryId, string NodeId, string Agent)
		{
			Query q = new Query("update NodeAgent set nodeid = ?NodeId_, agent = ?Agent_, statusdate = ?StatusDate_, status = ?Status_ where EntryId = ?EntryId", "UpdateNodeAgent", GS.MonitorConnection);
			q.Parameters.Add("?EntryId", EntryId, ParameterCollection.FieldType.Numeric);
			q.Parameters.Add("?NodeId_", NodeId, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Agent_", Agent, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?StatusDate_", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?Status_", "UPDATED", ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();

		}

		public Entities.IAgent GetAgent(int EntryId)
		{
			Entities.IAgent Agent;
			Query q = new Query("select * from NodeAgent where EntryId = ?EntryId", "GetNodeAgent", GS.MonitorConnection);
			q.Parameters.Add("?EntryId", EntryId, ParameterCollection.FieldType.Numeric);

			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			Agent = Entities.Agent.Bind(Dt.Rows[0], GS);

			return Agent;
		}

		public Collections.IAgent GetAgents(string NodeName)
		{
			Collections.IAgent Agent;
			Query q = new Query("select * from NodeAgent where NodeId = ?NodeName", "GetAllNodeAgent", GS.MonitorConnection);
			q.Parameters.Add("?NodeName", NodeName, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			Agent = Entities.Agent.Bind(Dt, GS);

			return Agent;
		}

		#endregion

		#region Settings

		public void InsertSetting(string settingName, string settingValue)
		{
			Query q = new Query("insert into Settings (SettingName, SettingValue) values (?SettingName, ?SettingValue)", "InsertSetting", GS.MonitorConnection);
			q.Parameters.Add("?SettingName", settingName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?SettingValue", settingValue, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public void DeleteSetting(string settingName)
		{
			Query q = new Query("delete from Settings where SettingName = ?SettingName", "DeleteSetting", GS.MonitorConnection);
			q.Parameters.Add("?SettingName", settingName, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public void UpdateSetting(string settingName, string settingValue)
		{
			Query q = new Query("update Settings set SettingValue = ?SettingValue where SettingName = ?SettingName", "UpdateSetting", GS.MonitorConnection);
			q.Parameters.Add("?SettingName", settingName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?SettingValue", settingValue, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public Entities.ISetting GetSetting(string settingName)
		{
			Query q = new Query("select * from Settings where SettingName = ?SettingName", "GetSetting", GS.MonitorConnection);
			q.Parameters.Add("?SettingName", settingName, ParameterCollection.FieldType.Text);

			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			return Entities.Setting.Bind(Dt.Rows[0], GS);
		}

		public Collections.ISetting GetAllSettings()
		{
			Query q = new Query("select * from Settings", "GetAllSettings", GS.MonitorConnection);
			DataTable Dt = q.RunQuery();

			return Entities.Setting.Bind(Dt, GS);
		}

		#endregion

		#region Total Agent Limits

		public void InsertTotalAgentLimit(string agentName, string minMax, int count)
		{
			Query q = new Query("insert into TotalAgentLimits (AgentName, MinMax, `Count`) values (?AgentName, ?MinMax, ?Count)", "InsertTotalAgentLimit", GS.MonitorConnection);
			q.Parameters.Add("?AgentName", agentName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Count", count, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}

		public void DeleteTotalAgentLimit(string agentName, string minMax)
		{
			Query q = new Query("delete from TotalAgentLimits where AgentName = ?AgentName and MinMax = ?MinMax", "DeleteTotalAgentLimit", GS.MonitorConnection);
			q.Parameters.Add("?AgentName", agentName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public void UpdateTotalAgentLimit(string agentName, string minMax, int count)
		{
			Query q = new Query("update TotalAgentLimits set `Count` = ?Count where AgentName = ?AgentName and MinMax = ?MinMax", "UpdateTotalAgentLimit", GS.MonitorConnection);
			q.Parameters.Add("?AgentName", agentName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Count", count, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}

		public Entities.IAgentLimit GetTotalAgentLimit(string agentName, string minMax)
		{
			Query q = new Query("select * from TotalAgentLimits where AgentName = ?AgentName and MinMax = ?MinMax", "GetTotalAgentLimit", GS.MonitorConnection);
			q.Parameters.Add("?AgentName", agentName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);

			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			return Entities.AgentLimit.Bind(Dt.Rows[0], GS);
		}

		public Collections.IAgentLimit GetAllTotalAgentLimits()
		{
			Query q = new Query("select * from TotalAgentLimits", "GetAllTotalAgentLimits", GS.MonitorConnection);
			DataTable Dt = q.RunQuery();

			return Entities.AgentLimit.Bind(Dt, GS);
		}

		#endregion

		#region Node Agent Limits

		public void InsertNodeAgentLimit(string agentName, string minMax, int count)
		{
			Query q = new Query("insert into NodeAgentLimits (AgentName, MinMax, `Count`) values (?AgentName, ?MinMax, ?Count)", "InsertNodeAgentLimit", GS.MonitorConnection);
			q.Parameters.Add("?AgentName", agentName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Count", count, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}

		public void DeleteNodeAgentLimit(string agentName, string minMax)
		{
			Query q = new Query("delete from NodeAgentLimits where AgentName = ?AgentName and MinMax = ?MinMax", "DeleteNodeAgentLimit", GS.MonitorConnection);
			q.Parameters.Add("?AgentName", agentName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public void UpdateNodeAgentLimit(string agentName, string minMax, int count)
		{
			Query q = new Query("update NodeAgentLimits set `Count` = ?Count where AgentName = ?AgentName and MinMax = ?MinMax", "UpdateNodeAgentLimit", GS.MonitorConnection);
			q.Parameters.Add("?AgentName", agentName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?Count", count, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}

		public Entities.IAgentLimit GetNodeAgentLimit(string agentName, string minMax)
		{
			Query q = new Query("select * from NodeAgentLimits where AgentName = ?AgentName and MinMax = ?MinMax", "GetNodeAgentLimit", GS.MonitorConnection);
			q.Parameters.Add("?AgentName", agentName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MinMax", minMax, ParameterCollection.FieldType.Text);

			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			return Entities.AgentLimit.Bind(Dt.Rows[0], GS);
		}

		public Collections.IAgentLimit GetAllNodeAgentLimits()
		{
			Query q = new Query("select * from NodeAgentLimits", "GetAllNodeAgentLimits", GS.MonitorConnection);
			DataTable Dt = q.RunQuery();

			return Entities.AgentLimit.Bind(Dt, GS);
		}

		#endregion

		#endregion
	}
}
