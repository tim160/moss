using System.Collections.Generic;

namespace EC.Constants
{
    /// <summary>
    /// Several constants for TransferStates.
    /// </summary>

    public static class TransferStateConstants
    {
        /// <summary>
        /// List of states a download automatic sync package (MainToRta) Main task is considered running (not finished).
        /// </summary>

        public static readonly IList<TransferStatesEnum> DownloadPackageRunningStates = new List<TransferStatesEnum>()
        {
            TransferStatesEnum.Created,
            TransferStatesEnum.InTransit
        };

        /// <summary>
        /// List of states an upload automatic sync package (RtaToMain) Main task is considered running (not finished).
        /// </summary>

        public static readonly IList<TransferStatesEnum> UploadPackageRunningStates = new List<TransferStatesEnum>()
        {
            TransferStatesEnum.Created,
            TransferStatesEnum.InTransit,
            TransferStatesEnum.TransferComplete
        };

        /// <summary>
        /// List of state a download automatic sync package (MainToRta) Main task is considered complete (final).
        /// </summary>

        public static readonly IList<TransferStatesEnum> DownloadPackageFinalStates = new List<TransferStatesEnum>()
        {
             TransferStatesEnum.Cancelled,
             TransferStatesEnum.TimedOut,
             TransferStatesEnum.TransferComplete
        };

        /// <summary>
        /// List of state an uploaded automatic sync package (RtaToMain) Main task is considered complete (final).
        /// </summary>

        public static readonly IList<TransferStatesEnum> UploadPackageFinalStates = new List<TransferStatesEnum>()
        {
             TransferStatesEnum.Cancelled,
             TransferStatesEnum.TimedOut,
             TransferStatesEnum.Applied
        }; 
    }
}
