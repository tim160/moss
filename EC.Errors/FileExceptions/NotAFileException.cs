using System;
using System.Runtime.Serialization;

namespace EC.Errors.FileExceptions
{
    public class NotAFileException : FaultableException<NotAFileFault>
    {
        public override NotAFileFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new NotAFileFault(Message, path, userInfo);
            f.FilePath = FilePath;
            return f;
        }

        public NotAFileException(string msg, string filePath, Exception innerException = null) : base(msg, innerException)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Path concerning this error.
        /// </summary>
        public string FilePath { get; set; }
    }

    /// <summary>
    /// Fault used to indicate path is not to a file
    /// </summary>
    [DataContract]
    public class NotAFileFault : BasicFault
    {
        public NotAFileFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Path that is not to a file
        /// </summary>
        [DataMember]
        public string FilePath { get; set; }
    }
}
