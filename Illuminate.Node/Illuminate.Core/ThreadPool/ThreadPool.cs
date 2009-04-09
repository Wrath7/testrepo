using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Illuminate
{
	public static class ThreadPool
	{
		public static int _maxThreadInPool = 20;
		public static List<Thread> _pool = new List<Thread>();
		public static Queue<object> _poolQueue = new Queue<object>();
		public static bool _quit = false;
		public static Thread _deQueueThread = null;
		public static object _lockObject = new object();

		public static bool Execute(WaitCallback callBack, object state, string id)
		{
			lock (_lockObject)
			{
				if (_deQueueThread == null)
				{
					_deQueueThread = new Thread(new ThreadStart(CheckQueue));
					_deQueueThread.Name = "DeQueue Thread for ThreadPool";
					_deQueueThread.Start();
				}
			}

			Thread threadToExecute = null;
			bool threadIsExecuting = false;

			lock (_pool)
			{
				if (_pool.Count != _maxThreadInPool)
				{
					Thread t = new Thread(new ParameterizedThreadStart(callBack));
					t.Name = id;
					_pool.Add(t);

					threadToExecute = t;
				}
				else
				{
					for (int i = 0; i < _maxThreadInPool; i++)
					{
						if (!_pool[i].IsAlive)
						{
							_pool[i] = new Thread(new ParameterizedThreadStart(callBack));
							_pool[i].Name = id;
							threadToExecute = _pool[i];
							break;
						}
					}
					foreach (Thread t in _pool)
					{
						
					}

					if (threadToExecute == null)
					{
						List<object> callBackObject = new List<object>();
						callBackObject.Add(callBack);
						callBackObject.Add(state);
						callBackObject.Add(id);

						_poolQueue.Enqueue(callBackObject);
					}
				}
			}

			if (threadToExecute != null)
			{
				//threadToExecute.Name = id;
				threadToExecute.Start(state);
				threadIsExecuting = true;
			}

			return threadIsExecuting;
		}

		public static void CheckQueue()
		{
			while (!_quit)
			{
				Thread.Sleep(1);

				lock (_poolQueue)
				{
					if (_poolQueue.Count > 0)
					{
						List<object> callBackObject = (List<object>)_poolQueue.Dequeue();

						Execute((WaitCallback)callBackObject[0], callBackObject[1], (string)callBackObject[2]);
					}
				}
			}
		}

		public static void Dump()
		{
			lock (_pool)
			{
				Console.Write("Id              ");
				Console.Write("State           ");
				Console.Write("Managed Id      ");
				Console.Write("                ");
				Console.WriteLine();
				Console.Write("================");
				Console.Write("================");
				Console.Write("================");
				Console.Write("================");
				Console.WriteLine();

				foreach (Thread t in _pool)
				{
					Console.Write(Illuminate.Tools.Tools.GetString(t.Name, 16));
					Console.Write(Illuminate.Tools.Tools.GetString(t.ThreadState.ToString(), 16));
					Console.Write(Illuminate.Tools.Tools.GetString(t.ManagedThreadId.ToString(), 16));
					Console.Write(Illuminate.Tools.Tools.GetString("", 16));
					Console.WriteLine();
				}
			}

			Console.WriteLine("Items Queued: " + _poolQueue.Count);
		}

		public static void DumpAllThreads()
		{
			Process[] allProcs = Process.GetProcesses();

			Console.Write("Managed Id      ");
			Console.Write("State           ");
			Console.Write("Start Time      ");
			Console.Write("Cpu Time        ");
			Console.WriteLine();
			Console.Write("================");
			Console.Write("================");
			Console.Write("================");
			Console.Write("================");
			Console.WriteLine();

			System.Diagnostics.ProcessThreadCollection myThreads = System.Diagnostics.Process.GetCurrentProcess().Threads;

			foreach (ProcessThread pt in myThreads)
			{
				DateTime startTime = pt.StartTime;
				TimeSpan cpuTime = pt.TotalProcessorTime;
				int priority = pt.BasePriority;
				System.Diagnostics.ThreadState ts = pt.ThreadState;

				Console.Write(Illuminate.Tools.Tools.GetString(pt.Id.ToString(), 16));
				Console.Write(Illuminate.Tools.Tools.GetString(ts.ToString(), 16));
				Console.Write(Illuminate.Tools.Tools.GetString(startTime.ToString(), 16));
				Console.Write(Illuminate.Tools.Tools.GetString(cpuTime.ToString(), 16));
				Console.WriteLine();
			}
		}
	}
}
