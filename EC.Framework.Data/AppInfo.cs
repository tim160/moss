using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EC.Framework.Data
{
    public static class AppInfo
    {
        public static string DatabaseName = string.Empty;
        public static Dictionary<string, Dictionary<int, string>> Dictionary = new Dictionary<string, Dictionary<int, string>>();
        public static Dictionary<string, PropertyInfo[]> PropertyDictionary = new Dictionary<string, PropertyInfo[]>();
        public static Dictionary<string, string> ObjectNameDictionary = new Dictionary<string, string>();

        public static PropertyInfo[] GetPropertiesInfo(Type persistentType)
        {
            if (PropertyDictionary.ContainsKey(persistentType.Name))
            {
                return PropertyDictionary[persistentType.Name];
            }
            else
            {
                PropertyInfo[] propertyInfos = persistentType.GetProperties();
                List<PropertyInfo> persistableProperties = new List<PropertyInfo>(); 
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (propertyInfo.GetCustomAttributes(typeof(NotPersistableAttribute), true).Length == 0)
                        persistableProperties.Add(propertyInfo); 
                }
                PropertyInfo[] persistablePropertiesArr = persistableProperties.ToArray(); 
                lock (PropertyDictionary)
                {
                    //If locked by another thread, the waiting thread now has the lock and should now recheck the dictionary.
                    if (PropertyDictionary.ContainsKey(persistentType.Name))
                        return PropertyDictionary[persistentType.Name];
                    
                    PropertyDictionary.Add(persistentType.Name, persistablePropertiesArr);
                }
                return persistablePropertiesArr;
            }
        }

        public static string GetTableName(Type persistentType)
        {
            if (ObjectNameDictionary.ContainsKey(persistentType.Name))
            {
                return ObjectNameDictionary[persistentType.Name];
            }
            else
            {
                // get the custom TableAttribute from the persistent type
                TableAttribute[] tas = (TableAttribute[])persistentType.GetCustomAttributes(typeof(TableAttribute), false);
                // if there were no custom TableAttributes found check the base type 
                if (tas.Length == 0)
                    tas = (TableAttribute[])persistentType.BaseType.GetCustomAttributes(typeof(TableAttribute), false);
                if (tas.Length == 0)
                {
                    throw new ArgumentException(
                        string.Format(
                            "Persistent type: {0} does not have any table attribute defined, cannot be used by Data Object Framework.",
                            persistentType.Name));
                }
                string tableName = tas[0].TableName;
                lock (ObjectNameDictionary)
                {
                    //If locked by another thread, the waiting thread now has the lock and should now recheck the dictionary.
                    if (ObjectNameDictionary.ContainsKey(persistentType.Name))
                        return ObjectNameDictionary[persistentType.Name];

                    ObjectNameDictionary.Add(persistentType.Name, tableName);
                }
                return tableName;
            }
        }

        public static bool IsView(Type persistentType)
        {
            // get the custom TableAttribute from the persistent type
            TableAttribute[] tas = (TableAttribute[])persistentType.GetCustomAttributes(typeof(TableAttribute), false);
            // if there were no custom TableAttributes found check the base type 
            if (tas.Length == 0)
                tas = (TableAttribute[])persistentType.BaseType.GetCustomAttributes(typeof(TableAttribute), false);
            if (tas.Length == 0)
            {
                throw new ArgumentException(
                    string.Format(
                        "Persistent type: {0} does not have any table attribute defined, cannot be used by Data Object Framework.",
                        persistentType.Name));
            }
            return tas[0].IsView;
        }
    }
}