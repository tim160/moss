using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Thrown if a package's path does not match the caller's path where the package was applied.
    /// </summary>

    public class PathsDoNotMatchException : FaultableException<PathsDoNotMatchFault>
    {
        public override PathsDoNotMatchFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new PathsDoNotMatchFault(Message, path, userInfo);
            f.PackagePath = PackagePath;
            f.AppliedPath = AppliedPath;
            return f;
        }

        public PathsDoNotMatchException(string msg, string packagePath, string appliedPath, Exception innerException = null) : base(msg, innerException)
        {
            this.PackagePath = packagePath;
            this.AppliedPath = appliedPath;
        }

        /// <summary>
        /// Path in the package.
        /// </summary>

        public string PackagePath { get; set; }

        /// <summary>
        /// Path where the package has been tried to be applied.
        /// </summary>

        public string AppliedPath { get; set; }
    }

    [DataContract]
    public class PathsDoNotMatchFault : BasicFault
    {
        public PathsDoNotMatchFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Path in the package.
        /// </summary>
        
        [DataMember]
        public string PackagePath { get; set; }

        /// <summary>
        /// Path where the package has been tried to be applied.
        /// </summary>
        
        [DataMember]
        public string AppliedPath { get; set; }
    }
}
