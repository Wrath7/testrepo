using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Communication
{
	[Serializable]
	public class Command
	{
		protected string _action;
		protected string _from;
		protected string _destination;
		protected List<string> _parameters;

		public string Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}

		public string From
		{
			get
			{
				return _from;
			}
			set
			{
				_from = value;
			}
		}

		public string Destination
		{
			get
			{
				return _destination;
			}
			set
			{
				_destination = value;
			}
		}

		public List<string> Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		public Command()
		{
			_parameters = new List<string>();
		}

		public Command(string Command)
		{
			Command cmd = Deserialize(Command);
			_action = cmd.Action;
			_from = cmd.From;
			_destination = cmd.Destination;
			_parameters = cmd.Parameters;
		}


		public override string ToString()
		{
			string command =  _action + " " + _from + " " + _destination + " ";

			if (_parameters != null)
			{
				for (int i = 0; i < _parameters.Count; i++)
				{
					command = command + _parameters[i];

					if (i != _parameters.Count - 1) command = command + " ";
				}
			}

			return command;
		}

		public static Illuminate.Communication.Command Deserialize(string Command)
		{
			Illuminate.Communication.Command d = new Illuminate.Communication.Command();

			string[] CmdParts = Command.Split(' ');
			d.Action = CmdParts[0];
			d.From = CmdParts[1];
			d.Destination = CmdParts[2];

			if (CmdParts.Length > 3)
			{
				d.Parameters = new List<string>(Command.Substring(d.Action.Length + d.From.Length + d.Destination.Length + 2).Trim().Split(' '));
			}

			return d;

		}

	}
}
