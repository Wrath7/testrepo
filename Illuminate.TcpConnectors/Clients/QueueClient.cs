using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Illuminate.Tools;

namespace Illuminate.Clients
{
	public class QueueClient
	{
		#region Queue Server Members

		private List<IPAddress> _queueServer = new List<IPAddress>();
		private List<int> _queueServerPort = new List<int>();

		private List<Illuminate.Providers.QueueProviderClient> _clientProvider = new List<Illuminate.Providers.QueueProviderClient>();
		private List<Illuminate.Tcp.Client.TcpReceiver> _tcpReceiver = new List<Illuminate.Tcp.Client.TcpReceiver>();
		private List<bool> _connected = new List<bool>();
		private List<int> _failCount = new List<int>();

		#endregion

		#region Client Members

		private int _roundRobin = 0;
		private Illuminate.Crypto.Hash.FNV1a _urlHasher = new Illuminate.Crypto.Hash.FNV1a();
		private int _numberOfServers = 0;

		#endregion

		private string LOGNAME;

		public bool IsConnected(int Index)
		{
			return _connected[Index];
		}

		public QueueClient(string logName)
		{
			LOGNAME = logName;

			#region Connect to all Queue Servers

			if (System.Configuration.ConfigurationManager.AppSettings["QueueServer"] == null)
			{
				throw new Illuminate.Tcp.Exceptions.TcpConnectionException("Queue Server was not specified in App.config");
			}

			string queueServers = System.Configuration.ConfigurationManager.AppSettings["QueueServer"].ToString();

			string[] queueServerParts = queueServers.Split(',');
			_numberOfServers = queueServerParts.Length;

			Tools.Logger.WriteLine("Found " + _numberOfServers.ToString() + " queue server(s)", Logger.Severity.Information, LOGNAME);

			for (int i = 0; i < queueServerParts.Length;i++)
			{
				Tools.Logger.WriteLine("Connecting to Queue Server: " + queueServerParts[i], Logger.Severity.Information, LOGNAME);

				string[] svrParts = queueServerParts[i].Split(':');

				string svrIP = svrParts[0];
				string svrPort = svrParts[1];

				_connected.Add(false);
				_failCount.Add(0);

				_queueServer.Add(IPAddress.Parse(svrIP));
				_queueServerPort.Add(int.Parse(svrPort));

				ConnectToServer(i, true);
			}
			
		#endregion
		}

		void _tcpReceiver_onConnected(bool connected, int index)
		{
			_connected[index] = connected;
		}

		public bool Push(string url, int siteId, int priority, string key)
		{
			int nodeIdx = GetNodeIdx(url);

			Tools.Logger.WriteLine("Pusing to nodeIdx: " + nodeIdx, Logger.Severity.Debug, LOGNAME);

			if (_connected[nodeIdx])
			{
				Illuminate.Tcp.Protocol.IProtocolCommand pushCommand = Illuminate.Providers.QueueProtocol.Push();
				pushCommand.Parameters.Add(url);
				pushCommand.Parameters.Add(siteId);
				pushCommand.Parameters.Add(key);
				pushCommand.Parameters.Add(priority);

				Illuminate.Tcp.Protocol.IProtocolCommand cmd = _clientProvider[nodeIdx].Send(pushCommand);

				if (cmd == null)
				{
					Tools.Logger.WriteLine("cmd is NULL", Logger.Severity.Debug, LOGNAME);

					IncrementFailCount(nodeIdx);

					return false;
				}

				_failCount[nodeIdx] = 0;

				if (cmd.CompareTo(Illuminate.Providers.QueueProtocol.PushAck()) == 0)
				{
					Tools.Logger.WriteLine("cmd is PUSHACK", Logger.Severity.Debug, LOGNAME);
					return true;
				}
			}
			else
			{
				IncrementFailCount(_roundRobin);
			}

			Tools.Logger.WriteLine("Client is not connected to server", Logger.Severity.Debug, LOGNAME);

			return false;
		}

		public string Pop()
		{
			Tools.Logger.WriteLine("Popping from: " + _roundRobin, Logger.Severity.Debug, LOGNAME);

			if (_connected[_roundRobin])
			{
				Illuminate.Tcp.Protocol.IProtocolCommand popCommand = Illuminate.Providers.QueueProtocol.Pop();

				Illuminate.Tcp.Protocol.IProtocolCommand cmd = _clientProvider[_roundRobin].Send(popCommand);

				if (cmd == null)
				{
					Tools.Logger.WriteLine("cmd is NULL", Logger.Severity.Debug, LOGNAME);

					IncrementFailCount(_roundRobin);

					IncrementRoundRobin();

					return null;
				}

				_failCount[_roundRobin] = 0;

				IncrementRoundRobin();

				if (cmd.Command == "EOF")
				{
					Tools.Logger.WriteLine("cmd is EOF", Logger.Severity.Debug, LOGNAME);

					return null;
				}

				Tools.Logger.WriteLine("cmd is return Url", Logger.Severity.Debug, LOGNAME);
				return cmd.Parameters[0].ToString();
			}
			else
			{
				IncrementFailCount(_roundRobin);
			}

			IncrementRoundRobin();

			return null;
		}

