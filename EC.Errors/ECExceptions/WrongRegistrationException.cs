using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{

    /// <summary>
    /// If the registration is found at a wrong path - maybe not matching up with a link path.
    /// </summary>
    /// <example>
    /// If a student evaluation link and an registrationId are passed in to a service endpoint we expect 
    /// the registration to be found along the student evaluation link. If that is not the case this exception
    /// is thrown.
    /// </example>

    public class WrongRegistrationException : FaultableException<WrongRegistrationFault>
    {
        public override WrongRegistrationFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new WrongRegistrationFault(Message, reqPath, userInfo);
            f.RegistrationId = this.RegistrationId;
            f.RegistrationPath = this.RegistrationPath;
            return f;
        }

        public WrongRegistrationException(string message, Guid registrationId, string registrationPath, Exception innerException = null)
            : base(message, innerException)
        {
            RegistrationId = registrationId;
            RegistrationPath = registrationPath;
        }

        public Guid RegistrationId { get; set; }
        public string RegistrationPath { get; set; }
    }

    /// <summary>
    /// If the registration is found at a wrong path - maybe not matching up with a link path.
    /// </summary>
    /// <example>
    /// If a student evaluation link and a registrationId are passed in to a service endpoint we expect 
    /// the registrationId to be found along the student evaluation link. If that is not the case this fault
    /// is thrown.
    /// </example>

    [DataContract]
    public class WrongRegistrationFault : BasicFault
    {
        public WrongRegistrationFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid RegistrationId { get; set; }

        [DataMember]
        public string RegistrationPath { get; set; }
    }
}
