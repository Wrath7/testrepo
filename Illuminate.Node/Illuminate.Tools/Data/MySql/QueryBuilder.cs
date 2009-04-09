using System;
using System.Text;

namespace Illuminate.Tools.Data.MySql
{
	/// <summary>
	/// This class is used the StringBuilder class to build queries
	/// </summary>
	public class QueryBuilder
	{
		#region Private Properties

		/// <summary>
		/// String storage of the query
		/// </summary>
		private StringBuilder _ReturnStr = new StringBuilder();

		#endregion

		#region Public Properties

		/// <summary>
		/// Appends a string to the query
		/// </summary>
		/// <param name="Str"></param>
		public void Append(string Str)
		{
			_ReturnStr.Append(" " + Str + " ");
		}

		/// <summary>
		/// Converst the string builder to a string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _ReturnStr.ToString();
		}

		#endregion
	}
}
