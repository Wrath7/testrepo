using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Text;

namespace Illuminate.Tools.Data.MySql
{
	/// <summary>
	/// Contains a list of parameters for a query
	/// </summary>
	public class ParameterCollection
	{

		#region Enum

		/// <summary>
		/// The different parameter types which can be a parameter
		/// </summary>
		public enum FieldType
		{
			/// <summary>
			/// Text only (including booleans for sql server 2005)
			/// </summary>
			Text = 0,
			/// <summary>
			/// Numeric (int, double, real, bit)
			/// </summary>
			Numeric = 1,
			/// <summary>
			/// A Date/Time parameter
			/// </summary>
			DateTime = 2,
			/// <summary>
			/// Same as text, but double byte version
			/// </summary>
			DoubleByteText = 3
		}

		#endregion

		#region Private Properties


		/// <summary>
		/// The collection to contain the items
		/// </summary>
		private ArrayList items = new ArrayList();

		#endregion

		#region Public Properties

		/// <summary>
		/// The number of parameters in the collection
		/// </summary>
		public int Count 
		{
			get 
			{
				return items.Count;
			}
		}

		/// <summary>
		/// Gets a parameter from an index
		/// </summary>
		/// <param name="Index">The index of the parameter you wish to retrieve</param>
		/// <returns></returns>
		public Parameter this[int Index]
		{
			get
			{
				if (Index <= items.Count && Index >= 0)
				{
					return (Parameter)items[Index];
				}
				else
				{
					throw new IndexOutOfRangeException("The index number which you have specified is out of range.  There are only" + items.Count + " item(s) in the collection");
				}
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor for the parameter collection
		/// </summary>
		public ParameterCollection()
		{
			items = new ArrayList();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Resets the paramter collection
		/// </summary>
		public void Reset()
		{
			items = new ArrayList();
		}

		/// <summary>
		/// Adds a parameter to the collection
		/// </summary>
		/// <param name="p">The parameter you wish to add</param>
		public void Add(Parameter p)
		{
            //for (int i = 0; i < items.Count; i++)
            //{
            //    if (this[i].Name == p.Name)
            //    {
            //        throw new Exceptions.DuplicateParameterException("Parementer already exists in collection\r\n\r\n" + p.Name);
            //    }
            //}

			items.Add(p);
		}

		/// <summary>
		/// Adds a parameter to the collection
		/// </summary>
		/// <param name="Name">The name of the parameter</param>
		/// <param name="Val">The value of the parameter</param>
		/// <param name="Type">The type of paramter you are adding</param>
		public void Add(string Name, object Val, FieldType Type)
		{
			Add(Name, Val, Type, false);
		}

		/// <summary>
		/// Adds a parameter to the collection
		/// </summary>
		/// <param name="Name">The name of the parameter</param>
		/// <param name="Val">The value of the parameter</param>
		/// <param name="Type">The type of paramter you are adding</param>
		/// <param name="Output">Determines whether the parameter is of type output</param>
		public void Add(string Name, object Val, FieldType Type, bool Output)
		{
			Parameter p = new Parameter(Name, Val, Type, Output);
			Add(p);
		}

		/// <summary>
		/// Replaces all the parameters from a SqlCommand with the values
		/// </summary>
		/// <param name="Cmd">The command you want to replace the parameters from</param>
		/// <returns></returns>
		public MySqlCommand Replace(MySqlCommand Cmd)
		{
			for (int i = 0; i < this.Count; i++)
			{
				ReplaceParameter(Cmd, this[i]);
			}

			return Cmd;
		}

		#endregion

		#region Private Method

		/// <summary>
		/// Replaces a parameter from a command
		/// </summary>
		/// <param name="cmd">The command you want to replace the parameter from</param>
		/// <param name="param">The parameter you want to peform the replace on</param>
		private void ReplaceParameter(MySqlCommand cmd, Parameter param)
		{
			string paramValue = string.Empty;

			if (param.Value.GetType() == typeof(string))
			{
				string Value = Tools.ConvertToUFT8(param.Value.ToString());
				
				cmd.Parameters.Add(new MySqlParameter(param.Name, Value));
			}
			else
			{
				cmd.Parameters.Add(new MySqlParameter(param.Name, param.Value));
			}
		}

		/// <summary>
		/// Pads a number with a 0 if the number is of single length
		/// </summary>
		/// <param name="i">The number you want to pad</param>
		/// <returns></returns>
		private string PadNumber(int i)
		{
			string s = i.ToString();
			if (s.Length == 1)
			{
				return "0" + s;
			}
			else
			{
				return s;
			}
		}


		#endregion
	}
}
