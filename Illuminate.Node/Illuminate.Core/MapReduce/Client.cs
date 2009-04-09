using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Illuminate.MapReduce
{
	public sealed class Client
	{
		#region Class Members

		private int _id = 0;
		private string _host = string.Empty;
		private int _port = 0;
		private bool _isAlive = true;
		private bool _inUse = false;
		private MapMarshal _marshal;
		private int _errorCount = 0;

		public int Id
		{
			get
			{
				return _id;
			}
		}

		public string Host
		{
			get
			{
				return _host;
			}
		}

		public int Port
		{
			get
			{
				return _port;
			}
		}

		public bool InUse
		{
			get
			{
				return _inUse;
			}
			set
			{
				_inUse = value;
			}
		}

		public bool IsAlive
		{
			get
			{
				return _isAlive;
			}
		}

		public MapMarshal Marshal
		{
			get
			{
				return _marshal;
			}
		}

		public int ErrorCount
		{
			get
			{
				return _errorCount;
			}
		}

		public Client(int Id, string host, int port)
		{
			_id = Id;
			_host = host;
			_port = port;
			_marshal = (MapMarshal)Activator.GetObject(typeof(MapMarshal), "tcp://" + host + ":" + port.ToString() + "/Mapper");
		}

		public void IncrementErrorCount()
		{
			lock (this)
			{
				_errorCount++;
			}
		}

		#endregion Class Members

		#region Static Members

		private static List<Client> _Clients = new List<Client>();
		
		public static List<Client> Clients
		{
			get
			{
				return _Clients;
			}
		}

		public static Client GetClient()
		{
			Client returnSvr = null;

			lock (_Clients)
			{
				bool foundClient = false;

				while (!foundClient)
				{
					foreach (Client svr in _Clients)
					{
						if (!svr.InUse && svr.IsAlive)
						{
							svr.InUse = true;
							returnSvr = svr;

							foundClient = true;
						}
					}
				}
			}

			return returnSvr;
		}

		public static void ReleaseClient(Client svr)
		{
			_Clients[svr.Id].InUse = false;
		}

		#endregion
	}
}
