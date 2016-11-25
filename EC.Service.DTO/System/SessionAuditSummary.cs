using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO to hold summary session audit information for a single user.
    /// </summary>
    
    [DataContract]
    public class SessionAuditSummary
    {
        /// <summary>
        /// Primary ID of user
        /// </summary>
        
        [DataMember]
        public Guid UserGuid { get; set; }
        
        /// <summary>
        /// Display name for user (included just so we don't need to call back into server
        /// to get this information)
        /// </summary>
        
        [DataMember]        
        public string UserDisplayName { get; set; }

        /// <summary>
        /// The number of sessions that this user has has (probably filtered over a given
        /// period by the service call that returns this DTO).
        /// </summary>

        [DataMember]
        public int SessionCount { get; set; }

        /// <summary>
        /// The start date of the most recent session for this user.
        /// </summary>

        [DataMember]
        public DateTime LastSessionStart { get; set; }
        [DataMember]
        public DateTime LastSessionStartSysTime { get; set; }
    }
}
