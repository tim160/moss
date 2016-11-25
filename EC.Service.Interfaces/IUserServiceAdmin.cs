using System;
using System.Collections.Generic;
using System.ServiceModel;
using EC.Common.Interfaces;
using EC.Service.DTO;
using EC.Errors;
using EC.Errors.ECExceptions;
using EC.Errors.CommonExceptions;
using EC.Errors.UserExceptions;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// User service operations that should only be accessed by trusted clients. These operations do not
    /// carry out permissions checks, and this service should never be exposed publicly.
    /// </summary>

    [ServiceContract]
    public interface IUserServiceAdmin
    {
        /// <summary>
        /// find all orphaned org profile value audits
        /// </summary>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        List<string> FindAllOrphanedOrgProfileValueAudits();

        /// <summary>
        /// Find all OrgProfileValue audits by ProfileField and ProfileValue. 
        /// Passing a null <paramref name="dateFilter"/> means result will be NOT be filtered by date/time.
        /// </summary>
        /// <param name="dateFilter">If present, only users for whom the profile value was set on or before that date will be returned.</param>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        List<DTO.OrgProfileValueAudit> FindAllOrgProfileValueAuditsByFieldAndValue(string orgPath, string orgProfileField, string orgProfileValue, DateTime? dateFilter);

        /// <summary>
        /// Create users within the <paramref name="csvUserString" />.
        /// A user who doesn't exist will be created (if <paramref name="updateOrCreateUsers"/> == <c>true</c>).
        /// If <paramref name="updateOrCreateUsers" /> == <c>true</c>: Users who already exist will be updated and non-existing users will be created.
        /// If <paramref name="updateOrCreateUsers" /> == <c>false</c>: Users who already exists will be added to [error] list and not updated. Non-existing users will be created.
        /// </summary>
        /// <remarks>
        /// CSV Format: [DX], OrgPath, OrgLoginId, LoginId, Password, FirstName, LastName, EmailAddress, ContactEmail, [OrgProfileFieldName, OrgProfileFieldValue]*
        /// Notes: 
        /// - The first column indicates whether a user should be deleted (D) or deactivated (X). Leave empty to create/update and activate the user
        /// - OrgPath: is optional if no OrgLoginId has been set. Add user to this organization
        /// - OrgLoginId: is optional if LoginId has been set. Don't update if empty. Used as a secondary look up key to find an existing user (if LoginId is empty)
        /// - LoginId: can be user name or email address. Optional if OrgLoginId has been set. Used as a primary look up key to find an existing user
        /// - Password: if the user already exists and left empty - the password is not updated
        /// - FirstName: will not be updated if empty
        /// - LastName: will not be updated if empty
        /// - EmailAddress: will not be updated if empty
        /// - ContactEmail: will not be updated if empty
        /// - OrgProfileFieldName: case-insensitive OrgProfileFieldName (must exist for the organization at OrgPath)
        /// - OrgProfileFieldValue: value of the OrgProfileValue (will not be updated if empty, *remove* to delete the value)
        /// - [OrgProfileFieldName, OrgProfileFieldValue]* means that those columns can occur 0, 1, n times
        /// - A new user is created if a non-existing OrgLoginId is speficied or no LoginId is set
        /// - *remove* can be used to remove a user property/fieldvalue since leaving it empty doesn't update the property/fieldvalue
        /// - *remove* can be used for all columns except: [DX], LoginId, Password and OrgProfileFieldName.
        /// </remarks>
        /// <param name="path">Path to check the permissions for</param>
        /// <param name="csvUserString">String with user definitions (see remarks for formatting details).</param>
        /// <param name="updateOrCreateUsers">
        /// Set <c>true</c> to update existing users or create users if they don't exist yet.
        /// Set <c>false</c> to create non-existing users only. Users who already exist will be added to the error list and not updated.
        /// </param>
        /// <param name="reactivateDeletedUsers">
        /// Attention: Only available on <paramref name="updateOrCreateUsers" /> == <c>true</c>
        /// Set <c>true</c> if a deleted user should be re-activated and updated. 
        /// Set <c>false</c> to leave the user deleted and added to the error list.</param>
        /// <returns>
        /// Return data bag of state after upload. 
        /// [SuccessMessages] is a list of successfully imported/updated users and other success messages. 
        /// [WarnMessages] is a list of warnings (i.e. wrong org profile field,...).
        /// [ErrorMessages] contains a list of not successfully created/updated users or other error messages.
        /// [ErrorMessages] may contain other error messages as well.
        /// </returns>
        /// <exception cref="ParameterValidationException">If the <paramref name="csvUserString"/> is null or empty.</exception>
        /// <exception cref="NullPathException">if the path is null</exception>
        /// <exception cref="PathFormatException">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundException">if there is no item corresponding to the path</exception>
        /// <exception cref="NotAuthorizedException">If the currently logged in user is not allowed to upload users (create/edit).</exception>
        /// <exception cref="Exception">For any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotAuthorizedFault))]

        CsvUserImportState UploadUsers(string path, string csvUserString, bool updateOrCreateUsers, bool reactivateDeletedUsers);

        /// <summary>
        /// Return user with specific login Id (user name or email address) for a not deleted user.
        /// </summary>
        /// <remarks>
        /// Must be active or non-active, but not deleted.
        /// </remarks>
        /// <param name="loginId">Case insensitive login Id (user name or email address or orgId). If orgId you must pass in a value for orgPath</param>
        /// <param name="detailLevel">Contains the level of detail on how much information is passed into the returning user item.</param>
        /// <param name="orgPath">used to look user up by orgId, pass null or empty string if not using orgId</param>
        /// <returns>Return user item.</returns>
        /// <exception cref="NotFoundFault">If the user does not exist or already marked as deleted.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        User GetUserByLoginId(string loginId, string detailLevel, string orgPath);

        /// <summary>
        /// Return user with specific user ID (no matter of its state - active/in-active/deleted).
        /// </summary>
        /// <param name="userId">User ID to get.</param>
        /// <param name="detailLevel">Contains the level of detail on how much information is passed into the returning user item.</param>
        /// <returns>Return user item.</returns>
        /// <exception cref="NotFoundFault">If the user does not exist.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        User GetUserById(Guid userId, string detailLevel);


        /// <summary>
        /// Return user with specific user ID (no matter of its state - active/in-active/deleted) or anonymous user
        /// </summary>
        /// <param name="userId">User ID to get.</param>
        /// <param name="detailLevel">Contains the level of detail on how much information is passed into the returning user item.</param>
        /// <returns>Return user item.</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        User GetUserByIdOrAnonymous(Guid userId, string detailLevel);

        /// <summary>
        /// Return the currently logged in user.
        /// </summary>
        /// <remarks>
        /// The parameter <paramref name="path"/> is optional. If a <paramref name="path"/> and the detail level <c>OrgProfileValues</c>
        /// are set only the path-specific <c>OrgProfileValues</c> are returned. That is if <paramref name="path"/> is at an organization.
        /// </remarks>
        /// <param name="path">Optional: Specify a path for location based information (i.e. org profile field values are returned based of the path).</param>
        /// <param name="detailLevel">Optional: Detail level to populate some user properties (i.e. <c>OrgProfileValues</c>)</param>
        /// <returns>
        /// Return the currently logged in user.
        /// Return <c>null</c> if no user is logged in.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        User GetCurrentUser(string path, string detailLevel);

        /// <summary>
        /// Gets all org path values (of all organizations) visible by the currently logged in user.
        /// </summary>
        /// <returns>
        /// Return the org profile values visible to the user. 
        /// Return empty list if no org profile values are available/visible
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        List<string> GetOrgPathValuesOfCurrentUser();


        /// <summary>
        /// Creates a membership tree for this user starting with the given path.
        /// </summary>
        /// <param name="path">Given path.</param>
        /// <param name="userId">User ID to get.</param>
        /// <returns>Return UserGroupDetails results for user.</returns>
        /// <exception cref="NotFoundFault">If the user does not exist.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]

        UserGroupDetails GetUserGroupDetailsForUser(string path, Guid? userId);

        /// <summary>
        /// Returns the contact email address for a user by (GUID) id.
        /// </summary>
        /// <param name="userId">User's (GUID) id</param>
        /// <returns>User's contact email address</returns>
        /// <exception cref="NotFoundFault">If the user does not exist.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        string GetContactEmailByUserId(Guid userId);

        /// <summary>
        /// Get all the users in the system whose user name contains the given substring. 
        /// </summary>
        /// <param name="matchText">partial string to match against user names</param>
        /// <returns>list of all users in the system with matching user names</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        List<User> GetMatchingUsers(string matchText);

        /// <summary>
        /// Returns users for which the given string is contained within any of: their first name, 
        /// their last name, their username, or their email.
        /// </summary>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        List<User> GetMatchingUsersByAllNameFields(string matchText, string detailLevel);

        /// <summary>
        /// Takes advantage of a new data structure, that allows the collection of users belonging to the same group within an organizational tree. The list
        /// of user ids is than converted into User data and returned in list form.
        /// </summary>
        /// <note>Even if this method is not called with an orgPath, lower level method calls ensure the usage of orgPaths.</note>
        /// <param name="orgPath">Path to Organization</param>
        /// <returns>List of course admin users</returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        List<User> GetAllOrganizationCourseAdmins(string orgPath);

        /// <summary>
        /// Return a set of group definitions for all groups that are defined at the
        /// given path. The group definitions that are returned only include information
        /// about group membership defined along that specific path. That is, group
        /// membership definitions on items not included in the path are not included
        /// in the result. In other words, the group definitions returned do not provide
        /// a global view of group membership, only membership relative to the path.
        /// </summary>
        /// <param name="path">the path to the location in the content tree for which to determine the groups that exist</param>
        /// <returns>a list of group definitions</returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        GroupDefinitions GetGroupsForPath(string path);

        /// <summary>
        /// Gets the entire group definitions tree starting with the given path
        /// </summary>
        /// <param name="path">current path string</param>
        /// <returns>GroupDefinitions</returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        GroupDefinitions GetGroupDefinitions(string path);

        /// <summary>
        /// Remove the given user from all group definitions attached to the item specified
        /// by the given path, and from all items descended from it.
        /// </summary>
        /// <param name="path">item at which to start removing</param>
        /// <param name="userId">user to remove</param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path, or the login Id</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void RemoveUserFromAllGroupsByPath(string path, Guid userId);

        /// <summary>
        /// Remove user from group, if user exists and user belongs to group and not a subgroup of said group.
        /// </summary>
        /// <param name="path">path at which we want remove a user from a group</param>
        /// <param name="userName">user name</param>
        /// <param name="groupName">group name</param>
        /// <exception cref="NotFoundException">If the user does not exist or the user does not belong the group.</exception>
        /// <exception cref="UsersBelongToSubGroupsException">If we try to remove a user that belongs to a subGroup rather then the group </exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UsersBelongToSubGroupsFault))]

        void RemoveUserFromGroup(string path, string userName, string groupName);

        /// <summary>
        /// Create a new empty user group for a given path
        /// </summary>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void CreateGroup(string path, string groupName);

        /// <summary>
        /// Add a group to the given group on the given item.
        /// </summary>
        /// <param name="path">path to the item on which the group definition resides</param>
        /// <param name="memberGroupName">Group name to add to <paramref name="groupName"/></param>
        /// <param name="groupName">name of the group to add <paramref name="memberGroupName"/></param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddSubGroup(string path, string memberGroupName, string groupName);

        /// <summary>
        /// Add a user to the given group using the user name
        /// </summary>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddUserToGroup(string path, string groupName, string userName);

        /// <summary>
        /// Delete a group from the given path
        /// </summary>
        /// /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteGroup(string path, string groupName);

        /// <summary>
        /// Print the current group cache tree from <paramref name="path"/> downward.
        /// </summary>
        /// <param name="path">Path to print the cache from</param>
        /// <param name="detailLevel">Add 'users' and/or 'subgroups' to print out user members and subgroups for a group</param>
        /// <returns>Return string representation (already formatted) of the group cache starting at <paramref name="path"/></returns>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        string PrintGroupCache(string path, string detailLevel);

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string PrintGroupCacheLinkMap();

        /// <summary>
        /// Return a representation of AttributeId to group name mapping of the current cache.
        /// This is for internal use only to check the consistency of the group cache.
        /// </summary>
        /// <returns>Return mapping represented as string</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string PrintAttributeIdGroupCacheMapping();

        /// <summary>
        /// Invalidate the whole group cache.
        /// </summary>
        /// <exception cref="UnknownFault">On any error</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void InvalidateGroupCache();

        /// <summary>
        /// Invalidate a certain <paramref name="groupName"/> at <paramref name="path"/>.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="groupName"/> doesn't exist or is not cached - nothing is done.
        /// However, the <paramref name="path"/> must exist.
        /// </remarks>
        /// <param name="path">Path to invalidate the group</param>
        /// <param name="groupName">Group name (case-sensitive) to invalidate</param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        /// <exception cref="UnknownFault">For any other error</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void InvalidateGroupCacheByPathAndGroup(string path, string groupName);

        /// <summary>
        /// Remove a group from a group membership definition on a particular item.
        /// </summary>
        /// <param name="path">path to the item where the group membership definition is</param>
        /// <param name="memberGroupName">Group to remove from a <paramref name="groupName"/></param>
        /// <param name="groupName">name of the group to remove them from</param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        /// <exception cref="UnknownFault">For any other error</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void RemoveGroupFromGroup(string path, string memberGroupName, string groupName);

        /// <summary>
        /// Updates the user by identifier.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="newPassword">The new password.</param>
        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(EmailFormatFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(OrganizationLogonStateFault))]
        [FaultContract(typeof(AlreadyOrgMemberFault))]

        void UpdateUserById(User user, string newPassword);

        /// <summary>
        /// add deleted user to the organization again
        /// in other word mark the deleted user as not deleted 
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void UnDeleteUser(string userName);

        /// <summary>
        /// Create a new user (at least an email address and password must be set). 
        /// </summary>
        /// <param name="newUser">New user profile (ID and user name will be set automatically).</param>
        /// <param name="password">User password - must not be empty or <c>null</c>.</param>
        /// <returns>Return the new user item (with ID and user name set correctly)</returns>
        /// <remarks>The email address must be unique among all active or inactive users.</remarks>
        /// <exception cref="ParameterValidationFault">If <c>password</c> is empty or <c>null</c>.</exception>
        /// <exception cref="AlreadyExistsFault">If the email address has already been taken by another user (active or inactive).</exception>
        /// <exception cref="DeletedUserFault">If the email address is blocked by a deleted user and the email address can't be used.</exception>
        /// <exception cref="EmailFormatFault">If no email address or an email address with the wrong format is set.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(DeletedUserFault))]
        [FaultContract(typeof(EmailFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        User CreateUser(User newUser, string password);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>

        [OperationContract]

        bool IsUserServiceAdmin();

        /// <summary>
        /// Gets the org profile fields.
        /// </summary>
        /// <param name="orgId">The org identifier.</param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        List<OrgProfileField> GetOrgProfileFields(Guid orgId);

        /// <summary>
        /// Get date which an org profile value has changed to a certain value
        /// </summary>
        /// <exception cref="UnknownFault">For any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        
        DateTime? GetOrgProfileValueChangedDate(Guid orgId, Guid userId, string profileField, string vesselValue);

        /// <summary>
        /// Gets org profile value state in a specific date
        /// </summary>
        /// <exception cref="UnknownFault">For any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        
        string GetOrgProfileValueStateByDate(Guid orgId, Guid userId, string profileField, DateTime? targetDate);

        /// <summary>
        /// Get all org profile fields defined for a given path (which should point
        /// to an organization page).
        /// </summary>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        /// <exception cref="OrganizationNotFoundFault">if the path does not correspond to an org page</exception>
        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(OrganizationNotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        List<OrgProfileField> GetOrgProfileFieldsByPath(string path);

        /// <summary>
        /// Gets the org profile values.
        /// </summary>
        /// <param name="fieldId">The profile field.</param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        List<OrgProfileValue> GetOrgProfileValues(Guid fieldId);

        /// <summary>
        /// Creates the or update org profile field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <exception cref="NotFoundFault">Org not found</exception>
        /// <exception cref="ParameterValidationFault">If the <paramref name="field"/> DisplayName is null or empty whitespace.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]

        void CreateOrUpdateOrgProfileField(OrgProfileField field);

        /// <summary>
        /// Deletes the org profile field.
        /// </summary>
        /// <param name="orgId">The Org identifier.</param>
        /// <param name="displayName">The field display name.</param>
        /// <exception cref="NotFoundFault">If there is no such field.</exception>
        /// <exception cref="ParameterValidationFault">If deleting the field is invalid.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteOrgProfileField(Guid orgId, string displayName);

        /// <summary>
        /// Gets the org profile values by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        List<OrgProfileValue> GetOrgProfileValuesByUserId(Guid userId);

        /// <summary>
        /// Deletes an org profile value for a user.
        /// </summary>
        /// <param name="orgId">The org identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <exception cref="NotFoundFault">If there is no such field or value for the user.</exception>
        /// <exception cref="ParameterValidationFault">If deleting the value is invalid.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteOrgProfileValue(Guid orgId, Guid userId, string fieldName);

        /// <summary>
        /// Set default OrgProfileField value for all users in the organization who don't have the OrgProfileField already assigned (must be active).
        /// Ignore all other users who have already this OrgProfileField.
        /// </summary>
        /// <param name="orgPath">Org path (search along this path to find the organization)</param>
        /// <param name="singleChoiceOrgProfileFieldName">Case-insensitive OrgProfileField name to set (must be a single choice field)</param>
        /// <param name="value">Value to set. Set <c>null</c> to set the (default) first choice.</param>
        /// <exception cref="NotFoundFault">If the path can't be found or no organization can be found at <paramref name="orgPath"/> or the <paramref name="singleChoiceOrgProfileFieldName"/> doesn't exist</exception>
        /// <exception cref="ParameterValidationFault">If the org profile field is not of type single choice string or the <paramref name="value"/> is wrong</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddOrgProfileValues(string orgPath, string singleChoiceOrgProfileFieldName, string value);

        /// <summary>
        /// Gets the available org profile fields by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the <paramref name="userId"/> doesn't exist or has been deleted.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        List<OrgProfileField> GetAvailableOrgProfileFieldsByUserId(Guid userId);

        /// <summary>
        /// Adds a new choice to a OrgProfileFieldSingleChoiceString.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// /// <param name="newChoice">The new choice to add.</param>
        /// <returns></returns>
        /// <exception cref="ParameterValidationFault">If the <paramref name="newChoice"/> contains an invalid character.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="fieldId"/> doesn't exist.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddOrgProfileFieldSingleChoiceStringChoice(Guid fieldId, string newChoice);

        /// <summary>
        /// Removes an existing choice from a OrgProfileFieldSingleChoiceString. You must specify an existing <paramref name="existingChoiceToChangeValuesTo"/>
        /// to change any existing OrgProfileValues to, for this field.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="choiceToRemove">The choice to remove.</param>
        /// <param name="existingChoiceToChangeValuesTo">The choice to change any existing values to.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the <paramref name="fieldId"/> doesn't exist, or the <paramref name="choiceToRemove"/> or
        /// <paramref name="existingChoiceToChangeValuesTo"/> doesn't exist.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void RemoveOrgProfileFieldSingleChoiceStringChoice(Guid fieldId, string choiceToRemove, string existingChoiceToChangeValuesTo);

        /// <summary>
        /// Replaces an existing choice for a OrgProfileFieldSingleChoiceString with a new choice. Any existing OrgProfileValues will be
        /// changed to the new choice.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="choiceToReplace">The choice to replace.</param>
        /// <param name="newChoice">The new choice.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the <paramref name="fieldId"/> doesn't exist, or the <paramref name="choiceToReplace"/></exception>
        /// <exception cref="ParameterValidationFault">If the <paramref name="newChoice"/> contains an invalid character.</exception>
        /// <exception cref="AlreadyExistsFault">If the <paramref name="newChoice"/> already exists.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(UnknownFault))]

        void ReplaceOrgProfileFieldSingleChoiceStringChoice(Guid fieldId, string choiceToReplace, string newChoice);

        /// <summary>
        /// Reorders the existing choices for a OrgProfileFieldSingleChoiceString.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="choices">The choices, in the order desired.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the <paramref name="fieldId"/> doesn't exist, or the <paramref name="choices" /> do not match
        /// (other than order) the existing choices in the field.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void ReorderOrgProfileFieldSingleChoiceStringChoices(Guid fieldId, List<string> choices);

        /// <summary>
        /// Renames an OrgProfileField.
        /// </summary>
        /// <param name="orgId">The orgId for the existing field.</param>
        /// <param name="displayName">The displayName of the existing field.</param>
        /// <param name="newDisplayName">The new display name for the field.</param>
        /// <exception cref="ParameterValidationFault">If the <paramref name="displayName"/> or <paramref name="newDisplayName"/> is null or empty whitespace.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="displayName"/> doesn't exist.</exception>
        /// <exception cref="AlreadyExistsFault">If the <paramref name="newDisplayName"/> already exists.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(UnknownFault))]

        void RenameOrgProfileField(Guid orgId, string displayName, string newDisplayName);

        /// <summary>
        /// Method used in CoreClient to allow performance testing on IsUserInGroup
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(OrganizationNotFoundFault))]

        bool GetIsUserInGroupPerformance(Guid userId, string path, string groupName);

        /// <summary>
        /// Gets org expiry warning duration 
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        bool IsUserPasswordExpired(Guid userId);

        ////---------------------------

        /// <summary>
        /// Gets password status
        /// </summary>
        /// <returns>
        /// current failed attempts
        /// next available login time
        /// is lockedout
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        Tuple<int, DateTime, bool> GetPasswordStatus(Guid userId);

        /// <summary>
        /// Check if user password has expired
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        int PasswordExpiryWarningDuration(Guid userId);

        /// <summary>
        /// Add Supervisor/Supervisee Relationship
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="superviseeId">User id of Supervisee</param>
        /// <param name="supervisorId">User id of Supervisor</param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        void AddSupervisor(string path, Guid superviseeId, Guid supervisorId);

        /// <summary>
        /// Remove Supervisor/Supervisee Relationship
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="superviseeId">User id of Supervisee</param>
        /// <param name="supervisorId">User id of Supervisor</param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        void RemoveSupervisor(string path, Guid superviseeId, Guid supervisorId);

        /// <summary>
        /// Add Mentor/Mentee Relationship
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="menteeId">User id of Mentee</param>
        /// <param name="mentorId">User id of Mentor</param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        void AddMentor(string path, Guid menteeId, Guid mentorId);

        /// <summary>
        /// Remove Mentor/Mentee Relationship
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="menteeId">User id of Mentee</param>
        /// <param name="mentorId">User id of Mentor</param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        void RemoveMentor(string path, Guid menteeId, Guid mentorId);
    }
}
