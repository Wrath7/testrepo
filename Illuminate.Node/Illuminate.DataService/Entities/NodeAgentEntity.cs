using System;
using System.Data;

namespace Illuminate.Node.Entities
{
	public class Agent : Illuminate.Entities.Entity, Illuminate.Node.Entities.IAgent
	{
		#region Protected Area

		protected int _EntryId = 0;
		protected string _NodeId = "";
		protected string _Agent = "";
		protected DateTime _StatusDate = new DateTime();
		protected string _Status = "";
		protected string _ErrorInformation = "";
		protected string _AgentId = "";
		
		#endregion

		#region Public Properties

		public int EntryId
		{
			get
			{
				return _EntryId;
			}
			set
			{
				_EntryId = value;
			}
		}

		public string NodeId
		{
			get
			{
				return _NodeId;
			}
			set
			{
				_NodeId = value;
			}
		}

		public string AgentName
		{
			get
			{
				return _Agent;
			}
			set
			{
				_Agent = value;
			}
		}

		public DateTime StatusDate
		{
			get
			{
				return _StatusDate;
			}
			set
			{
				_StatusDate = value;
			}
		}

		public string Status
		{
			get
			{
				return _Status;
			}
			set
			{
				_Status = value;
			}
		}

		public string ErrorInformation
		{
			get
			{
				return _ErrorInformation;
			}
			set
			{
				_ErrorInformation = value;
			}
		}

		public string AgentId
		{
			get
			{
				return _AgentId;
			}
			set
			{
				_AgentId = value;
			}
		}


		#endregion

		#region Constuctors

		public Agent(int EntryId, string NodeId, string Agent, DateTime StatusDate, string Status, string ErrorInformation, string AgentId, Illuminate.Node.Service GS)
		{
			_EntryId = EntryId;
			_NodeId = NodeId;
			_Agent = Agent;
			_StatusDate = StatusDate;
			_Status = Status;
			_ErrorInformation = ErrorInformation;
			_AgentId = AgentId;
			this.GS = GS;

		}
		public Agent(string NodeId, string Agent, DateTime StatusDate, string Status, string ErrorInformation, string AgentId, Illuminate.Node.Service GS)
		{
			_NodeId = NodeId;
			_Agent = Agent;
			_StatusDate = StatusDate;
			_Status = Status;
			_ErrorInformation = ErrorInformation;
			_AgentId = AgentId;
			this.GS = GS;

		}

		
		public Agent()
		{

		}

		#endregion

		#region Bind

		public static Collections.Agent Bind(DataTable Dt, Illuminate.Node.Service GS)
		{
			Collections.Agent Agents = new Collections.Agent();

			for (int i = 0; i < Dt.Rows.Count; i++)
			{
				DataRow Dr = Dt.Rows[i];
				Agent Agent = Bind(Dr, GS);

				Agents.Add(Agent);
			}

			return Agents;
		}

		public static Agent Bind(DataRow Dr, Illuminate.Node.Service GS)
		{
			int EntryId = 0;
			string NodeId = "";
			string AgentName = "";
			DateTime StatusDate = new DateTime();
			string Status = "";
			string ErrorInformation = "";
			string AgentId = "";

			if (Dr.Table.Columns.Contains("EntryId")) EntryId = int.Parse(Dr["EntryId"].ToString());
			if (Dr.Table.Columns.Contains("NodeId")) NodeId = Dr["NodeId"].ToString();
			if (Dr.Table.Columns.Contains("Agent")) AgentName = Dr["Agent"].ToString();
			if (Dr.Table.Columns.Contains("StatusDate")) StatusDate = DateTime.Parse(Dr["StatusDate"].ToString());
			if (Dr.Table.Columns.Contains("Status")) Status = Dr["Status"].ToString();
			if (Dr.Table.Columns.Contains("ErrorInformation")) ErrorInformation = Dr["ErrorInformation"].ToString();
			if (Dr.Table.Columns.Contains("AgentId")) AgentId = Dr["AgentId"].ToString();

			Agent Agent = new Agent(EntryId, NodeId, AgentName, StatusDate, Status, ErrorInformation, AgentId, GS);

			return Agent;

		}

		#endregion
		
	}
}
