using System;

namespace Illuminate.Node.Entities
{
	public interface INode
	{
		int EntryId { get; set; }
		string NodeName { get; set; }
		string Status { get; set; }
		DateTime StatusDate { get; set; }
	}
}
