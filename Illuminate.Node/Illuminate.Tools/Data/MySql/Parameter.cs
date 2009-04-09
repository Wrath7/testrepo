using System;
using System.Data;

namespace Illuminate.Tools.Data.MySql
{
	/// <summary>
	/// A parameter for SQL objects
	/// </summary>
	public class Parameter
	{
		#region Private Properties

		/// <summary>
		/// The name of the parameter
		/// </summary>
		private string _Name = string.Empty;

		/// <summary>
		/// The value of the parameter
		/// </summary>
		private object _Value = string.Empty;

		/// <summary>
		/// The type of parameter
		/// </summary>
		private ParameterCollection.FieldType  _Type = ParameterCollection.FieldType.Text;

		/// <summary>
		/// Determines if the parameter is of type output
		/// </summary>
		private bool _Output = false;

		#endregion

		#region Public Properties

		/// <summary>
		/// The name of the parameter
		/// </summary>
		public string Name 
		{

			get
			{
				return _Name;
			}

			set
			{
				if (value.Length == 0) { throw new Exceptions.ErrorException("Name can not be zero length"); }
				_Name = value;
			}
		}

		/// <summary>
		/// The value of the parameter
		/// </summary>
		public object Value
		{
			get 
			{
				return _Value;
			}
			set 
			{
				SetValue();
				_Value = value;
			}
		}

		/// <summary>
		/// The type of parameter
		/// </summary>
		public ParameterCollection.FieldType Type
		{
			get
			{
				return _Type;
			}
			set {
				_Type = value;
			}
		}

		/// <summary>
		/// Determines if the parameter is of type output
		/// </summary>
		public bool Output
		{
			get
			{
				return _Output;
			}
		}
		#endregion

		#region Constructors

		/// <summary>
		/// Constructor fot the parameter
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="Val">The value of the parameter</param>
		/// <param name="Type">The type of parameter</param>
		/// <param name="Output">Determines if the parameter is of type output</param>
		public Parameter(string name, object Val, ParameterCollection.FieldType Type, bool Output)
		{
			if (name.Length == 0) { throw new Exceptions.ErrorException("Name can not be zero length"); }

			_Type = Type;
			_Name = name;
			_Value = Val;
			_Output = Output;

			SetValue();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the default value if ther is no value passed to the parameter contstructor
		/// </summary>
		private void SetValue() 
		{
			if (_Type == ParameterCollection.FieldType.Numeric) 
			{
				if (_Value == null) 
				{
					_Value = 0;
				}
				else if (! Tools.IsNumeric(_Value.ToString())) 
				{
					_Value = 0;
				} 
			} 
			else if (_Type == ParameterCollection.FieldType.DateTime) 
			{
				if ( _Value != null) 
				{

					try 
					{
						DateTime d = DateTime.Parse(_Value.ToString());
					}
					catch (Exception)
					{
						_Value = new DateTime(1900,1,1);
					}
				}
				else 
				{
					_Value = new DateTime(1900,1,1);
				}
			}
			else if (_Type == ParameterCollection.FieldType.Text || _Type == ParameterCollection.FieldType.DoubleByteText)
			{
				if (_Value == null) 
				{
					_Value = string.Empty;
				}
			}
		}

		#endregion

	}
}
