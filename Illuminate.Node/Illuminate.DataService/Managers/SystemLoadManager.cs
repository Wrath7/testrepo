using System;
using System.Data;
using System.Collections.Generic;
using Illuminate.Tools.Data.MySql;

namespace Illuminate.Node.Managers
{
	public class SystemLoadManager : Manager, ISystemLoadManager
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Service">Reference to the Service object</param>
		public SystemLoadManager(Illuminate.Node.Service service)
		{
			GS = service;
		}

		#endregion

		#region System Load

		public Collections.ISystemLoad GetSystemLoadAliveNodes()
		{
			Query q = new Query("select SystemLoad.* from SystemLoad, Node where SystemLoad.NodeName = Node.NodeName and Node.Status = 'ALIVE'", "GetSystemLoadAliveNodes", GS.MonitorConnection);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			return Entities.SystemLoad.Bind(Dt, GS);
		}

		public Entities.ISystemLoad GetSystemLoad(string nodeName)
		{
			Query q = new Query("select * from SystemLoad where NodeName = ?NodeName", "GetSystemLoad", GS.MonitorConnection);
			q.Parameters.Add("?NodeName", nodeName, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			return Entities.SystemLoad.Bind(Dt.Rows[0], GS);
		}

		public void UpdateSystemLoad(string nodeName, double load1Min, double load5Min, double load15Min, long freeMemory, int processorCount)
		{
			Query q = new Query("update SystemLoad set LastUpdated = ?LastUpdated, Load1Min = ?Load1Min, Load5Min = ?Load5Min, Load15Min = ?Load15Min, FreeMemory = ?FreeMemory, ProcessorCount = ?ProcessorCount where NodeName = ?NodeName", "UpdateSystemLoad", GS.MonitorConnection);
			q.Parameters.Add("?NodeName", nodeName, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?LastUpdated", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?Load1Min", load1Min, ParameterCollection.FieldType.Numeric);
			q.Parameters.Add("?Load5Min", load5Min, ParameterCollection.FieldType.Numeric);
			q.Parameters.Add("?Load15Min", load15Min, ParameterCollection.FieldType.Numeric);
			q.Parameters.Add("?FreeMemory", freeMemory, ParameterCollection.FieldType.Numeric);
			q.Parameters.Add("?ProcessorCount", processorCount, ParameterCollection.FieldType.Numeric);
			int numRowsAffected = q.RunQueryNoResult();

			//the row may not exist in the table causing the update to fail, try to insert instead
			if (numRowsAffected == 0)
			{
				q = new Query("insert into SystemLoad (NodeName, LastUpdated, Load1Min, Load5Min, Load15Min, FreeMemory, ProcessorCount) values (?NodeName, ?LastUpdated, ?Load1Min, ?Load5Min, ?Load15Min, ?FreeMemory, ?ProcessorCount)", "InsertSystemLoad", GS.MonitorConnection);
				q.Parameters.Add("?NodeName", nodeName, ParameterCollection.FieldType.Text);
				q.Parameters.Add("?LastUpdated", DateTime.Now, ParameterCollection.FieldType.DateTime);
				q.Parameters.Add("?Load1Min", load1Min, ParameterCollection.FieldType.Numeric);
				q.Parameters.Add("?Load5Min", load5Min, ParameterCollection.FieldType.Numeric);
				q.Parameters.Add("?Load15Min", load15Min, ParameterCollection.FieldType.Numeric);
				q.Parameters.Add("?FreeMemory", freeMemory, ParameterCollection.FieldType.Numeric);
				q.Parameters.Add("?ProcessorCount", processorCount, ParameterCollection.FieldType.Numeric);
				q.RunQueryNoResult();
			}
		}

		#endregion

		#region Queue Levels
		//TODO: Gazaro specific implementation, need to generalize

		public DateTime GetUrlQueueMinProcessDate()
		{
			//TODO: fix connection string
			Query q = new Query("select min(ProcessDate) from UrlQueue", "GetUrlQueueMinProcessDate", "server=192.168.1.225;database=GazaroAdamB;uid=root;charset=utf8;pwd=1q2w3e4r!;");
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return DateTime.Now;
			}

			string minProcessDate = Dt.Rows[0][0].ToString();
			if (string.IsNullOrEmpty(minProcessDate))
			{
				return DateTime.Now;
			}
			return DateTime.Parse(minProcessDate);
		}

		public int GetHtmlQueueNumEntries()
		{
			//TODO: fix connection string
			Query q = new Query("select count(*) from HtmlQueue", "GetHtmlQueueNumEntries", "server=192.168.1.225;database=GazaroAdamB;uid=root;charset=utf8;pwd=1q2w3e4r!;");
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return 0;
			}

			string htmlQueueCount = Dt.Rows[0][0].ToString();
			if (string.IsNullOrEmpty(htmlQueueCount))
			{
				return 0;
			}
			return int.Parse(htmlQueueCount);
		}

		public int GetEmailQueueNumEntries()
		{
			//TODO: fix connection string
			Query q = new Query("select count(*) from EmailQueue", "GetEmailQueueNumEntries", "server=192.168.1.225;database=GazaroAdamB;uid=root;charset=utf8;pwd=1q2w3e4r!;");
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return 0;
			}

			string emailQueueCount = Dt.Rows[0][0].ToString();
			if (string.IsNullOrEmpty(emailQueueCount))
			{
				return 0;
			}
			return int.Parse(emailQueueCount);
		}

		#endregion

		#region Platform-dependent Operations

		public virtual bool Implemented
		{
			get { return false; }
		}

		/// <summary>
		/// Returns the amount of free memory on the node in kilobytes.
		/// </summary>
		/// <returns>Amount of free memory in kilobytes</returns>
		public virtual long FreeMemory()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the load averages for the node, corrected for the number of processors in the machine.
		/// </summary>
		/// <param name="oneMin">One minute load average</param>
		/// <param name="fiveMin">Five minute load average</param>
		/// <param name="fifteenMin">Fifteen minute load average</param>
		public virtual void LoadAverages(out double oneMin, out double fiveMin, out double fifteenMin)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the number of processors in the machine.  For the purposes of this number, a processor core is considered a processor.
		/// </summary>
		/// <returns>Number of processors</returns>
		public virtual int NumProcessors()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
