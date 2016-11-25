using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EC.Constants;

namespace EC.Service.DTO
{
    /// <summary>
    /// This DTO aggregates a number of pieces of information that are typically used to specify a notification.
    /// </summary>

    [DataContract]
  ////////  [KnownType(typeof(ExamNotificationDetails))]
    [KnownType(typeof(RegistrationNotificationDetails))]

    public class NotificationDetails
    {
        /// <summary>
        /// The name of the built-in template to use. May be null if an ad hoc notification is
        /// being sent.
        /// </summary>

        [DataMember]
        public string TemplateName { get; set; }

        /// <summary>
        /// A set of name/value pairs that are used for substitutions within the body of the
        /// template.
        /// </summary>

        [DataMember]
        public Dictionary<string, string> TemplateVariables { get; set; }

        /// <summary>
        /// Link attributes the notification has been created for (i.e. assessment link, course link,...).
        /// </summary>

        [DataMember]
        public List<AttributeItem> LinkAttributes { get; set; }

        /// <summary>
        /// The category for this notification. This is used in notification rules to determine
        /// who will receive the notification.
        /// </summary>

        [DataMember]
        public NotificationCategoriesEnum Category { get; set; }

        /// <summary>
        /// The subcategory for this notification. This is also used in notification rules to
        /// determine who will receive the notification.
        /// </summary>

        [DataMember]
        public NotificationSubCategoriesEnum Subcategory { get; set; }

        /// <summary>
        /// The priority for this notification. This is also used in notification rules; typically
        /// to select a particular channel for the notification, but it could also be used to
        /// select who will receive the notification.
        /// </summary>

        [DataMember]
        public NotificationPrioritiesEnum Priority { get; set; }

        /// <summary>
        /// Notifications may optionally be associated with the path to a content item in the
        /// core system.
        /// Optional path to an item.
        /// </summary>
        
        [DataMember]
        public String ReferencePath { get; set; }

        /// <summary>
        /// Notifications may optionally be associated with the path to a content item in the
        /// core system.
        /// Optional Id of an item.
        /// </summary>

        [DataMember]
        public Guid? ReferenceId { get; set; }

        /// <summary>
        /// User Id of the initiator (creator) of the notification.
        /// </summary>

        [DataMember]
        public Guid InitiatorId { get; set; }

        /// <summary>
        /// Optional organization path where this notification belongs to/is delivered to.
        /// Set to <c>null</c> to be a system-wide notification across all organization borders.
        /// </summary>

        [DataMember]
        public string OrganizationPath { get; set; }

        /// <summary>
        /// As this is a data bag, anyone can call the constructor.
        /// </summary>
        
        public NotificationDetails()
        {
            TemplateVariables = new Dictionary<string, string>();
        }
    }
}
