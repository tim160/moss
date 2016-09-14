using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.NotificationExceptions
{
    /// <summary>
    /// Exception thrown when there is an inconsistency in the configuration of the 
    /// system that prevents an operation from completing.
    /// </summary>

    public class NotificationConfigException : FaultableException<NotificationConfigFault>
    {
        public override NotificationConfigFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NotificationConfigFault(Message, reqPath, userInfo);
            f.InnerDetails = InnerException.ToString();
            return f;
        }

        public NotificationConfigException(string message, Exception inner) : base(message, inner) 
        { 
        }
    }

    /// <summary>
    /// Fault thrown when there is an internal configuration issue preventing the
    /// notification service from completing the operation.
    /// </summary>

    [DataContract]
    public class NotificationConfigFault : BasicFault
    {
        public NotificationConfigFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Information about inner exception if there is one.
        /// </summary>

        [DataMember]
        public string InnerDetails { get; set; }
    }
}
