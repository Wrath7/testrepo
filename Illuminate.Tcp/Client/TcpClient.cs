using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Illuminate.Tools;

namespace Illuminate.Tcp.Client
{
	public class TcpClient
	{
		private System.Net.Sockets.TcpClient _tc;
		private IPAddress _serverAddress;
		private int _port;
		private BinaryWriter _writer;
		private TcpClientProvider _provider;
		private string LOGNAME = string.Empty;
		private int _writeErrorCount = 0;

		public System.Net.Sockets.TcpClient SocketClient
		{
			get
			{
				return _tc;
			}
		}

		public TcpClient(IPAddress serverAddress, int port, TcpClientProvider provider, string logName)
		{
			_tc = new System.Net.Sockets.TcpClient();

			_tc.ReceiveBufferSize = 1024;
			_tc.ReceiveTimeout = 5000;

			_tc.SendBufferSize = 1024;
			_tc.SendTimeout = 5000;

			_serverAddress = serverAddress;
			_port = port;
			_provider = provider;

			LOGNAME = logName;
		}

		public void Reconnect()
		{
			Logger.WriteLine("Attemting to reconnect to socket server", Logger.Severity.Information, LOGNAME);

			_tc = new System.Net.Sockets.TcpClient();

			Start();
		}

		public void Start()
		{
			_tc.Connect(_serverAddress, _port);

			NetworkStream s = _tc.GetStream();
			_writer = new BinaryWriter(s);	
		}

		public Protocol.IProtocolCommand Send(byte[] dataToSend)
		{
			try
			{
				if (dataToSend.Length < 1024)
				{
					List<byte> b = new List<byte>(dataToSend);
					for (int i = 0; i < 1024 - dataToSend.Length; i++)
					{
						b.Add(new byte());
					}

					dataToSend = b.ToArray();
				}

				_writer.Write(dataToSend, 0, dataToSend.Length);
			}
			catch (Exception e)
			{
				_writeErrorCount++;

				Logger.WriteLine("Error writing to the stream: " + e.Message, Logger.Severity.Error, LOGNAME);
				Console.WriteLine("Error writing to the stream: " + e.Message);

				if (_writeErrorCount == 10)
				{
					_writeErrorCount = 0;
					Reconnect();
				}

				return null;
			}


			#region Read CallBack from Server

			byte[] buffer = new byte[1024];

			try
			{

				NetworkStream Ns = _tc.GetStream();
				BinaryReader Sr = new BinaryReader(Ns);

				
				Sr.Read(buffer, 0, 1024);
			}
			catch (Exception e)
			{
				Logger.WriteLine("A timeout of other receival type error occured: " + e.Message, Logger.Severity.Error, LOGNAME);
				Console.WriteLine("A timeout of other receival type error occured: " + e.Message);

				return null;
			}

			try
			{
				Protocol.IProtocolCommand cmd = Protocol.ProtocolCommand.Deserialize(buffer);

				return cmd;
			}
			catch (Exception e)
			{
				Logger.WriteLine("An error occured while deserializing command: " + e.Message, Logger.Severity.Error, LOGNAME);
				Console.WriteLine("An error occured while deserializing command: " + e.Message);
			}

			#endregion

			return null;
		}
	}
}
