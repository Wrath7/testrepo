using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Communication
{
	[Serializable]
	public class Communicator : Illuminate.Interfaces.INodeCom
	{
		public delegate void OnDataOutDelegate(Illuminate.Communication.Command Command);
		public delegate void OnDataInDelegate(Illuminate.Communication.Command Command);

		public static string MONITORNAME = "MON";
		public static string MANAGERNAME = "MGR";

		public event OnDataOutDelegate OnDataOut;
		public event OnDataInDelegate OnDataIn;

		private string _logname = string.Empty;

		public string LOGNAME
		{
			get
			{
				return _logname;
			}
			set
			{
				_logname = value;
			}
		}

		public void RaiseDataIn(Illuminate.Communication.Command Command)
		{
			if (OnDataIn != null)
			{
				OnDataIn(Command);
			}
		}

		public void RaiseDataOut(Illuminate.Communication.Command Command)
		{
			if (OnDataOut != null)
			{
				OnDataOut(Command);
			}
		}

	}
}
