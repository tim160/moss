using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// This exception indicates a possible cookie theft.
    /// A cookie theft occurs if the seriesId is correct, but the tokenId is wrong.
    /// </summary>

    public class CookieTheftException : FaultableException<CookieTheftFault>
    {
        public override CookieTheftFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new CookieTheftFault(Message, path, userInfo);
            f.SeriesId = SeriesId;
            f.TokenId = TokenId;
            f.UserName = LogonName;
            return f;
        }

        public CookieTheftException(string message, string logonName, string seriesId, string tokenId, Exception innerException = null) : base(message, innerException)
        {
            this.LogonName = logonName;
            this.SeriesId = seriesId;
            this.TokenId = tokenId;
        }

        public override string Message
        {
            get
            {
                var msg = base.Message;
                msg += String.Format(", LogonName = {0}", String.IsNullOrWhiteSpace(LogonName) ? "n/a" : LogonName);
                msg += String.Format(", SeriesId = {0}", String.IsNullOrWhiteSpace(SeriesId) ? "n/a" : SeriesId);
                msg += String.Format(", TokenId = {0}", String.IsNullOrWhiteSpace(TokenId) ? "n/a" : TokenId);
                return msg;
            }
        }

        /// <summary>
        /// Logon name of the assumed cookie theft victim.
        /// </summary>

        public string LogonName { get; set; }

        /// <summary>
        /// Series Id of the cookie (which is valid).
        /// </summary>

        public string SeriesId { get; set; }

        /// <summary>
        /// Token Id of the cookie (which is invalid).
        /// </summary>

        public string TokenId { get; set; }
    }

    /// <summary>
    /// Thrown if a cookie theft is assumed (seriesId is valid, but tokenId is not valid).
    /// All cookie information is deleted.
    /// </summary>

    [DataContract]
    public class CookieTheftFault : BasicFault
    {
        public CookieTheftFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// User name for which the theft is assumed.
        /// </summary>
        
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Series ID from cookie.
        /// </summary>
        
        [DataMember]
        public string SeriesId { get; set; }

        /// <summary>
        /// TokenID from cookie.
        /// </summary>
        
        [DataMember]
        public string TokenId { get; set; }
    }
}
