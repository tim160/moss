using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.ImportAndSyncExceptions
{
    public class ImportException : FaultableException<ImportFault>
    {
        public override ImportFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new ImportFault(Message, path, userInfo);
            f.FileOrDirectory = FileOrDirectory;
            return f;
        }

        public ImportException(string msg, string fileOrDirectory, Exception innerException = null) : base(msg, innerException)
        {
            FileOrDirectory = fileOrDirectory;
        }

        /// <summary>
        /// File or directory where the error happened (if available).
        /// </summary>

        public string FileOrDirectory { get; set; }
    }

    /// <summary>
    /// Exception used to indicate that an error happened during import.
    /// </summary>

    [DataContract]
    public class ImportFault : BasicFault
    {
        public ImportFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// File or directory where the error happened (if available).
        /// </summary>

        [DataMember]
        public string FileOrDirectory { get; set; }
    }
}
