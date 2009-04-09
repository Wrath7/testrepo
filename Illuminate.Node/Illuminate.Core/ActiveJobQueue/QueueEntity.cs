using System;
using System.Data;
using System.Collections.Generic;

namespace Illuminate.ActiveJob
{
	public class QueueEntity : Illuminate.ActiveJob.IQueueEntity
	{
		#region Protected Area

		private int _jobId = 0;
		private string _name;
		private string _dll;
		private DateTime _nextRunTime = Illuminate.Tools.Tools.GetDefaultDate();
		private string _agentId;
		private string _status;
		private DateTime _lastStatusUpdate = Illuminate.Tools.Tools.GetDefaultDate();
		private int _interval = 0;
		private List<string> _parameters = new List<string>();

		#endregion

		#region Public Properties

		public int JobId
		{
		  get { return _jobId; }
		  set { _jobId = value; }
		}

		public string Name
		{
		  get { return _name; }
		  set { _name = value; }
		}

		public string DLL
		{
		  get { return _dll; }
		  set { _dll = value; }
		}

		public DateTime NextRunTime
		{
		  get { return _nextRunTime; }
		  set { _nextRunTime = value; }
		}

		public string AgentId
		{
		  get { return _agentId; }
		  set { _agentId = value; }
		}

		public string Status
		{
		  get { return _status; }
		  set { _status = value; }
		}

		public DateTime LastStatusUpdate
		{
			get { return _lastStatusUpdate; }
			set { _lastStatusUpdate = value; }
		}

		public int Interval
		{
			get { return _interval; }
			set { _interval = value; }
		}

		public List<string> Parameters
		{
			get { return _parameters; }
		}

		#endregion

		#region Constuctors

		public QueueEntity(int jobId, string name, string dll, DateTime nextRunTime, string agentId, string status, DateTime lastStatusUpdate, int interval, string parameters)
		{
			_jobId = jobId;
			_name = name;
			_dll = dll;
			_nextRunTime = nextRunTime;
			_agentId = agentId;
			_status = status;
			_lastStatusUpdate = lastStatusUpdate;
			_interval = interval;

			_parameters = new List<string>(parameters.Split(','));
		}

		public QueueEntity()
		{

		}

		#endregion

		#region Bind

		/// <summary>
		/// Binds a datarow to an entity
		/// </summary>
		/// <param name="Dr">The datarow you want to bind</param>
		/// <param name="GS">Reference to the Gazaro Service</param>
		/// <returns>A binded entity</returns>
		public static QueueEntity Bind(DataRow Dr)
		{
			int jobId = 0;
			string name = string.Empty;
			string dll = string.Empty;
			DateTime nextRunTime = Illuminate.Tools.Tools.GetDefaultDate();
			string agentId = string.Empty;
			string status = string.Empty;
			DateTime lastStatusUpdate = Illuminate.Tools.Tools.GetDefaultDate();
			int interval = 0;
			string parameters = string.Empty;

			if (Dr.Table.Columns.Contains("JobId")) jobId = int.Parse(Dr["JobId"].ToString());
			if (Dr.Table.Columns.Contains("Name")) name = Dr["Name"].ToString();
			if (Dr.Table.Columns.Contains("DLL")) dll = Dr["DLL"].ToString();
			if (Dr.Table.Columns.Contains("NextRunTime")) nextRunTime = DateTime.Parse(Dr["NextRunTime"].ToString());
			if (Dr.Table.Columns.Contains("AgentId")) agentId = Dr["AgentId"].ToString();
			if (Dr.Table.Columns.Contains("Status")) status = Dr["Status"].ToString();
			if (Dr.Table.Columns.Contains("LastStatusUpdate")) lastStatusUpdate = DateTime.Parse(Dr["LastStatusUpdate"].ToString());
			if (Dr.Table.Columns.Contains("interval")) interval = int.Parse(Dr["interval"].ToString());
			if (Dr.Table.Columns.Contains("parameters")) parameters = Dr["parameters"].ToString();

			return new QueueEntity(jobId, name, dll, nextRunTime, agentId, status, lastStatusUpdate, interval, parameters);
		}

		#endregion
	}
}
