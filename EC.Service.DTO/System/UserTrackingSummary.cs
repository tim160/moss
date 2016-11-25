using System;
using System.Runtime.Serialization;
//using EC.Core.Common;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO to hold summry information about the URLs that a user has visited.
    /// </summary>
    
    [DataContract]
    public class UserTrackingSummary
    {
        /// <summary>
        /// URL that the user has visited.
        /// </summary>
        
        [DataMember]
        public string URL { get; set; }

        /// <summary>
        /// Number of times that URL was accessed.
        /// </summary>
        
        [DataMember]
        public int AccessCount { get; set; }

        /// <summary>
        /// Datetime of user's last access to URL.
        /// </summary>
       
        [DataMember]
        public DateTime LastAccess { get; set; }
        public DateTime LastAccessSysTime { get; set; }
    }
}
