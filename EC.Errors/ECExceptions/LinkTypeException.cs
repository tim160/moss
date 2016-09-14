using EC.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// If a wrong link type has been found/encountered.
    /// </summary>

    public class LinkTypeException : FaultableException<LinkTypeFault>
    {
        public override LinkTypeFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new LinkTypeFault(Message, path, userInfo);
            f.ExpectedLinkTypeName = this.ExpectedLinkType != null ? this.ExpectedLinkType.Name : "n/a";
            f.FoundLinkTypeName = this.FoundLinkType != null ? this.FoundLinkType.Name : "n/a";
            return f;
        }

        /// <summary>
        /// Exception if a wrong link type has been encountered.
        /// </summary>
        /// <param name="message">Error message</param>
        ///  <param name="expectedLinkType">Expected link type</param>
        ///  <param name="foundLinkType">Link type found</param>
        ///  <param name="linkPath">Link path where the wrong link type has been found</param>
        /// <param name="innerException">Any inner exception.</param>

        public LinkTypeException(string message, string linkPath, Type expectedLinkType, Type foundLinkType = null, Exception innerException = null) : base(message, innerException)
        {
            this.ExpectedLinkType = expectedLinkType;
            this.FoundLinkType = foundLinkType;
        }

        /// <summary>
        /// Exception if the link type is wrong.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="expectedLinkType">expected link type</param>
        /// <param name="innerException">Any inner exception</param>

        public LinkTypeException(string message, Type expectedLinkType = null, Type foundLinkType = null, Exception innerException = null) : base(message, innerException)
        {
            this.ExpectedLinkType = expectedLinkType;
            this.FoundLinkType = foundLinkType;
        }

        /// <summary>
        /// Expected link type.
        /// </summary>

        public Type ExpectedLinkType { get; set; }

        /// <summary>
        /// Found link type.
        /// </summary>

        public Type FoundLinkType { get; set; }
    }

    /// <summary>
    /// Thrown if a wrong link type has been found/encountered.
    /// </summary>

    [DataContract]
    public class LinkTypeFault : BasicFault
    {
        public LinkTypeFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Link type name expected ('n/a' if not available).
        /// </summary>

        [DataMember]
        public string ExpectedLinkTypeName { get; set; }

        /// <summary>
        /// Found link type ('n/a' if not available).
        /// </summary>

        [DataMember]
        public string FoundLinkTypeName { get; set; }
    }

}
