using System;
using System.Data;

namespace Illuminate.Node.Entities
{
	public class SystemLoad : Illuminate.Entities.Entity, Illuminate.Node.Entities.ISystemLoad
	{
		#region Protected Area

		protected string _nodeName;
		protected DateTime _lastUpdated;
		protected double _load1Min;
		protected double _load5Min;
		protected double _load15Min;
		protected long _freeMemory;
		protected int _processorCount;

		#endregion

		#region Public Properties

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

		public DateTime LastUpdated
		{
			get
			{
				return _lastUpdated;
			}
		}

		public double Load1Min
		{
			get
			{
				return _load1Min;
			}
			set
			{
				_load1Min = value;
			}
		}

		public double Load5Min
		{
			get
			{
				return _load5Min;
			}
			set
			{
				_load5Min = value;
			}
		}

		public double Load15Min
		{
			get
			{
				return _load15Min;
			}
			set
			{
				_load15Min = value;
			}
		}

		public long FreeMemory
		{
			get
			{
				return _freeMemory;
			}
			set
			{
				_freeMemory = value;
			}
		}

		public int ProcessorCount
		{
			get
			{
				return _processorCount;
			}
			set
			{
				_processorCount = value;
			}
		}


		#endregion

		#region Constuctors

		public SystemLoad(string nodeName, DateTime lastUpdated, double load1Min, double load5Min, double load15Min, long freeMemory, int processorCount, Illuminate.Node.Service GS)
		{
			_nodeName = nodeName;
			_lastUpdated = lastUpdated;
			_load1Min = load1Min;
			_load5Min = load5Min;
			_load15Min = load15Min;
			_freeMemory = freeMemory;
			_processorCount = processorCount;
			this.GS = GS;
		}

		public SystemLoad()
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
		public static Collections.SystemLoad Bind(DataTable Dt, Illuminate.Node.Service GS)
		{
			Collections.SystemLoad systemLoads = new Collections.SystemLoad();

			for (int i = 0; i < Dt.Rows.Count; i++)
			{
				DataRow Dr = Dt.Rows[i];
				SystemLoad systemLoad = Bind(Dr, GS);

				systemLoads.Add(systemLoad);
			}

			return systemLoads;
		}

		/// <summary>
		/// Binds a datarow to an entity
		/// </summary>
		/// <param name="Dr">The datarow you want to bind</param>
		/// <param name="GS">Reference to the Illuminate Service</param>
		/// <returns>A binded entity</returns>
		public static SystemLoad Bind(DataRow Dr, Illuminate.Node.Service GS)
		{
			string nodeName = string.Empty;
			DateTime lastUpdated = DateTime.MinValue;
			double load1Min = 0;
			double load5Min = 0;
			double load15Min = 0;
			long freeMemory = 0;
			int processorCount = 0;

			if (Dr.Table.Columns.Contains("NodeName")) nodeName = Dr["NodeName"].ToString();
			if (Dr.Table.Columns.Contains("LastUpdated")) DateTime.TryParse(Dr["LastUpdated"].ToString(), out lastUpdated);
			if (Dr.Table.Columns.Contains("Load1Min")) double.TryParse(Dr["Load1Min"].ToString(), out load1Min);
			if (Dr.Table.Columns.Contains("Load5Min")) double.TryParse(Dr["Load5Min"].ToString(), out load5Min);
			if (Dr.Table.Columns.Contains("Load15Min")) double.TryParse(Dr["Load15Min"].ToString(), out load15Min);
			if (Dr.Table.Columns.Contains("FreeMemory")) long.TryParse(Dr["FreeMemory"].ToString(), out freeMemory);
			if (Dr.Table.Columns.Contains("ProcessorCount")) int.TryParse(Dr["ProcessorCount"].ToString(), out processorCount);

			return new SystemLoad(nodeName, lastUpdated, load1Min, load5Min, load15Min, freeMemory, processorCount, GS);
		}

		#endregion
	}
}
