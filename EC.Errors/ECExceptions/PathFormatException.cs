using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown when a path is not in the correct format.
    /// </summary>

    public class PathFormatException : FaultableException<PathFormatFault>
    {
        public override PathFormatFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new PathFormatFault(Message, path, userInfo);
            f.InvalidPath = InvalidPath;
            return f;
        }

        public PathFormatException(string path, string message, Exception innerException = null) : base(message, innerException)
        {
            this.InvalidPath = path;
        }

        public override string Message
        {
            get
            {
                var msg = base.Message;
                msg += String.Format("Path = {0}", String.IsNullOrWhiteSpace(InvalidPath) ? "n/a" : InvalidPath);
                return msg;
            }
        }

        public string InvalidPath { get; set; }
    }

    /// <summary>
    /// Exception thrown when a null path is passed into a content handling function.
    /// </summary>

    public class NullPathException : PathFormatException
    {
        public NullPathException(string msg, Exception innerException = null)
            : base("<empty path>", msg, innerException)
        {
        }
    }

    /// <summary>
    /// Fault used to indicate a format error in a content path
    /// </summary>

    [DataContract]
    public class PathFormatFault : BasicFault
    {
        public PathFormatFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string InvalidPath { get; set; }
    }
}
