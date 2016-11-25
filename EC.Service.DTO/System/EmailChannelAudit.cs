using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Service.DTO
{
    /// <summary>
    /// This DTO corresponds to the model class IEmailChannelAudit. It holds information
    /// specifically about emails that have been processed by the email notification
    /// channel.
    /// </summary>
    
    [DataContract]

    public class EmailChannelAudit : BasicChannelAudit
    {
        /// <summary>
        /// Email address to send the email from.
        /// </summary>

        [DataMember]
        public string From { get; set; }

        /// <summary>
        /// Email address to send the email to.<br/>
        /// </summary>
        
        [DataMember]
        public string To { get; set; }

        /// <summary>
        /// Subject text of the email.
        /// </summary>
        
        [DataMember]
        public string Subject { get; set; }

        /// <summary>
        /// Body text of the email.
        /// </summary>
        
        [DataMember]
        public string Body { get; set; }

        /// <summary>
        /// Flag whether the email has been sent as html.
        /// </summary>
        
        [DataMember]
        public bool IsHtmlBody { get; set; }
    }
}
