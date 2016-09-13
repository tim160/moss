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
    /// Exception used to indicate that the file (name) already exists.
    /// </summary>

    public class FileAlreadyExistsException : FaultableException<FileAlreadyExistsFault>
    {
        public override FileAlreadyExistsFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new FileAlreadyExistsFault(Message, path, userInfo);
            f.FilePath = FilePath;
            return f;
        }

        public FileAlreadyExistsException(string fileName, Exception innerException = null) : base(string.Format("File '{0}' already exists.", fileName), innerException)
        {
            this.FilePath = fileName;
        }

        /// <summary>
        /// Get or set the file name which already exists.
        /// </summary>

        public string FilePath { get; set; }
    }

    /// <summary>
    /// Fault used to indicate that the file (name) already exists.
    /// </summary>

    [DataContract]
    public class FileAlreadyExistsFault : BasicFault
    {
        public FileAlreadyExistsFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Get or set the file name which already exists.
        /// </summary>

        [DataMember]
        public string FilePath { get; set; }
    }
}
