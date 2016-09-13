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
    /// This exception indicates that an authentication Id has expired.
    /// </summary>

    public class AuthenticationExpiredException : FaultableException<AuthenticationExpiredFault>
    {
        public override AuthenticationExpiredFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new AuthenticationExpiredFault(base.Message, path, userInfo);
            f.ExpiryDate = this.ExpiryDate;
            f.AuthenticationId = this.AuthenticationId;
            return f;
        }

        public AuthenticationExpiredException(string msg, Guid authenticationId, DateTime expiryDate, Exception innerException = null)
            : base(msg, innerException)
        {
            this.ExpiryDate = expiryDate;
            this.AuthenticationId = authenticationId;
        }

        public AuthenticationExpiredException(Guid authenticationId, DateTime expiryDate, Exception innerException = null)
            : base(null, innerException)
        {
            this.ExpiryDate = expiryDate;
            this.AuthenticationId = authenticationId;
        }

        public override string Message
        {
            get
            {
                var msg = base.Message;
                msg += string.Format("AuthenticationId {0} expired at '{1}'.", this.AuthenticationId, this.ExpiryDate.ToString("dd.MM.yyy HH:mm:ss"));
                return msg;
            }
        }

        public Guid AuthenticationId { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    /// <summary>
    /// Fault used to indicate that the something expired (i.e. authentication token).
    /// </summary>

    [DataContract]
    public class AuthenticationExpiredFault : BasicFault
    {
        public AuthenticationExpiredFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid AuthenticationId { get; set; }
        
        [DataMember]
        public DateTime ExpiryDate { get; set; }
    }
}
