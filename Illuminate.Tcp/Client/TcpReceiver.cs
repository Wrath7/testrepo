using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Illuminate.Tcp.Client
{
	public class TcpReceiver
	{
		private IPAddress _address;
		private int _port;
		private TcpClientProvider _provider;
		private TcpClient _client;
		private WaitCallback _starterCallBack;
		private bool _connected = false;
		private int _index;

		public delegate void OnConnectedDelegate(bool connected, int index);
		public event OnConnectedDelegate onConnected;

		public bool Connected
		{
			get
			{
				return _connected;
			}
		}

		public TcpReceiver(IPAddress address, int port, TcpClientProvider provider, string logName, int index)
		{
			_address = address;
			_port = port;
			_provider = provider;
			_index = index;

			_client = new TcpClient(address, port, provider, logName);
			provider.Client = _client;

			_starterCallBack = new WaitCallback(StarterCallBack_Handler);
			
		}

		private void StarterCallBack_Handler(object state)
		{
			try
			{
				_connected = true;

				TcpReceiver rec = (TcpReceiver)state;

				rec._client.Start();
			}
			catch (Exceptions.TcpConnectionException)
			{
				_connected = false;
			}
			catch (SocketException)
			{
				_connected = false;
			}

			if (onConnected != null) onConnected(_connected, _index);
		}

		public void Start()
		{
			ThreadPool.QueueUserWorkItem(_starterCallBack, this);
		}

		public void Disconnect()
		{
			_client.SocketClient.Close();
		}
	}
}