        //TODO : Temporary Fix
        public string PopQueueAgent()
        {
            Tools.Logger.WriteLine("Popping from: " + _roundRobin, Logger.Severity.Debug, LOGNAME);

            if (_connected[_roundRobin])
            {
                Illuminate.Tcp.Protocol.IProtocolCommand popCommand = Illuminate.Providers.QueueProtocol.Pop();

                Illuminate.Tcp.Protocol.IProtocolCommand cmd = _clientProvider[_roundRobin].Send(popCommand);

                if (cmd == null)
                {
                    Tools.Logger.WriteLine("cmd is NULL", Logger.Severity.Debug, LOGNAME);

                    IncrementFailCount(_roundRobin);

                    IncrementRoundRobin();

                    return null;
                }

                _failCount[_roundRobin] = 0;

                IncrementRoundRobin();

                if (cmd.Command == "EOF")
                {
                    Tools.Logger.WriteLine("cmd is EOF", Logger.Severity.Debug, LOGNAME);

                    return "EOF";
                }

                Tools.Logger.WriteLine("cmd is return Url", Logger.Severity.Debug, LOGNAME);
                return cmd.Parameters[0].ToString();
            }
            else
            {
                IncrementFailCount(_roundRobin);
            }

            IncrementRoundRobin();

            return null;
        }
        
		private void IncrementRoundRobin()
		{
			_roundRobin++;

			if (_roundRobin >= _clientProvider.Count)
			{
				_roundRobin = 0;
			}
		}

		private int GetNodeIdx(string key)
		{
			uint itemKeyHash = BitConverter.ToUInt32(_urlHasher.ComputeHash(Encoding.Unicode.GetBytes(key)), 0);

			int nodeIdx = (int)(itemKeyHash % _clientProvider.Count);

			return nodeIdx;
		}

		private void ConnectToServer(int nodeIdx, bool addServer)
		{
			_failCount[nodeIdx] = 0;
			_connected[nodeIdx] = false;

			if (addServer)
			{
				_clientProvider.Add(new Illuminate.Providers.QueueProviderClient());
				_tcpReceiver.Add(new Illuminate.Tcp.Client.TcpReceiver(_queueServer[nodeIdx], _queueServerPort[nodeIdx], _clientProvider[nodeIdx], LOGNAME, nodeIdx));
			}
			else
			{
				_clientProvider[nodeIdx] = new Illuminate.Providers.QueueProviderClient();
				_tcpReceiver[nodeIdx] = new Illuminate.Tcp.Client.TcpReceiver(_queueServer[nodeIdx], _queueServerPort[nodeIdx], _clientProvider[nodeIdx], LOGNAME, nodeIdx);
			}

			_tcpReceiver[nodeIdx].onConnected += new Illuminate.Tcp.Client.TcpReceiver.OnConnectedDelegate(_tcpReceiver_onConnected);
			_tcpReceiver[nodeIdx].Start();
		}

		private void IncrementFailCount(int nodeIdx)
		{
			_failCount[nodeIdx]++;

			if (_failCount[nodeIdx] > 100)
			{
				ConnectToServer(nodeIdx, false);
			}

		}

		public void Clear()
		{

			for (int i = 0; i < _numberOfServers; i++)
			{
				Illuminate.Tcp.Protocol.IProtocolCommand clearCommand = Illuminate.Providers.QueueProtocol.Clear();

				Illuminate.Tcp.Protocol.IProtocolCommand cmd = _clientProvider[i].Send(clearCommand);
			}
		}

		//public string GetNextDate()
		//{
		//    if (_connected)
		//    {
		//        Illuminate.Tcp.Protocol.IProtocolCommand getNextTimeCommand = Illuminate.Providers.QueueProtocol.GetNextTime();

		//        Illuminate.Tcp.Protocol.IProtocolCommand cmd = _clientProvider.Send(getNextTimeCommand);

		//        if (cmd == null)
		//        {
		//            return null;
		//        }

		//        if (cmd.Command == "EOF")
		//        {
		//            return null;
		//        }

		//        return cmd.Parameters[0].ToString();

		//    }

		//    return null;
		//}
	}
}
