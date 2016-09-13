using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Constants;
using System.Runtime.Serialization;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Thrown if there is a conflict with a NavPage tag.
    /// </summary>

    public class NavPageTagConflictException : FaultableException<NavPageTagConflictFault>
    {
        public override NavPageTagConflictFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NavPageTagConflictFault(Message, reqPath, userInfo);
            f.ConflictingTag = ConflictingTag;
            return f;
        }

        public NavPageTagConflictException(string msg, NavPageTagTypesEnum? conflictingTag, Exception innerException = null) : base(msg, innerException)
        {
            this.ConflictingTag = conflictingTag;
        }

        public NavPageTagTypesEnum? ConflictingTag { get; set; }
    }


    /// <summary>
    /// Thrown if there is a conflict with a NavPage tag.
    /// </summary>

    [DataContract]
    public class NavPageTagConflictFault : BasicFault
    {
        public NavPageTagConflictFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public NavPageTagTypesEnum? ConflictingTag { get; set; }
    }
}
