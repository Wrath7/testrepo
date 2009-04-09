using System;

namespace Illuminate.Node.Collections
{
	public interface INode
	{
		Illuminate.Node.Entities.INode this[int Index] { get; set; }
		int Count { get; }
		void Add(Entities.INode node);
	}
}
