using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// These are constants used as cookie names. Since cookies will be shared between the
    /// main application and the extension, extension cookie names are included in here as
    /// well.
    /// </summary>
    
    public static class CookieConstants
    {
        /// <summary>
        /// 'EC'. Name to identify the cookie.
        /// </summary>

        public const string COOKIE_NAME = "EC";

        /// <summary>
        /// Cookie lifetime in days.
        /// </summary>

        public const int COOKIE_LIFETIME_IN_DAYS = 5;

    }

    /// <summary>
    /// These are constants for temp data slots (essentially session data). Because this data
    /// is actually stored in the cookie, extension slot names are included here as well.
    /// </summary>
    
    public static class TempDataConstants
    {
        /// <summary>
        /// Temp data slot used to identify the first request of a session.
        /// </summary>
        
        public const string ExtSessionExists = "EXT_SESSION_EXISTS";  
    }
}
