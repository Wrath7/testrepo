using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;

namespace Illuminate.Tools.Data.MySql
{
    public class BulkUpdate
    {
        protected Query _query;

        protected string _conString;

        protected string _baseQueryString;

        protected List<MappedParameter> _parameters;

        protected class MappedParameter
        {
            public string QueryName;
            public string PropertName;
            public PropertyInfo PropertyInfo;
            public ParameterCollection.FieldType Type;
        }


        public BulkUpdate(string connectionString, string queryString)
        {
            _conString = connectionString;
            _baseQueryString = queryString;

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
            StringBuilder cmdText = new StringBuilder();
            
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
                string singleUpdate = _baseQueryString;

                foreach (MappedParameter mp in _parameters)
                {
                    singleUpdate = singleUpdate.Replace(mp.QueryName, mp.QueryName + i.ToString());
                }

                cmdText.Append(singleUpdate + " ");
                i++;
            }

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
