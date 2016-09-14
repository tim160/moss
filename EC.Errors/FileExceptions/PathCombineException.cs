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
    /// Exception used to indicate an error combining 2 paths.
    /// </summary>

    public class PathCombineException : FaultableException<PathCombineFault>
    {
        public override PathCombineFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new PathCombineFault(Message, path, userInfo);
            f.Path1 = Path1;
            f.Path2 = Path2;
            return f;
        }

        public PathCombineException(string path1, string path2, Exception innerException = null) : base(string.Format("Error combining the two paths '{0}' and '{1}'", path1, path2), innerException)
        {
            this.Path1 = path1;
            this.Path2 = path2;
        }

        public string Path1 { get; set; }
        public string Path2 { get; set; }
    }

    /// <summary>
    /// Fault used to indicate an error combining 2 paths.
    /// </summary>

    [DataContract]
    public class PathCombineFault : BasicFault
    {
        public PathCombineFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string Path1 { get; set; }

        [DataMember]
        public string Path2 { get; set; }
    }
}
