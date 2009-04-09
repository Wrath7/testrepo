using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Tcp.Server;
using Illuminate.Tcp.Protocol;
using Illuminate.Providers;
using Illuminate.Queue.QueueData;
using Illuminate.Tools;
using Illuminate.UrlPriorityQueue;

namespace Illuminate.Providers
{
	public class QueueProviderServer : TcpServerProvider
	{
		protected static DateTime _nextClearDate = DateTime.Now.AddSeconds(30);
		protected static object _clearObjectLock = new object();
		protected static bool _nextClearDateSet = false;

		public override object Clone()
		{
			return new QueueProviderServer();
		}

		public override void OnAcceptConnection(ConnectionState state)
		{
			Logger.WriteLine("Client Connected: " + state.RemoteEndPoint.ToString(), Logger.Severity.Information, state.LOGNAME);
		}

		public override void OnReceiveData(ConnectionState state)
		{
			#region Setting Next Clear Date

			lock (_clearObjectLock)
			{
				if (!_nextClearDateSet)
				{
					_nextClearDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 50, 00);

					if (_nextClearDate < DateTime.Now)
					{
						_nextClearDate = _nextClearDate.AddDays(1);
					}

					Console.WriteLine("Next Clear Date Set To: " + _nextClearDate.ToString());

					_nextClearDateSet = true;
				}
			}

			#endregion

			byte[] buffer = new byte[1024];

			while (state.AvailableData > 0)
			{
				int readBytes = state.Read(buffer, 0, 1024);
				if (readBytes > 0)
				{
					try
					{
						#region Check if we need to clear Queue

						lock (_clearObjectLock)
						{
							if (DateTime.Now > _nextClearDate)
							{
								Console.WriteLine("Clearing Queue");

								//Data.DataService.Crawler.UrlQueue.ClearMemcached();

								//Clear Queue
								Data.Queue.Clear();

								_nextClearDate = _nextClearDate.AddDays(1);

								Console.WriteLine("Next Clear Date Set To: " + _nextClearDate.ToString());
							}
						}

						#endregion

						IProtocolCommand cmd = ProtocolCommand.Deserialize(buffer);

						Logger.WriteLine("Received Command from: " + state.RemoteEndPoint.ToString() + " - " + cmd.Command, Logger.Severity.Debug, state.LOGNAME);

						IProtocolCommand sendCmd;

						switch (cmd.Command)
						{
							case "PUSH":
								bool pushStatus = Data.Queue.Insert(new UrlQueueEntity(cmd.Parameters[0].ToString(), 1, int.Parse(cmd.Parameters[1].ToString()), cmd.Parameters[2].ToString()));

								sendCmd = QueueProtocol.PushAck();
								sendCmd.Id = cmd.Id;

								Send(sendCmd, state);
								break;
							case "POP":

								Illuminate.UrlPriorityQueue.UrlQueueEntity urlEntity = (Illuminate.UrlPriorityQueue.UrlQueueEntity)Data.Queue.Pop();

								if (urlEntity != null)
								{
									sendCmd = QueueProtocol.PopAck();
									sendCmd.Id = cmd.Id;
									sendCmd.Parameters.Add(urlEntity.Data);

									Send(sendCmd, state);
								}
								else
								{
									Send(QueueProtocol.Eof(), state);
								}

								break;
						} // End Switch
					} //End Try
					catch 
					{
						//TODO Send Error Back to Client
						state.EndConnection();
					}

				} // End If
				else
				{
					state.EndConnection();
				}
			}
		}

		public override void OnDropConnection(ConnectionState state)
		{
			Logger.WriteLine("Connection Dropped: " + state.RemoteEndPoint.ToString(), Logger.Severity.Information, state.LOGNAME);
		}

		private void Send(IProtocolCommand cmd, ConnectionState state)
		{
			Logger.WriteLine("Sending Command from: " + state.RemoteEndPoint.ToString() + " - " + cmd.Command, Logger.Severity.Debug, state.LOGNAME);

			byte[] dataToSend = ProtocolCommand.Serialize(cmd);

			bool status = state.Write(dataToSend, 0, dataToSend.Length);

			if (!status)
			{
				state.EndConnection();
			}
		}
	}
}
