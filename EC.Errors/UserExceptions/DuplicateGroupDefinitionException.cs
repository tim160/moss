using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Errors.UserExceptions
{
    public class DuplicateGroupDefinitionException : FaultableException<DuplicateGroupDefinitionFault>
    {
        public override DuplicateGroupDefinitionFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new DuplicateGroupDefinitionFault(Message, path, userInfo);
            f.DuplicateGroupNames = (List<string>) DuplicateGroupNames;
            return f;
        }

        public DuplicateGroupDefinitionException() : base() { }

        public DuplicateGroupDefinitionException(string message,  IList<string> duplicateGroupNames, Exception innerException = null) : base(message, innerException)
        {
            DuplicateGroupNames = duplicateGroupNames;
        }
        
        /// <summary>
        /// List of duplicate group names found.
        /// </summary>

        public IList<string> DuplicateGroupNames { get; set; }
    }

    /// <summary>
    /// Exception thrown if a user is accessed but is in a deleted state.
    /// </summary>

    [DataContract]
    public class DuplicateGroupDefinitionFault : BasicFault
    {
        public DuplicateGroupDefinitionFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
        
        /// <summary>
        /// List of duplicate group names found.
        /// </summary>

        [DataMember]
        public List<string> DuplicateGroupNames { get; set; }
    }
    
}
