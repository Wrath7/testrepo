using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace Illuminate.Node.NodeConsole
{
	public class Commands
	{
		public static bool Run(string cmd)
		{
			Console.Clear();

			string[] cmdParts = cmd.Trim().Split(' ');

			switch (cmdParts[0].ToLower())
			{
				case "quit":
				case "exit":
					Commands.Quit();
					break;
				case "":
				case "status":
					if (cmdParts.Length == 1)
					{
						Commands.ClearLogs();
						Commands.WriteStatus(IlluminateConsole.Id, IlluminateConsole.AgentCollection);
					}
					else
					{
						Commands.ShowStatus(cmd, IlluminateConsole.AgentCollection);
					}
					break;
				case "help":
					Commands.ClearLogs();
					Console.Clear();
					Commands.ShowHelp();
					break;
				case "showlogs":
				case "sl":
					Commands.ShowLog(cmd, IlluminateConsole.AgentCollection);
					break;
				default:
					return false;
			}

			return true;
		}

		#region Console Commands

		public static void ClearLogs()
		{
			Illuminate.Tools.Logger.LogNameToTrace = string.Empty;
		}

		public static void ShowLog(string Cmd, Illuminate.Core.Node.AgentCollection agentCollection)
		{
			Console.Clear();

			try
			{
				string[] logCommand = Cmd.Split(' ');

				if (logCommand.Length == 2)
				{
					if (logCommand[1] == "all")
					{
						Illuminate.Tools.Logger.LogNameToTrace = "all";
					}
					else
					{
						int agentNumber = int.Parse(logCommand[1]);

						if (agentNumber < 0 || agentNumber >= agentCollection.Count)
						{
							Console.WriteLine("*** The agent you are trying to show the logs for does not exists");
							return;
						}

						Illuminate.Tools.Logger.LogNameToTrace = agentCollection[agentNumber].LogName;
					}

				}

			}
			catch (Exception e)
			{
				Console.WriteLine("Error trying to retreive log: " + e.Message);
			}
		}

		public static void ShowStatus(string Cmd, Illuminate.Core.Node.AgentCollection agentCollection)
		{
			Console.Clear();

			try
			{
				string[] command = Cmd.Split(' ');

				if (command.Length == 2)
				{
					int agentNumber = int.Parse(command[1]);

					if (agentNumber < 0 || agentNumber >= agentCollection.Count)
					{
						Console.WriteLine("*** The agent you are trying to show the status for does not exists");
						return;
					}

					Console.WriteLine("Detailed Status");
					Console.WriteLine("---------------");
					Console.WriteLine("");
					Console.WriteLine(Tools.Tools.GetString("#:", 25) + agentNumber);
					Console.WriteLine(Tools.Tools.GetString("Agent Id:", 25) + agentCollection[agentNumber].AgentId);
					Console.WriteLine(Tools.Tools.GetString("Agent Type:", 25) + agentCollection[agentNumber].AgentType);
					Console.WriteLine(Tools.Tools.GetString("Log Name:", 25) + agentCollection[agentNumber].LogName);
					Console.WriteLine(Tools.Tools.GetString("Run Count:", 25) + agentCollection[agentNumber].RunCount);
					Console.WriteLine(Tools.Tools.GetString("Last Execution Time:", 25) + agentCollection[agentNumber].LastExecutionTime);

					if (agentCollection[agentNumber].Status == Illuminate.Core.Node.Agent.AgentStatus.Executing)
					{
						Console.WriteLine(Tools.Tools.GetString("Start Execution Time:", 25) + agentCollection[agentNumber].StartExecutionTime);
						
						TimeSpan sp = DateTime.Now.Subtract(agentCollection[agentNumber].StartExecutionTime);
						Console.WriteLine(Tools.Tools.GetString("Elapsed time:", 25) + sp.TotalSeconds.ToString());

					}
					else
					{
						Console.WriteLine(Tools.Tools.GetString("Next Execution Time:", 25) + agentCollection[agentNumber].NextExecutionTime);

						TimeSpan sp = DateTime.Now.Subtract(agentCollection[agentNumber].StartExecutionTime);
						Console.WriteLine(Tools.Tools.GetString("Seconds to Execution:", 25) + sp.TotalSeconds.ToString());
					}

					Console.WriteLine(Tools.Tools.GetString("Status:", 25) + agentCollection[agentNumber].Status.ToString());
					Console.WriteLine("");
					Console.WriteLine("Internal Status");
					Console.WriteLine("---------------");
					Console.WriteLine("");
					Console.WriteLine(agentCollection[agentNumber].InternalStatus);
					Console.WriteLine("");
				}

			}
			catch (Exception e)
			{
				Console.WriteLine("Error trying to retreive log: " + e.Message);
			}
		}

		public static void WriteStatus(string nodeId, Illuminate.Core.Node.AgentCollection agentCollection)
		{
			Commands.ClearLogs();
			Console.Clear();

			ConsoleStatus status = new ConsoleStatus(agentCollection);
			status.ShowConsole();
		}

		public static string GetStatus(string nodeId, Illuminate.Core.Node.AgentCollection agentCollection)
		{
			System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();

			StringBuilder output = new StringBuilder();

			output.Append("Illuminate Agent Status\n");
			output.Append("--------------------\n");
			output.Append("\n");
			output.Append("Node Identifier: " + nodeId);
			output.Append("\n\n");
			output.Append(Tools.Tools.GetString("#", 3) + Tools.Tools.GetString("Agent Type", 40) + Tools.Tools.GetString("Id", 21) + Tools.Tools.GetString("Cnt", 6) + Tools.Tools.GetString("Status", 8) + "\n");
			output.Append(Tools.Tools.GetUnderline(36 + 7 + 25 + 11) + "\n");
			output.Append("\n");

			for (int i = 0; i < agentCollection.Count; i++)
			{
				output.Append(Tools.Tools.GetString(i.ToString(), 3) + Tools.Tools.GetString(agentCollection[i].AgentType, 39) + " " + Tools.Tools.GetString(agentCollection[i].AgentId, 20) + " ");
				output.Append(Tools.Tools.GetString(agentCollection[i].RunCount.ToString(), 6));
				output.Append(Tools.Tools.GetString(agentCollection[i].Status.ToString(), 15));
				output.Append("\n");
			}

			return output.ToString();
		}

		public static string GetStatusFromFile()
		{
			if (System.IO.Directory.Exists("status"))
			{
				try
				{
					StringBuilder output = new StringBuilder();

					output.Append("\n");
					output.Append(Tools.Tools.GetUnderline(79) + "\n");
					output.Append(Tools.Tools.GetString("PID", 6) + Tools.Tools.GetString("AG", 15) + Tools.Tools.GetString("PORT", 5) + Tools.Tools.GetString("RC", 6) + Tools.Tools.GetString("ST", 10) + Tools.Tools.GetString("MEM", 8) + Tools.Tools.GetString("THR", 5) + "\n");
					output.Append(Tools.Tools.GetUnderline(79) + "\n");

					string[] files = Directory.GetFiles("status", "*.sts");

					for (int i = 0; i < files.Length; i++)
					{
						StreamReader sr = new StreamReader(files[i]);
						string content = sr.ReadToEnd();
						sr.Close();

						content = content.Replace("\r", "");
						string[] data = content.Split('\n');

						string[] agentTypeParts = data[1].Split('.');
						string agentType = agentTypeParts[agentTypeParts.Length - 2];

						string memStr = data[4];
						int mem = int.Parse(memStr) / 1024;

						output.Append(Tools.Tools.GetString(data[5], 6) + Tools.Tools.GetString(agentType, 14) + " " + Tools.Tools.GetString(data[0], 5));
						output.Append(Tools.Tools.GetString(data[2], 6));
						output.Append(Tools.Tools.GetString(Tools.Tools.Left(data[3], 9), 10));
						output.Append(Tools.Tools.GetString(mem.ToString() + "k", 8));
						output.Append(Tools.Tools.GetString(data[6], 5));
						output.Append("\n");
					}

					return output.ToString();
				}
				catch
				{
					return "Error reading status files...";
				}
			}

			return string.Empty;
		}

		public static void Quit()
		{
			System.Threading.Thread terminateThread = new Thread(new ThreadStart(TerminateAgents));
			terminateThread.Start();

			ThreadPool._quit = true;
		}

		private static void TerminateAgents()
		{
			Console.Clear();
			Console.WriteLine("Terminating all agents... This might take a few moments");
			Console.WriteLine("");

			List<string> terminatedAgents = new List<string>();

			//Tell all agents to stop all at once, if one hangs this shouldn't hold up stopping the other agents.
			bool agentStillRunning = false;
			do
			{
				agentStillRunning = false; //reset
				for (int i = 0; i < IlluminateConsole.AgentCollection.Count; i++)
				{
					if (IlluminateConsole.AgentCollection[i].Status != Illuminate.Core.Node.Agent.AgentStatus.Stopped)
					{
						IlluminateConsole.AgentCollection[i].Stop();
						agentStillRunning = true;
					}

				}

				for (int i = 0; i < IlluminateConsole.AgentCollection.Count; i++)
				{
					if (!terminatedAgents.Contains(IlluminateConsole.AgentCollection[i].AgentId))
					{
						if (IlluminateConsole.AgentCollection[i].Status == Illuminate.Core.Node.Agent.AgentStatus.Stopped)
						{
							Console.WriteLine(IlluminateConsole.AgentCollection[i].AgentId + " " + IlluminateConsole.AgentCollection[i].AgentType + " was stopped.");
							terminatedAgents.Add(IlluminateConsole.AgentCollection[i].AgentId);
						}
					}
				}

				if (agentStillRunning)
					Thread.Sleep(500);
			} while (agentStillRunning);


			IlluminateConsole.Quit = true;

			Console.WriteLine("All consoles have been terminated.");
			Console.WriteLine("");
			Console.WriteLine("Press any key to exit...");
		}

		public static void ShowHelp()
		{
			Console.WriteLine("");
			Console.WriteLine("Illuminate Node HELP");
			Console.WriteLine("");
			Console.WriteLine("General Commands");
			Console.WriteLine("-----------------");
			Console.WriteLine(Tools.Tools.GetString("status: ", 15) + "Shows status of all the agents currently running on the node");
			Console.WriteLine(Tools.Tools.GetString("status #: ", 15) + "Shows the detailed status of a single agent");
			Console.WriteLine(Tools.Tools.GetString("clear: ", 15) + "Clears the screen");
			Console.WriteLine(Tools.Tools.GetString("quit: ", 15) + "Terminates all the agents and exists the Node");
			Console.WriteLine(Tools.Tools.GetString("ex it: ", 15) + "Terminates all the agents and exists the Node");
			Console.WriteLine(Tools.Tools.GetString("showlogs #: ", 15) + "Outputs the logs entries of an agent on the console.");
			Console.WriteLine(Tools.Tools.GetString("showlogs all: ", 15) + "Outputs the logs entries of all agents to the console");
			Console.WriteLine(Tools.Tools.GetString("clearlog: ", 15) + "Stops the outputs of all the agent logs to the console");
			Console.WriteLine(Tools.Tools.GetString("help: ", 15) + "I have no idea why programmers put the 'help' command");
			Console.WriteLine(Tools.Tools.GetString(" ", 15) + "in the help list since a user obviously knows how to see the");
			Console.WriteLine(Tools.Tools.GetString(" ", 15) + "help, but here it is anyways!");
			Console.WriteLine("");
		}

		#endregion
	}
}
