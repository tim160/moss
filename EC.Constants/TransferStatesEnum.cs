using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Defines the states of an automatic sync package while uploading/downloading onto/from Main.
    /// Main receives (upload) and deliveres (download) sync packages from/to an RTA.
    /// </summary>
    /// <remarks>
    /// Download Main sync package onto RTA states:
    /// 1. Created
    /// 2. InTransit
    /// 3. TransferComplete (final state)
    /// 3. Cancelled (final state)
    /// 3. TimedOut (final state)
    /// 
    /// Upload RTA sync package to Main states:
    /// 1. Created
    /// 2. InTransit
    /// 3. TransferComplete
    /// 4. Applying
    /// 5. Applied (final state)
    /// 5. Cancelled (final state)
    /// 6. TimedOut (final state)
    /// </remarks>

    [DataContract]
    public enum TransferStatesEnum
    {
        /// <summary>
        /// Package has been created, prepared for upload/download.
        /// </summary>
        
        [EnumMember]
        Created = 0,

        /// <summary>
        /// A  sync package (for Main or RTA) is in transit - start upload/download has been called or at least one chunk has been uploaded/downloaded.
        /// </summary>

        [EnumMember]
        InTransit = 1,

        /// <summary>
        /// The package has been successfully transferred.
        /// </summary>
        /// <remarks>
        /// For an RTA package this means it has been successfully uploaded to Main but not applied yet.
        /// For a Main package this means the package has been successfully downloaded from Main to RTA.
        /// <para>
        /// This is the final state for downloading a package from Main to RTA.
        /// </para>
        /// </remarks>

        [EnumMember]
        TransferComplete = 2,
        
        /// <summary>
        /// If the sync package has been applied on Main.
        /// </summary>
        /// <remarks>
        /// This is the final state for a package uploaded from RTA to Main.
        /// </remarks>

        [EnumMember]
        Applied = 3,

        /// <summary>
        /// If the package upload/download has been cancelled.
        /// </summary>
        /// <remarks>
        /// This is a final state for a package upload/download.
        /// </remarks>

        [EnumMember]
        Cancelled = 4,

        /// <summary>
        /// If the package has expired (i.e. not been applied/completed/cancelled within a specific time frame).
        /// This will allow the disk-cleanup task to delete the package file on disk.
        /// </summary>
        /// <remarks>
        /// This is a final state for a package upload/download.
        /// </remarks>

        [EnumMember]
        TimedOut = 5
    }
}
