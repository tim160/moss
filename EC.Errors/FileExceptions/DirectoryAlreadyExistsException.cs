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
    /// Exception used to indicate that the directory already exists.
    /// </summary>

    public class DirectoryAlreadyExistsException : FaultableException<DirectoryAlreadyExistsFault>
    {
        public override DirectoryAlreadyExistsFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new DirectoryAlreadyExistsFault(Message, path, userInfo);
            f.DirectoryPath = DirectoryPath;
            return f;
        }

        public DirectoryAlreadyExistsException(string msg, string directoryPath, Exception innerException = null) : base(msg, innerException)
        {
            this.DirectoryPath = directoryPath;
        }

        public DirectoryAlreadyExistsException(string directory, Exception innerException = null) : base(string.Format("Directory already exists '{0}'.", directory), innerException)
        {
            this.DirectoryPath = directory;
        }

        public string DirectoryPath { get; set; }
    }

    /// <summary>
    /// Fault used to indicate that the directory already exists.
    /// </summary>

    [DataContract]
    public class DirectoryAlreadyExistsFault : BasicFault
    {
        public DirectoryAlreadyExistsFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string DirectoryPath { get; set; }
    }
}
