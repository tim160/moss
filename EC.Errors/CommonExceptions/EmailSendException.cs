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
    /// Exception thrown if the email couldn't be sent.
    /// </summary>

    public class EmailSendException : FaultableException<EmailSendFault>
    {
        public override EmailSendFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new EmailSendFault(Message, path, userInfo);
            f.FromEmailAddress = FromEmailAddress;
            f.ToEmailAddress = ToEmailAddress;
            return f;
        }

        public EmailSendException(string message, string fromEmailAddress, string toEmailAddress, Exception innerException = null) : base(message, innerException)
        {
            this.FromEmailAddress = fromEmailAddress;
            this.ToEmailAddress = ToEmailAddress;
        }

        public string FromEmailAddress { get; set; }
        public string ToEmailAddress { get; set; }
    }

    /// <summary>
    /// Fault thrown if the email couldn't be sent.
    /// </summary>

    [DataContract]
    public class EmailSendFault : BasicFault
    {
        public EmailSendFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string FromEmailAddress { get; set; }

        [DataMember]
        public string ToEmailAddress { get; set; }
    }
}
