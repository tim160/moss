using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.RegistrationExceptions
{
    /// <summary>
    /// If a user is not registered in an course offering.
    /// </summary>

    public class NotRegisteredInOfferingException : FaultableException<NotRegisteredInOfferingFault>
    {
        public override NotRegisteredInOfferingFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NotRegisteredInOfferingFault(Message, reqPath, userInfo);
            f.UserName = UserName;
            f.UserId = UserId;
            f.OfferingId = OfferingId;
            return f;
        }

        public NotRegisteredInOfferingException(string userName, Guid? userId, Guid offeringId, Exception innerException = null) : base(string.Format("User '{0}' (Id='{1}') is not registered in course offering {2}", userName, userId.HasValue ? userId.Value.ToString() : "n/a", offeringId.ToString()), innerException)
        {
            this.UserName = userName;
            this.UserId = userId;
            this.OfferingId = offeringId;
        }

        public string UserName { get; set; }
        public Guid? UserId { get; set; }
        public Guid OfferingId { get; set; }
    }

    /// <summary>
    /// Fault thrown if a user is not registered as a student in a course offering.
    /// </summary>

    [DataContract]
    public class NotRegisteredInOfferingFault : BasicFault
    {
        public NotRegisteredInOfferingFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public Guid? UserId { get; set; }

        [DataMember]
        public Guid OfferingId { get; set; }
    }
}
