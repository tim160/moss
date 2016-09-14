using System.Runtime.Serialization;

namespace EC.Constants
{
    // Once registrations are overhauled we'll actually have a pending state and we can remove this
    // RegistrationReportStatus enum and replace it with RegistrationState. But for now, don't.
    [DataContract]
    public enum RegistrationReportStatus
    {
        [EnumMember]
        Pending = 0,

        [EnumMember]
        Active = 1,

        [EnumMember]
        Completed = 2,
    }

    [DataContract]
    public enum RegistrationReportStatusFilter
    {
        [EnumMember]
        All = 0,

        [EnumMember]
        Pending = 1,

        [EnumMember]
        Active = 2,

        [EnumMember]
        Completed = 3
    }

    [DataContract]
    public enum RegistrationReportHasSessionAuditsFilter
    {
        [EnumMember]
        All = 0,

        [EnumMember]
        NoSessionAudits = 1,

        [EnumMember]
        YesSessionAudits = 2
    }
}