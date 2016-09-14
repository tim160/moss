using System;
using System.Runtime.Serialization;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Thrown if an operation is not supported on an RTA.
    /// </summary>

    public class NotSupportedOnRtaException : FaultableException<NotSupportedOnRtaFault>
    {
        public override NotSupportedOnRtaFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NotSupportedOnRtaFault(Message, reqPath, userInfo);
            return f;
        }

        public NotSupportedOnRtaException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }

    [DataContract]
    public class NotSupportedOnRtaFault : BasicFault
    {
        public NotSupportedOnRtaFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
