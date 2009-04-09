using System;
using System.Data.SqlClient;
using System.Text;

namespace Illuminate
{
	/// <summary>
	/// Base Manager class
	/// </summary>
	public class Manager
	{
		protected const int DEFAULT_EXPIRE_TIME = 15;

		/// <summary>
		/// Reference to the Illuminate Service object
		/// </summary>
		protected Illuminate.Node.Service GS;
	}
}