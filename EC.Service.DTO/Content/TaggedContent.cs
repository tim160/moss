using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO for tagged content
    /// </summary>

    [DataContract]
    public class TaggedContent : BasicItem
    {
        /// <summary>
        /// list of tags 
        /// </summary>

        [DataMember]
        public List<Tag> Tags { get; set; }

        /// <summary>
        /// simple content path
        /// NavPage/-File/relative path to the file
        /// </summary>

        [DataMember]
        public string SimpleContentPath { get; set; }

        /// <summary>
        /// actual content
        /// </summary>

        [DataMember]
        public string Content { get; set; }
    }
}
