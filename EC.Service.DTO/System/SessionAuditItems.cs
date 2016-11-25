using System;
using System.Runtime.Serialization;
using EC.Constants;


namespace EC.Service.DTO
{
    /// <summary>
    /// DTO for a session audit record
    /// </summary>
    
    [DataContract]
    public class SessionAudit : Audit
    {
        /// <summary>
        /// Type of sessions audit record
        /// </summary>

        [DataMember]
        public SessionAuditActions Action { get; set; }

        /// <summary>
        /// Length of session in seconds (only valid for Action type EndSession)
        /// </summary>

        [DataMember]
        public int SessionLength { get; set; }
    }

    /// <summary>
    /// This class has no model representation and is only a data bag for a the course session tracking report
    /// </summary>
    
    [DataContract]
    public class CourseSessionAudit
    {
        /// <summary>
        /// The username of the user CourseSessionAudit is on
        /// </summary>
        
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Id of User CourseSessionAudit is on
        /// </summary>
        
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// The total number of session the user has had
        /// </summary>
        
        [DataMember]
        public int SessionCount { get; set; }

        /// <summary>
        /// Total duration the user has had an active session in seconds
        /// </summary>
        
        [DataMember]
        public int TotalDuration { get; set; }

        /// <summary>
        /// Date of the most recent session
        /// </summary>
        
        [DataMember]
        public DateTime MostRecentSession { get; set; }
        [DataMember]
        public DateTime MostRecentSessionSysTime { get; set; }
    }

    /// <summary>
    /// Sort options for course session tracking report
    /// </summary>

    [DataContract]
    public enum CourseSessionsAuditSortTypes
    {
        [EnumMember]
        UserName,
        
        [EnumMember]
        SessionCount,
        
        [EnumMember]
        TotalDuration,
        
        [EnumMember]
        MostRecent
    }

    /// <summary>
    /// Sort options for users session tracking report
    /// </summary>
    
    [DataContract]
    public enum SessionAuditSortTypes
    {
        [EnumMember]
        Duration,
        
        [EnumMember]
        Date
    }
}
