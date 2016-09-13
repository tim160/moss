using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Common.Base
{
    public class SQLUtils
    {
        /// <summary>
        /// Apparently SQL Server can't deal with dates smaller than this.
        /// </summary>
        
        public static DateTime MIN_SQL_DATE = new DateTime(1753, 1, 1);
    }
}
