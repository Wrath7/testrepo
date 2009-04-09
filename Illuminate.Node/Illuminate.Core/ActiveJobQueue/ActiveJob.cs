using System;
using System.Collections.Generic;
using Illuminate.Interfaces;
using Illuminate.Tools;
using Illuminate.ActiveJob;

namespace Illuminate.ActiveJob
{
	public class ActiveJob : Illuminate.ActiveJob.IActiveJob
	{
		private string LOGNAME = "";

		public void ExecuteJob(string logName, Illuminate.Contexts.AgentContext context)
		{
			LOGNAME = logName;

			Logger.WriteLine("QueueRunner is executing", Logger.Severity.Debug, LOGNAME);

			IQueueEntity queueEntry = Illuminate.ActiveJob.Queue.Instance.GetActiveJobQueueEntryRuntimeNow();

			if (queueEntry != null)
			{
				Illuminate.ActiveJob.Queue.Instance.UpdateAgentId(queueEntry.JobId, queueEntry.Status, "Running", queueEntry.NextRunTime, context.AgentId);

				//get it again to make sure we are the agent registered in the databases
				queueEntry = Illuminate.ActiveJob.Queue.Instance.GetActiveJobQueueEntry(queueEntry.JobId, context.AgentId);
				if (queueEntry != null)
				{
					//run it
					Logger.WriteLine("Invoking agent...: " + queueEntry.Name, Logger.Severity.Information, LOGNAME);

					IAgent job = InvokeJob(queueEntry.DLL);

					Illuminate.Contexts.AgentContext jobContext = new Illuminate.Contexts.AgentContext(context.NodeId, context.SectionName, LOGNAME, context.Communicator, context.Agent);
					jobContext.Parameters = queueEntry.Parameters;
                    jobContext.LogName = LOGNAME;

					if (job != null)
					{
						DateTime startTime = DateTime.Now;

						try
						{
							Logger.WriteLine("Starting Job: " + jobContext.AgentId, Logger.Severity.Debug, LOGNAME);

							Logger.WriteLine("Initialize Job: " + jobContext.AgentId, Logger.Severity.Debug, LOGNAME);
							job.InitializeAgent(jobContext);

							Logger.WriteLine("Executing Job: " + jobContext.AgentId, Logger.Severity.Debug, LOGNAME);
							job.Run();

							Logger.WriteLine("Cleanup Job: " + jobContext.AgentId, Logger.Severity.Debug, LOGNAME);
							job.Cleanup();

						}
						catch (Illuminate.Exceptions.CriticalException ce)
						{
							string body = "There was a critical error running job: " + jobContext.AgentId + ". " + ce.Message + " - " + ce.StackTrace.ToString();

							//Illuminate.EmailQueue.Queue.Instance.Push(DateTime.Now, "dominic@dplouffe.ca", "yourflyer@gazaro.com", "Critical Error on Active Job", body);

							Logger.WriteLine(body, Logger.Severity.Fatal, LOGNAME);
						}
						catch (Illuminate.Exceptions.ErrorException ee)
						{
							string body = "There was a serious error running job: " + jobContext.AgentId + ". " + ee.Message + " - " + ee.StackTrace.ToString();

							//Illuminate.EmailQueue.Queue.Instance.Push(DateTime.Now, "dominic@dplouffe.ca", "yourflyer@gazaro.com", "Serious Error on Active Job", body);

							Logger.WriteLine(body, Logger.Severity.Error, LOGNAME);

						}
						catch (Exception e)
						{
							string body = "There was a unknown error running job: " + jobContext.AgentId + ". " + e.Message + " - " + e.StackTrace.ToString();

							//Illuminate.EmailQueue.Queue.Instance.Push(DateTime.Now, "dominic@dplouffe.ca", "yourflyer@gazaro.com", "Serious Error on Active Job", body);

							Logger.WriteLine(body, Logger.Severity.Error, LOGNAME);
						}
						finally
						{
							DateTime nextRunTime = startTime.AddSeconds(queueEntry.Interval);

							if (nextRunTime < DateTime.Now) nextRunTime = DateTime.Now;

							Logger.WriteLine("Resetting next execution time on Job: " + jobContext.AgentId + "Date/Time: " + nextRunTime.ToString(), Logger.Severity.Information, LOGNAME);

							//process done
							Illuminate.ActiveJob.Queue.Instance.UpdateAgentId(queueEntry.JobId, "Running", "Idle", nextRunTime, null);
						}
					} //End job != null
					else
					{
						Logger.WriteLine("Invoked Agent is NULL...", Logger.Severity.Debug, LOGNAME);

						//process error
						Illuminate.ActiveJob.Queue.Instance.UpdateAgentId(queueEntry.JobId, "Running", "Idle", queueEntry.NextRunTime, null);
					}


				} //Second End QueueEntry != null
			} //First End QueueEntry != null

			CheckExpiredAgent();

		} //End Execute Job

		private void CheckExpiredAgent()
		{
			//Get Agents which have not run in the last 5 hours
			List<IQueueEntity> queueEntries = Illuminate.ActiveJob.Queue.Instance.GetExpiredActiveJobQueueEntries();
			
			//Loop through agents
			foreach (IQueueEntity queueEntity in queueEntries)
			{
				DateTime nextRunTime = DateTime.Now;

				Logger.WriteLine("Resetting EXPIRED next execution time on Job: " + queueEntity.Name + "Date/Time: " + nextRunTime.ToString(), Logger.Severity.Information, LOGNAME);

				//process done
				Illuminate.ActiveJob.Queue.Instance.UpdateAgentId(queueEntity.JobId, "Running", "Idle", nextRunTime, null);
			}

			//Update Id
		}

		private IAgent InvokeJob(string path)
		{
			Illuminate.Tools.Invoker invoker = new Illuminate.Tools.Invoker();
			IAgent job;

			try
			{
				job = (IAgent)invoker.Invoke(path, typeof(IAgent));
			}
			catch (System.IO.FileNotFoundException)
			{
				Logger.WriteLine("Active Job Agent was not able to find DLL: " + path, Logger.Severity.Error, LOGNAME);
				job = null; //treat unknown dll and not able to find the type in a dll as the same case
			}

			return job;
		} //End InvokeJob
	}
}
