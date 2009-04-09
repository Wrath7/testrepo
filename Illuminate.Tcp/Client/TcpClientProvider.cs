using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Tcp.Client
{
	public abstract class TcpClientProvider : ICloneable
	{
		public virtual object Clone()
		{
			throw new Exception("Derived clases" + " must override Clone method.");
		}
		public abstract TcpClient Client { get; set; }
		public abstract bool Ready { get; set; }

		public abstract Protocol.IProtocolCommand Send(Protocol.IProtocolCommand command);
	}
}
