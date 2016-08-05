using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

namespace EC.Business
{
    public class Culture
    {
        public const string CultureKey = "culture"; 

        public static CultureInfo GetCulture()
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx == null || 
                ctx.Session == null)
                return GetDefault();
            CultureInfo ci = ctx.Session[CultureKey] as CultureInfo;
            if (ci == null)
                return GetDefault(); 

            return ci; 
        }

        static CultureInfo GetDefault()
        {
            return new CultureInfo("en-US"); 
        }
    }
}
