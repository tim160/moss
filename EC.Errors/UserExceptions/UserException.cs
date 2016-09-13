using EC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Exception concerning a user. NOTE: This is an odd class that seems to overlap with other
    /// specific types of exceptions, e.g. there is an AlreadyRegisteredException that should 
    /// probably be used rather than this exception with with a specific enum value.
    /// </summary>

    public class UserException : FaultableException<UserFault>
    {
        public override UserFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new UserFault(Message, reqPath, userInfo);
            f.UserId = UserId;
            f.UserName = UserName;
            f.ErrorType = ErrorType;
            return f;
        }

        public UserException(UserFaultEnum errorType, string userName = null, Guid? userId = null, string message = null, Exception innerException = null)  : base(message, innerException)
        {
            this.ErrorType = errorType;
            this.UserName = userName;
            this.UserId = userId;
        }

        /// <summary>
        /// Optional user name.
        /// </summary>

        public string UserName { get; set; }

        /// <summary>
        /// Optional user id.
        /// </summary>

        public Guid? UserId { get; set; }

        /// <summary>
        /// Type of the user error.
        /// </summary>

        public UserFaultEnum ErrorType { get; set; }
    }

    /// <summary>
    /// User fault to specify the user and type of error.
    /// </summary>

    [DataContract]
    public class UserFault : BasicFault
    {
        public UserFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Optional user name
        /// </summary>

        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Optional user id.
        /// </summary>

        [DataMember]
        public Guid? UserId { get; set; }

        /// <summary>
        /// Type of the user error.
        /// </summary>

        [DataMember]
        public UserFaultEnum ErrorType { get; set; }
    }
}
