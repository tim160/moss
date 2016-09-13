using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.RegistrationExceptions
{
    /// <summary>
    /// Thrown when registering a student into an offering fails because the did not
    /// agree to the terms.
    /// </summary>

    public class RegistrationAgreementFailureException : FaultableException<RegistrationAgreementFailureFault>
    {
        public override RegistrationAgreementFailureFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new RegistrationAgreementFailureFault(Message, reqPath, userInfo);
            return f;
        }
    }

    /// <summary>
    /// Fault thrown when a student cannot be registered for some reason.
    /// </summary>

    [DataContract]
    public class RegistrationAgreementFailureFault : BasicFault
    {
        public RegistrationAgreementFailureFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
