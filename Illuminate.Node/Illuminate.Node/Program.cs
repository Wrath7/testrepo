using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Data;
using System.Diagnostics;
using Illuminate.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Illuminate.Node.NodeConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			CommandLine.Utility.Arguments arguments = new CommandLine.Utility.Arguments(args);
			Configuration.ReadConfiguration();

			#region Command Line Arguments Processing

			if (arguments["v"] != null) Illuminate.Tools.Logger.LogNameToTrace = "all";	

			if (arguments["help"] != null)
			{
				Console.WriteLine("Usage: Node.exe [options]");
				Console.WriteLine("");
				Console.WriteLine("Options:");
				Console.WriteLine("-v :       Verbose mode.  This option will display all the log entries");
				Console.WriteLine("           which are being outputted for every agent.");
				Console.WriteLine("");

				return;
			}

			#endregion

			IlluminateConsole.IntroText();

			IlluminateConsole.Initialization();

			IlluminateConsole.WaitForCommand();
		}	
	}
}
