using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using EC.Constants;
using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.ECExceptions;
using EC.Errors.UserExceptions;
using EC.Service.DTO;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// This interface defines the web service operations for user based interactions 
    /// (e.g. logon, logout, user registration, edit user profiles,...).
    /// </summary>

    [ServiceContract]
    public interface IUserServiceSecure
    {
        /// <summary>
        /// Return user with specific user ID.
        /// </summary>
        /// <remarks>
        /// Beware of detail level "ProfilePicture". If the profile picture is larger than the XmlDictionaryReaderQuotas.MaxArrayLength 
        /// this call will fail in a CommunicationException because the SOAP message will be too big. Get the user profile picture
        /// separately with <see cref="GetUserProfilePictureAsStream"/>.
        /// </remarks>
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
        /// Return user with the specific forgot password token.
        /// Validates if the forgot password is not expired
        /// </summary>
        /// <remarks>
        /// Beware of detail level "ProfilePicture". If the profile picture is larger than the XmlDictionaryReaderQuotas.MaxArrayLength 
        /// this call will fail in a CommunicationException because the SOAP message will be too big. Get the user profile picture
        /// separately with <see cref="GetUserProfilePictureAsStream"/>.
        /// </remarks>
        /// <exception cref="NotFoundFault"></exception>
        /// <exception cref="ForgotPasswordTokenExpiredFault"></exception>
        /// <exception cref="UnknownFault"></exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ForgotPasswordTokenExpiredFault))]

        User GetUserByForgotPasswordToken(Guid tokenId, string detailLevel);

        /// <summary>
        /// Gets the users by group (walking up the NavPath and include users from groups within groups).
        /// </summary>
        /// <remarks>
        /// Beware of detail level "ProfilePicture". If the profile picture is larger than the XmlDictionaryReaderQuotas.MaxArrayLength 
        /// this call will fail in a CommunicationException because the SOAP message will be too big. Get the user profile picture
        /// separately with <see cref="GetUserProfilePictureAsStream"/>.
        /// </remarks>
        /// <param name="path">The path.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">group is not found</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        List<User> GetUsersByGroup(string path, string groupName, string detailLevel);

        /// <summary>
        /// Return all users (paged).
        /// </summary>
        /// <remarks>
        /// Beware of detail level "ProfilePicture". If the profile picture is larger than the XmlDictionaryReaderQuotas.MaxArrayLength 
        /// this call will fail in a CommunicationException because the SOAP message will be too big. Get the user profile picture
        /// separately with <see cref="GetUserProfilePictureAsStream"/>.
        /// <p/>
        /// The <c>StartIndex</c> will skip the first <c>StartIndex</c> elements.
        /// To deactivate Paging set <c>PageSize</c> to -1 and <c>StartIndex</c> to 0 - all elements are returned.
        /// If the <c>PageSize</c> = -1 and <c>StartIndex</c> > 0 all items, starting with <c>StartIndex</c> are returned.
        /// </remarks>
        /// <example>
        /// - If a user is active and is not deleted:
        ///     (this user is always returned regardless of the parameters <c>activeOnly</c>)
        /// 
        ///  - If a user is set inactive, but is not deleted:
        ///     (this user is only returned if <c>activeOnly</c> == <c>false</c>)
        /// 
        ///  - If a user is deleted and is inactive:
        ///     (this user will be returned if <c>activeOnly</c> == <c>null</c>).
        /// 
        ///  Note: A user can't be deleted and active at the same time!
        /// </example>
        /// <param name="pageInfo">Range of list to fetch. If <c>null</c> all users are returned as if no paging is set.</param>
        /// <param name="activeOnly">
        /// Set <c>true</c> to get only active course offerings. 
        /// Set <c>false</c> to return all active and inactive course offerings (no deleted course offerings included).
        /// Set <c>null</c> to return all course offerings (active, inactive and deleted).
        /// </param>
        /// <returns>The requested range of users (if possible).</returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<User> GetAllUsers(PageInfo pageInfo, bool? activeOnly, string detailLevel);

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        UserRelationships GetUserRelationshipsById(string path, Guid userId);

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        List<User> GetUsersByRelationship(string path, Guid userId, UserRelationshipType relationship, bool? activeOnly = true, int? limit = -1, int? offset = 0, string detailLevel = null);

        /// <summary>
        /// Create a forgot password token and send it via email.
        /// </summary>
        /// <param name="email">Email address for the user to create and send the token for.</param>
        /// <exception cref="NotFoundFault">If the user with this email address doesn't exist, is not active or has been deleted.</exception>
        /// <exception cref="EmailFormatFault">If the email address has a wrong format.</exception>
        /// <exception cref="UnknownFault">In any other case.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(EmailFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void SendForgotPasswordEmail(string email);

        /// <summary>
        /// Set a new password for a user using a forgot password token
        /// </summary>
        /// <param name="forgotPasswordToken">The forgot password token sent in an email</param>
        /// <param name="newPassword">The new password to set</param>
        /// <param name="userId">The User Id</param>
        /// TODO: Shan: comments for service method 'SetUserPasswordWithForgotPasswordToken(Guid forgotPasswordToken, string newPassword)'
        /// <exception cref="NotFoundFault"></exception>
        /// <exception cref="ParameterValidationFault"></exception>
        /// <exception cref="UnknownFault"></exception>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(PasswordComplexityFault))]
        [FaultContract(typeof(PasswordHistoryFault))]

        User SetUserPasswordWithForgotPasswordToken(Guid forgotPasswordToken, string newPassword, Guid userId);

        /// <summary>
        /// Return user with specific login Id (user name or email address) for a not deleted user.
        /// </summary>
        /// <remarks>
        /// Beware of detail level "ProfilePicture". If the profile picture is larger than the XmlDictionaryReaderQuotas.MaxArrayLength 
        /// this call will fail in a CommunicationException because the SOAP message will be too big. Get the user profile picture
        /// separately with <see cref="GetUserProfilePictureAsStream"/>.
        /// </remarks>
        /// <param name="loginId">Case insensitive login Id (user name or email address or organization path). If organization path you must pass in users orgPath</param>
        /// <param name="detailLevel">Contains the level of detail on how much information is passed into the returning user item.</param>
        /// <param name="onlyActiveUsers">If true will only return active users. If false only</param>
        /// <param name="orgPath">Organization path, used to login with orgId. Pass null or empty string for username or email login. </param>
        /// <returns>Return user item.</returns>
        /// <exception cref="NotFoundFault">If the user does not exist or already marked as deleted.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        User GetUserByLoginId(string loginId, string detailLevel, bool onlyActiveUsers, string orgPath);

        /// <summary>
        /// Delete user with specific user id.
        /// If the user is already deleted - do nothing.
        /// </summary>
        /// <remarks>
        /// The property 
        /// <c>IsDeleted</c> is set to <c>true</c> and 
        /// <c>IsActive</c> is set to <c>false</c>.
        /// If you get all active users, no user is able to be deleted and active at the same time.
        /// </remarks>
        /// <param name="userId">User id to delete.</param>
        /// <exception cref="NotFoundFault">If the user does not exist.</exception>
        /// <exception cref="NotAuthorizedFault">If not enough permissions to delete the user.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteUserById(Guid userId);

        /// <summary>
        /// Set or delete a user's profile picture.
        /// Do nothing if the user profile picture has already been deleted.
        /// </summary>
        /// <param name="userId">ID of the user (must not be deleted).</param>
        /// <exception cref="NotFoundFault">If the user doesn't exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        void DeleteUserProfilePicture(Guid userId);

        /// <summary>
        /// Set a new user's profile picture.
        /// Overwrites an already existing picture.
        /// </summary>
        /// <remarks>
        /// Every user is allowed to set their own user profile picture (no permission is checked).
        /// If <paramref name="userId"/> is different from the logged in user the following rules apply:
        /// - The currently logged in user must have the permission to 'CanEditUsers' at <paramref name="path"/>.
        /// - AND the <paramref name="userId"/> must be either member of the organization along <paramref name="path"/> or
        ///   if there is no organization the currently logged in user is allowed to edit the user <paramref name="userId"/>.
        /// </remarks>
        /// <param name="path">Path to check permission if <paramref name="userId"/> is not the currently logged in user</param>
        /// <param name="userId">User Id (must not be deleted)</param>
        /// <param name="uploadToken">Upload token of the file which has been previously uploaded</param>
        /// <exception cref="InvalidUploadTokenFault">If the picture hasn't been uploaded yet (<paramref name="uploadToken"/> is invalid)</exception>
        /// <exception cref="NotFoundFault">If the user doesn't exist</exception>
        /// <exception cref="NotAuthorizedFault">
        /// If no user is logged in.
        /// If the <paramref name="userId"/> is different from the currently logged in user AND the currently logged in user doesn't have permission to edit the user.
        /// If the <paramref name="userId"/> is different from the currently logged in user AND the <paramref name="userId"/> is not part of the organization along <paramref name="path"/>.
        /// </exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        /// <exception cref="ConfigurationFault">If the organization is missing mandatory attributes for the user group</exception>
        [OperationContract]
        [FaultContract(typeof(InvalidUploadTokenFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        void SetUserProfilePicture(string path, Guid userId, Guid uploadToken);

        /// <summary>
        /// Get user profile picture as stream. Return <c>null</c> if the user doesn't have
        /// a profile picture
        /// </summary>
        /// <param name="userId">User Id (user must not be deleted)</param>
        /// <returns>Return profile picture as stream. Return <c>null</c> if the user doesn't have a user profile picture</returns>
        /// <exception cref="NotFoundFault">If the user doesn't exist.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        Stream GetUserProfilePictureAsStream(Guid userId);

        /// <summary>
        /// Verify a user's password, as well as initiating a new session for them.
        /// <remarks>
        /// NOTE: Logging of NotFoundExceptions is suppressed (since they will happen whenever someone
        /// makes a mistake with their credentials on login).
        /// <br/>
        /// The user must be allowed to interactively logon via UI (InteractiveLogon = <c>true</c>)
        /// </remarks>
        /// </summary>
        /// <param name="loginId">User name or email address to validate against. This is an OrgLoginId if <paramref name="orgPath"/> has been set</param>
        /// <param name="password">Password to check.</param>
        /// <param name="orgPath">Path at or along an organization the user belongs to (walk the path to find an organization</param>
        /// <returns>Return <c>true</c> if user is valid. Return <c>false</c> if user name/email address and password do not match.</returns>
        /// <exception cref="ParameterValidationFault">Throw an exception if at least one parameter is <c>null</c>, empty or only contains of white spaces.</exception>
        /// <exception cref="NotFoundFault">If the user login Id does not exist, is not active or has been deleted.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to logon via UI</exception>
        /// <exception cref="UnknownFault">Throw this exception in any other cases.</exception>
        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        bool ValidateUserWithLoginId(string loginId, string password, string orgPath);

        /// <summary>
        /// Verify a user's password, as well as initiating a new session for them.
        /// <remarks>
        /// NOTE: Logging of NotFoundExceptions is suppressed (since they will happen whenever someone
        /// makes a mistake with their credentials on login).
        /// <br/>
        /// The user must be allowed to logon via UI (InteractiveLogon = <c>true</c>).
        /// </remarks>
        /// </summary>
        /// <param name="userId">User Id to validate against.</param>
        /// <param name="password">Password to check.</param>
        /// <returns>Return <c>true</c> if user is valid. Return <c>false</c> if user Id and password do not match.</returns>
        /// <exception cref="ParameterValidationFault">Throw an exception if <paramref name="password"/> is <c>null</c>, empty or only contains of white spaces.</exception>
        /// <exception cref="NotFoundFault">If the user Id does not exist, is not active or has been deleted.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to logon via UI</exception>
        /// <exception cref="UnknownFault">Throw this exception in any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        bool ValidateUserWithUserId(Guid userId, string password);

        /// <summary>
        /// Logon a user with a remember me cookie (<c>userName</c>, <c>seriesId</c> and <c>tokenId</c> must be provided).
        /// </summary>
        /// <remarks>
        /// The user must be allowed to logon via UI (InteractiveLogon = <c>true</c>).
        /// </remarks>
        /// <param name="userId">User name.</param>
        /// <param name="seriesId">Series id.</param>
        /// <param name="tokenId">Token id.</param>
        /// <returns>
        /// Return a new tokenId for the next logon and new expiry date. Also return other cookie information.
        /// </returns>
        /// <exception cref="InvalidCookieFault">If the user is not valid - either the cookie has expired or doesn't exist.</exception>
        /// <exception cref="ParameterValidationFault">Throw an exception if at least one parameter is <c>null</c>, empty or only contains of white spaces.</exception>
        /// <exception cref="NotFoundFault">If the user name doesn't exist or is not active or has been deleted.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to logon via UI</exception>
        /// <exception cref="CookieTheftFault">If the <c>tokenId</c> is not valid and therefore a cookie theft is assumed. All existing cookies are deleted.</exception>
        /// <exception cref="UnknownFault">Throw this exception in any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(InvalidCookieFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(CookieTheftFault))]
        [FaultContract(typeof(UnknownFault))]

        CookieItem ValidateUserWithToken(Guid userId, string seriesId, string tokenId);

        /// <summary>
        /// Validate the given user/password pair and return a one time use nonce bound to the
        /// authenticated user's ID. If the user/password pair is not valid for any reason,
        /// then the return value is null. Otherwise it is the nonce.
        /// </summary>
        /// <remarks>
        /// The user must be allowed to logon via UI (InteractiveLogon = <c>true</c>).
        /// </remarks>
        /// <param name="loginId">login Id (user name/email address or OrgId) to validate. If using orgId login you must provide orgPath</param>
        /// <param name="password">password to validate</param>
        /// <param name="orgPath">An organization path. Required for orgId logIn.</param>
        /// <returns>SingleUseNonce</returns>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to logon via UI</exception>
        /// <exception cref="NotFoundFault">If the user doesn't exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        SingleUseNonce ValidateUserAndGenerateNonce(string loginId, string password, string orgPath);

        /// <summary>
        /// Creates a new cookie for the user. Validate the user first.
        /// </summary>
        /// <param name="loginId">User name, email address or orgId. If using orgId to logIn you must specify orgPath</param>
        /// <param name="password">Password to check.</param>
        /// <param name="orgPath">Path to organization. Required for orgId login, pass null for email or username login.</param>
        /// <returns>
        /// Return new cookie item which contains the user name (=logon name), tokenId, seriesId and the expiry date.
        /// Return <c>null</c> if the user is not validated.
        /// </returns>
        /// <exception cref="ParameterValidationFault">Throw an exception if at least one parameter is <c>null</c>, empty or only contains of white spaces.</exception>
        /// <exception cref="NotFoundFault">If the user logon name doesn't exist, is not active or has been deleted.</exception>
        /// <exception cref="UnknownFault">Throw this exception in any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        CookieItem CreateRememberMeCookie(string loginId, string password, string orgPath);

        /// <summary>
        /// Removes a cookie of the user. If the user doesn't have this cookie do nothing.
        /// </summary>
        /// <param name="userId">Users Id.</param>
        /// <param name="seriesId">Series Id.</param>
        /// <param name="tokenId">Token Id.</param>
        /// <exception cref="ParameterValidationFault">Throw an exception if at least one parameter is <c>null</c>, empty or only contains of white spaces.</exception>
        /// <exception cref="NotFoundFault">If the user login Id doesn't exist, is not active or has been deleted.</exception>
        /// <exception cref="UnknownFault">Throw this exception in any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        void DeleteRememberMeCookie(Guid userId, string seriesId, string tokenId);

        /// <summary>
        /// Change a user profile with new values.
        /// </summary>
        /// <remarks>
        /// The following properties can't be changed:
        /// <c>Id</c>
        /// <c>UserName</c>
        /// <c>IsDeleted</c>
        /// <c>ProfilePicture</c>
        /// </remarks>
        /// <param name="user">User item to set (set <c>Id</c> to identify the user to update).</param>
        /// <param name="newPassword">Set new password. Set <c>null</c> or empty to leave it unchanged.</param>
        /// <exception cref="ParameterValidationFault">If the parameter <paramref name="user"/> is set to null.</exception>
        /// <exception cref="PasswordComplexityFault">If organization has password complexity or password history rules and new password doesn't follow the rule</exception>
        /// <exception cref="AlreadyExistsFault">If the email address already exists for another user in the system.</exception>
        /// <exception cref="NotFoundFault">If the user doesn't exist.</exception>
        /// <exception cref="EmailFormatFault">If an email address with the wrong format is set. Not thrown if the email address is <c>null</c>.</exception>
        /// <exception cref="UnknownFault">In any other error.</exception>
        /// <exception cref="OrganizationLogonStateFault">If user has OrganizationLoginId set but OrgPath is not set.</exception>
        /// <exception cref="AlreadyOrgMemberFault">If OrganizationalLogoId already exists for another user in Organization.</exception>
        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(PasswordComplexityFault))]
        [FaultContract(typeof(PasswordHistoryFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(EmailFormatFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(OrganizationLogonStateFault))]
        [FaultContract(typeof(AlreadyOrgMemberFault))]

        void UpdateUserById(User user, string newPassword);

        /// <summary>
        /// Set a new password for a user.
        /// </summary>
        /// <param name="userId">Id of the user.</param>
        /// <param name="oldPassword">Old password to verify.</param>
        /// <param name="newPassword">New password to set.</param>
        /// <exception cref="ParameterValidationFault">Throw an exception if one of the parameters are missing/wrong (i.e. wrong old password).</exception>
        /// <exception cref="NotFoundFault">If the user login Id does not exist, is not active or has been deleted.</exception>
        /// <exception cref="EmailFormatFault">If an email address is set and has a wrong format. Not thrown if the email address is <c>null</c>.</exception>
        /// <exception cref="UnknownFault">Throw this exception in any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(PasswordComplexityFault))]
        [FaultContract(typeof(PasswordHistoryFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void SetUserPasswordById(Guid userId, string oldPassword, string newPassword);

        /// <summary>
        /// Set the user's home path (they will be re-directed here if they navigate to 
        /// just the plain site name with no path)
        /// </summary>
        /// <param name="Id">the user's primary key</param>
        /// <param name="homePath">the path (within the site) to set as their home</param>
        /// <exception cref="NotFoundFault">Thrown if there is no user with the given ID</exception>
        /// <exception cref="UnknownFault">Thrown if an unexpected error occurs</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        void SetUserHomePathById(Guid Id, string homePath);

        /// <summary>
        /// Get all users for an organization.
        /// Every organization contains a Users and Admins group.
        /// <remarks>
        /// <para>
        /// Beware of detail level "ProfilePicture". If the profile picture is larger than the XmlDictionaryReaderQuotas.MaxArrayLength 
        /// this call will fail in a CommunicationException because the SOAP message will be too big. Get the user profile picture
        /// separately with <see cref="GetUserProfilePictureAsStream"/>.
        /// </para>
        /// The <c>StartIndex</c> will skip the first <c>StartIndex</c> elements.
        /// To deactivate Paging set <c>PageSize</c> to -1 and <c>StartIndex</c> to 0 - all elements are returned.
        /// If the <c>PageSize</c> = -1 and <c>StartIndex</c> > 0 all items, starting with <c>StartIndex</c> are returned.
        /// </remarks>
        /// <example>
        /// - If a user is active and is not deleted:
        ///     (this user is always returned regardless of the parameters <c>activeOnly</c>)
        ///  - If a user is set inactive, but is not deleted:
        ///     (this user is only returned if <c>activeOnly</c> == <c>false</c>)
        ///  - If a user is deleted and is inactive:
        ///     (this user will be returned if <c>activeOnly</c> == <c>null</c>).
        /// 
        ///  Note: A user can't be deleted and active at the same time!
        /// </example>
        /// </summary>
        /// <param name="path">Path to start looking for organizational users/admins.</param>
        /// <param name="pageInfo">Range of list to fetch. If <c>null</c>, all items.</param>
        /// <param name="activeOnly">
        /// Set <c>true</c> to get only active users. 
        /// Set <c>false</c> to return all active and inactive users (no deleted users included).
        /// Set <c>null</c> to return all users (active, inactive and deleted).
        /// </param>
        /// <param name="sortDirection">Set sort direction for the column <paramref name="sortColumn"/>.</param>
        /// <param name="sortColumn">Column to sort by.</param>
        /// <param name="textFilter">Optional: filter result by first name, last name, email and login Id. Set <c>null</c> if no filter is applied</param>
        /// <param name="detailLevel">Detail level of the user to return.</param>
        /// <returns>Return paged users. Return empty list if no users has been found.</returns>
        /// <exception cref="NotFoundFault">If the path (item) doesn't exist or a group definition doesn't exist.</exception>
        /// <exception cref="ConfigurationFault">If no tag for a NavPage is found or mandatory attributes on the NavPage are missing (see fault for details what is missing).</exception>
        /// <exception cref="NotAuthorizedFault">If access for the users is denied.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<User> GetAllOrganizationalUsersByPath(string path, PageInfo pageInfo = null, bool? activeOnly = true, 
            SortDirectionEnum sortDirection = SortDirectionEnum.None, OrganizationalUserSortEnum sortColumn = OrganizationalUserSortEnum.LoginId, 
            string textFilter = null, string detailLevel = null);

        /// <summary>
        /// Get all users for an organization with is Admin property in tuple.
        /// Every organization contains a Users and Admins group.
        /// Look on the 'Organization' node and all 'Course' nodes for Users and Admins group definitions.
        /// Look up and down!
        /// <remarks>
        /// The <c>StartIndex</c> will skip the first <c>StartIndex</c> elements.
        /// To deactivate Paging set <c>PageSize</c> to -1 and <c>StartIndex</c> to 0 - all elements are returned.
        /// If the <c>PageSize</c> = -1 and <c>StartIndex</c> greater than 0 all items, starting with <c>StartIndex</c> are returned.
        /// </remarks>
        /// <example>
        /// - If a user is active and is not deleted:
        /// (this user is always returned regardless of the parameters <c>activeOnly</c>)
        /// - If a user is set inactive, but is not deleted:
        /// (this user is only returned if <c>activeOnly</c> == <c>false</c>)
        /// - If a user is deleted and is inactive:
        /// (this user will be returned if <c>activeOnly</c> == <c>null</c>).
        /// Note: A user can't be deleted and active at the same time!
        /// </example>
        /// </summary>
        /// <param name="path">Path to start looking for organizational users/admins.</param>
        /// <param name="pageInfo">Range of list to fetch. If <c>null</c>, all items.</param>
        /// <param name="activeOnly">Set <c>true</c> to get only active course offerings.
        /// Set <c>false</c> to return all active and inactive course offerings (no deleted course offerings included).
        /// Set <c>null</c> to return all course offerings (active, inactive and deleted).</param>
        /// <param name="sortDirection">Set sort direction for the column <paramref name="sortColumn"/>.</param>
        /// <param name="sortColumn">Column to sort by.</param>
        /// <param name="textFilter">Optional: filter result by first name, last name, email and login Id. Set <c>null</c> if no filter is applied</param>
        /// <param name="detailLevel">Detail level of the user to return.</param>
        /// <returns>
        /// Return paged users with isAdmin property in tuple. Return empty list if no users has been found.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<Tuple<User, bool>> GetAllOrganizationalUsersByPathWithIsAdminProp(string path, PageInfo pageInfo = null, bool? activeOnly = true,
            SortDirectionEnum sortDirection = SortDirectionEnum.None, OrganizationalUserSortEnum sortColumn = OrganizationalUserSortEnum.LoginId,
            string textFilter = null, string detailLevel = null);

        /// <summary>
        /// Get the UserAdminGroupName
        /// </summary>
        /// <param name="path">Path to start looking for organizational users</param>
        /// <returns>Returns string for UserAdminGroupName for this organization</returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        AdminGroupNames GetOrganizationGroupNames(string path);

        /// <summary>
        /// Check if a user is member in any of the <paramref name="groupNames"/>.
        /// If a group name doesn't exist, the flag is set to <c>false</c>.
        /// </summary>
        /// <param name="path">Path to check the membership from (walking up the path).</param>
        /// <param name="userId">Existing user Id (active or inactive)</param>
        /// <param name="groupNames">Group names to check.</param>
        /// <returns>Returns a list of group names and corresponding flag whether the user is member of this group.</returns>
        /// <exception cref="NotFoundFault">If the path (item) doesn't exist or the user doesn't exist.</exception>
        /// <exception cref="NotAuthorizedFault">If access for the users is denied.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        List<Tuple<string, bool>> IsMemberOfGroupsByUserId(string path, Guid userId, List<string> groupNames);

        /// <summary>
        /// Edit a user (must be within an organization).
        /// </summary>
        /// <remarks>
        /// The OrganizationLoginId and OrgPath of a user can only be changed under certain circumstances:
        /// - The OrganizationLoginId can only be changed if the OrgPath of the user matches the current organization
        ///   the user is edited in.
        /// - OrganziationLoginId and OrgPath can be set to null only if the existing OrgPath of a user
        ///   matches the current organization the user is edited on.
        /// - Either both (OrganizationLoginId AND OrgPath) is set or both must be null.
        /// </remarks>
        /// <param name="path">Path to edit the user from (used as starting path to look for the user).</param>
        /// <param name="editedUser">
        /// User information to save ('Id', 'UserName', 'IsDeleted', 'ProfilePictureDateAdded' and 'ProfilePicture' are ignored).
        /// Set 'UserName' to identify the existing user in the system.
        /// </param>
        /// <param name="orgPath">Organization path.</param>
        /// <param name="newPassword">Set new password. Leave <c>null</c> to not to change the password.</param>
        /// <param name="userGroupDetails">Tree data structure that reflects the membership of a given user for each Group along all path starting with the orgPath</param>
        /// <exception cref="ConfigurationFault">If wrong tag type of the NavPage or a mandatory attribute is missing.</exception>
        /// <exception cref="AlreadyExistsFault">If the email address to which the user should be changed already exists for another user.</exception>
        /// <exception cref="NotAuthorizedFault">If not enough permissions to edit the user.</exception>
        /// <exception cref="NotFoundFault">If the path or user couldn't be found.</exception>
        /// <exception cref="OrganizationLogonStateFault">
        /// If the <c>editedUser.OrgAdminPaths</c> contains invalid data.
        /// If user has OrganizationLoginId set but OrgPath is not set.
        /// If the OrgPath can't change because the user has a different OrgPath (see remarks).
        /// </exception>
        /// <exception cref="AlreadyOrgMemberFault">If the same orgId is used by another user within the organization.</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>
        /// <exception cref="UsersBelongToSubGroupsException">If we try to remove a user that belongs to a subGroup rather then the group </exception>
        [OperationContract]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(OrganizationLogonStateFault))]
        [FaultContract(typeof(AlreadyOrgMemberFault))]
        [FaultContract(typeof(PasswordComplexityFault))]
        [FaultContract(typeof(PasswordHistoryFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(UsersBelongToSubGroupsFault))]

        void EditOrganizationalUserByUserName(string path, User editedUser, string orgPath, string newPassword, UserGroupDetails userGroupDetails);

        /// <summary>
        /// Unlock an organizational user by Id
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(OrganizationLogonStateFault))]
        [FaultContract(typeof(AlreadyOrgMemberFault))]
        [FaultContract(typeof(PasswordComplexityFault))]
        [FaultContract(typeof(PasswordHistoryFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(UsersBelongToSubGroupsFault))]

        void UnlockOrganizationalUserByUserId(Guid userId);

        /// <summary>
        /// Create a new user in the system and add it to the organization to which the <paramref name="path"/> belongs to.
        /// </summary>
        /// <param name="path">Path within an organization or at the root path of the organization.</param>
        /// <param name="newUser">New organizational user (set optional admin rights). The 'Id', 'UserName', 'IsDeleted' are not used and newly created.</param>
        /// <param name="password">Password for the new user.</param>
        /// <param name="userGroupDetails">Tree data structure that reflects the membership of a given user for each Group along all path starting with the orgPath</param>
        /// <return>Return the user Id.</return>
        /// <exception cref="NotFoundFault">If the path doesn't exist.</exception>
        /// <exception cref="ConfigurationParameterFault">If no organization has been found for <paramref name="path"/>.</exception>
        /// <exception cref="NotAuthorizedFault">If not enough permissions to create the user.</exception>
        /// <exception cref="AlreadyExistsFault">If the email address already exists for another (active, inactive) user.</exception>
        /// <exception cref="AlreadyMemberFault">If the email address is already used by a user who is already member of the organization.</exception>
        /// <exception cref="EmailFormatFault">If no email address or wrong formatted email address has been set.</exception>
        /// <exception cref="ParameterValidationFault">If no password has been set or <paramref name="newUser"/> is <c>null</c>.</exception>
        /// <exception cref="AlreadyOrgMemberException">User already exists with OrganizationalLogonId in organization.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(AlreadyMemberFault))]
        [FaultContract(typeof(AlreadyOrgMemberFault))]
        [FaultContract(typeof(DeletedUserFault))]
        [FaultContract(typeof(EmailFormatFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(PasswordComplexityFault))]
        [FaultContract(typeof(UnknownFault))]

        Guid CreateNewOrganizationalUser(string path, User newUser, string password, UserGroupDetails userGroupDetails);

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
        /// Gets all org profile values (of all organizations) visible by the currently logged in user.
        /// </summary>
        /// <returns>
        /// Return the org profile values visible to the user. 
        /// Return empty list if no org profile values are available/visible
        /// </returns>
        
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        
        List<OrgProfileValue> GetOrgProfileValuesOfCurrentUser();

        /// <summary>
        /// Gets the user available org profile fields.
        /// </summary>
        /// <returns></returns>
        
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
       
        List<OrgProfileField> GetCurrentUserAvailableOrgProfileFields();

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

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>
        [OperationContract]
        bool IsUserServiceSecure();
    }
}
