using System;
using System.Runtime.Serialization;

namespace EC.Errors.LMSExceptions
{
    public class CannotDeleteException : FaultableException<CannotDeleteFault>
    {
        public override CannotDeleteFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new CannotDeleteFault(Message, path, userInfo)
            {
                ToBeDeletedType = ToBeDeletedType.Name
            };
            return f;
        }

        public CannotDeleteException() { }

        public CannotDeleteException(string message, Type toBeDeletedType, Exception innerException = null) : base(message, innerException)
        {
            ToBeDeletedType = toBeDeletedType;
        }

        public Type ToBeDeletedType { get; set; }
    }

    [DataContract]
    public class CannotDeleteFault : BasicFault
    {
        public CannotDeleteFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Type of element we are trying to delete
        /// </summary>

        [DataMember]
        public string ToBeDeletedType { get; set; }
    }
}
