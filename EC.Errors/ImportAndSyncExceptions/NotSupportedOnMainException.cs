using System;
using System.Runtime.Serialization;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Thrown if an operation is not supported on Main.
    /// </summary>

    public class NotSupportedOnMainException: FaultableException<NotSupportedOnMainFault>
    {
        public override NotSupportedOnMainFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NotSupportedOnMainFault(Message, reqPath, userInfo);
            return f;
        }

        public NotSupportedOnMainException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }

    [DataContract]
    public class NotSupportedOnMainFault : BasicFault
    {
        public NotSupportedOnMainFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }
    }
}
