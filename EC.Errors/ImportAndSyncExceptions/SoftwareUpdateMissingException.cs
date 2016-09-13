using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// If a sync package can't be applied on Main because the RTA is missing
    /// a software update - so the data in the package is not compatible 
    /// with Main. There are breaking changes between the software version
    /// on the RTA and Main.
    /// </summary>

    public class SoftwareUpdateMissingException : FaultableException<SoftwareUpdateMissingFault>
    {
        public override SoftwareUpdateMissingFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new SoftwareUpdateMissingFault(Message, path, userInfo);
            f.PackageId = PackageId;
            return f;
        }

        public SoftwareUpdateMissingException(string msg, Guid packageId, string path, Exception innerException = null) : base(msg, innerException)
        {
            this.PackageId = packageId;
            this.Path = path;
        }

        /// <summary>
        /// The package Id.
        /// </summary>

        public Guid PackageId { get; set; }

        /// <summary>
        /// Path where the package has been tried to be applied.
        /// </summary>

        public string Path { get; set; }
    }

    /// <summary>
    /// If a sync package can't be applied on Main because the RTA is missing
    /// a software update - so the data in the package is not compatible 
    /// with Main. There are breaking changes between the software version
    /// on the RTA and Main.
    /// </summary>

    [DataContract]
    public class SoftwareUpdateMissingFault : BasicFault
    {
        public SoftwareUpdateMissingFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// The package Id.
        /// </summary>

        [DataMember]
        public Guid PackageId { get; set; }
    }
}
