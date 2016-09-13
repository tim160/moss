using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Exception thrown if a user is created or updated with a orgId but has no orgPath
    /// </summary>

    public class OrganizationLogonStateException : FaultableException<OrganizationLogonStateFault>
    {
        public override OrganizationLogonStateFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new OrganizationLogonStateFault(Message, reqPath, userInfo);
            f.OrganizationLogonId = OrganizationLogonId;
            f.OrganizationPath = OrgPath;
            return f;
        }

        public OrganizationLogonStateException(string message, string organizationLogonId, string orgPath, Exception innerException = null) : base(message, innerException)
        {
            this.OrganizationLogonId = organizationLogonId;
            this.OrgPath = orgPath;
        }

        public string OrganizationLogonId;
        public string OrgPath;
    }

    /// <summary>
    /// Fault thrown if a user is created or updated with a orgId but has no orgPath
    /// </summary>
    
    [DataContract]
    public class OrganizationLogonStateFault : BasicFault
    {
        public OrganizationLogonStateFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string OrganizationLogonId { get; set; }

        [DataMember]
        public string OrganizationPath { get; set; }
    }
}
