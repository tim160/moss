using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO representation of a user notification rule.
    /// </summary>

    [DataContract]
    public class UserNotificationRule : NotificationRule
    {
        /// <summary>
        /// User Id the notification rule belongs to.
        /// </summary>

        [DataMember]
        public Guid UserId { get; set; }
    }
}
