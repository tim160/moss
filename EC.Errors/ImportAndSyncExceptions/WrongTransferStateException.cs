using System;
using System.Runtime.Serialization;
using EC.Constants;

namespace EC.Errors.ImportAndSyncExceptions
{
    public class WrongTransferStateException : FaultableException<WrongTransferStateFault>
    {
        public override WrongTransferStateFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new WrongTransferStateFault(Message, reqPath, userInfo);
            f.TransferState = TransferState;
            return f;
        }

        public WrongTransferStateException(string msg, Exception innerException = null) : base(msg, innerException)
        {
        }

        public WrongTransferStateException(string msg, TransferStatesEnum transferState)
            : base(msg)
        {
            TransferState = transferState;
        }

        /// <summary>
        /// Root path of the organization.
        /// </summary>

        public TransferStatesEnum TransferState { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>

        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Exception thrown if the sync package is in a wrong transfer state.
    /// </summary>

    [DataContract]
    public class WrongTransferStateFault : BasicFault
    {
        public WrongTransferStateFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public TransferStatesEnum TransferState { get; set; }
    }
}
