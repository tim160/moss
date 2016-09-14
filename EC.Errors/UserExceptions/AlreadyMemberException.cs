using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Exception thrown if a user is already member of an organization checked by email.
    /// </summary>

    public class AlreadyMemberException : FaultableException<AlreadyMemberFault>
    {
        public override AlreadyMemberFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new AlreadyMemberFault(Message, reqPath, userInfo);
            f.OrgPath = OrgPath;
            f.UserId = UserId;
            return f;
        }

        public AlreadyMemberException(string msg, Guid userId, Exception innerException = null) : base(msg, innerException)
        {
            this.UserId = userId;
        }

        public AlreadyMemberException(string msg, string path, Guid userId) : base(msg)
        {
            this.OrgPath = path;
            this.UserId = userId;
        }

        /// <summary>
        /// Root path of the organization.
        /// </summary>

        public string OrgPath { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>

        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Exception thrown if a user is already member of an organization. Found by Email.
    /// </summary>

    [DataContract]
    public class AlreadyMemberFault : BasicFault
    {
        public AlreadyMemberFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string OrgPath { get; set; }
    }
}
