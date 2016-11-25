using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO notification for a registration/de-registration of a user.
    /// </summary>

    [DataContract]
    public class RegistrationNotificationDetails : NotificationDetails
    {
        [DataMember]
        public Guid CourseOfferingId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }
    }
}
