using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Tcp.Client;
using Illuminate.Tcp.Protocol;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

namespace Illuminate.Providers
{
	public class QueueProviderClient : TcpClientProvider
	{
		private TcpClient _client;
		private bool _ready = false;

		public QueueProviderClient()
		{

		}

		public override object Clone()
		{
			return new QueueProviderClient();
		}

		public override TcpClient Client
		{
			get
			{
				return _client;
			}
			set
			{
				_client = value;
			}
		}

		public override bool Ready
		{
			get 
			{
				return _ready;
			}
			set
			{
				_ready = value;
			}
		}

		public override IProtocolCommand Send(IProtocolCommand cmd)
		{
			IProtocolCommand receiveCmd = _client.Send(ProtocolCommand.Serialize(cmd));

			return receiveCmd;
		}
	}
}
