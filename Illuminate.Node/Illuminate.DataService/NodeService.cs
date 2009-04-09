using System;
using System.Collections.Generic;
using System.Text;
using Illuminate;

namespace Illuminate.Node
{
	public class Service : INodeService
	{
		protected string _monitorConnection = string.Empty;

		protected Managers.INodeManagerMonitor _monitor;
		protected Managers.IAdmin _admin;
		protected Managers.ISystemLoadManager _systemLoadManager;

		public Managers.INodeManagerMonitor Monitor
		{
			get
			{
				if (_monitor == null)
				{
					_monitor = new Illuminate.Node.Managers.Monitor(this);
				}

				return _monitor;
			}
		}

		public Managers.IAdmin Admin
		{
			get
			{
				if (_admin == null)
				{
					_admin = new Illuminate.Node.Managers.Admin(this);
				}

				return _admin;
			}
		}

		public Managers.ISystemLoadManager SystemLoad
		{
			get
			{
				if (_systemLoadManager == null)
				{
					//system load uses commands specific to the platform, load the one for the platform we are using
					if (Environment.OSVersion.Platform == PlatformID.Unix)
						_systemLoadManager = new Illuminate.Node.Managers.LinuxSystemLoadManager(this);
					else
						_systemLoadManager = new Illuminate.Node.Managers.SystemLoadManager(this); //not implemented
				}

				return _systemLoadManager;
			}
		}

		public string MonitorConnection
		{
			get
			{
				if (_monitorConnection.Length == 0)
				{
					if (System.Configuration.ConfigurationManager.ConnectionStrings["MonitorConnection"] == null)
					{
						throw new Exception("Cannot find Monitor Connection");
					}

					_monitorConnection = System.Configuration.ConfigurationManager.ConnectionStrings["MonitorConnection"].ToString();
				}

				return _monitorConnection;
			}
		}

		public Service()
		{
		}
	}
}
