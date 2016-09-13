using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// This exception indicates that a user logon is required.
    /// </summary>

    public class AuthenticationRequiredException : FaultableException<AuthenticationRequiredFault>
    {
        public override AuthenticationRequiredFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new AuthenticationRequiredFault(Message, path, userInfo);
            return f;
        }

        public AuthenticationRequiredException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Fault returned if a logon/authentication is required.
    /// </summary>

    [DataContract]
    public class AuthenticationRequiredFault : BasicFault
    {
        public AuthenticationRequiredFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
