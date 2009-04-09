using System;

namespace Illuminate.Node.Collections
{
	public interface IAgent
	{
		Illuminate.Node.Entities.IAgent this[int Index] { get; set; }
		int Count { get; }
		void Add(Entities.IAgent agent);
		void Remove(Entities.IAgent agent);
		bool Contains(string agent);
	}
}
