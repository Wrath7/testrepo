using System;

namespace Illuminate.Node.Managers
{
	/// <summary>
	/// TODO: This file contains Gazaro specific changes.  Need to eliminate.
	/// </summary>
	public interface ISystemLoadManager
	{
		Entities.ISystemLoad GetSystemLoad(string nodeName);
		void UpdateSystemLoad(string nodeName, double load1Min, double load5Min, double load15Min, long freeMemory, int processorCount);
		Collections.ISystemLoad GetSystemLoadAliveNodes();
		DateTime GetUrlQueueMinProcessDate();
		int GetHtmlQueueNumEntries();
		int GetEmailQueueNumEntries();

		#region Platform-dependent Operations

		bool Implemented { get; }

		long FreeMemory();
		void LoadAverages(out double oneMin, out double fiveMin, out double fifteenMin);
		int NumProcessors();

		#endregion
	}
}
