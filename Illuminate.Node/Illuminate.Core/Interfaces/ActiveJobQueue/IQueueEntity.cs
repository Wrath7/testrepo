using System;
using System.Collections.Generic;

namespace Illuminate.ActiveJob
{
	public interface IQueueEntity
	{
		string AgentId { get; set; }
		string DLL { get; set; }
		int JobId { get; set; }
		string Name { get; set; }
		DateTime NextRunTime { get; set; }
		string Status { get; set; }
		DateTime LastStatusUpdate { get; set; }
		int Interval { get; set; }
		List<string> Parameters { get; }
	}
}
