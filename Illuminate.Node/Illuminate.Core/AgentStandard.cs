using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Illuminate.Tools;
using System.IO;

namespace Illuminate.Core.Node
{
	public class AgentStandard : Illuminate.Core.IlluminateObject, Illuminate.Interfaces.IAgent
	{
		protected Illuminate.Contexts.AgentContext _context;

		public void InitializeAgent(Illuminate.Contexts.AgentContext context)
		{
			_context = context;
			LOGNAME = context.LogName;
		} //InitializeAgent

		public int GetInterval()
		{
			return _context.Interval;
		} //GetInterval

		public void Run()
		{
			Illuminate.Tools.Logger.WriteLine("From Standard", Logger.Severity.Debug, LOGNAME);
		} //Run

		/// <summary>
		/// Override to perform any last minute cleanup before this agent is stopped.
		/// </summary>
		public virtual void Cleanup()
		{

		} //Cleanup

		public virtual string GetStatus()
		{
			return "Not Implemented";
		} //GetStatus
	}
}
