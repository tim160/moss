using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Thrown if a packages can't be applied because it is not meant for this site.
    /// </summary>

    public class WrongPackageException : FaultableException<WrongPackageFault>
    {
        public override WrongPackageFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new WrongPackageFault(Message, path, userInfo);
            f.PackageApplyPath = PackageApplyPath;
            return f;
        }

        public WrongPackageException(string msg, string path, Exception innerException = null) : base(msg, innerException)
        {
            this.PackageApplyPath = path;
        }

        /// <summary>
        /// Path where the package has been tried to be applied.
        /// </summary>

        public string PackageApplyPath { get; set; }
    }

    /// <summary>
    /// Thrown if a packages can't be applied because it is not meant for this site.
    /// </summary>

    [DataContract]
    public class WrongPackageFault : BasicFault
    {
        public WrongPackageFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public string PackageApplyPath { get; set; }
    }
}
