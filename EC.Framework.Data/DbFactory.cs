using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection; 
using System.Text;
using System.Xml;
using System.Data.SqlClient;
using EC.Framework.Logger;

namespace EC.Framework.Data
{
    public static class DbFactory
    {
        #region Property(s)
        private static readonly ICustomLog m_Log = CustomLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //public static string ConnectionType = string.Empty;
        #endregion

        #region Method(s)

        static System.Reflection.PropertyInfo GetProperty(object entity, string propertyName)
        {
            // all oracle parameters start with a "p"
            if (propertyName.StartsWith("p"))
            {
                propertyName = propertyName.Substring(1, propertyName.Length - 1);
            }
            System.Reflection.PropertyInfo pi = entity.GetType().GetProperty(propertyName);
            if (pi == null)
            {
                string message = string.Format("Property {0} does not exist for object type {1}", 
                    propertyName, entity.GetType().Name); 
                //m_Log.Error(message);
                throw new ArgumentException(message); 
            }
            return pi; 
        }
        public static IDbCommand Add(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            Type propertyType = pi.PropertyType;

            if (propertyType == typeof(String))
                return AddString(command, entity, propertyName);
            else if (propertyType == typeof(XmlDocument))
                return AddXml(command, entity, propertyName);
            else
                return AddParameterOfGeneral(command, entity, propertyName);
        }

        public static IDbCommand AddGuid(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);

            command.Parameters.Add(new SqlParameter("@" + propertyName, value.ToString()));

            return command;
        }

        public static IDbCommand AddNullableGuid(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);

                if (value != null)
                    command.Parameters.Add(new SqlParameter("@" + propertyName, value.ToString()));
                else
                    command.Parameters.Add(new SqlParameter("@" + propertyName, DBNull.Value));

            return command;
        }

        public static IDbCommand AddString(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName);
            object value = pi.GetValue(entity, null);
            if (value != null)
                command.Parameters.Add(new SqlParameter("@" + propertyName, value.ToString()));
            else
                command.Parameters.Add(new SqlParameter("@" + propertyName, DBNull.Value));

            return command;
        }

        public static IDbCommand AddNText(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName);
            object value = pi.GetValue(entity, null);
            if (value != null)
                command.Parameters.Add(new SqlParameter("@" + propertyName, value.ToString()));
            else
                command.Parameters.Add(new SqlParameter("@" + propertyName, DBNull.Value));

            return command;
        }

        public static IDbCommand AddInt(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);

            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddNullableInt(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            if (value == null)
                value = DBNull.Value;

            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddBool(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            if ((bool)value == true)
                value = 1;
            else
                value = 0;
            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddNullableBool(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            if (value == null)
                value = DBNull.Value;
            else if ((bool)value == true)
                value = 1;
            else
                value = 0;

            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddDecimal(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddNullableDecimal(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            if (value == null)
                value = DBNull.Value;

            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddDateTime(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            if ((DateTime)value == new DateTime(1, 1, 1))
                value = new DateTime(1900, 1, 1);
            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddNullableDateTime(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            if (value == null)
                value = DBNull.Value;
            else if ((DateTime)value == new DateTime(1, 1, 1))
                value = new DateTime(1900, 1, 1);

            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddDouble(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddNullableDouble(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            if (value == null)
                value = DBNull.Value;

            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }

        public static IDbCommand AddXml(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            command.Parameters.Add(new SqlParameter("@" + propertyName, ((XmlDocument)value).OuterXml.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "")));

            return command;
        }

        private static IDbCommand AddParameterOfGeneral(IDbCommand command, object entity, string propertyName)
        {
            PropertyInfo pi = GetProperty(entity, propertyName); 
            object value = pi.GetValue(entity, null);
            if (value == null)
                value = DBNull.Value;

            command.Parameters.Add(new SqlParameter("@" + propertyName, value));

            return command;
        }
        #endregion
    }
}