using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Exception thrown if a user is already member of an organization checked by orgId.
    /// </summary>

    public class AlreadyOrgMemberException : FaultableException<AlreadyOrgMemberFault>
    {
        public override AlreadyOrgMemberFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new AlreadyOrgMemberFault(Message, reqPath, userInfo);
            f.OrgPath = OrgPath;
            f.OrgId = OrgId;
            return f;
        }

        public AlreadyOrgMemberException(string msg, string orgPath, string orgId, Exception innerException = null) : base(msg, innerException)
        {
            this.OrgId = orgId;
            this.OrgPath = orgPath;
        }

        public string OrgPath { get; set; }
        public string OrgId { get; set; }
    }

    /// <summary>
    /// Exception thrown if a user is already member of an organization. Found by OrgId.
    /// </summary>

    [DataContract]
    public class AlreadyOrgMemberFault : BasicFault
    {
        public AlreadyOrgMemberFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Root path of the organization.
        /// </summary>

        [DataMember]
        public string OrgPath { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>

        [DataMember]
        public string OrgId { get; set; }
    }
}
