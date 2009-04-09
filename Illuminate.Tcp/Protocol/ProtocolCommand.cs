using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Illuminate.Tcp.Protocol
{
	[Serializable]
	public class ProtocolCommand : IProtocolCommand, IComparable
	{
		//TODO
		private IPAddress _from = null;

		//TODO
		private IPAddress _to = null;

		private string _command = string.Empty;
		private List<object> _parameters = new List<object>();
		private Guid _id = Guid.NewGuid();

		public IPAddress From
		{
			get
			{
				return _from;
			}
		}

		public IPAddress To
		{
			get
			{
				return _to;
			}
		}

		public string Command
		{
			get
			{
				return _command;
			}
		}

		public List<object> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		public Guid Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public ProtocolCommand(string command)
		{
			_command = command;
		}

		public static byte[] Serialize(IProtocolCommand command)
		{
			StringBuilder Sb = new StringBuilder();
			Sb.Append(command.Command + "`" + command.From + "`" + command.To + "`" + command.Id.ToString() + "`");

			foreach (object o in command.Parameters)
			{
				Sb.Append(o.ToString() + "`");
			}

			string commandStr = Sb.ToString(0, Sb.Length - 1);

			System.Text.ASCIIEncoding encoding = new ASCIIEncoding();
			return encoding.GetBytes(commandStr);
		}

		public static IProtocolCommand Deserialize(byte[] buffer)
		{
			System.Text.ASCIIEncoding encoding = new ASCIIEncoding();
			string commandStr = encoding.GetString(buffer);

			int nullPosition = commandStr.IndexOf('\0');

			if (nullPosition != -1)
			{
				commandStr = commandStr.Substring(0, nullPosition);
			}

			string[] commandParts = commandStr.Split('`');

			IProtocolCommand command = new ProtocolCommand(commandParts[0]);
			//command.From = commandParts[1];
			//command.To = commandParts[2];
			command.Id = new Guid(commandParts[3]);

			for (int i = 4; i < commandParts.Length; i++)
			{
				command.Parameters.Add(commandParts[i]);
			}

			return command;
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is ProtocolCommand)
			{
				ProtocolCommand otherProtocolCommand = (ProtocolCommand)obj;
				return this.Command.CompareTo(otherProtocolCommand.Command);
			}
			else
			{
				throw new Exception("Object is not command");
			}
		}

		#endregion
	}
}
