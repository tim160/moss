using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Thrown if a package has already been applied.
    /// </summary>

    public class PackageAlreadyAppliedException : FaultableException<PackageAlreadyAppliedFault>
    {
        public override PackageAlreadyAppliedFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new PackageAlreadyAppliedFault(Message, path, userInfo);
            f.PackageId = PackageId;
            f.PackageApplyPath = PackageApplyPath;
            return f;
        }

        public PackageAlreadyAppliedException(string msg, Guid packageId, string path, Exception innerException = null) : base(msg, innerException)
        {
            this.PackageId = packageId;
            this.PackageApplyPath = path;
        }

        /// <summary>
        /// The package Id.
        /// </summary>
        
        public Guid PackageId { get; set; }

        /// <summary>
        /// Path where the package has been tried to be applied.
        /// </summary>

        public string PackageApplyPath { get; set; }
    }

    /// <summary>
    /// Fault thrown if a package has already been applied.
    /// </summary>
    
    [DataContract]
    public class PackageAlreadyAppliedFault : BasicFault
    {
        public PackageAlreadyAppliedFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// The package Id.
        /// </summary>
        
        [DataMember]
        public Guid PackageId { get; set; }

        [DataMember]
        public string PackageApplyPath { get; set; }
    }
}
