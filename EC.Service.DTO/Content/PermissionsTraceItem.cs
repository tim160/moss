using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// This DTO is used to return a permissions trace. A permissions trace consists of a
    /// list of PermissionsTraceItems representing the set of links traversed during the
    /// permissions check.
    /// </summary>

    [DataContract]
    public class PermissionTrace
    {
        /// <summary>
        /// List of permission trace items that make up a complete trace.
        /// </summary>
        
        [DataMember]
        public List<PermissionTraceItem> Trace { get; set; }
    }

    /// <summary>
    /// One element of a permission trace.
    /// </summary>
    
    [DataContract]
    public class PermissionTraceItem
    {
        /// <summary>
        /// The path for the link where the check was carried out.
        /// </summary>
        
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// The result of the permissions check.
        /// </summary>
        
        [DataMember]
        public bool? Result { get; set; }

        /// <summary>
        /// The 'strength' of the permissions result.
        /// </summary>
        
        [DataMember]
        public int Level;

        /// <summary>
        /// Indicates that the permission result is incomplete because the user was not 
        /// authorized and therefore not all predicates could be evaluated.
        /// </summary>
        
        [DataMember]
        public bool NeedsAuth;

        /// <summary>
        /// Indicates that this trace item is a result read from the permissions cache.
        /// NOTE: The trace will contain further items which will provide details on
        /// the value that would have been returned if the cache had not been consulted.
        /// </summary>

        public bool IsCacheResult;
    }
}
