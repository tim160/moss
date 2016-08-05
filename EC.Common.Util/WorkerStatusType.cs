using System;
using System.Reflection;

namespace EC.Common.Util
{
    public enum WorkerStatusType
    {
        NoStatus = -1,
        Default = 0,
        [InnerStringValue("Signed On")]
        SignOn = 10,
        Enroute = 20,
        OnSite = 30,
        [InnerStringValue("Signed Off")]
        SignOff = 40
    }
    public class InnerStringValue : Attribute
    {
        private string _value;

        public InnerStringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

    }

    public static class StringEnum
    {
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            InnerStringValue[] attrs = fi.GetCustomAttributes(typeof(InnerStringValue), false) as InnerStringValue[];

            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }
}
