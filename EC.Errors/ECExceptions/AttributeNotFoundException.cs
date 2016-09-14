using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception if an attribute can't be found.
    /// </summary>

    public class AttributeNotFoundException : FaultableException<AttributeNotFoundFault>
    {
        public override AttributeNotFoundFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new AttributeNotFoundFault(Message, reqPath, userInfo);
            f.AttributeName = AttributeName;
            f.AttributePath = AttributePath;
            return f;
        }

        /// <summary>
        /// Exception if an attribute can't be found.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="attributeName">Attribute name/key.</param>
        /// <param name="attrPath">Path where the attribute has not been found.</param>
        /// <param name="innerException">Any inner exception.</param>

        public AttributeNotFoundException(string attributeName, string attrPath = null, string message = null, Exception innerException = null)  : base(message, innerException)
        {
            this.AttributeName = attributeName;
            this.AttributePath = attrPath;
        }

        /// <summary>
        /// Attribute name which has not been found.
        /// </summary>

        public string AttributeName { get; set; }

        /// <summary>
        /// Optional path where the attribute has not been found.
        /// </summary>

        public string AttributePath { get; set; }
    }

    /// <summary>
    /// Fault returned when an attribute cannot be found.
    /// </summary>

    [DataContract]
    public class AttributeNotFoundFault : BasicFault
    {
        public AttributeNotFoundFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string AttributeName { get; set; }

        [DataMember]
        public string AttributePath { get; set; }
    }
}
