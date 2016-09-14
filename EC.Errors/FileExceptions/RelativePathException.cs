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
    /// Exception thrown when a relative path is used where one is not allowed.
    /// </summary>

    public class RelativePathException : FaultableException<RelativePathFault>
    {
        public override RelativePathFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new RelativePathFault(Message, path, userInfo);
            f.FilePath = FilePath;
            return f;
        }

        public RelativePathException(string filePath, Exception innerException = null) : base(string.Format("Relative path not allowed", filePath), innerException)
        {
            this.FilePath = filePath;
        }

        public string FilePath { get; set; }
    }

    /// <summary>
    /// Fault to indicate that a relative path is used where one is not allowed.
    /// </summary>

    [DataContract]
    public class RelativePathFault : BasicFault
    {
        public RelativePathFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string FilePath { get; set; }
    }
}
