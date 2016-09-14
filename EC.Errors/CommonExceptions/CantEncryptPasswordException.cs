using System;
using System.Runtime.Serialization;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// This exception indicates that the plain password was not able to get encrypted. 
    /// </summary>

    public class CantEncryptPasswordException : FaultableException<CantEncryptPasswordFault>
    {
        public override CantEncryptPasswordFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new CantEncryptPasswordFault(Message, path, userInfo);
            return f;
        }

        /// <summary>
        /// This exception indicates that the plain password was not able get encrypted. 
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Any inner exception</param>

        public CantEncryptPasswordException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// This fault indicates that the plain password was not able to get encrypted. 
    /// </summary>

    [DataContract]
    public class CantEncryptPasswordFault : BasicFault
    {
        public CantEncryptPasswordFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }
    }
}
