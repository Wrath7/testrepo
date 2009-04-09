using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Tcp
{
	public class Exceptions
	{
		/// <summary>
		/// Exception which handles parameters which are not valid
		/// </summary>
		public class TcpConnectionException : Exception
		{
			/// <summary>
			/// Exception which handles parameters which are not valid
			/// </summary>
			/// <param name="Message">Message to pass the exception handler</param>
			public TcpConnectionException(string Message) { }
		}
	}
}
