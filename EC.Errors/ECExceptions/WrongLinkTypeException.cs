using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown when a path leads to a link that cannot be traversed because it is of the
    /// wrong type.
    /// </summary>

    public class WrongLinkTypeException : FaultableException<WrongLinkTypeFault>
    {
        public override WrongLinkTypeFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new WrongLinkTypeFault(Message, path, userInfo);
            f.LinkName = ElementName;
            f.LinkPath = ElementPath;
            return f;
        }

        public WrongLinkTypeException(string message, string path, string elementName, Exception innerException = null) : base(message, innerException)
        {
            ElementPath = path;
            ElementName = elementName;
        }

        public string ElementPath { get; set; }
        public string ElementName { get; set; }
    }

    /// <summary>
    /// This fault is thrown when attempting to traverse a path through the content
    /// tree and encountering a link which points to the wrong type of object
    /// (e.g. a NavPageLink pointing to a FileCollection).
    /// </summary>

    [DataContract]
    public class WrongLinkTypeFault : BasicFault
    {
        public WrongLinkTypeFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// The specific link which produced the problem.
        /// </summary>

        [DataMember]
        public string LinkName { get; set; }

        [DataMember]
        public string LinkPath { get; set; }
    }
}
