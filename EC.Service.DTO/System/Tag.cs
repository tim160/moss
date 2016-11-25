using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EC.Constants;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO for BasicItem. A basic item is anything in the system that has attributes.
    /// </summary>
    
    [DataContract]
    public class Tag
    {
        /// <summary>
        /// Primary ID for the item.
        /// Set to Guid.Empty to not to set an Id.
        /// </summary>
        
        [DataMember]
        public Guid? Id { get; set; }

        /// <summary>
        /// Tag name. It's unique and constant over the tag's lifetime.
        /// </summary>

        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Tag display name
        /// </summary>

        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Tag state
        /// </summary>

        [DataMember]
        public ModelState State { get; set; }
    }

    /// <summary>
    /// DTO for BasicItem. A basic item is anything in the system that has attributes.
    /// </summary>

    [DataContract]
    public class TagSet
    {
        /// <summary>
        /// Primary ID for the item.
        /// Set to Guid.Empty to not to set an Id.
        /// </summary>

        [DataMember]
        public Guid? Id { get; set; }

        /// <summary>
        /// TagSet name. It's unique and constant over the tagset's lifetime.
        /// </summary>

        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Tag display name
        /// </summary>

        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// TagSet Liftime
        /// </summary>

        [DataMember]
        public TagLifeTimes LifeTime { get; set; }

        /// <summary>
        /// State , can be active=0, deleted=1 and orphaned=2
        /// </summary>

        [DataMember]
        public ModelState State { get; set; }

        /// <summary>
        /// List of all associated tags
        /// </summary>

        [DataMember]
        public List<Tag> Tags { get; set; }
     }
}
