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
    /// Exception thrown when a file cannot be deleted.
    /// </summary>

    public class FileDeletionException : FaultableException<FileDeletionFault>
    {
        public override FileDeletionFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new FileDeletionFault(Message, path, userInfo);
            f.FilePath = FilePath;
            return f;
        }

        public FileDeletionException(string filePath, Exception innerException = null) : base("File could not be deleted", innerException)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Path to the file which couldn't be deleted.
        /// </summary>

        public string FilePath { get; set; }
    }

    /// <summary>
    /// Fault thrown when a file cannot be created.
    /// </summary>
    
    [DataContract]
    public class FileDeletionFault : BasicFault
    {
        public FileDeletionFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string FilePath { get; set; }
    }
}
