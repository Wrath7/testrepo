using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Contexts
{
	[Serializable]
	public class AgentContext
	{
		protected string _nodeId = string.Empty;
		protected string _agentId = string.Empty;
		protected Interfaces.INodeCom _com;
		protected int _interval = 1000;
		protected string _logName = string.Empty;
		protected Core.Node.Agent _agent;
		protected List<string> _parameters;
        protected string _sectionName;

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

		public string AgentId
		{
			get
			{
				return _agentId;
			}
			set
			{
				_agentId = value;
			}
		}

        public string SectionName 
        {
            get { return _sectionName; }
        }

		public Interfaces.INodeCom Communicator
		{
			get
			{
				return _com;
			}
			set
			{
				_com = value;
			}
		}

		public int Interval
		{
			get
			{
				return _interval;
			}
			set
			{
				_interval = value;
			}
		}

		public Core.Node.Agent Agent
		{
			get
			{
				return _agent;
			}
			set
			{
				_agent = value;
			}
		}

		public string LogName
		{
			get
			{
				return _logName;
			}
			set
			{
				_logName = value;
			}
		}

		public List<string> Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		public AgentContext(string nodeId, string sectionName, string agentId, Interfaces.INodeCom com, Core.Node.Agent agent)
		{
			_nodeId = nodeId;
			_agentId = agentId;
			_com = com;
			_agent = agent;
            _sectionName = sectionName;

			if (Illuminate.Node.Configuration.SettingExists(sectionName, "interval"))
				_interval = Illuminate.Node.Configuration.GetSettingsInt(sectionName, "interval");

		}
	}
}
