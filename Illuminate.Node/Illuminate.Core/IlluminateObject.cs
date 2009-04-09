using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Core
{
	public class IlluminateObject : MarshalByRefObject, Interfaces.IIlluminateObject
	{
		private string _logName = string.Empty;

		public string LOGNAME
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
	}
}
