using System;
using System.Runtime.Serialization;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// Exception thrown if a path is not a path (either the format is not correct or the path doesn't exist - check inner exception).
    /// </summary>

    public class NotAPathException : FaultableException<NotAPathFault>
    {
        public override NotAPathFault ToFault(string requestPath, CurrentUserInfo userInfo)
        {
            var f = new NotAPathFault(Message, requestPath, userInfo);
            f.Path = Path;
            return f;
        }

        public NotAPathException(string msg, string path, Exception innerEx = null) : base(msg, innerEx)
        {
            Path = path;
        }


        public override string Message
        {
            get 
            {
                string msg = base.Message;
                if (Path != null) { msg += string.Format(", Path = {0}", Path); }
                return msg;
            }
        }

        /// <summary>
        /// The not found/existing/wrong path.
        /// </summary>

        public string Path { get; set; }
    }

    /// <summary>
    /// Fault returned when a path is not a path (either the format is not correct or the path doesn't exist).
    /// </summary>

    [DataContract]
    public class NotAPathFault : BasicFault
    {
        public NotAPathFault(string msg, string requestPath, CurrentUserInfo userInfo)
            : base(msg, requestPath, userInfo)
        {
        }

        /// <summary>
        /// The not found/existing/wrong path.
        /// </summary>

        [DataMember]
        public string Path { get; set; }
    }
}
