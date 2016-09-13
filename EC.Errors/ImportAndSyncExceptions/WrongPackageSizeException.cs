using System.Runtime.Serialization;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Thrown if a sync package has the wrong size.
    /// Usually used for the automatic sync of packages.
    /// </summary>

    public class WrongPackageSizeException: FaultableException<WrongPackageSizeFault>
    {
        public override WrongPackageSizeFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new WrongPackageSizeFault(Message, reqPath, userInfo);
            f.ExpectedPackageSize = ExpectedPackageSize;
            f.CurrentPackageSize = CurrentPackageSize;
            return f;
        }
 
        /// <summary>
        /// Current package size in bytes.
        /// </summary>

        public long CurrentPackageSize { get; set; }
        
        /// <summary>
        /// Package size in bytes.
        /// </summary>
       
        public long ExpectedPackageSize { get; set; }
    }

    /// <summary>
    /// Fault thrown if a folder or file doesn't exist.
    /// </summary>

    [DataContract]
    public class WrongPackageSizeFault : BasicFault
    {
        public WrongPackageSizeFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Current package size in bytes.
        /// </summary>

        [DataMember]
        public long CurrentPackageSize { get; set; }

        /// <summary>
        /// Package size in bytes.
        /// </summary>

        [DataMember]
        public long ExpectedPackageSize { get; set; }
    }
}
