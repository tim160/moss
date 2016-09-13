using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown when an attribute is null.
    /// </summary>

    public class NullAttributeException : FaultableException<NullAttributeFault>
    {
        public override NullAttributeFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new NullAttributeFault(Message, path, userInfo);
            return f;
        }

        public NullAttributeException() : base("Attributes cannot be null")
        {
        }
    }

    /// <summary>
    /// Fault if attribute is null
    /// </summary>

    [DataContract]
    public class NullAttributeFault : BasicFault
    {
        public NullAttributeFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
