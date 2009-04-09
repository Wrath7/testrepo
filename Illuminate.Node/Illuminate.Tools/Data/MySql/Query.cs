using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;


namespace Illuminate.Tools.Data.MySql
{
	/// <summary>
	/// Class helper to perform a query to the SQL database
	/// </summary>
	public class Query
	{
		public static void test()
		{
			MySqlConnection con = new MySqlConnection("server=192.168.1.236;uid=root;pwd=1q2w3e4r!;database=IlluminateExtraction");
			MySqlCommand cmd = new MySqlCommand("insert into CategoryNgrams (CategoryId, NGram, Weight, DocumentFrequency, TotalFrequency) values (?CategoryId, ?NGram, ?Weight, ?DocumentFrequency, ?TotalFrequency);", con);
			cmd.Parameters.AddWithValue("?CategoryId", 1);
			cmd.Parameters.AddWithValue("?NGram", "apc's");
			cmd.Parameters.AddWithValue("?Weight", 0.44455);
			cmd.Parameters.AddWithValue("?DocumentFrequency", 100);
			cmd.Parameters.AddWithValue("?TotalFrequency", 1);
			con.Open();
			cmd.ExecuteNonQuery();
			con.Close();

		}

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
		private MySqlCommand sqlCmd;

		/// <summary>
		/// The SqlConnection used by the Query
		/// </summary>
		private MySqlConnection sqlCon;

		private MySqlTransaction sqlTran;

		/// <summary>
		/// Determines whether the query will be traced for debugging purposes
		/// </summary>
		private bool traceQuery = false;

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
		public MySqlCommand Command 
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
		public Query(string CmdText, string ConnectionString) 
		{
			if (CmdText.Length == 0) { throw new Exceptions.ErrorException("Command Text cannot be zero length"); }

			sqlCon = new MySqlConnection(ConnectionString);
			sqlCmd = new MySqlCommand(CmdText, sqlCon);
            parameters = new ParameterCollection();
		}

		public Query(string CmdText, string Name, string ConnectionString)
		{
			//if (CmdText.Length == 0) { throw new Exceptions.InvalidParameterException("Command Text cannot be zero length"); }

			sqlCon = new MySqlConnection(ConnectionString);
			sqlCmd = new MySqlCommand(CmdText, sqlCon);
			parameters = new ParameterCollection();
		}

		public Query(string CmdText, Transaction transaction)
		{
			if (CmdText.Length == 0) { throw new Exceptions.ErrorException("Command Text cannot be zero length"); }

			sqlCon = transaction.Connection;
			sqlTran = transaction.SqlTransaction;
			sqlCmd = new MySqlCommand(CmdText, sqlCon);
			sqlCmd.Transaction = sqlTran;

			parameters = new ParameterCollection();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the command text of the SqlCommand object
		/// </summary>
		/// <param name="CmdText">The SQL command you wish to execute</param>
		public void SetCommandText(string CmdText) 
		{
			if (CmdText.Length == 0) { throw new Exceptions.ErrorException("Command Text cannot be zero length"); }

			sqlCmd.CommandText = CmdText;
			//sqlCmd = new MySqlCommand(CmdText, sqlCon);
            //parameters = new ParameterCollection();
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
				sqlCmd.CommandTimeout = 9999;
				numRowsAffected = sqlCmd.ExecuteNonQuery();
			}
			catch (Exception e) 
			{

				throw new Exceptions.ErrorException(e.Message + "\r\n" + sqlCmd.CommandText);
			}
			finally 
			{
				if (sqlCmd.Transaction == null) if (sqlCon != null) { sqlCon.Close(); }
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

			MySqlDataAdapter sqlData = new MySqlDataAdapter();
            string Exception = string.Empty;
            DataSet ds = new DataSet();

			try 
			{
				sqlCmd.CommandTimeout = 99999;
				sqlData = new MySqlDataAdapter(sqlCmd);
                sqlCmd = parameters.Replace(sqlCmd);
                sqlData.Fill(ds, "MysqlFill");
			} 
			catch (Exception e) 
			{
                Exception = e.Message;

				throw new Exceptions.ErrorException(e.Message + "\r\n" + sqlCmd.CommandText); 
				
			} 
			finally
			{
				sqlData.Dispose();

				if (sqlCmd.Transaction == null) if (sqlCon != null) { sqlCon.Close(); }
			}

			DateTime EndTime = DateTime.Now;
			TimeSpan Sp = EndTime.Subtract(StartTime);

			if (traceQuery) QueryTrace.Add(name, sqlCmd.CommandText, Sp.TotalMilliseconds);

            return ds.Tables[0];
		}

		#endregion
	}
}
