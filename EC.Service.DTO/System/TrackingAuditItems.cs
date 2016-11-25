using System;
using System.Runtime.Serialization;
using EC.Constants;

namespace EC.Service.DTO
{
    [DataContract]
    public class TrackingAudit : Audit
    {
        /// <summary>
        /// Gets or sets action. Use AuditAction enum to set;
        /// </summary>
        [DataMember]
        public TrackingAuditActions Action { get; set; }

        /// <summary>
        /// Gets or sets path for the action item
        /// </summary>
        [DataMember]
        public string ItemPath { get; set; }

        /// <summary>
        /// Gets or sets Id of the action item
        /// </summary>
        [DataMember]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Display test for link
        /// </summary>
        [DataMember]
        public string DisplayText { get; set; }

        /// <summary>
        /// Id of file collection simple content is in.  Only used with file links
        /// </summary>
        [DataMember]
        public Guid? FileCollectionId { get; set; }

        /// <summary>
        /// Relative path for simple content. Only used with file links
        /// </summary>
        [DataMember]
        public string RelativePath { get; set; }


        /// <summary>
        /// URl, only used for external links
        /// </summary>
        [DataMember]
        public string Url { get; set; }
    }

    /// <summary>
    /// DTO for holding information about a link that a user has viewed.
    /// </summary>
    
    [DataContract]
    public class LinkAuditEntry
    {
        /// <summary>
        /// Gets or sets path internal path or external url
        /// </summary>
        [DataMember]
        public string path { get; set; }

        /// <summary>
        /// The ID of the last link on the path.
        /// </summary>
        
        [DataMember]
        public Guid LinkId { get; set; }
        
        /// <summary>
        /// Gets or sets IsInternal for item
        /// </summary>
        [DataMember]
        public bool IsInternal { get; set; }

        /// <summary>
        /// Number of times user has view this link
        /// </summary>
        [DataMember]
        public int ViewCount { get; set; }

        /// <summary>
        /// Gets or sets IsInternal for item
        /// </summary>
        [DataMember]
        public bool IsMandatory { get; set; }

        /// <summary>
        /// Display test for link
        /// </summary>
        [DataMember]
        public string DisplayText { get; set; }

        /// <summary>
        /// Id of file collection simple content is in.  Only used with file links
        /// </summary>
        [DataMember]
        public Guid? FileCollectionId { get; set; }

        /// <summary>
        /// Relative path for simple content. Only used with file links
        /// </summary>
        [DataMember]
        public string RelativePath { get; set; }

        /// <summary>
        /// Url. Only used with external links
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackingAuditLinkType linkType {get;set;}
    }

    [DataContract]
    public enum LinkAuditEntrySort
    {
        [EnumMember]
        DisplayText = 0,

        [EnumMember]
        TimesViewed = 1,

        [EnumMember]
        Location = 2,

        [EnumMember]
        Mandatory = 4
    }
   

    [DataContract]
    public enum TrackingAuditLinkType
    {
        [EnumMember]
        File = 0,

        [EnumMember]
        External = 1,

        [EnumMember]
        Exam = 2,

        [EnumMember]
        NavPage = 3,

        [EnumMember]
        StudentEvaluation = 4
    }
  
    /// <summary>
    /// Filter for link types (mandatory, optional).
    /// Set filter to get the specific link type.
    /// </summary>

    [DataContract]
    public enum AllLinksUserCanSeeFilterEnum
    {
        /// <summary>
        /// Return both - mandatory and optional links.
        /// </summary>

        [EnumMember]
        Both = 0,

        /// <summary>
        /// Return only mandatory links.
        /// </summary>

        [EnumMember]
        Mandatory = 1,

        /// <summary>
        /// Return only optional links.
        /// </summary>

        [EnumMember]
        Optional = 2
    }

}
