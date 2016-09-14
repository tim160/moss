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
    /// Thrown if a folder or file doesn't exist.
    /// </summary>

    public class FolderOrFileNotFoundException : FaultableException<FolderOrFileNotFoundFault>
    {
        public override FolderOrFileNotFoundFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new FolderOrFileNotFoundFault(Message, reqPath, userInfo);
            f.FilePath = FilePath;
            return f;
        }

        public FolderOrFileNotFoundException(string path, string msg, Exception innerException = null) : base(msg, innerException)
        {
            this.FilePath = path;
        }

        public string FilePath { get; set; }
    }

    /// <summary>
    /// Fault thrown if a folder or file doesn't exist.
    /// </summary>
    
    [DataContract]
    public class FolderOrFileNotFoundFault : BasicFault
    {
        public FolderOrFileNotFoundFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string FilePath { get; set; }
    }
}
