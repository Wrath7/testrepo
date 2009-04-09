using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Illuminate.Node.NodeConsole
{
	public class IlluminateConsole
	{
		#region Console Members

		private static bool _quit = false; //Used to determine if the application will exit
		private static string _Id = string.Empty;
		private static List<string> _commandBuffer = new List<string>();
		private static string _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

		public static bool Quit
		{
			get
			{
				return _quit;
			}
			set
			{
				_quit = value;
			}
		}

		public static string Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

		public static List<string> CommandBuffer
		{
			get
			{
				return _commandBuffer;
			}
		}

		public static string ConsolePrompt
		{
			get
			{
				return _Id + "> ";
			}
		}
		

		#endregion

		#region Illuminate Members

		private static Core.Node.AgentCollection _agentCollection;

		public static Core.Node.AgentCollection AgentCollection
		{
			get
			{
				return _agentCollection;
			}
			set
			{
				_agentCollection = value;
			}
		}

		#endregion

		#region Console Input

		public static void WaitForCommand()
		{
			string command = string.Empty;
			
			int bufferCount = 0;

			Console.Write(ConsolePrompt);
			while (!_quit)
			{
				ConsoleKeyInfo cki = Console.ReadKey(true);

				if (cki.Key == ConsoleKey.Backspace)
				{
					if (command.Length > 0)
					{
						Console.SetCursorPosition(ConsolePrompt.Length, Console.CursorTop);
						command = command.Substring(0, command.Length - 1);
						Console.Write(command + " ");
						Console.SetCursorPosition(ConsolePrompt.Length + command.Length, Console.CursorTop);					
					}
				}
				else if (cki.Key == ConsoleKey.UpArrow)
				{
					if (_commandBuffer.Count != 0)
					{
						if (bufferCount != _commandBuffer.Count)
						{
							int bufferPos = _commandBuffer.Count - 1 - bufferCount;

							Console.SetCursorPosition(ConsolePrompt.Length, Console.CursorTop);
							Console.Write(Tools.Tools.GetString("", command.Length));
							Console.SetCursorPosition(ConsolePrompt.Length, Console.CursorTop);

							command = _commandBuffer[bufferPos];

							Console.Write(command);

							Console.SetCursorPosition(command.Length + ConsolePrompt.Length, Console.CursorTop);

							bufferCount++;
						}
					}
				}
				else if (cki.Key == ConsoleKey.DownArrow)
				{
					if (_commandBuffer.Count != 0)
					{
						if (bufferCount > 0)
						{
							bufferCount--;
							int bufferPos = _commandBuffer.Count - 1 - (bufferCount -1);

							if (bufferPos < _commandBuffer.Count)
							{
								Console.SetCursorPosition(ConsolePrompt.Length, Console.CursorTop);
								Console.Write(Tools.Tools.GetString("", command.Length));
								Console.SetCursorPosition(ConsolePrompt.Length, Console.CursorTop);

								command = _commandBuffer[bufferPos];

								Console.Write(command);

								Console.SetCursorPosition(command.Length + ConsolePrompt.Length, Console.CursorTop);
							}
						}
					}
				}
				else if (cki.Key == ConsoleKey.Enter)
				{
					if (command.Length != 0)
					{
						_commandBuffer.Add(command);
						bufferCount = 0;
					}

					bool success = Commands.Run(command);

					if (!success)
					{
						Console.WriteLine("");
						Console.WriteLine("'" + command + "' is not a recognized command");
					}

					command = string.Empty;
					Console.WriteLine("");
					Console.Write(ConsolePrompt);
				}
				else
				{
					string s = cki.KeyChar.ToString().ToLower();
					char c = s.ToCharArray()[0];

					if (c != '\0')
					{
						command = command + cki.KeyChar.ToString().ToLower();
						Console.Write(cki.KeyChar.ToString());
					}
				}

			}
		} // End WaitForCommand

		#endregion

		#region Initialization

		public static void Initialization()
		{
			Console.WriteLine("Initializing Node...  This might take a few moments");

			//Get the Node Id from the configuration file.
			_Id = Configuration.GetSettings("node", "nodeid");
			
			_agentCollection = new Core.Node.AgentCollection(_Id);

			#region Initialize Agents

			Illuminate.Tools.Logger.WriteLine("Initializing Agents", Illuminate.Tools.Logger.Severity.Debug, _Id);

			_agentCollection.StartAgents();

			Console.WriteLine("");

			#endregion
		}

		#endregion

		#region Console Writter Helpers

		public static void IntroText()
		{
			Console.WriteLine(@" _____ _      _     _    _ __  __ _____ _   _       _______ ______ ");
			Console.WriteLine(@"|_   _| |    | |   | |  | |  \/  |_   _| \ | |   /\|__   __|  ____|");
			Console.WriteLine(@"  | | | |    | |   | |  | | \  / | | | |  \| |  /  \  | |  | |__   ");
			Console.WriteLine(@"  | | | |    | |   | |  | | |\/| | | | | . ` | / /\ \ | |  |  __|  ");
			Console.WriteLine(@" _| |_| |____| |___| |__| | |  | |_| |_| |\  |/ ____ \| |  | |____ ");
			Console.WriteLine(@"|_____|______|______\____/|_|  |_|_____|_| \_/_/    \_\_|  |______|");
			Console.WriteLine(@"Version: " + _version);

			Console.WriteLine("");
			Console.WriteLine("");

			Console.WriteLine("Use the 'help' command to see what you can do here!");
			Console.WriteLine("");
		}

		#endregion

	}
}
