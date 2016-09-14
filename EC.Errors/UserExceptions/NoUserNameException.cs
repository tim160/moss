using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Thrown if a user doesn't have a user name. NOTE: I think this should actually 
    /// be a ModelValidationException.
    /// </summary>

    public class NoUserNameException : FaultableException<NoUserNameFault>
    {
        public override NoUserNameFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NoUserNameFault(Message, reqPath, userInfo);
            return f;
        }

        public NoUserNameException(string msg, Exception innerException = null) : base(msg, innerException)
        {
        }
    }

    /// <summary>
    /// Thrown if a user doesn't have a user name.
    /// </summary>

    [DataContract]
    public class NoUserNameFault : BasicFault
    {
        public NoUserNameFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
