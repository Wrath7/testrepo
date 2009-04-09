using System;

namespace Illuminate.Node.Collections
{
	public interface ISetting
	{
		Illuminate.Node.Entities.ISetting this[int Index] { get; set; }
		int Count { get; }
		void Add(Entities.ISetting node);
	}
}
