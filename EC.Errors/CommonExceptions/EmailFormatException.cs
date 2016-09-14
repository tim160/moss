using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// Exception thrown when the email address is not in the correct format.
    /// </summary>

    public class EmailFormatException : FaultableException<EmailFormatFault>
    {
        public override EmailFormatFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new EmailFormatFault(Message, path, userInfo);
            f.EmailAddress = EmailAddress;
            return f;
        }

        public EmailFormatException(string message, string emailAddress, Exception innerException = null) : base(message, innerException)
        {
            this.EmailAddress = emailAddress;
        }

        /// <summary>
        /// Faulty email address.
        /// </summary>

        public string EmailAddress { get; set; }
    }

    /// <summary>
    /// Fault used to indicate an error for the email address.
    /// </summary>

    [DataContract]
    public class EmailFormatFault : BasicFault
    {
        public EmailFormatFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string EmailAddress { get; set; }
    }
}
