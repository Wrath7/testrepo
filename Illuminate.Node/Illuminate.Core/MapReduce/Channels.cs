using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Illuminate.MapReduce
{
	public class Channels
	{
		private static IChannel server = null;
		private static Dictionary<string, MapMarshal> mapClients = new Dictionary<string, MapMarshal>();
		private static Dictionary<string, int> clientErrors = new Dictionary<string, int>();

		public static IChannel MapServer
		{
			get
			{
				return server;
			}
		}

		public static void StartMapServer(int port)
		{
			server = new TcpChannel(port);
			ChannelServices.RegisterChannel(server, false);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(Illuminate.MapReduce.MapMarshal), "Mapper", WellKnownObjectMode.SingleCall);
		}

		public static void InitiateClientChannel()
		{
			IChannel channel = new TcpChannel();
			ChannelServices.RegisterChannel(channel, false);
		}


		//public static MapMarshal GetMapClient(string server)
		//{
		//    if (mapClients.ContainsKey(server))
		//    {
		//        return mapClients[server];
		//    }

		//    return null;
		//}

		//public static void IncrementClientError(string server)
		//{
		//    if (clientErrors.ContainsKey(server))
		//    {
		//        clientErrors[server]++;
		//    }
		//    else
		//    {
		//        clientErrors.Add(server, 1);
		//    }
		//}

		//public static int ClientErrorCount(string server)
		//{
		//    if (clientErrors.ContainsKey(server))
		//    {
		//        return clientErrors[server];
		//    }

		//    return 0;
		//}



		//public static void StartChannels()
		//{
		//    if (channel == null)
		//    {
		//        channel = new TcpChannel();
		//        ChannelServices.RegisterChannel(channel, false);
		//        RemotingConfiguration.RegisterWellKnownServiceType(typeof(Illuminate.MapReduce.MapMarshal), "Mapper", WellKnownObjectMode.SingleCall);
		//        mapServer = (MapMarshal)Activator.GetObject(typeof(MapMarshal), "tcp://localhost:1990/Mapper");
		//    }
		//}
	}
}
