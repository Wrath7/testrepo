using System;

namespace Illuminate.Node.Entities
{
	public interface IAgentLimit
	{
		string AgentName { get; set; }
		string MinMax { get; set; }
		int Count { get; set; }
	}
}
