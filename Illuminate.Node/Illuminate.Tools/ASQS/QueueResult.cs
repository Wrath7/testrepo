using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Tools.ASQS
{
	public class QueueResult
	{
		public enum GazaroASQSErrorType
		{
			Amazon = 0,
			dotNet = 1,
			None = 2
		}

		private bool _success = false;
		private string _responseData = string.Empty;

		private bool _isError = false;
		private Exception _exception = null;
		private string _errorMessage = string.Empty;
		private GazaroASQSErrorType _errorType = GazaroASQSErrorType.None;

		public bool Success
		{
			get
			{
				return _success;
			}
			set
			{
				_success = value;
			}
		}

		public string ResponseData
		{
			get
			{

				return _responseData;
			}
			set
			{

				_responseData = value;
			}
		}

		public bool isError
		{
			get
			{
				return _isError;
			}
			set
			{
				_isError = value;
			}
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
			set
			{
				_exception = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}
			set
			{
				_errorMessage = value;
			}
		}

		public GazaroASQSErrorType ErrorType
		{
			get
			{
				return _errorType;
			}
			set
			{
				_errorType = value;
			}
		}
	}
}
