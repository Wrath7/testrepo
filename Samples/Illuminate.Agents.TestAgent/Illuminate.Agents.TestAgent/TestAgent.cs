using System;
using Illuminate.Tools;
using System.IO;

namespace Illuminate.Agents.TestAgent2
{
	public class TestAgent2 : Illuminate.Core.Node.AgentStandard,  Illuminate.Interfaces.IAgent
	{
		public new void InitializeAgent(Illuminate.Contexts.AgentContext context)
		{
			base.InitializeAgent(context);

			Logger.WriteLine("Initializing Test Agent...", Logger.Severity.Debug, LOGNAME);

			StreamWriter sw = new StreamWriter("test.txt");
			sw.WriteLine("222");
			sw.Close();

		}

		public new void Run()
		{
			Logger.WriteLine("Test Agent is executing 222", Logger.Severity.Debug, LOGNAME);

			string ss = string.Empty;
			for (int i = 0; i < 10000; i++)
			{
				ss = ss + i.ToString();
			}
		}

		public override void Cleanup()
		{
		}

	}
}
