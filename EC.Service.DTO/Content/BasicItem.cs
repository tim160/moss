using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO for BasicItem. A basic item is anything in the system that has attributes.
    /// </summary>
    
    [DataContract]
    [KnownType(typeof(FileCollection))]
    
    public class BasicItem
    {
        /// <summary>
        /// The Guid that is used to uniquely identify this instance.
        /// </summary>

        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// The name for the item that is used in building URL's that refer to the item.
        /// </summary>

        [DataMember]
        public string URLName { get; set; }

        /// <summary>
        /// A set of key/value pairs associated with the item. Used for various purposes
        /// in the system (permissions, dynamic content generation, configuration, etc).
        /// </summary>
        
        [DataMember]
        public List<AttributeItem> Attributes { get; set; }
    }
}
