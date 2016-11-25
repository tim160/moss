using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using EC.Constants;

namespace EC.Service.DTO
{
    /// <summary>
    /// Essentially a row in the Registration Report.
    /// </summary>

    [DataContract]
    public partial class RegistrationReportItem
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public bool HasSessionAudits { get; set; }

        [DataMember]
        public string CourseName { get; set; }

        [DataMember]
        public string OfferingShortId { get; set; }

        [DataMember]
        public DateTime? CompletionDate { get; set; }

        [DataMember]
        public RegistrationReportStatus RegistrationReportStatus { get; set; }
    }

    [DataContract]
    public enum RegistrationReportItemSort
    {
        [Description("FirstName")]
        [EnumMember]
        FirstName = 0,

        [Description("LastName")]
        [EnumMember]
        LastName = 1,

        [Description("Email")]
        [EnumMember]
        Email = 2,

        [Description("HasSessionAudits")]
        [EnumMember]
        HasSessionAudits = 3,

        [Description("CourseName")]
        [EnumMember]
        CourseName = 4,

        [Description("OfferingShortId")]
        [EnumMember]
        OfferingShortId = 5,

        [Description("CompletionDate")]
        [EnumMember]
        CompletionDate = 6,

        [Description("RegistrationReportStatus")]
        [EnumMember]
        RegistrationReportStatus = 7
    }
}