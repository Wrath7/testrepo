using System;
using System.Data;

namespace Illuminate.Node.Entities
{
	public class AgentLimit : Illuminate.Entities.Entity, Illuminate.Node.Entities.IAgentLimit
	{
		#region Protected Area

		protected string _agentName = string.Empty;
		protected string _minMax = string.Empty;
		protected int _count = 0;

		#endregion

		#region Public Properties

		public string AgentName
		{
			get
			{
				return _agentName;
			}
			set
			{
				_agentName = value;
			}
		}

		public string MinMax
		{
			get
			{
				return _minMax;
			}
			set
			{
				_minMax = value;
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}


		#endregion

		#region Constuctors

		public AgentLimit(string agentName, string minMax, int count, Illuminate.Node.Service GS)
		{
			_agentName = agentName;
			_minMax = minMax;
			_count = count;
			this.GS = GS;

		}

		public AgentLimit()
		{

		}

		#endregion

		#region Bind

		/// <summary>
		/// Binds a datatable to a collection of entities
		/// </summary>
		/// <param name="Dt">The datatable to bind</param>
		/// <param name="GS">Reference to the Illuminate Service</param>
		/// <returns>Collection of Entities</returns>
		public static Collections.IAgentLimit Bind(DataTable Dt, Illuminate.Node.Service GS)
		{
			Collections.IAgentLimit agentLimits = new Collections.AgentLimit();

			for (int i = 0; i < Dt.Rows.Count; i++)
			{
				DataRow Dr = Dt.Rows[i];
				AgentLimit agentLimit = Bind(Dr, GS);

				agentLimits.Add(agentLimit);
			}

			return agentLimits;
		}

		/// <summary>
		/// Binds a datarow to an entity
		/// </summary>
		/// <param name="Dr">The datarow you want to bind</param>
		/// <param name="GS">Reference to the Illuminate Service</param>
		/// <returns>A binded entity</returns>
		public static AgentLimit Bind(DataRow Dr, Illuminate.Node.Service GS)
		{
			string agentName = string.Empty;
			string minMax = string.Empty;
			int count = 0;

			if (Dr.Table.Columns.Contains("AgentName")) agentName = Dr["AgentName"].ToString();
			if (Dr.Table.Columns.Contains("MinMax")) minMax = Dr["MinMax"].ToString();
			if (Dr.Table.Columns.Contains("Count")) count = int.Parse(Dr["Count"].ToString());

			return new AgentLimit(agentName, minMax, count, GS);
		}

		#endregion
	}
}
