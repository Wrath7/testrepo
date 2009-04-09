using System;

namespace Illuminate.Node.Collections
{
	public interface IAgentLimit
	{
		Illuminate.Node.Entities.IAgentLimit this[int Index] { get; set; }
		int Count { get; }
		void Add(Entities.IAgentLimit agentLimit);
	}
}
