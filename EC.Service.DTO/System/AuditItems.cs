using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// A DTO Audit entry
    /// </summary>
 
    [DataContract]
    [KnownType(typeof(BasicChannelAudit))]

    public class Audit 
    {
        /// <summary>
        /// The Guid that is used to uniquely identify this instance.
        /// </summary>

        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// The audit action being performed.
        /// </summary>

        [DataMember]
        public string ActionString { get; set; }

        /// <summary>
        /// Gets or sets user Id
        /// </summary>
        [DataMember]
        public Guid CreatedByUserID { get; set; }

        /// <summary>
        /// Get or set first name 
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets users email 
        /// </summary>
        [DataMember]
        public string Email { get; set; }
                
        /// <summary>
        ///Registration Id. Not filled in for all audits.
        /// </summary>

        [DataMember]
        public Guid? RegistrationId { get; set; }
    }
    
    /// <summary>
    /// Report for the course offering progress. 
    /// E.g. how many students already visited how many links in the course.
    /// External links are ignored at the moment.
    /// </summary>
    
    [DataContract]
    public class CourseOfferingProgressAudit
    {
        /// <summary>
        /// Short Id of the course offering.
        /// </summary>

        [DataMember]
        public string ShortId { get; set; }

        /// <summary>
        /// Course Id.
        /// </summary>

        [DataMember]
        public Guid CourseId { get; set; }

        /// <summary>
        /// Course offering Id.
        /// </summary>

        [DataMember]
        public Guid CourseOfferingId { get; set; }

        /// <summary>
        /// Number of students enrolled for this course offering.
        /// A value of -1 means N.A. (not available)
        /// </summary>

        [DataMember]
        public int StudentCount { get; set; }

        /// <summary>
        /// When the date range starts for the report.
        /// A value of <c>null</c> means N.A. (not available).
        /// </summary>

        [DataMember]
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// When the date range ends for the report.
        /// A value of <c>null</c> means N.A. (not available).
        /// </summary>

        [DataMember]
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Count of all links (not only paged links).
        /// A value of -1 means N.A. (not available).
        /// </summary>

        [DataMember]
        public int LinkCount { get; set; }

        /// <summary>
        /// Percentage of visits of all links (not only paged links).
        /// A value of -1 means N.A. (not available).
        /// </summary>

        [DataMember]
        public double LinkVisitsAsPercentage { get; set; }

        /// <summary>
        /// Count of all mandatory links (not only paged links).
        /// A value of -1 means N.A. (not available).
        /// </summary>

        [DataMember]
        public int MandatoryLinkCount { get; set; }

        /// <summary>
        /// Percentage of total visits of all mandatory links (not only paged links).
        /// A value of -1 means N.A. (not available).
        /// </summary>

        [DataMember]
        public double MandatoryLinkVisitsAsPercentage { get; set; }

        /// <summary>
        /// Count of all optional links (not only paged links).
        /// A value of -1 means N.A. (not available).
        /// </summary>

        [DataMember]
        public int OptionalLinkCount { get; set; }

        /// <summary>
        /// Percentage of visits of all optional links (not only paged links).
        /// A value of -1 means N.A. (not available).
        /// </summary>

        [DataMember]
        public double OptionalLinkVisitsAsPercentage { get; set; }

        /// <summary>
        /// Indicate the currently set filter which type(s) the <c>LinkList</c> contains.
        /// </summary>

        [DataMember]
        public LinkTypeFilterEnum CurrentLinkFilter { get; set; }

        /// <summary>
        /// Paged list of links with display name, unique visits and is mandatory flag.
        /// </summary>

        [DataMember]
        public CollectionSubset<LinkProgress> LinkList { get; set; }
    }

    /// <summary>
    /// Progress for a single link (count of unique visits).
    /// </summary>

    [DataContract]
    public class LinkProgress
    {
        /// <summary>
        /// Display name of the link.
        /// </summary>

        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Count of unique visits for this link. If one user visited the link multiple times it only counts as one visit.
        /// A value of -1 means N.A. (not available).
        /// </summary>

        [DataMember]
        public int VisitsCount { get; set; }

        /// <summary>
        /// Percentage of unique visits (2 precisions) of the link (if all registered students visits the link = 100%).
        /// A value of -1 means N.A. (not available).
        /// </summary>
        
        [DataMember]
        public double VisitsAsPercentage { get; set; }

        /// <summary>
        /// This is the path to the link, used as tool tip to help differentiate between link with the same display text
        /// </summary>
        
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// Flag whether the link is mandatory (<c>true</c>) or optional (<c>false</c>).
        /// </summary>

        [DataMember]
        public bool IsMandatory { get; set; }
    }

    /// <summary>
    /// Columns to be able to sort for the link progresses.
    /// </summary>

    [DataContract]
    public enum CourseOfferingProgressAuditSortTypesEnum
    {
        /// <summary>
        /// Sort by link name.
        /// </summary>
        
        [EnumMember]
        LinkName = 0,

        /// <summary>
        /// Sort by progress in percentage per link.
        /// </summary>

        [EnumMember]
        LinkVisitsAsPercentage = 1,

        /// <summary>
        /// Sort by Number of students who have visited a link.
        /// </summary>

        [EnumMember]
        LinkVisitsCount = 4,

        /// <summary>
        /// Sort by mandatory/optional links.
        /// </summary>

        [EnumMember]
        IsMandatory = 8
    }
}
