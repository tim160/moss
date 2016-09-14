using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown when a capability does not match the currently defined
    /// set of capabilities.
    /// </summary>

    public class CapabilityNotFoundException : FaultableException<CapabilityNotFoundFault>
    {
        public override CapabilityNotFoundFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new CapabilityNotFoundFault(Message, reqPath, userInfo);
            f.CapabilityName = CapabilityName;
            return f;
        }

        public CapabilityNotFoundException(string capabilityName) : base("Capability not found")
        {
            CapabilityName = capabilityName;
        }

        public string CapabilityName { get; set; }
    }


    /// <summary>
    /// Fault thrown when a capability cannot be found.
    /// </summary>

    [DataContract]
    public class CapabilityNotFoundFault : BasicFault
    {
        public CapabilityNotFoundFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string CapabilityName;
    }
}
