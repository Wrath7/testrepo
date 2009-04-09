using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Illuminate.Tcp.Protocol
{
	public interface IProtocolCommand : IComparable
	{
		IPAddress From { get; }
		IPAddress To { get; }
		string Command { get; }
		List<object> Parameters { get; }
		Guid Id { get; set; }
	}
}
