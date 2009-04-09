using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;

namespace Illuminate.Tcp.Server
{
	/// <SUMMARY>
	/// This class holds useful information
	/// for keeping track of each client connected
	/// to the server, and provides the means
	/// for sending/receiving data to the remote
	/// host.
	/// </SUMMARY>

	public class ConnectionState
	{
		internal Socket _conn;
		internal TcpServer _server;
		internal TcpServerProvider _provider;
		internal byte[] _buffer;
		internal string logName = string.Empty;

		/// <SUMMARY>
		/// Tells you the IP Address of the remote host.
		/// </SUMMARY>
		public EndPoint RemoteEndPoint
		{
			get { return _conn.RemoteEndPoint; }
		}

		/// <SUMMARY>
		/// Returns the number of bytes waiting to be read.
		/// </SUMMARY>

		public int AvailableData
		{
			get { return _conn.Available; }
		}

		/// <SUMMARY>
		/// Tells you if the socket is connected.
		/// </SUMMARY>
		public bool Connected
		{
			get 
			{ 
				return _conn.Connected; 
			}
		}

		public string LOGNAME
		{
			get
			{
				return logName;
			}
			set
			{
				logName = value;
			}
		}

		/// <SUMMARY>
		/// Reads data on the socket, returns
		/// the number of bytes read.
		/// </SUMMARY>
		public int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				if (_conn.Available > 0)
				{
					return _conn.Receive(buffer, offset, count, SocketFlags.None);
				}
				else
				{
					return 0;
				}
			}
			catch
			{
				return 0;
			}
		}

		/// <SUMMARY>
		/// Sends Data to the remote host.
		/// </SUMMARY>
		public bool Write(byte[] buffer, int offset, int count)
		{
			try
			{
				_conn.Send(buffer, offset, count, SocketFlags.None);
				Thread.Sleep(0);
				return true;
			}
			catch
			{
				return false;
			}
		}


		/// <SUMMARY>
		/// Ends connection with the remote host.
		/// </SUMMARY>
		public void EndConnection()
		{
			if (_conn != null && _conn.Connected)
			{
				_conn.Shutdown(SocketShutdown.Both);
				_conn.Close();
			}

			_server.DropConnection(this);
		}
	}
}