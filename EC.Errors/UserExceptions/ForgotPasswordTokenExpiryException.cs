using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Exception thrown when a "forgot password token" is used after it has expired.
    /// </summary>

    public class ForgotPasswordTokenExpiryException : FaultableException<ForgotPasswordTokenExpiredFault>
    {
        public override ForgotPasswordTokenExpiredFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new ForgotPasswordTokenExpiredFault(Message, reqPath, userInfo);
            f.UserName = UserName;
            f.Token = TokenId;
            return f;
        }

        public ForgotPasswordTokenExpiryException(string userName, Guid tokenId, Exception innerException = null) : base(String.Format("Expired forgot password token: User = {0}, Token = {1}", userName, tokenId), innerException)
        {
            UserName = userName;
            TokenId = tokenId;
        }

        public string UserName = null;
        public Guid TokenId = Guid.Empty;
    }

    /// <summary>
    /// Fault thrown when a "forgot password token" is used after it has expired.
    /// </summary>

    [DataContract]
    public class ForgotPasswordTokenExpiredFault : BasicFault
    {
        public ForgotPasswordTokenExpiredFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public Guid Token { get; set; }
    }
}
