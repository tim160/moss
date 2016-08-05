using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Common
{
    public class Users
    {
        public readonly static long SiteAdminById = 1;
        public readonly static long CsiApiById = 2;
        public readonly static long HostCsiById = 3;

        public readonly static long[] SystemUsersById = new long[] { SiteAdminById, CsiApiById, HostCsiById };
        public readonly static long[] HiddenUsersById = new long[] { CsiApiById }; 
    }
}
