using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Thrown if a remember me cookie is invalid or doesn't exist.
    /// </summary>

    public class InvalidCookieException : FaultableException<InvalidCookieFault>
    {
        public override InvalidCookieFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new InvalidCookieFault(Message, reqPath, userInfo);
            f.UserId = UserId;
            f.SeriesId = SeriesId;
            f.TokenId = TokenId;
            return f;
        }

        public InvalidCookieException(string msg, Guid userId, string seriesId, string tokenId, Exception innerException = null) : base(msg, innerException)
        {
            this.UserId = userId;
            this.SeriesId = seriesId;
            this.TokenId = tokenId;
        }

        public override string Message
        {
            get
            {
                var msg =  base.Message;
                msg += String.Format(", userId = {0}, seriesId = {1}, tokenId = {2}", UserId, SeriesId, TokenId);
                return msg;
            }
        }

        public Guid UserId { get; set; }
        public string SeriesId { get; set; }
        public string TokenId { get; set; }
    }

    /// <summary>
    /// Thrown if a remember me cookie doesn't exist or has expired for a user.
    /// </summary>

    [DataContract]
    public class InvalidCookieFault : BasicFault
    {
        public InvalidCookieFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string SeriesId { get; set; }

        [DataMember]
        public string TokenId { get; set; }
    }
}
