using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.NotificationExceptions
{
    /// <summary>
    /// Exception thrown when a notification target uses an invalid property name.
    /// </summary>

    public class BadReceiverPropertyException : FaultableException<BadReceiverPropertyFault>
    {
        public override BadReceiverPropertyFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new BadReceiverPropertyFault(Message, reqPath, userInfo);
            f.PropertyName = PropertyName;
            return f;
        }

        public BadReceiverPropertyException(string msg, string propertyName) : base(msg)
        {
            PropertyName = propertyName;
        }

        public string PropertyName = null;
    }

    /// <summary>
    /// Fault thrown when the object-based receiver for a notification uses a non-existent
    /// property of the object.
    /// </summary>

    [DataContract]
    public class BadReceiverPropertyFault : BasicFault
    {
        public BadReceiverPropertyFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// The property name that could not be found.
        /// </summary>

        [DataMember]
        public string PropertyName { get; set; }
    }
}
