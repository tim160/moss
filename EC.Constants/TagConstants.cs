using System.Runtime.Serialization;

namespace EC.Constants
{
    [DataContract]
    public enum TagLifeTimes
    {
        [EnumMember]
        Session = 0,

        [EnumMember]
        Permanent = 1,

        [EnumMember]
        None = 2,
    }

    public class TagConstants
    {
        public const string XmlTagMappingFile = "/tag-mapping.xml";
    }
}
