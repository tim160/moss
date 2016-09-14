using System;
using System.Runtime.Serialization;

namespace EC.Errors.Synchronization
{
    public class NoConnectionException : FaultableException<NoConnectionFault>
    {
        public override NoConnectionFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new NoConnectionFault(Message, path, userInfo);
            return f;
        }

        public NoConnectionException(string msg, Exception innerException = null)
            : base(msg ?? "No connection could be found.", innerException)
        {
        }       
    }

    /// <summary>
    /// Fault thrown if no connection is available.
    /// </summary>

    [DataContract]
    public class NoConnectionFault: BasicFault
    {
        public NoConnectionFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }
    }
}
