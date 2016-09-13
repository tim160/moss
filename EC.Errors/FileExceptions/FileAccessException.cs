using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.FileExceptions
{
    /// <summary>
    /// Common file access exception.
    /// </summary>

    public class FileAccessException : FaultableException<FileAccessFault>
    {
        public override FileAccessFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new FileAccessFault(Message, path, userInfo);
            f.FilePath = FilePath;
            return f;
        }

        public FileAccessException(string msg, string filePath, Exception innerException = null) : base(msg, innerException)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Path concerning this error.
        /// </summary>

        public string FilePath { get; set; }
    }

    /// <summary>
    /// Fault used to indicate failed access to a file
    /// </summary>

    [DataContract]
    public class FileAccessFault : BasicFault
    {
        public FileAccessFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Path to the file/directory that could not be accessed.
        /// </summary>
        
        [DataMember]
        public string FilePath { get; set; }
    }
}
