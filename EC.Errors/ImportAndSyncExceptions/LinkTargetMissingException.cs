using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Exception thrown after the import process if the target of a link is 
    /// non-existent.
    /// </summary>

    public class LinkTargetMissingException : FaultableException<LinkTargetMissingFault>
    {
        public override LinkTargetMissingFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new LinkTargetMissingFault(Message, reqPath, userInfo);
            f.TargetPath = TargetPath;
            return f;
        }

        public LinkTargetMissingException(string message, string targetPath, Exception innerException = null) : base(message, innerException)
        {
            TargetPath = targetPath;
        }

        public string TargetPath { get; set; }
    }

    [DataContract]
    public class LinkTargetMissingFault : BasicFault
    {
        public LinkTargetMissingFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string TargetPath { get; set; }
    }
}
