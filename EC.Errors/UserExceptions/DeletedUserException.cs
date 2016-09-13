using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.UserExceptions
{
    /// <summary>
    /// Exception thrown if a user is accessed but is in a deleted state.
    /// </summary>

    public class DeletedUserException : FaultableException<DeletedUserFault>
    {
        public override DeletedUserFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new DeletedUserFault(Message, path, userInfo);
            f.UserId = UserId;
            return f;
        }

        public DeletedUserException() : base() { }

        public DeletedUserException(string message, Guid userId, Exception innerException = null) : base(message, innerException)
        {
            this.UserId = userId;
        }

        /// <summary>
        /// User Id which is deleted.
        /// </summary>
        
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Exception thrown if a user is accessed but is in a deleted state.
    /// </summary>

    [DataContract]
    public class DeletedUserFault : BasicFault
    {
        public DeletedUserFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// User Id which is marked as deleted.
        /// </summary>

        [DataMember]
        public Guid UserId { get; set; }
    }
}
