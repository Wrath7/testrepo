using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Illuminate.Tools;

namespace Illuminate.QueueServer
{
	class Program
	{
		static bool _quit = false;
		static bool _statusLoop = false;
		static bool _loopOnce = false;

        static int _port = 8099;

		static Illuminate.Providers.QueueProviderServer _serverProvider;
		static Illuminate.Tcp.Server.TcpServer _tcpServer;

		static void Main(string[] args)
		{
            if (System.Configuration.ConfigurationManager.AppSettings["port"] == null)
            {
                Console.WriteLine("The port value was not found in the configuration file, defaulting to 8099");
            }
            else
            {
                _port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"].ToString());
            }

			//Illuminate.Queue.QueueData.Data.Queue.Load();

			_serverProvider = new Illuminate.Providers.QueueProviderServer();
            _tcpServer = new Illuminate.Tcp.Server.TcpServer(_serverProvider, _port);
			_tcpServer.Start();

			Console.WriteLine("Illuminate Queueing Server");
			Console.WriteLine("");
            Console.WriteLine("Running on port: " + _port.ToString());
			Console.WriteLine("");

			Console.WriteLine("Use the 'help' command to see what you can do here!");
			Console.WriteLine("");

			string previousCmd = string.Empty;
			

			Console.Write("Queue Server>");

			while (!_quit)
			{
				string cmd = Console.ReadLine();

				string[] cmdParts = cmd.Split(' ');

				switch (cmdParts[0])
				{
					case "exit":
					case "quit":
						Illuminate.Queue.QueueData.Data.Queue.Quit();
						_quit = true;
						break;
					case "showlogs":
						Illuminate.Tools.Logger.LogNameToTrace = "QUEUE_SERVER";
						break;
					case "help":
						ShowHelp();
						break;
					case "dump":

						if (cmdParts.Length > 1)
						{
							if (cmdParts[1] == "-q")
							{
								_statusLoop = true;

								if (cmdParts.Length == 2)
								{
									_loopOnce = true;
								}
								else
								{
									if (cmdParts[2] == "-i")
									{
										_loopOnce = false;
									}
								}

								Thread t = new Thread(new ThreadStart(DumpQ));
								t.Start();
							}
						}

						break;
					case "status":
						ShowStatus();
						break;
					case "":
						Logger.LogNameToTrace = string.Empty;
						break;
					default:
						Console.WriteLine("Unknown Command");
						break;
				}

				Console.Write("Queue Server>");
			}
		}

		static void ShowHelp()
		{
			Console.WriteLine("");
			Console.WriteLine("Illuminate Queue Server HELP");
			Console.WriteLine("");
			Console.WriteLine(GetString("showlogs: ", 15) + "Verbose mode.  Shows the logs entries on the screen.");
			Console.WriteLine(GetString("quit: ", 15) + "Shutdowns the queue server and exits.");
			Console.WriteLine(GetString("exit: ", 15) + "Shutdowns the queue server and exits.");
			Console.WriteLine(GetString("dump al: ", 15) + "Dumps activity log");
			Console.WriteLine(GetString("dump q: ", 15) + "Dumps the queue");
			Console.WriteLine(GetString("help: ", 15) + "I have no idea why programmers put the 'help' command");
			Console.WriteLine(GetString(" ", 15) + "in the help list since a user obviously knows how to see the");
			Console.WriteLine(GetString(" ", 15) + "help, but here it is anyways!");
			Console.WriteLine("");
		}

		static void ShowStatus()
		{
			Console.WriteLine("");
			Console.WriteLine("Illuminate Queue Server STATUS");
			Console.WriteLine("");
			Console.WriteLine("Next Url Queue Save: " + Illuminate.Queue.QueueData.Data.Queue.NextSaveDate.ToString());
			Console.WriteLine("");
            Console.WriteLine("Current Queue Size: " + Illuminate.Queue.QueueData.Data.Queue.Count.ToString());
            //Console.WriteLine("Activity Log Size: " + Illuminate.Queue.QueueData.Data.Queue.ActvityLogCount.ToString());
            Console.WriteLine("Next Crawl Time: " + Illuminate.Queue.QueueData.Data.Queue.NextCrawlTime.ToString());
            Console.WriteLine(Illuminate.Queue.QueueData.Data.Queue.Status);
            //Console.Write("Queue Server>");
		}

		private static void DumpQ()
		{
			while (_statusLoop)
			{
				Logger.LogNameToTrace = string.Empty;
				Console.Clear();
				Illuminate.UrlPriorityQueue.UrlQueueEntity[] urls = Illuminate.Queue.QueueData.Data.Queue.Peek(10);

				for (int i = 0; i < urls.Length; i++)
				{
					Console.WriteLine(GetString(urls[i].Date.ToString(), 22) + " : " + urls[i].Data);
				}

				Console.WriteLine();
				Console.Write("Queue Server>");

				Thread.Sleep(5000);

				if (_loopOnce) _statusLoop = false;
			}
		}

		private static string GetString(string Text, int MaxLength)
		{
			if (Text.Length > MaxLength)
			{
				return Text.Substring(0, MaxLength);
			}
			else if (Text.Length == MaxLength)
			{
				return Text;
			}
			else
			{
				int Diff = MaxLength - Text.Length;

				for (int i = 0; i < Diff; i++)
				{
					Text = Text + " ";
				}
			}

			return Text;
		}
	}

	
}
