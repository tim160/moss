using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    
    /// <summary>
    /// Exception thrown when a capability name does not have the correct format.
    /// </summary>

    public class InvalidCapabilityException : FaultableException<InvalidCapabilityFault>
    {
        public override InvalidCapabilityFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new InvalidCapabilityFault(Message, reqPath, userInfo);
            return f;
        }

        public InvalidCapabilityException(string msg) : base(msg)
        {
        }
    }

    /// <summary>
    /// Fault thrown when a capability name does not have the correct format.
    /// </summary>

    [DataContract]
    public class InvalidCapabilityFault : BasicFault
    {
        public InvalidCapabilityFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
