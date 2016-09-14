using System;
using System.Runtime.Serialization;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// Fault used to indicate that a UOW isolation error happened (usually a DBUpdateException or SqlException). 
    /// See inner exception for more details.
    /// </summary>
    
    public class UowIsolationException  : FaultableException<UowIsolationFault>
    {
        public override UowIsolationFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new UowIsolationFault(Message, path, userInfo);
            return f;
        }

        public UowIsolationException(string errorMsg, Exception inner = null) : base(errorMsg ?? "Isolation error", inner)
        {
        }
    }

    /// <summary>
    /// Fault used to indicate that a UOW isolation error happened (usually a DBUpdateException or SqlException). 
    /// See inner exception for more details.
    /// </summary>

    [DataContract]
    public class UowIsolationFault : BasicFault
    {
        public UowIsolationFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
