using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.NotificationExceptions
{
    /// <summary>
    /// Exception thrown on an error in the notification expression for a notification rule/ target.
    /// </summary>

    public class NotificationExpressionException : FaultableException<NotificationExpressionFault>
    {
        public override NotificationExpressionFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NotificationExpressionFault(Message, reqPath, userInfo);
            return f;
        }

        public NotificationExpressionException(string msg, Exception innerException = null) : base(msg, innerException) 
        { 
        }
    }

    /// <summary>
    /// Fault thrown on an error in the notification expression for a notification rule/ target.
    /// </summary>
    
    [DataContract]
    public class NotificationExpressionFault : BasicFault
    {
        public NotificationExpressionFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
