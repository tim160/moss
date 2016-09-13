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
    /// This exception indicates that the encrypted db password was not able to get decrypted. 
    /// Usually this will happen because the service is not running as the user who encrypted the password. 
    /// </summary>

    public class CantDecryptDBPasswordException : FaultableException<CantDecryptDBPasswordFault>
    {
        public override CantDecryptDBPasswordFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new CantDecryptDBPasswordFault(Message, path, userInfo);
            f.DBName = DBName;
            return f;
        }

        /// <summary>
        /// This exception indicates that the encrypted db password was not able to get decrypted. 
        /// Usually this will happen because the service is not running as the user who encrypted the password.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="dbName">DB for which the password couldn't be decrypted</param>
        /// <param name="innerException">Any inner exception</param>

        public CantDecryptDBPasswordException(string message,string dbName, Exception innerException = null)
            : base(message, innerException)
        {
            DBName = dbName;
        }

        /// <summary>
        /// Name of Db that passwords decryption failed on
        /// </summary>

        public string DBName  { get; set; }
    }

    /// <summary>
    /// This fault indicates that the encrypted db password was not able to get dycrypted. 
    /// Usually this will happen because the service is not running as the user who encrypted the password.
    /// </summary>

    [DataContract]
    public class CantDecryptDBPasswordFault : BasicFault
    {
        public CantDecryptDBPasswordFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Name of Db that passwords decryption failed on
        /// </summary>
        [DataMember]
        public string DBName { get; set; }

    }
}
