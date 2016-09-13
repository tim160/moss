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
    /// Exception used to indicate that the directory doesn't exist.
    /// </summary>

    public class DirectoryDoesNotExistException : FaultableException<DirectoryDoesNotExistFault>
    {
        public override DirectoryDoesNotExistFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new DirectoryDoesNotExistFault(Message, path, userInfo);
            f.DirectoryPath = DirectoryPath;
            return f;
        }

        public DirectoryDoesNotExistException(string directory, Exception innerException = null) : base(string.Format("Directory doesn't exist '{0}'.", directory), innerException)
        {
            this.DirectoryPath = directory;
        }

        public string DirectoryPath { get; set; }
    }

    /// <summary>
    /// Fault used to indicate that the directory doesn't exist.
    /// </summary>

    [DataContract]
    public class DirectoryDoesNotExistFault : BasicFault
    {
        public DirectoryDoesNotExistFault(string msg, string path, CurrentUserInfo userInfo): base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string DirectoryPath { get; set; }
    }
}
