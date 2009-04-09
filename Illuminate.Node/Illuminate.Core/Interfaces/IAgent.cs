using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Interfaces
{


	public interface IAgent : IIlluminateObject
	{
		void InitializeAgent(Contexts.AgentContext agentContext);
		void Run();
		int GetInterval();
		void Cleanup();
		string GetStatus();
	}
}
