using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception used to indicate that a capability attribute is syntactically invalid 
    /// </summary>

    public class WrongCapabilityAttributeFormat : FaultableException<WrongCapabilityAttributeFormatFault>
    {
        public override WrongCapabilityAttributeFormatFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new WrongCapabilityAttributeFormatFault(Message, reqPath, userInfo);
            f.CapabilityName = CapabilityName;
            f.AttributId = AttributeId;
            return f;
        }

        public WrongCapabilityAttributeFormat(string message, Guid? attributeId, string capabilityName, Exception innerException = null) : base(string.Format("Capability attribute ({0}) has a wrong format (capability='{1}'). {2}", attributeId.HasValue ? attributeId.Value.ToString() : "n/a", capabilityName, message), innerException)
        {
            this.CapabilityName = capabilityName;
            this.AttributeId = attributeId;
        }

        public string CapabilityName { get; set; }
        public Guid? AttributeId { get; set; }
    }

    /// <summary>
    /// Fault thrown if a capability attribute has a wrong value format.
    /// See <paramref name="Message"/> for details.
    /// </summary>

    [DataContract]
    public class WrongCapabilityAttributeFormatFault : BasicFault
    {
        public WrongCapabilityAttributeFormatFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid? AttributId { get; set; }

        [DataMember]
        public string CapabilityName { get; set; }
    }
}
