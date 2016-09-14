using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{

    /// <summary>
    /// Exception thrown if the file collection has not been found.
    /// </summary>

    public class FileCollectionNotFoundException : FaultableException<FileCollectionNotFoundFault>
    {
        public override FileCollectionNotFoundFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new FileCollectionNotFoundFault(Message, reqPath, userInfo);
            f.FileCollectionPath = FileCollectionPath;
            return f;
        }

        public FileCollectionNotFoundException(string message, string path, Exception innerException = null) : base(message ?? string.Format("File collection not found at {0}", path), innerException)
        {
            this.FileCollectionPath = path;
        }

        public string FileCollectionPath { get; set; }
    }

    /// <summary>
    /// Fault thrown if the file collection has not been found.
    /// </summary>
    
    [DataContract]
    public class FileCollectionNotFoundFault : BasicFault
    {
        public FileCollectionNotFoundFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string FileCollectionPath { get; set; }
    }
}
