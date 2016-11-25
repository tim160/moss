using EC.Constants;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// Data structure that allows for the construction of a tree that represents a users group membership within an organization.
    /// </summary>
    [DataContract]
    public class UserGroupDetails
    {
        /// <summary>
        /// Path where current node is located.
        /// <remarks>
        /// The path was required to orient ourselves within the tree.
        /// </remarks>
        /// </summary>

        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// DisplayName for the path. 
        /// </summary>

        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Indicates which type of page this path is marked.
        /// </summary>

        [DataMember]
        public NavPageTagTypesEnum PageType { get; set; }


        /// <summary>
        /// List of GroupMemberships for the current UserGroupDefinitions tree node.
        /// <remarks>
        /// Including the GroupName and isMember for a given user.
        /// </remarks>
        /// </summary>

        [DataMember]
        public List<GroupMembership> GroupInformation { get; set; }

        /// <summary>
        /// List of UserGroupDefinitions which represent the children of the current tree node.
        /// <remarks>
        /// Each SubPages include GroupInformation and Subpages of their own.
        /// </remarks>
        /// </summary>

        [DataMember]
        public List<UserGroupDetails> SubPages { get; set; }
    }
    /// <summary>
    /// If used internally in UserGroupDetails, the IsMember is not only determined by the users defined within the local group, but by the
    /// subgroups that are defined higher up in the tree structure as well.
    /// </summary>
    [DataContract]
    public class GroupMembership
    {
        /// <summary>
        /// Name of the group
        /// </summary>

        [DataMember]
        public string GroupName { get; set; }

        /// <summary>
        /// If a given user is a member of the current group, this value is set to true, otherwise false
        /// </summary>

        [DataMember]
        public bool IsMember { get; set; }

    }
    /// <summary>
    /// Data structure that allows for the construction of a tree structure that represents 
    /// an organization and its group definitions, which indludes the total user count for each group
    /// including users defined in subgroups if they are defined higher up in the tree structure.
    /// </summary>
    [DataContract]
    public class GroupDefinitions
    {
        /// <summary>
        /// Path where the group(s) is (are) defined and the subpages belong to.
        /// </summary>
        
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// List of LocalGroupDefinition for all subpages at this level.
        /// </summary>

        [DataMember]
        public List<LocalGroupDefinition> GroupInformation { get; set; }

        /// <summary>
        /// List with GroupDefinitions for this path.
        /// </summary>

        [DataMember]
        public List<GroupDefinitions> SubPages { get; set; }

    }
    /// <summary>
    /// If used as an internal data structure in GroupDefinitions, this data structure will include
    /// all users for the group including the subgroups, if they are defined in higher up in the tree.
    /// The data structure can be used on its own and depending in the methods it may or may not include
    /// the total number of users.
    /// </summary>
    [DataContract]
    public class LocalGroupDefinition
    {
        /// <summary>
        /// GroupName 
        /// </summary>

        [DataMember]
        public string GroupName { get; set; }

        /// <summary>
        /// List of users defined at this level (without user defined in the GroupNames)
        /// </summary>

        [DataMember]
        public List<Guid> UserIds { get; set; }

        /// <summary>
        /// List of GroupNames defined on this level
        /// </summary>

        [DataMember]
        public List<string> GroupNames { get; set; }
    }

    /// <summary>
    /// Data bag for major customer group names
    /// </summary>
    [DataContract]
    public class AdminGroupNames
    {
        /// <summary>
        /// UserAdminGroupName for Organization
        /// </summary>

        [DataMember]
        public string UserAdminGroupName { get; set; }

        /// <summary>
        /// CourseAdminGroupName for Organization
        /// </summary>

        [DataMember]
        public string CourseAdminGroupName { get; set; }

        /// <summary>
        /// UserGroupName for Organization
        /// </summary>

        [DataMember]
        public string UserGroupName { get; set; }
    }

}
