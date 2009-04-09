using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Tools;

namespace Illuminate.Agents
{
	public class Monitor : Illuminate.Core.Node.AgentStandard, Illuminate.Interfaces.IAgent
	{
		public new void InitializeAgent(Illuminate.Contexts.AgentContext context)
		{
			Logger.WriteLine("Initializing Monitor...", Logger.Severity.Debug, LOGNAME);
		}

		public new void Run()
		{
			Logger.WriteLine("Monitor is executing", Logger.Severity.Debug, LOGNAME);
		}

		public override void Cleanup()
		{
		}
	}
}
