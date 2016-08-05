using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common
{
    class Status
    {
        public readonly static string Pending = "Pending";
        public readonly static Int32 Pending_Value = 1;

        public readonly static string Active = "Active";
        public readonly static Int32 Active_Value = 2;

        public readonly static string Inactive = "Inactive";
        public readonly static Int32 Inactive_Value = 3;
    }
}
