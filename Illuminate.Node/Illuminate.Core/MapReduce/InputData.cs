using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.MapReduce
{
	[Serializable]
	public class InputData : Illuminate.MapReduce.IInputData
	{
		private object _key;
		private object _value;
		private object _intermediateResult = null;
		private bool _done = false;
		private int _errorCode = 0;
		private string _errorMessage = string.Empty;

		public object Key
		{
			get
			{
				return _key;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
		}

		public bool Done
		{
			get
			{
				return _done;
			}
			set
			{
				_done = value;
			}
		}

		public int ErrorCode
		{
			get
			{
				return _errorCode;
			}
			set
			{
				_errorCode = value;
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

		public object IntermediateResult
		{
			get
			{
				return _intermediateResult;
			}
			set
			{
				_intermediateResult = value;
			}
		}

		public InputData(object key, object value)
		{
			_key = key;
			_value = value;
		}

	}
}
