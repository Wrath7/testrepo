using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Illuminate.Interfaces;
using Illuminate.Tools;
using Illuminate.ActiveJob;

namespace Illuminate.Agents.ActiveJob
{
	public class ActiveJobAgent : Illuminate.Core.Node.AgentStandard, Illuminate.Interfaces.IAgent
	{
		public new void InitializeAgent(Illuminate.Contexts.AgentContext context)
		{
			Logger.WriteLine("Initializing QueueRunner...", Logger.Severity.Debug, LOGNAME);
		}

		public new void Run()
		{
			Illuminate.ActiveJob.IActiveJob ac = new Illuminate.ActiveJob.ActiveJob();
			ac.ExecuteJob(LOGNAME, _context);
		}

		
	}
}
