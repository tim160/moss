using System;
using System.Runtime.Serialization;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Thrown if a sync package couldn't be applied
    /// </summary>

    public class CantApplySyncPackageException : FaultableException<CantApplySyncPackageFault>
    {
        public override CantApplySyncPackageFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new CantApplySyncPackageFault(Message, reqPath, userInfo);
            f.PackageId = PackageId;
            return f;
        }

        public CantApplySyncPackageException(string msg, Guid packageId, Exception innerException = null) : base(msg, innerException)
        {
            PackageId = packageId;
        }

        public CantApplySyncPackageException(string msg, Guid packageId)
            : base(msg)
        {
            PackageId = packageId;
        }

        /// <summary>
        /// Root path of the organization.
        /// </summary>

        public Guid PackageId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>

        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Exception thrown if a sync package couldn't be applied.
    /// </summary>

    [DataContract]
    public class CantApplySyncPackageFault : BasicFault
    {
        public CantApplySyncPackageFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid PackageId { get; set; }
    }
}
