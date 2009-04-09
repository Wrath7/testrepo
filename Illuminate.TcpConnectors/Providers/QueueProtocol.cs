using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Tcp.Protocol;

namespace Illuminate.Providers
{
	public class QueueProtocol
	{
		public static IProtocolCommand Eof()
		{
			return new ProtocolCommand("EOF");
		}

		public static IProtocolCommand Connected()
		{
			return new ProtocolCommand("CONNECTED");
		}

		public static IProtocolCommand ConnectedAck()
		{
			return new ProtocolCommand("CONNECTEDACK");
		}

		public static IProtocolCommand Push()
		{
			return new ProtocolCommand("PUSH");
		}

		public static IProtocolCommand PushAck()
		{
			return new ProtocolCommand("PUSHACK");
		}

		public static IProtocolCommand Pop()
		{
			return new ProtocolCommand("POP");
		}

		public static IProtocolCommand PopAck()
		{
			return new ProtocolCommand("POPACK");
		}

		public static IProtocolCommand GetNextTime()
		{
			return new ProtocolCommand("GETNEXTTIME");
		}

		public static IProtocolCommand GetNextTimeAck()
		{
			return new ProtocolCommand("GETNEXTTIMEACK");
		}

		public static IProtocolCommand Clear()
		{
			return new ProtocolCommand("CLEAR");
		}

		public static IProtocolCommand ClearAck()
		{
			return new ProtocolCommand("CLEARACK");
		}
	}
}
