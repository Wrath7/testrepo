using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Illuminate.Tools.Data.Sql
{
	public class Transaction : IDisposable
	{
		#region Private Members

		private bool _IsDisposed = false;

		private SqlConnection _connection;
		private SqlTransaction _transaction;

		#endregion

		#region Properties

		public SqlConnection Connection
		{
			get { return _connection; }
		}

		public SqlTransaction SqlTransaction
		{
			get { return _transaction; }
		}

		#endregion

		public Transaction(string ConnectionString)
		{
			_connection = new SqlConnection(ConnectionString);
			_connection.Open();
			_transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public void Commit()
		{
			_transaction.Commit();

			Cleanup();
		}

		public void RollBack()
		{
			_transaction.Rollback();

			Cleanup();
		}

		private void Cleanup()
		{
			if (_transaction != null)
			{
				_transaction.Dispose();
				_transaction = null;
			}

			if (_connection != null)
			{
				_connection.Close();
				_connection.Dispose();
				_connection = null;
			}
		}

		#region IDisposable Interface

		~Transaction()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			// Tell the garbage collector not to call the finalizer
			// since all the cleanup will already be done.
			GC.SuppressFinalize(true);
		}

		protected virtual void Dispose(bool IsDisposing)
		{
			if (_IsDisposed)
				return;

			if (IsDisposing)
			{
				// Free any managed resources in this section
				if (_transaction != null)
				{
					//Disposing but have not committed, force rollback to cleanup
					_transaction.Rollback();
				}

				Cleanup();
			}

			// Free any unmanaged resources in this section
			_IsDisposed = true;
		}

		#endregion
	}
}
