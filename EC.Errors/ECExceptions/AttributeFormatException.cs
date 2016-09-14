using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// If an attribute has a wrong format - either in the key or value.
    /// </summary>

    public class AttributeFormatException : FaultableException<AttributeFormatFault>
    {
        public override AttributeFormatFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new AttributeFormatFault(Message, reqPath, userInfo);
            f.Key = Key;
            f.Value = Value;
            f.AttributeId = AttributeId;
            return f;
        }

        public AttributeFormatException(string message, string key, string value, Guid? attributeId = null, Exception innerException = null) : base(message, innerException)
        {
            this.Key = key;
            this.Value = value;
            this.AttributeId = attributeId;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public Guid? AttributeId { get; set; }
    }

    /// <summary>
    /// Fault thrown if an attribute (usually other than a capability) has a wrong key or value format.
    /// See <paramref name="Message"/> for details.
    /// </summary>

    [DataContract]
    public class AttributeFormatFault : BasicFault
    {
        public AttributeFormatFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Attribute key.
        /// </summary>

        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Attribute value.
        /// </summary>

        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Optional attribute Id with the wrong format.
        /// </summary>

        [DataMember]
        public Guid? AttributeId { get; set; }
    }
}
