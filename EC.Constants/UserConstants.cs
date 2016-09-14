using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// View helper for typed OrgProfileFields
    /// </summary>
    [DataContract]
    public enum OrgProfileFieldTypes
    {
        [EnumMember]
        OrgProfileFieldString = 0,

        [EnumMember]
        OrgProfileFieldInteger = 1,

        [EnumMember]
        OrgProfileFieldSingleChoiceString = 3
    }
}