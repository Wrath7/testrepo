using System;
using System.Collections;
using System.Text;

namespace Illuminate.Tools.Data.Sql
{
	/// <summary>
	/// Class to track queries for debugging purposes
	/// </summary>
	public class QueryTrace
	{
		private static string _queryConnectionString = string.Empty;

		/// <summary>
		/// Adds a query to the list
		/// </summary>
		/// <param name="Name">The name of the query</param>
		/// <param name="Query">The SQL command which was ran, with the parameter replaced</param>
		/// <param name="ExecutionTime">The amount of time the query took to execute</param>
		public static void Add(string Name, string Query, double ExecutionTime)
		{
			if (_queryConnectionString.Length == 0) _queryConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["QueryDebugConnection"].ConnectionString.ToString();

			Query q = new Query("insert into QueryTracking (Name, Query, ExecutionTime) values (@Name, @Query, @ExecutionTime)", "InsertExecutionTime", _queryConnectionString);
			q.Parameters.Add("@Name", Name, ParameterCollection.FieldType.Text);
			q.Parameters.Add("@Query", Query, ParameterCollection.FieldType.Text);
			q.Parameters.Add("@ExecutionTime", ExecutionTime, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}
	}
}
