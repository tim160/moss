using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    public class IllegalLogonCharacterException : FaultableException<IllegalLogonCharacterFault>
    {
        public override IllegalLogonCharacterFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new IllegalLogonCharacterFault(Message, reqPath, userInfo);
            f.UserName = UserName;
            f.Email = Email;
            f.OrgLogonId = OrgLogOnId;
            return f;
        }

        public IllegalLogonCharacterException(string message, string userName, string orgLogOnId, string email, Exception innerException = null) : base(message, innerException)
        {
            UserName = userName;
            OrgLogOnId = orgLogOnId;
            Email = email;
        }

        public string UserName;
        public string OrgLogOnId;
        public string Email;
    }

    /// <summary>
    /// Exception when a user is created or updated with a # in the userName, Email, LogonId
    /// </summary>
    
    [DataContract]
    public class IllegalLogonCharacterFault : BasicFault
    {
        public IllegalLogonCharacterFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string OrgLogonId { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}
