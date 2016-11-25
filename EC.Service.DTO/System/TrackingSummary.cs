using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO used to hold summary information about all page visits.
    /// </summary>
    
    [DataContract]
    public class TrackingSummary
    {
        /// <summary>
        /// The URL this summary information applies to.
        /// </summary>
        
        [DataMember]
        public String URL { get; set; }
        
        /// <summary>
        /// The number of unique users who accessed the URL.
        /// </summary>
        
        [DataMember]
        public int UniqueUsers { get; set; }
        
        /// <summary>
        /// The total number of accesses to the URL.
        /// </summary>
        
        [DataMember]
        public int TotalAccesses { get; set; }
    }
}
