using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Collections.Generic;


namespace Illuminate.Tools.Data.Sql
{
	/// <summary>
	/// Class helper to perform a query to the SQL database
	/// </summary>
	public class Query
	{
		public static bool WriteErrorsToLog = false;

		#region Private Properties

		/// <summary>
		/// The name of the query
		/// </summary>
		private string name = string.Empty;

		/// <summary>
		/// The dynamic parameter of the query
		/// </summary>
		private ParameterCollection parameters;

		/// <summary>
		/// The SQL command used by the Query
		/// </summary>
		private SqlCommand sqlCmd;

		/// <summary>
		/// The SqlConnection used by the Query
		/// </summary>
		private SqlConnection sqlCon;

		/// <summary>
		/// Determines whether the query will be traced for debugging purposes
		/// </summary>
		private bool traceQuery = false;

		private SqlTransaction sqlTran;

		#endregion

		#region Public Properties

		/// <summary>
		/// The name of the query
		/// </summary>
		public string Name 
		{
			get 
			{
				return name;
			}
		}

		/// <summary>
		/// The dynamic parameter of the query
		/// </summary>
		public ParameterCollection Parameters
		{
			get 
			{
				return parameters;
			}
		}

		/// <summary>
		/// The SQL command used by the Query
		/// </summary>
		public SqlCommand Command 
		{
			get 
			{
                return sqlCmd;
			}
		}

		/// <summary>
		/// Determines whether the query will be traced for debugging purposes
		/// </summary>
		public bool TraceQuery
		{
			get
			{
				return traceQuery;
			}
			set
			{
				traceQuery = value;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor for a Query
		/// </summary>
		/// <param name="CmdText">The SQL command you wish to execute</param>
		/// <param name="Name">The name of the query</param>
		/// <param name="ConnectionString">The connection string that the query will be executed againsts</param>
		public Query(string CmdText, string Name, string ConnectionString) 
		{
			if (CmdText.Length == 0) { throw new Exceptions.InvalidParameterException("Command Text cannot be zero length"); }
			if (Name.Length == 0) { throw new Exceptions.InvalidParameterException("Name cannot be zero length"); }

            sqlCon = new SqlConnection(ConnectionString);
			sqlCmd = new SqlCommand(CmdText, sqlCon);
            parameters = new ParameterCollection();
            name = Name;

		}

		public Query(string CmdText, Transaction transaction)
		{
			if (CmdText.Length == 0) { throw new Exceptions.InvalidParameterException("Command Text cannot be zero length"); }

			sqlCon = transaction.Connection;
			sqlTran = transaction.SqlTransaction;
			sqlCmd = new SqlCommand(CmdText, sqlCon);
			sqlCmd.Transaction = sqlTran;
			parameters = new ParameterCollection();
			name = "none";
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the command text of the SqlCommand object
		/// </summary>
		/// <param name="CmdText">The SQL command you wish to execute</param>
		public void SetCommandText(string CmdText) 
		{
			if (CmdText.Length == 0) { throw new Exceptions.InvalidParameterException("Command Text cannot be zero length"); }

			sqlCmd = new SqlCommand(CmdText, sqlCon);
            parameters = new ParameterCollection();
		}

		/// <summary>
		/// Runs the Query which produces no results
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int RunQueryNoResult()  
		{
			DateTime StartTime = DateTime.Now;
			int numRowsAffected = 0;

			try 
			{
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

				sqlCmd = parameters.Replace(sqlCmd);
				if (sqlTran != null) sqlCmd.Transaction = sqlTran;

				numRowsAffected = sqlCmd.ExecuteNonQuery();
			}
			catch (Exception e) 
			{
				if (WriteErrorsToLog)
				{
					//Logger.WriteLine(e.Message + "---" + sqlCmd.CommandText, Logger.Severity.Error, "Query");
				}
				else
				{
					throw new Exceptions.QueryErrorException(e.Message + "\r\n" + sqlCmd.CommandText);
				}
			}
			finally 
			{
                if (sqlCon != null) { sqlCon.Close(); }
			}

			DateTime EndTime = DateTime.Now;
			TimeSpan Sp = EndTime.Subtract(StartTime);

			if (traceQuery) QueryTrace.Add(name, sqlCmd.CommandText, Sp.TotalMilliseconds);

			return numRowsAffected;
		}

		/// <summary>
		/// Runs the query which produces a result
		/// </summary>
		/// <returns></returns>
		public DataTable RunQuery() 
		{
			DateTime StartTime = DateTime.Now;

			SqlDataAdapter sqlData = new SqlDataAdapter();
            string Exception = string.Empty;
            DataSet ds = new DataSet();

			try 
			{
				sqlCmd.CommandTimeout = 99999;
                sqlData = new SqlDataAdapter(sqlCmd);
                sqlCmd = parameters.Replace(sqlCmd);

				if (sqlTran != null) sqlCmd.Transaction = sqlTran;

                sqlData.Fill(ds, Name);
			} 
			catch (Exception e) 
			{
                Exception = e.Message;

				if (WriteErrorsToLog)
				{
					//Logger.WriteLine(e.Message + "---" + sqlCmd.CommandText, Logger.Severity.Error, "Query");
				}
				else
				{
					throw new Exceptions.QueryErrorException(e.Message + "\r\n" + sqlCmd.CommandText);
				}
			} 
			finally
			{
				sqlData.Dispose();
			}

			DateTime EndTime = DateTime.Now;
			TimeSpan Sp = EndTime.Subtract(StartTime);

			if (traceQuery) QueryTrace.Add(name, sqlCmd.CommandText, Sp.TotalMilliseconds);

            return ds.Tables[0];
		}

		#endregion
	}
}
