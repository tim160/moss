using System;
using System.Runtime.Serialization;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// If transaction conflicts persist.
    /// This is used if there are collisions (i.e. dead lock aborted the snapshot transaction) with other processes to
    /// change/read lock states.
    /// Usually checking again should solve that issue.
    /// </summary>

    public class LockFailedException : FaultableException<LockFailedFault>
    {
        public override LockFailedFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new LockFailedFault(Message, reqPath, userInfo);
            return f;
        }

        public LockFailedException(string msg, Exception innerException = null) : base(msg, innerException) { }

    }

    /// <summary>
    /// If transaction conflicts persist.
    /// This is used if there are collisions (i.e. dead lock aborted the snapshot transaction) with other processes to
    /// change/read lock states.
    /// Usually checking again should solve that issue.
    /// </summary>

    [DataContract]
    public class LockFailedFault : BasicFault
    {
        public LockFailedFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo) { }
    }
}
