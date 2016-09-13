using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Errors.UserExceptions
{
    public class UsersBelongToSubGroupsException : FaultableException<UsersBelongToSubGroupsFault>
    {
         public override  UsersBelongToSubGroupsFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new  UsersBelongToSubGroupsFault(Message, path, userInfo);
             f.SubGroupNames = (List<string>) SubGroupNames;
             f.SubGroupUsersToBeRemoved = (List<Guid>) SubGroupUsersToBeRemoved;
            return f;
        }

        public  UsersBelongToSubGroupsException() : base() { }

        public UsersBelongToSubGroupsException(string message, IList<Guid> subGroupUsersToBeRemoved, IList<string> subGroupNames, Exception innerException = null)
            : base(message, innerException)
        {
            SubGroupNames = subGroupNames;
            SubGroupUsersToBeRemoved = subGroupUsersToBeRemoved;
        }
        
        /// <summary>
        /// List of sub group names found.
        /// </summary>

        public IList<string> SubGroupNames { get; set; }

        /// <summary>
        /// List of sub group names found.
        /// </summary>

        public IList<Guid> SubGroupUsersToBeRemoved { get; set; }
    }
        /// <summary>
        /// Exception thrown if a user(s) can't be deleted from a group definition, because they do belong to a subgroup of said group definition.
        /// </summary>

        [DataContract]
        public class UsersBelongToSubGroupsFault : BasicFault
        {
            public UsersBelongToSubGroupsFault(string msg, string path, CurrentUserInfo userInfo)
                : base(msg, path, userInfo)
            {
            }

            /// <summary>
            /// List of subgroup names found.
            /// </summary>

            [DataMember]
            public List<string> SubGroupNames { get; set; }

            /// <summary>
            /// List of subgroup users with a removal attempt found.
            /// </summary>

            [DataMember]
            public List<Guid> SubGroupUsersToBeRemoved { get; set; }
        }
    
}
