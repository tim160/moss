using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EC.Business
{
    public class CSVEntity
    {
        /// <summary>
        /// Converts this object to a comma separated string.  The specified columns must have a get accessor
        /// defined of the same name.
        /// </summary>
        /// <param name="columns">Names of the column names to include</param>
        /// <returns></returns>
        public virtual string ToCSVString(string[] columns)
        {
            string result = string.Empty;
            List<object> values = new List<object>();
            foreach (string column in columns)
            {
                if (!string.IsNullOrWhiteSpace(column))
                {
                    PropertyInfo pi = this.GetType().GetProperty(column);                    
                    if (pi != null)
                    {
                        object value = pi.GetValue(this, null);
                        string csvValue = string.Empty;
                        if (value is DateTime)
                        {
                            csvValue = ((DateTime)value).ToString("s");                            
                        }
                        else if (value != null)
                        {
                            csvValue = value.ToString().Replace("\"", "\"\"\"\"");                            
                        }
                        csvValue = string.Format("\"{0}\"", csvValue);
                        values.Add(csvValue);
                    }
                }
            }
            result = string.Join(",", values);
            return result;
        }

        /// <summary>
        /// Converts this object to a comma separated string.  The specified columns must have a get accessor
        /// defined of the same name.
        /// </summary>
        /// <param name="columns">Names of the column names to include</param>
        /// <param name="mappedFields">The column names to be used in place of the respective column name 
        /// in the resulting string.  For example, OrderStatusId maps to OrderStatusName means the OrderStatusName
        /// will be in the resulting comma separated string.</param>
        /// <returns></returns>
        public virtual string ToCSVString(string[] columns, Dictionary<string, string> mappedFields)
        {
            string result = string.Empty;
            List<object> values = new List<object>();
            foreach (string column in columns)
            {
                if (!string.IsNullOrWhiteSpace(column))
                {
                    string mappedColumn = column;
                    if (mappedFields != null && mappedFields.ContainsKey(column))
                    {
                        mappedColumn = mappedFields[column];
                    }
                    if (!string.IsNullOrWhiteSpace(mappedColumn))
                    { 
                        PropertyInfo pi = this.GetType().GetProperty(mappedColumn);
                        if (pi != null)
                        {
                            object value = pi.GetValue(this, null);
                            string csvValue = string.Empty;
                            if (value is DateTime)
                            {
                                csvValue = ((DateTime)value).ToString("s");
                            }
                            else if (value != null)
                            {
                                csvValue = value.ToString().Replace("\"", "\"\"\"\"");
                            }
                            csvValue = string.Format("\"{0}\"", csvValue);
                            values.Add(csvValue);
                        }
                    }
                }
            }
            result = string.Join(",", values);
            return result;
        }
    }
}
