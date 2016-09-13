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
    /// Exception used to indicate an error copying a file.
    /// </summary>

    public class FileCopyException : FaultableException<FileCopyFault>
    {
        public override FileCopyFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new FileCopyFault(Message, path, userInfo);
            f.SourcePath = SourcePath;
            f.TargetPath = TargetPath;
            return f;
        }

        public FileCopyException(string source, string target, Exception innerException = null) : base(string.Format("Error copying file from '{0}' to '{1}'", source, target), innerException)
        {
            this.TargetPath = target;
            this.SourcePath = source;
        }

        public string TargetPath { get; set; }
        public string SourcePath { get; set; }
    }

    /// <summary>
    /// Fault used to indicate an error copying a file.
    /// </summary>
    
    [DataContract]
    public class FileCopyFault : BasicFault
    {
        public FileCopyFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string TargetPath { get; set; }

        [DataMember]
        public string SourcePath { get; set; }
    }
}
