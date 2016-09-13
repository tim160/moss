using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// During automatic synchronization sync packages are being up- and downloaded. 
    /// This represents which direction of transfer.
    /// </summary>

    [DataContract]
    public enum TransferTypesEnum
    {
        /// <summary>
        /// Indicates that a sync package is being downloaded from Main.
        /// </summary>
        /// <remarks>
        /// This is always a MainToRta sync package.
        /// </remarks>

        [EnumMember]
        Download = 0,

        /// <summary>
        /// Indicates that a sync package is being uploaded onto Main.
        /// </summary>
        /// <remarks>
        /// This is always an RtaToMain sync package.
        /// </remarks>

        [EnumMember]
        Upload = 1
    }
}
