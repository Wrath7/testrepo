using System;

namespace Illuminate.Node.Entities
{
	public interface ISystemLoad
	{
		string NodeName { get; set; }
		DateTime LastUpdated { get; }
		double Load1Min { get; set; }
		double Load5Min { get; set; }
		double Load15Min { get; set; }
		long FreeMemory { get; set; }
		int ProcessorCount { get; set; }
	}
}
