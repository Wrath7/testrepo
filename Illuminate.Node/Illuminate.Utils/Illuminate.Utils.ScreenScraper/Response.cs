using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminate.Utils.ScreenScraper
{
	public sealed class Response
	{
		private int _returnCode = 900;
		private string _content = string.Empty;
		private byte[] _binaryData = null;
		private string _statusMessage = string.Empty;
		private Dictionary<string, string> _headers = new Dictionary<string,string>();

		public int ReturnCode
		{
			get
			{
				return _returnCode;
			}
			set
			{
				_returnCode = value;
			}
		}

		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}

		public string StatusMessage
		{
			get
			{
				return _statusMessage;
			}
			set
			{
				_statusMessage = value;
			}
		}

		public int ContentLength
		{
			get
			{
				return _binaryData.Length;
			}
		}

		public byte[] BinaryData
		{
			get
			{
				return _binaryData;
			}
			set
			{
				_binaryData = value;
			}
		}

		public Dictionary<string, string> Headers
		{
			get
			{
				return _headers;
			}
		}

		public Response()
		{

		}
	}
}
