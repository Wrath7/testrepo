using System;
using System.Data;

namespace Illuminate.Node.Entities
{
	public class Node : Illuminate.Entities.Entity, Illuminate.Node.Entities.INode
	{
		#region Protected Area

		protected int _entryId = 0;
		protected string _nodeName = string.Empty;
		protected string _status = string.Empty;
		protected DateTime _statusDate = new DateTime();
		
		#endregion

		#region Public Properties

		public int EntryId
		{
			get
			{
				return _entryId;
			}
			set
			{
				_entryId = value;
			}
		}

		public string NodeName
		{
			get
			{
				return _nodeName;
			}
			set
			{
				_nodeName = value;
			}
		}

		public string Status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
			}
		}

		public DateTime StatusDate
		{
			get
			{
				return _statusDate;
			}
			set
			{
				_statusDate = value;
			}
		}


		#endregion

		#region Constuctors

		public Node(int EntryId, string NodeName, string Status, DateTime StatusDate, Illuminate.Node.Service GS)
		{
			_entryId = EntryId;
			_nodeName = NodeName;
			_status = Status;
			_statusDate = StatusDate;
			this.GS = GS;

		}

		public Node()
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
		public static Collections.Node Bind(DataTable Dt, Illuminate.Node.Service GS)
		{
			Collections.Node Nodes = new Collections.Node();

			for (int i = 0; i < Dt.Rows.Count; i++)
			{
				DataRow Dr = Dt.Rows[i];
				Node Node = Bind(Dr, GS);

				Nodes.Add(Node);
			}

			return Nodes;
		}

		/// <summary>
		/// Binds a datarow to an entity
		/// </summary>
		/// <param name="Dr">The datarow you want to bind</param>
		/// <param name="GS">Reference to the Illuminate Service</param>
		/// <returns>A binded entity</returns>
		public static Node Bind(DataRow Dr, Illuminate.Node.Service GS)
		{
			int EntryId = 0;
			string NodeName = string.Empty;
			string Status = string.Empty;
			DateTime StatusDate = new DateTime();

			if (Dr.Table.Columns.Contains("EntryId")) EntryId = int.Parse(Dr["EntryId"].ToString());
			if (Dr.Table.Columns.Contains("NodeName")) NodeName = Dr["NodeName"].ToString();
			if (Dr.Table.Columns.Contains("Status")) Status = Dr["Status"].ToString();
			if (Dr.Table.Columns.Contains("StatusDate")) DateTime.TryParse(Dr["StatusDate"].ToString(), out StatusDate);

			return new Node(EntryId, NodeName, Status, StatusDate, GS);
		}

		#endregion
	}
}
