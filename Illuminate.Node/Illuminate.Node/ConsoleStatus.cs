using System;
using Illuminate;
using System.Threading;

namespace Illuminate.Node.NodeConsole
{
	public class ConsoleStatus
	{
		const int x = 0;
		const int y = 1;

		double maxMemory = 0;
		double minMemory = 99999999;
		bool quit = false;
		Illuminate.Core.Node.AgentCollection _agents;
		bool firstPass = true;

		public ConsoleStatus(Illuminate.Core.Node.AgentCollection agents)
		{
			_agents = agents;
		}

		public void ShowConsole()
		{
			Console.Clear();

			Thread t = new Thread(new ThreadStart(StartRefresh));
			t.Start();

			while (!quit)
			{
				ConsoleKeyInfo cmd = Console.ReadKey(true);

				switch (cmd.Key)
				{
					case ConsoleKey.X:
						Console.Clear();
						quit = true;
						break;
					case ConsoleKey.S:
						Stop();
						break;
					case ConsoleKey.A:
						Start();
						break;
					case ConsoleKey.K:
						Kill();
						break;
				}
			}
		}

		private void WriteConsole()
		{
			double currentMemory = GC.GetTotalMemory(false) / 1024;
			System.Diagnostics.Process process =  System.Diagnostics.Process.GetCurrentProcess();

			if (currentMemory < minMemory) minMemory = currentMemory;
			if (currentMemory > maxMemory) maxMemory = currentMemory;

			#region Header

			#region Row 1

			Console.SetCursorPosition(1, 1);
			Console.Write(Illuminate.Tools.Tools.GetString(" Mem: " + currentMemory.ToString("N0") + "k", 20));
			Console.Write(Illuminate.Tools.Tools.GetString(" Min: " + minMemory.ToString("N0") + "k", 20));
			Console.Write(Illuminate.Tools.Tools.GetString(" Max: " + maxMemory.ToString("N0") + "k", 20));

			#endregion

			#region Row 2

			Console.SetCursorPosition(1, 2);

			#region Get User Times

			double cpuTime = process.TotalProcessorTime.TotalMilliseconds;
			double usrTime = process.UserProcessorTime.TotalMilliseconds;
			string timeUnit = "ms";

			if (cpuTime > 1000 || usrTime > 1000)
			{
				cpuTime = process.TotalProcessorTime.TotalSeconds;
				usrTime = process.UserProcessorTime.TotalSeconds;
				timeUnit = "s";
			}
			if (cpuTime > 1000 || usrTime > 1000)
			{
				cpuTime = process.TotalProcessorTime.TotalMinutes;
				usrTime = process.UserProcessorTime.TotalMinutes;
				timeUnit = "m";
			}

			#endregion

			Console.Write(Illuminate.Tools.Tools.GetString(" Threads: " + process.Threads.Count.ToString(), 20));
			Console.Write(Illuminate.Tools.Tools.GetString(" CPU Time: " + cpuTime.ToString("N0") + timeUnit, 20));
			Console.Write(Illuminate.Tools.Tools.GetString(" Usr Time: " + usrTime.ToString("N0") + timeUnit, 20));

			#endregion

			#region Row 3

			Console.SetCursorPosition(1, 3);
			Console.Write(Illuminate.Tools.Tools.GetString(" Res: " + process.Responding.ToString(), 20));

			#endregion

			#endregion

			#region Table Header

			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;

			Console.SetCursorPosition(0, 5);
			Console.Write(" ");
			Console.Write(Illuminate.Tools.Tools.GetString("PID", 4));
			Console.Write(Illuminate.Tools.Tools.GetString("TYPE", 16));
			Console.Write(Illuminate.Tools.Tools.GetString("ID", 28));
			Console.Write(Illuminate.Tools.Tools.GetString("CNT", 9));
			Console.Write(Illuminate.Tools.Tools.GetString("STS", 10));
			Console.Write(Illuminate.Tools.Tools.GetString("NEXT", 12));

			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;

			#endregion

			int agentRow = 6;
			for (int i = 0; i < _agents.Count; i++)
			{
				Illuminate.Core.Node.Agent agent = _agents[i];
				//System.Diagnostics.ProcessThread t = GetThread(process.Threads, agent.NativeThreadId);

				string agentType = agent.AgentType.Replace(".dll", "");
				if (agentType.Contains(".") && !agentType.EndsWith("."))
					agentType = agentType.Substring(agentType.LastIndexOf(".") + 1);

				Console.SetCursorPosition(1, agentRow);
				Console.Write(Illuminate.Tools.Tools.GetString(i.ToString(), 4));
				Console.Write(Illuminate.Tools.Tools.GetString(agentType, 16));
				Console.Write(Illuminate.Tools.Tools.GetString(agent.AgentId, 28));
				Console.Write(Illuminate.Tools.Tools.GetString(agent.RunCount.ToString(), 9));
				Console.Write(Illuminate.Tools.Tools.GetString(agent.Status.ToString(), 10));

				TimeSpan ts = agent.NextExecutionTime.Subtract(DateTime.Now);
				Console.Write(Illuminate.Tools.Tools.GetString(GetTime(ts), 12));

				//if (t != null)
				//{
				//    
				//}
				//else
				//{
				//    Console.Write(Illuminate.Tools.Tools.GetString("n/a", 12));
				//}

				agentRow++;
			}

			agentRow++;
			Console.SetCursorPosition(0, agentRow);

			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write(Illuminate.Tools.Tools.GetString(" HELP: e[X]it - [S]top - St[A]rt - [K]ill - [D]etails", 80));
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private void WriteInverse(string t, int length)
		{
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write(Illuminate.Tools.Tools.GetString(t, length));
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private System.Diagnostics.ProcessThread GetThread(System.Diagnostics.ProcessThreadCollection threads, int id)
		{
			foreach (System.Diagnostics.ProcessThread t in threads)
			{
				if (t.Id == id)
				{
					return t;
				}
			}

			return null;
		}

		private string GetTime(TimeSpan processTime)
		{
			double time = processTime.TotalMilliseconds;
			string timeUnit = "ms";

			if (time > 1000 || time > 1000)
			{
				time = processTime.TotalSeconds;
				timeUnit = "s";
			}
			if (time > 1000 || time > 1000)
			{
				time = processTime.TotalMinutes;
				timeUnit = "m";
			}
			if (time > 1000 || time > 1000)
			{
				time = processTime.Hours;
				timeUnit = "h";
			}

			if (time < 0) time = 0;

			return time.ToString("N0") + timeUnit;
		}

		private void StartRefresh()
		{
			while (!quit)
			{
				WriteConsole();

				Thread.Sleep(1000);
			}
		}

		#region Commands 

		private void Kill()
		{
			Console.SetCursorPosition(0, 0);
			WriteInverse(" KILL ", 6);

			ConsoleKeyInfo val = Console.ReadKey(true);

			int agent;

			if (int.TryParse(val.KeyChar.ToString(), out agent))
			{
				if (agent < _agents.Count)
				{

					_agents[agent].KillAgent();
				}
			}

			Console.SetCursorPosition(0, 0);
			Console.Write(Illuminate.Tools.Tools.GetString(" ", 80));
		}

		private void Start()
		{
			Console.SetCursorPosition(0, 0);
			WriteInverse(" START ", 7);

			ConsoleKeyInfo val = Console.ReadKey(true);

			int agent;

			if (int.TryParse(val.KeyChar.ToString(), out agent))
			{
				if (agent < _agents.Count)
				{
					_agents[agent].Start();
				}
			}

			Console.SetCursorPosition(0, 0);
			Console.Write(Illuminate.Tools.Tools.GetString(" ", 80));
		}

		private void Stop()
		{
			Console.SetCursorPosition(0, 0);
			WriteInverse(" STOP ", 6);

			ConsoleKeyInfo val = Console.ReadKey(true);

			int agentToStop;

			if (int.TryParse(val.KeyChar.ToString(), out agentToStop))
			{
				if (agentToStop < _agents.Count)
				{
					_agents[agentToStop].Stop();
				}
			}

			Console.SetCursorPosition(0, 0);
			Console.Write(Illuminate.Tools.Tools.GetString(" ", 80));
		}

		#endregion

	}
}
