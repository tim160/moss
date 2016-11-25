using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{

    /// <summary>
    /// Attribute item of a content item (e.g. IndexPage, TOC, Repository,...).
    /// </summary>
    
    [DataContract]
    public class AttributeItem
    {
        /// <summary>
        /// Primary ID for the item.
        /// </summary>

        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Get or set the key value of the attribute item.
        /// </summary>

        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Get or set the value of the attribute item.
        /// </summary>

        [DataMember]
        public string Value { get; set; }
    }
}
