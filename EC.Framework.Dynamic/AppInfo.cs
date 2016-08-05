using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace EC.Framework.Dynamic
{
    public static class AppInfo
    {
        public static Dictionary<Type, InstantiateObjectHandler> EntityDictionary = new Dictionary<Type, InstantiateObjectHandler>();
        public static Dictionary<PropertyInfo, SetHandler> SetDictionary = new Dictionary<PropertyInfo, SetHandler>();
        public static Dictionary<PropertyInfo, GetHandler> GetDictionary = new Dictionary<PropertyInfo, GetHandler>();
    }
}