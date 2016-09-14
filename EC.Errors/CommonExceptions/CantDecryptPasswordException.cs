using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// This exception indicates that the encrypted password was not able to get decrypted. 
    /// Usually this will happen because the service is not running as the user who encrypted the password. 
    /// </summary>

    public class CantDecryptPasswordException : FaultableException<CantDecryptPasswordFault>
    {
        public override CantDecryptPasswordFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new CantDecryptPasswordFault(Message, path, userInfo);
            return f;
        }

        /// <summary>
        /// This exception indicates that the encrypted db password was not able to get dycrypted. 
        /// Usually this will happen because the service is not running as the user who encrypted the password.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Any inner exception</param>

        public CantDecryptPasswordException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// This fault indicates that the encrypted password was not able to get decrypted. 
    /// Usually this will happen because the service is not running as the user who encrypted the password.
    /// </summary>

    [DataContract]
    public class CantDecryptPasswordFault : BasicFault
    {
        public CantDecryptPasswordFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }
    }
}
