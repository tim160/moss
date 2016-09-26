
using System;
using System.Collections.Generic;
using System.Linq;

namespace EC.Core.Common
{

    /// <summary>
    /// Contains constant strings for role names.
    /// These role names are used for static RoleProvider access to administration pages. 
    /// </summary>
    
    public static class RoleConstants
    {
        /// <summary>
        /// 'SuperAdmin' role name string.
        /// </summary>
        
        public const string SUPER_ADMIN = "SuperAdmin";

        /// <summary>
        /// 'NormalUser' role name string.
        /// </summary>
        
        public const string NORMAL_USER = "NormalUser";
    }

    /// <summary>
    /// Stats bin templates and generator.
    /// </summary>

    public static class StatsCollectorConstants
    {
        /// <summary>
        /// 10 bin array from [0-2] seconds with 200ms intervals between the bins.
        /// </summary>

        public static readonly double[] BIN_200_MS_2_SEC = { 200, 400, 600, 800, 1000, 1200, 1400, 1600, 1800, 2000 };

        /// <summary>
        /// 20 bin array from [0-1] seconds with 50ms intervals between the bins.
        /// </summary>

        public static readonly double[] BIN_50_MS_1_SEC = { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000 };

        /// <summary>
        /// 51 bin array from [0-4] seconds. 1ms intervals between [1-25], 25ms intervals between [0-250], 50ms intervals between [250-500]ms, 250ms intervals between [500-4000]ms.
        /// </summary>

        public static readonly double[] BIN_SPECIAL_4_SEC = { 
                    1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,
                    25, 50, 75, 100, 125, 150, 175, 200, 225, 250, 300, 350, 400, 450, 500, 
                    750, 1000, 
                    1250, 1500, 1750, 2000, 
                    2250, 2500, 2750, 3000, 
                    3250, 3500, 3750, 4000 };

        /// <summary>
        /// 20 bin histogram with reasonably high precision at the lower end
        /// </summary>

        public static readonly double[] BIN_DEFAULT_TIMING = { 1, 2, 5, 10, 15, 20, 25, 50, 75, 100, 150, 200, 250, 500, 750, 1000, 1500, 2000, 3000, 4000 };

        /// <summary>
        /// 5 min flush period in ms.
        /// </summary>

        public static readonly int FIVE_MIN_FLUSH_PERIOD = 300000;

        /// <summary>
        /// 1 min flush period in ms.
        /// </summary>

        public static readonly int ONE_MIN_FLUSH_PERIOD = 60000;

        /// <summary>
        /// 30 sec. flush period in ms.
        /// </summary>

        public static readonly int HALF_MIN_FLUSH_PERIOD = 30000;
    }
}
