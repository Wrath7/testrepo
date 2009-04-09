using System;

namespace Illuminate.Node.Collections
{
	public interface ISystemLoad
	{
		Illuminate.Node.Entities.ISystemLoad this[int Index] { get; set; }
		Illuminate.Node.Entities.ISystemLoad this[string nodeName] { get; }
		int Count { get; }
		void Add(Entities.ISystemLoad node);
	}
}
