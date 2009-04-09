using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;

namespace Illuminate.Tools.Data.MySql
{
    /// <summary>
    /// Class helper to perform a bulk insert to the MySQL database
    /// </summary>
    public class BulkCopy
    {
        protected Query _query;

        protected string _conString;

        protected string _baseQueryString;

        protected List<MappedParameter> _parameters;

        protected string _valueRow;

        protected class MappedParameter
        {
            public string QueryName;
            public string PropertName;
            public PropertyInfo PropertyInfo;
            public ParameterCollection.FieldType Type;
        }


        public BulkCopy(string connectionString, string queryString, string valueRow)
        {
            _conString = connectionString;
            _baseQueryString = queryString;
            _valueRow = valueRow;

            _parameters = new List<MappedParameter>();
        }

        public void AddParamter(string queryName, string propertyName, ParameterCollection.FieldType type)
        {
            MappedParameter parameter = new MappedParameter();
            parameter.QueryName = queryName;
            parameter.PropertName = propertyName;
            parameter.Type = type;

            _parameters.Add(parameter);
        }

        public void WriteToServer(IEnumerable data)
        {
            StringBuilder cmdText = new StringBuilder(_baseQueryString);
            
            IEnumerator ie = data.GetEnumerator();
            ie.MoveNext();

            object first = ie.Current;

            Type objectType = first.GetType();
            for (int j = 0; j < _parameters.Count; j++ )
            {
                MappedParameter mp = _parameters[j];
                mp.PropertyInfo = objectType.GetProperty(mp.PropertName);
            }

            int i = 0;
            foreach (object o in data)
            {
                string row = _valueRow;

                foreach (MappedParameter mp in _parameters)
                {
                    row = row.Replace(mp.QueryName, mp.QueryName + i.ToString());
                }

                cmdText.Append(row + ", ");
                i++;
            }

            cmdText = cmdText.Remove(cmdText.Length - 2,2);

            Query q = new Query(cmdText.ToString(), _conString);

            i = 0;
            foreach (object o in data)
            {
                foreach (MappedParameter mp in _parameters)
                {
                    q.Parameters.Add(mp.QueryName + i.ToString(), mp.PropertyInfo.GetValue(o, null), mp.Type);
                }
                i++;
            }

            q.RunQueryNoResult();
        }
    }
}
