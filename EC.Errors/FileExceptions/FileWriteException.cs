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
    /// Exception thrown when a file on disk cannot be created.
    /// </summary>

    public class FileWriteException : FaultableException<FileWriteFault>
    {
        public override FileWriteFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new FileWriteFault(Message, path, userInfo);
            f.FilePath = FilePath;
            return f;
        }

        public FileWriteException(string filePath, Exception innerException = null) : base("File could not be written to", innerException)
        {
            FilePath = filePath;
        }

        public string FilePath { get; set; }
    }

    /// <summary>
    /// Fault thrown when a file cannot be written to.
    /// </summary>

    [DataContract]
    public class FileWriteFault : BasicFault
    {
        public FileWriteFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string FilePath { get; set; }
    }
}
