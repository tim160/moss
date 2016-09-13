using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EC.Errors.RegistrationExceptions
{
    /// <summary>
    /// If a user already has been registered for a course offering.
    /// </summary>

    public class AlreadyRegisteredException : FaultableException<AlreadyRegisteredFault>
    {
        public override AlreadyRegisteredFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new AlreadyRegisteredFault(Message, path, userInfo);
            f.OfferingId = OfferingId;
            f.OfferingShortId = OfferingShortId;
            f.UserId = UserId;
            f.UserDisplayName = UserDisplayName;
            return f;
        }

        public AlreadyRegisteredException(string msg, Guid? userId, string userDisplayName, Guid? offeringId, string offeringName, Exception innerException = null) : base(msg, innerException)
        {
            UserId = userId;
            UserDisplayName = userDisplayName;
            OfferingId = offeringId;
            OfferingShortId = offeringName;
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
        /// Offering Id the user is registered in.
        /// </summary>

        public Guid? OfferingId { get; set; }

        /// <summary>
        /// Name of the offering.
        /// </summary>

        public string OfferingShortId { get; set; }
    }

    /// <summary>
    /// Fault thrown if a user has been already registered in a course offering.
    /// </summary>

    [DataContract]
    public class AlreadyRegisteredFault : BasicFault
    {
        public AlreadyRegisteredFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// ID of the user who is already registered.
        /// </summary>
        
        [DataMember]
        public Guid? UserId { get; set; }

        [DataMember]
        public string UserDisplayName { get; set; }

        [DataMember]
        public Guid? OfferingId { get; set; }

        [DataMember]
        public string OfferingShortId { get; set; }
    }
}
