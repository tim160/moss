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
    /// Exception thrown when a directory cannot be created.
    /// </summary>

    public class DirectoryCreationException : FaultableException<DirectoryCreationFault>
    {
        public override DirectoryCreationFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new DirectoryCreationFault(Message, path, userInfo);
            f.DirectoryPath = DirectoryPath;
            return f;
        }

        public DirectoryCreationException(string directoryPath, Exception innerException) : base("Could not create directory", innerException)
        {
            DirectoryPath = directoryPath;
        }

        /// <summary>
        /// Path which couldn't be created.
        /// </summary>

        public string DirectoryPath { get; set; }
    }

    /// <summary>
    /// Fault thrown when a directory cannot be created.
    /// </summary>

    [DataContract]
    public class DirectoryCreationFault : BasicFault
    {
        public DirectoryCreationFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Path to NavPage associated with this fault.
        /// </summary>

        [DataMember]
        public string DirectoryPath { get; set; }
    }
}
