using System;

namespace Illuminate.ActiveJob
{
	public interface IActiveJob
	{
		void ExecuteJob(string logName, Illuminate.Contexts.AgentContext context);
	}
}
