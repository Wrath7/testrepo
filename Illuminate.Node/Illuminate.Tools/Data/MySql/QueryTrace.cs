using System;
using System.Collections;
using System.Text;

namespace Illuminate.Tools.Data.MySql
{
	/// <summary>
	/// Class to track queries for debugging purposes
	/// </summary>
	public class QueryTrace
	{
		/// <summary>
		/// List containing all the queries which were tracked
		/// </summary>
		public static ArrayList TraceList = new ArrayList();

		/// <summary>
		/// The total execution time for all queries which were tracked
		/// </summary>
		public static double TotalExecutionTime = 0;

		/// <summary>
		/// Adds a query to the list
		/// </summary>
		/// <param name="Name">The name of the query</param>
		/// <param name="Query">The SQL command which was ran, with the parameter replaced</param>
		/// <param name="ExecutionTime">The amount of time the query took to execute</param>
		public static void Add(string Name, string Query, double ExecutionTime)
		{
			Query = "<hr/>" + Query + "<br/><br/><strong>Execution Time: " + ExecutionTime.ToString() + "ms</strong>";

			TraceList.Add(Query);

			TotalExecutionTime = TotalExecutionTime + ExecutionTime;
		}

		/// <summary>
		/// Converts the list into HTML for debugging purposes
		/// </summary>
		/// <returns></returns>
		public static new string ToString()
		{
			StringBuilder Sb = new StringBuilder();

			Sb.Append("<hr/><span style='font-weight: bold; color: maroon'>Total Query Execution Time: " + TotalExecutionTime.ToString() + "ms</span>");

			for (int i = 0; i < TraceList.Count; i++)
			{
				Sb.Append((string)TraceList[i]);
			}

			return Sb.ToString();

		}

		/// <summary>
		/// Resets the Trace list
		/// </summary>
		public static void Reset()
		{
			TotalExecutionTime = 0;
			TraceList = new ArrayList();
		}
	}
}
