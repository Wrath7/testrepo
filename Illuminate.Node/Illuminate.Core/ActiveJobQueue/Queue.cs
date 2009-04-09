using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Tools.Data.MySql;
using System.Data;

namespace Illuminate.ActiveJob
{
	public sealed class Queue : Illuminate.ActiveJob.IQueueManager
	{
		private static Queue _queue = new Queue();

		public static Queue Instance
		{
			get
			{
				return _queue;
			}
		}

		private string _compilerQueueConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CompilerQueueConnection"].ConnectionString;

		public Queue()
		{

		}

		/// <summary>
		/// Update a compiler queue job in the database.
		/// </summary>
		/// <param name="jobId">Id of the Job to update</param>
		/// <param name="oldStatus">Current status of the job</param>
		/// <param name="newStatus">New status for the job</param>
		/// <param name="nextRunTime">Next time that this job should be run</param>
		/// <param name="newAgentId">New agent id for this job</param>
		/// <returns>Boolean indicating if a job was updated</returns>
		public bool UpdateAgentId(int jobId, string oldStatus, string newStatus, DateTime nextRunTime, string newAgentId)
		{
			Query q = new Query("update ActiveJobQueue set AgentId = ?AgentId, NextRunTime = ?NextRunTime, Status = ?NewStatus, LastStatusUpdate = ?LastStatusUpdate where JobId = ?JobId and Status = ?OldStatus", "UpdateAgentId", _compilerQueueConnection);
			q.Parameters.Add("?AgentId", newAgentId, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?NextRunTime", nextRunTime, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?NewStatus", newStatus, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?OldStatus", oldStatus, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?LastStatusUpdate", DateTime.Now, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?JobId", jobId, ParameterCollection.FieldType.Numeric);
			int numRowsAffected = q.RunQueryNoResult();

			return numRowsAffected == 1;
		}

		/// <summary>
		/// Gets the compiler queue entry that has a NextRuntime in the past that is not currently running.
		/// </summary>
		/// <returns>ICompilerQueue entity for the next eligible job to run</returns>
		public IQueueEntity GetActiveJobQueueEntryRuntimeNow()
		{
			Query q = new Query("select * from ActiveJobQueue where NextRunTime < ?CurrentDateTime_ and AgentId = '' order by NextRunTime asc limit 1", "GetCompilerQueueEntry", _compilerQueueConnection);
			q.Parameters.Add("?CurrentDateTime_", DateTime.Now, ParameterCollection.FieldType.DateTime);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			return QueueEntity.Bind(Dt.Rows[0]);
		}

		/// <summary>
		/// Gets the compiler queue entry that has a NextRuntime in the past that is not currently running.
		/// </summary>
		/// <returns>ICompilerQueue entity for the next eligible job to run</returns>
		public List<IQueueEntity> GetExpiredActiveJobQueueEntries()
		{
			Query q = new Query("select * from ActiveJobQueue where LastStatusUpdate < ?CurrentDateTime_ and AgentId != '' order by NextRunTime asc limit 1", "GetCompilerQueueEntry", _compilerQueueConnection);
			q.Parameters.Add("?CurrentDateTime_", DateTime.Now.AddHours(-5), ParameterCollection.FieldType.DateTime);
			DataTable Dt = q.RunQuery();

			List<IQueueEntity> expiredQueueEntries = new List<IQueueEntity>();

			foreach (DataRow dr in Dt.Rows)
			{
				expiredQueueEntries.Add(QueueEntity.Bind(dr));
			}

			return expiredQueueEntries;
		}

		/// <summary>
		/// Gets the compiler queue entry that has the specified job id and agent id.
		/// </summary>
		/// <param name="jobId">Id of the Job to get</param>
		/// <param name="agentId">Agent Id of the job to get</param>
		/// <returns>ICompilerQueue entity</returns>
		public IQueueEntity GetActiveJobQueueEntry(int jobId, string agentId)
		{
			Query q = new Query("select * from ActiveJobQueue where JobId = ?JobId and AgentId = ?AgentId", "GetCompilerQueueEntry", _compilerQueueConnection);
			q.Parameters.Add("?JobId", jobId, ParameterCollection.FieldType.Numeric);
			q.Parameters.Add("?AgentId", agentId, ParameterCollection.FieldType.Text);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			return QueueEntity.Bind(Dt.Rows[0]);
		}

		/// <summary>
		/// Gets the compiler queue entry that has the specified job id and does not have an agent id.
		/// </summary>
		/// <param name="jobId">Id of the Job to get</param>
		/// <returns>ICompilerQueue entity</returns>
		public IQueueEntity GetActiveJobQueueEntryWithNoAgentId(int jobId)
		{
			Query q = new Query("select * from ActiveJobQueue where JobId = ?JobId and (AgentId is null or AgentId = '')", "GetCompilerQueueEntry", _compilerQueueConnection);
			q.Parameters.Add("?JobId", jobId, ParameterCollection.FieldType.Numeric);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			return QueueEntity.Bind(Dt.Rows[0]);
		}
	}
}
