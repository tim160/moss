using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Library.Xml
{
    public class XmlString
    {
        public static string Escape(string value)
        {
            return value.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");
        }

        public static bool IsValidForXml(string value)
        {
            return !(value.IndexOf("&") >= 0 ||
                value.IndexOf("<") >= 0 ||
                value.IndexOf(">") >= 0 ||
                value.IndexOf("\"") >= 0);
        }
    }
}
