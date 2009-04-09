using System;

namespace Illuminate.Node.Entities
{
	public interface IAgent
	{
		string AgentId { get; set; }
		string AgentName { get; set; }
		int EntryId { get; set; }
		string ErrorInformation { get; set; }
		string NodeId { get; set; }
		string Status { get; set; }
		DateTime StatusDate { get; set; }
	}
}
