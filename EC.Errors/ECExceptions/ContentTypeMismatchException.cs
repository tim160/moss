using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Thrown when a specific type of content item is required, but a different
    /// type is provided.
    /// </summary>

    public class ContentTypeMismatchException : FaultableException<ContentTypeMismatchFault>
    {
        public override ContentTypeMismatchFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new ContentTypeMismatchFault(Message, path, userInfo);
            f.ActualType = ActualType;
            f.DesiredType = DesiredType;
            return f;
        }

        public ContentTypeMismatchException(string desiredType, string actualType, Exception innerException = null) : base("Content item type mismatch", innerException)
        {
            DesiredType = desiredType;
            ActualType = actualType;
        }

        public string DesiredType { get; set; }
        public string ActualType { get; set; }
    }

    /// <summary>
    /// Fault thrown when one type of content item is needed, but the content item provided
    /// is of a different type.
    /// </summary>

    [DataContract]
    public class ContentTypeMismatchFault : BasicFault
    {
        public ContentTypeMismatchFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string ActualType;

        [DataMember]
        public string DesiredType;
    }
}
