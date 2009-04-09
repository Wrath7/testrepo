using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Illuminate.Tools;
using System.Diagnostics;

namespace Illuminate.MapReduce
{
	public class MapMarshal : MarshalByRefObject
	{
		public static string LOGNAME = string.Empty;

		public InputData Map(string mapPath, InputData id)
		{
			Illuminate.Tools.Logger.WriteLine("Received map request: " + id.Key, Illuminate.Tools.Logger.Severity.Information, LOGNAME);

			Logger.WriteLine("Invoking Map Pluging: " + mapPath, Logger.Severity.Debug, LOGNAME);

			Stopwatch sw = new Stopwatch();
			sw.Start();

			Illuminate.Tools.Invoker inv = new Illuminate.Tools.Invoker();
			Illuminate.Interfaces.IMapReduceTask mapTask = (Illuminate.Interfaces.IMapReduceTask)inv.Invoke(mapPath, typeof(Illuminate.Interfaces.IMapReduceTask));
			id = mapTask.Task(id);

			sw.Stop();

			Logger.WriteLine("Invoking the Map has been completed: " + sw.ElapsedMilliseconds.ToString() + "ms", Logger.Severity.Debug, LOGNAME);

			return id;
		}
	}
}
