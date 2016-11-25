using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// Basic DTO representation of a notification rule.
    /// This has common properties of different notification rule implementations.
    /// </summary>

    [DataContract]
    [KnownType(typeof(UserNotificationRule))]
   ////// [KnownType(typeof(OrganizationNotificationRule))]

    public class NotificationRule
    {
        /// <summary>
        /// Id of the organization rule.
        /// Set to <c>null</c> to create a new rule
        /// </summary>

        [DataMember]
        public Guid? Id { get; set; }

        /// <summary>
        /// Expression to evaluate whether this rule applies to a notification.
        /// If the expression evaluates to <c>true</c> the notification is sent to all targets the <c>TargetGeneratorExpression</c> creates.
        /// </summary>

        [DataMember]
        public string PredicateExpression { get; set; }

        /// <summary>
        /// Channel name defines through with channel (e.g. Email, WeeklyEmail, Twitter,...) are sent.
        /// </summary>

        [DataMember]
        public string ChannelName { get; set; }
    }
}
