using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace EC.Errors.RegistrationExceptions
{
    /// <summary>
    /// If a user already has been registered for a course offering which is overlapping
    /// with the offering we wanted to register the user for.
    /// </summary>

    public class OverlappingRegistrationException : FaultableException<OverlappingRegistrationFault>
    {
        public override OverlappingRegistrationFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new OverlappingRegistrationFault(Message, path, userInfo);
            f.OverlappingOfferingId = OverlappingOfferingId;
            f.OverlappingOfferingShortId = OverlappingOfferingShortId;
            f.UserId = UserId;
            f.UserDisplayName = UserDisplayName;
            return f;
        }

        public OverlappingRegistrationException(string msg, Guid? userId, string userDisplayName, Guid? overlappingOfferingId, string overlappingOfferingShortId, Exception innerException = null) : base(msg, innerException)
        {
            UserId = userId;
            UserDisplayName = userDisplayName;
            OverlappingOfferingId = overlappingOfferingId;
            OverlappingOfferingShortId = overlappingOfferingShortId;
        }

        /// <summary>
        /// User Id who is registered.
        /// </summary>

        public Guid? UserId { get; set; }

        /// <summary>
        /// User display name.
        /// </summary>

        public string UserDisplayName { get; set; }

        /// <summary>
        /// Overlapping offering Id in which the user is registered.
        /// </summary>

        public Guid? OverlappingOfferingId { get; set; }

        /// <summary>
        /// Overlapping offering short Id.
        /// </summary>

        public string OverlappingOfferingShortId { get; set; }
    }

    /// <summary>
    /// If a user already has been registered for a course offering which is overlapping
    /// with the offering we wanted to register the user for.
    /// </summary>

    [DataContract]
    public class OverlappingRegistrationFault : BasicFault
    {
        public OverlappingRegistrationFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// User Id who is registered.
        /// </summary>

        [DataMember]
        public Guid? UserId { get; set; }

        /// <summary>
        /// User display name.
        /// </summary>

        [DataMember]
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Overlapping offering Id in which the user is registered.
        /// </summary>

        [DataMember]
        public Guid? OverlappingOfferingId { get; set; }

        /// <summary>
        /// Offering short Id.
        /// </summary>

        [DataMember]
        public string OverlappingOfferingShortId { get; set; }
    }
}
