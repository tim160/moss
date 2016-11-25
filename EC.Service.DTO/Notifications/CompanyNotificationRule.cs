using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO representation of a notification rule for an organization.
    /// </summary>

    [DataContract]
    
    public class CompanyNotificationRule : NotificationRule
    {
        /// <summary>
        /// Path to the organization this notification rule applies to
        /// </summary>

        [DataMember]
        public string CompanyPath { get; set; }

        /// <summary>
        /// Expression(s) which creates targets to send the notification to.
        /// </summary>

        [DataMember]
        public string TargetGeneratorExpression { get; set; }
    }
}
