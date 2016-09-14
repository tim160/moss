using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown if nav page template file is not cshtml
    /// </summary>

    public class FileNotCshtmlException : FaultableException<FileNotCshtmlFault>
    {
        public override FileNotCshtmlFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new FileNotCshtmlFault(Message, reqPath, userInfo);
            f.FileCollectionPath = FileCollectionPath;
            f.RelativeFilePath = RelativeFilePath;
            return f;
        }

        public FileNotCshtmlException(string message, string relativeFilePath, string fileCollectionPath, Exception innerException = null) : base(message, innerException)
        {
            this.FileCollectionPath = fileCollectionPath;
            this.RelativeFilePath = relativeFilePath;
        }

        public string FileCollectionPath { get; set; }
        public string RelativeFilePath { get; set; }
    }

    /// <summary>
    /// Fault thrown if nav page template file is not cshtml
    /// </summary>
    
    [DataContract]
    public class FileNotCshtmlFault : BasicFault
    {
        public FileNotCshtmlFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string FileCollectionPath { get; set; }

        [DataMember]
        public string RelativeFilePath { get; set; }
    }
}
