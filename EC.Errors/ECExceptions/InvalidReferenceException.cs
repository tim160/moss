using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// This exception is used by the copy/move code. It is thrown when a copy or move operation would
    /// result in invalid references. For example, if a content subtree rooted a page 'A' contains a
    /// reference to a FileCollection attached to a page above 'A', then copying that subtree to a 
    /// location above the FileCollection would result in an invalid reference to that FileCollection.
    /// </summary>

    public class InvalidReferenceException : FaultableException<InvalidReferenceFault>
    {
        public override InvalidReferenceFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new InvalidReferenceFault(Message, path, userInfo);
            f.InvalidReferencePath = InvalidReferencePath;
            f.InvalidReferenceTargetType = InvalidReferenceTargetType.ToString();
            return f;
        }

        public InvalidReferenceException(string msg, string path, Type invalidReferenceTargetType, Exception innerException = null) : base(msg, innerException)
        {
            this.InvalidReferencePath = path;
            this.InvalidReferenceTargetType = invalidReferenceTargetType;
        }

        /// <summary>
        /// Optional type which couldn't be reached.
        /// </summary>

        public Type InvalidReferenceTargetType { get; set; }

        /// <summary>
        /// Optional path which couldn't be reached.
        /// </summary>

        public string InvalidReferencePath { get; set; }
    }

    /// <summary>
    /// Thrown if an object is out of reach (e.g. if a FileCollection is not reachable).
    /// </summary>

    [DataContract]
    public class InvalidReferenceFault : BasicFault
    {
        public InvalidReferenceFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Optional type which couldn't be reached.
        /// </summary>

        [DataMember]
        public string InvalidReferenceTargetType { get; set; }

        /// <summary>
        /// Path of the invalid reference.
        /// </summary>

        [DataMember]
        public string InvalidReferencePath { get; set; }
    }
}
