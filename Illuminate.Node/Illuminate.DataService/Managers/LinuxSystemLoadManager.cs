using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Illuminate.Node.Managers
{
	public class LinuxSystemLoadManager : SystemLoadManager
	{
		//The number of processors cannot change on a running system, so it can be cached
		private int _numProcessors = -1;

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Service">Reference to the Service object</param>
		public LinuxSystemLoadManager(Illuminate.Node.Service parentService) : base(parentService)
		{ }

		#endregion

		public override bool Implemented
		{
			get { return true; }
		}

		public override long FreeMemory()
		{
			using (Process p = new Process())
			{
				p.StartInfo.FileName = "free";
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.Start();
				string output = p.StandardOutput.ReadToEnd();

				//get the value under the free column beside 'buffers/cache'
				Match reg = Regex.Match(output, "buffers/cache:\\s+[0-9]+\\s+([0-9]+)");

				long numFreeMemory = -1;
				if (reg.Success && reg.Groups.Count >= 2)
				{
					long.TryParse(reg.Groups[1].Value, out numFreeMemory);
				}

				return numFreeMemory;
			}
		}

		public override void LoadAverages(out double oneMin, out double fiveMin, out double fifteenMin)
		{
			using (FileStream loadAvgFile = File.OpenRead("/proc/loadavg"))
			{
				using (StreamReader loadAvgStream = new StreamReader(loadAvgFile))
				{
					string output = loadAvgStream.ReadToEnd();
					string[] loadAverages = output.Split(' ');

					if (loadAverages.Length < 3)
						throw new ArgumentException("Invalid number of entries from load average file");

					//divide the load averages by the number of processors
					int numProcessors = NumProcessors();

					oneMin = double.Parse(loadAverages[0]) / numProcessors;
					fiveMin = double.Parse(loadAverages[1]) / numProcessors;
					fifteenMin = double.Parse(loadAverages[2]) / numProcessors;
				}
			}
		}

		public override int NumProcessors()
		{
			//use cached value if its available
			if (_numProcessors == -1)
			{
				using (Process p = new Process())
				{
					p.StartInfo.FileName = "grep";
					p.StartInfo.Arguments = "-c ^processor /proc/cpuinfo";
					p.StartInfo.UseShellExecute = false;
					p.StartInfo.RedirectStandardOutput = true;
					p.Start();
					string output = p.StandardOutput.ReadToEnd();

					int.TryParse(output, out _numProcessors);
				}
			}

			return _numProcessors;
		}
	}
}
