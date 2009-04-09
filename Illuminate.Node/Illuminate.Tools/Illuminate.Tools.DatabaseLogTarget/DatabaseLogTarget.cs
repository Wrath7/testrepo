using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace Illuminate.Tools
{
	[Target("DatabaseLog")]
	public sealed class DatabaseLogTarget : TargetWithLayout
	{
		private string _nodeId;
		private Illuminate.Node.Service _IS;

		public string NodeId
		{
			get
			{
				return _nodeId;
			}
			set
			{
				_nodeId = value;
			}
		}

		public DatabaseLogTarget()
		{
			_IS = new Illuminate.Node.Service();
		}

		protected override void Write(LogEventInfo logEvent)
		{
			_IS.Admin.WriteToLog(_nodeId, logEvent.Level.UppercaseName, logEvent.TimeStamp, logEvent.Message);
		}
	}
}
