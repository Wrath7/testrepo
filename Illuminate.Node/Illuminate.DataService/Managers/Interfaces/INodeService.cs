using System;

namespace Illuminate.Node
{
	public interface INodeService
	{
		Managers.INodeManagerMonitor Monitor { get; }
		Managers.IAdmin Admin { get; }
		Managers.ISystemLoadManager SystemLoad { get; }
	}
}
