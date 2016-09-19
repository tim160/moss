using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Model.Interfaces
{
    public interface IUser
    {

        /*  #region Methods
       
          /// <summary>
          /// Remove OrganizationLoginId and OrgPath (set to <c>null</c>).
          /// </summary>

          void RemoveOrganizationLogin();

          /// <summary>
          /// markes user as deleted
          /// by setting email and org login Id to null
          /// </summary>

          void MarkAsDeleted();

          /// <summary>
          /// undelete user
          /// </summary>

          void UnDelete();

          /// <summary>
          /// Return users display name. In format: FirstName LastName.
          /// If first and last name are null or white space only returns User Name.
          /// </summary>
          /// <returns></returns>
        
          string GetUserDisplayName();

          /// <summary>
          /// Return users display name. In format: FirstName LastName (UserName).
          /// If first and last name are null or white space only returns User Name.
          /// </summary>
          /// <returns></returns>

          string GetUserDisplayNameWithUserName();

          /// <summary>
          /// Adds the org profile value.
          /// </summary>
          /// <param name="orgProfileValue">The org profile value.</param>

          void AddOrgProfileValue(IOrgProfileValue orgProfileValue);

          /// <summary>
          /// Get org profile values by OrgId/>.
          /// </summary>
          IEnumerable<IOrgProfileValue> GetOrgProfileValuesByOrgId();

          /// <summary>
          /// Get active org profile values by OrgId/>.
          /// </summary>
          IEnumerable<IOrgProfileValue> GetActiveOrgProfileValuesByOrgId();

          /// <summary>
          /// Get all active org profile values that match the given regular expression
          /// </summary>
          /// <param name="regEx">regular expression</param>
          /// <returns>returns all matching org profile values</returns>

          IEnumerable<IOrgProfileValue> GetActiveOrgProfileValuesByOrgIdAndRegEx(Regex regEx);

          /// <summary>
          /// Predicate indicating whether this user has an active org profile field value for the given
          /// org profile field (specific by Id).
          /// </summary>

          bool HasActiveOrgProfileValueFor(Guid orgProfileFieldId);

          /// <summary>
          /// Get the active IOrgProfileValue for a <paramref name="fieldName"/> if one exists.
          /// Null if not.
          /// </summary>

          IOrgProfileValue GetActiveOrgProfileValueFor(string fieldName);

          /// <summary>
          /// Gets the active IOrgProfileValue by a <paramref name="fieldId"/>. Null if not.
          /// </summary>

          IOrgProfileValue GetActiveOrgProfileValueFor(Guid fieldId);

          /// <summary>
          /// Set org profile value for org profile field, creating it if necessary
          /// </summary>
          void SetOrgProfileValue(IOrgProfileField field, string value);

          /// <summary>
          /// Set a profile picture for the user. 
          /// If a picture already exists it overwrites the existing one.
          /// </summary>
          /// <param name="profilePicture">Profile picture. Set to <c>null</c> to delete an existing profile picture.</param>
          /// <param name="picDateAdded">Set a specific data added. Set <c>null</c> to get the DateTime.UtcNow for the date added.</param>

          void SetProfilePicture(IUserProfilePicture profilePicture, DateTime? picDateAdded = null);

          /// <summary>
          /// Add a new forgot password token
          /// </summary>
          void AddForgotPassword(IForgotPasswordToken newToken);
       
          /// <summary>
          /// Remove a forgot password token
          /// </summary>
          void RemoveForgotPassword(IForgotPasswordToken oldToken);

          /// <summary>
          /// Remove expired forgot password tokens
          /// </summary>
        
          void CleanExpiredForgotPassword();

          /// <summary>
          /// Add a new cookie.
          /// </summary>
          /// <param name="newCookie">New cookie.</param>
          /// <exception cref="AlreadyExistsException">If the same cookie already exists.</exception>
        
          void AddCookie(ICookieItem newCookie);

          /// <summary>
          /// Remove cookie.
          /// </summary>
          /// <param name="oldCookie">Cookie to be removed.</param>
        
          void RemoveCookie(ICookieItem oldCookie);

          /// <summary>
          /// Remove expired cookies.
          /// </summary>
        
          void CleanExpiredCookies();

          /// <summary>
          /// Delete all cookies (e.g. if a cookie theft has been detected).
          /// </summary>
        
          void DeleteAllCookies();

          /// <summary>
          /// Delete a specific cookie. If the cookie doesn't exist, nothing to do.
          /// </summary>
          /// <remarks>
          /// Commit changes after calling this method.
          /// </remarks>
          /// <param name="seriesId">Series Id.</param>
          /// <param name="tokenId">Token Id.</param>
        
          void DeleteCookie(string seriesId, string tokenId);

           /// <summary>
          /// Validate the user with token (= remember me logon).
          /// </summary>
          /// <remarks>
          /// - You have to grab a new token if the user is successfully validated <see cref="UpdateToken"/>.
          /// - If a <c>CookieTheftException</c> is thrown the cookies are no longer safe - it is recommended that all
          ///   remaining cookies are deleted.
          /// </remarks>
          /// <param name="seriesId">The series id.</param>
          /// <param name="tokenId">The token id.</param>
          /// <returns>Return <c>true</c> if the user has the correct token. Return <c>false</c> if the user has no cookie.</returns>
          /// <exception cref="CookieTheftException">If a cookie theft is assumed (seriesId valid, but the tokenId is wrong).</exception>

          bool ValidateLogonWithToken(string seriesId, string tokenId);

          /// <summary>
          /// Create new tokenId for the cookie with <c>seriesId</c> and <c>tokenId</c>.
          /// </summary>
          /// <remarks>
          /// Remember to commit the changes after calling this method.
          /// </remarks>
          /// <param name="seriesId">Series Id</param>
          /// <param name="tokenId">Token Id which should be replaced.</param>
          /// <returns>Return the new token Id and update the cookie. Return <c>null</c> if the user has no cookie.</returns>
        
          string UpdateToken(string seriesId, string tokenId);

          /// <summary>
          /// Set new primary Id. 
          /// </summary>
          /// <remarks>
          /// If the user already exists - the primary Id can't be changed.
          /// Primary keys can't be changed once they have been added to the SQL DB.
          /// </remarks>
          /// <param name="newId">New primary Id to set</param>

          void SetId(Guid newId);

          /// <summary>
          /// Get or set the first name.
          /// </summary>

          void SetFirstName(string newFirstName);

          /// <summary>
          /// Get or set the last name.
          /// </summary>

          void SetLastName(string newLastName);

          /// <summary>
          /// Get or set the email address.
          /// </summary>

          void SetEmailAddress(string newEmailAddress);

          /// <summary>
          /// The contact email address, used for notifications and communications (does not to be unique system-wide).
          /// NOTE: Generally GetContactEmail() should be called, rather than accessing this property, as the method
          /// provides fallback logic.
          /// </summary>

          void SetContactEmail(string newContactEmail);

          /// <summary>
          /// Optional phone number.
          /// </summary>

          void SetPhoneNumber(string newPhoneNumber);
          */
        /// <summary>
        /// Validate the password base on organization password rules and hash and set the password.
        /// </summary>

        void SetPasswordAndValidate(string newPassword);

        /// <summary>
        /// Sets Password without hashing and without any validation
        /// </summary>
        void SetPasswordHash(string newPasswordHash);

        /// <summary>
        /// Hashes the password and sets it, without any validation
        /// </summary>

        void SetPasswordNoValidation(string newPassword);
        /*
                /// <summary>
                /// Get or set the home path for the user.
                /// </summary>

                void SetHomePath(string newHomePath);

                /// <summary>
                /// Set interactive logon flag.
                /// </summary>
                /// <param name="newInteractiveLogon">New flag value</param>

                void SetInteractiveLogon(bool newInteractiveLogon);

                /// <summary>
                /// Get or set flag of the user's state (active == <c>true</c>, inactive = <c>false</c>).
                /// </summary>

                void SetIsActive(bool newIsActive);

                /// <summary>
                /// Get or set flag for the user whether he/she is deleted (<c>true</c>: deleted, <c>false</c> not deleted).
                /// </summary>

                void SetIsDeleted(bool isDeleted);

                /// <summary>
                /// Get or set the user specific language - either neutral ('en') or specific language ('en-ca').
                /// If no language is set, use the default/browser language.
                /// </summary>
                void SetUserLanguage(string newUserLanguage);

                /// <summary>
                /// Set the user name.
                /// </summary>
                void SetUserName(string newUserName);

                /// <summary>
                /// Set the user name.
                /// Specially created for import-related callers (for performance)
                /// </summary>
                void SetUserNameForImport(string newUserName);

                /// <summary>
                /// Get the date time when the user profile picture was added.
                /// </summary>

                void SetProfilePictureDateAdded(DateTime? newProfilePictureDateAdded);
        
                /// <summary>
                /// Get or set the super user flag (<c>true</c>: the user can do god-like everything, <c>false</c>: normal user).
                /// Note: This is a workaround until roles are implemented in the system.
                /// </summary>

                void SetIsSuperUser(bool isSuperUser);

                /// <summary>
                /// The assigned max. permission level the user can access or set permissions for.
                /// <example>
                /// A user with level 50 can access items with permission levels 0-50.
                /// The user can also set permissions to items with max. level of 50
                /// </example>
                /// </summary>

                void SetPermissionLevel(int newPermissionLevel);

                /// <summary>
                /// to set organization for user in import
                /// </summary>

                void SetOrganizationForImport(INavPage org);

                /// <summary>
                /// Id used for organizational level login, will be used in combination with OrgPath.
                /// </summary>
                /// <param name="newOrgLoginId">New org Login Id</param>
                void SetOrganizationLogin(string newOrgLoginId);
        
                /// <summary>
                /// Date and time of the last login.
                /// If <c>null</c> the user hasn't logged in ever.
                /// This information is used as read-only and will not be used at service level for any purpose.
                /// </summary>

                void SetLastLogin(DateTime? newLastLoginDate);

                /// <summary>
                /// Time zone string the user has chosen to display content.
                /// This is also used for notifications.
                /// </summary>

                void SetTimeZone(string newTimeZone);

                /// <summary>
                /// Whether the user can view reports.
                /// </summary>

                void SetCanViewReports(bool newCanViewReports);

                /// <summary>
                /// Finds user item audit
                /// </summary>
                /// <returns></returns>

                IUserItemAudit FindUserItemAudit();

                /// <summary>
                /// Adds the supervisor.
                /// </summary>
                /// <param name="user">The user.</param>

                void AddSupervisor(IUser user);

                /// <summary>
                /// Removes the supervisor.
                /// </summary>
                /// <param name="user">The user.</param>

                void RemoveSupervisor(IUser user);

                /// <summary>
                /// Adds the mentor.
                /// </summary>
                /// <param name="user">The user.</param>

                void AddMentor(IUser user);

                /// <summary>
                /// Removes the mentor.
                /// </summary>
                /// <param name="user">The user.</param>

                void RemoveMentor(IUser user);

                /// <summary>
                /// Sets first failed login time and next available login time
                /// </summary>
                void SetFirstFailedLoginTimeAndNextAvailableLoginTime(DateTime? value, int window);

                /// <summary>
                /// Reset first failed login time as well as next available login time and current failed attempt and lockout
                /// </summary>

                void ResetFailedLoginTime();

                /// <summary>
                /// Set current failed attempts
                /// </summary>
                void SetCurrentFailedAttempts(int currentFailedAttempts);

                /// <summary>
                /// Set lockout
                /// </summary>
                void SetLockout(bool lockout);

                /// <summary>
                /// Sets ForcePasswordChange
                /// </summary>
                void SetForcePasswordChange(bool forcePasswordChange);

                /// <summary>
                /// Verify Password.
                /// </summary>
                bool VerifyPassword(string password);

                /// <summary>
                /// Validate Password and check it against password lockout rules and lock user if necessary.
                /// </summary>
                /// <remarks>
                /// SuperUsers and non-interactive users are exempted from the lockout rules.
                /// </remarks>
                /// <returns>Return <c>true</c> if logon was valid. Return <c>false</c> if the user is either locked out or the password was wrong</returns>
                bool ValidatePasswordAndUpdateLockoutState(string password);

                /// <summary>
                /// Check if password existed in the last number of days.
                /// </summary>
                /// <param name="historyInterval">History interval in days</param>
                void ValidatePasswordHistory(string password, int historyInterval);

                /// <summary>
                /// Returns the date of the last time that the user changed their password. If the user has never changed their password, this 
                /// will be the date that the user was created.
                /// </summary>

                DateTime PasswordChangeDate();

                #endregion

                #region Mapped Scalars

                /// <summary>
                /// Whether the user is in the organization.
                /// </summary>
                bool IsInOrganization(INavPage orgNavPage);

                /// <summary>
                /// Get unique user ID.
                /// </summary>

                Guid Id { get; }

                /// <summary>
                /// Get unique user name.
                /// </summary>
        
                string UserName { get; }

                /// <summary>
                /// Get or set the first name.
                /// </summary>
        
                string FirstName { get; }

                /// <summary>
                /// Get or set the last name.
                /// </summary>
        
                string LastName { get; }

                /// <summary>
                /// Get or set the email address.
                /// </summary>
        
                string EmailAddress { get; }

                /// <summary>
                /// The contact email address, used for notifications and communications (does not to be unique system-wide).
                /// NOTE: Generally GetContactEmail() should be called, rather than accessing this property, as the method
                /// provides fallback logic.
                /// </summary>

                string ContactEmail { get; }

                /// <summary>
                /// Gets or sets the contact email or default.
                /// </summary>
                /// <value>
                /// The contact email or default.
                /// </value>
                string ContactEmailOrDefault { get; }

                /// <summary>
                /// Optional phone number.
                /// </summary>

                string PhoneNumber { get; }

                /// <summary>
                /// Get or set encrypted user password.
                /// </summary>
        
                string Password { get; }

                /// <summary>
                /// Get or set the home path for the user.
                /// </summary>
        
                string HomePath { get; }

                /// <summary>
                /// Flag whether a user is allowed to logon interactively (through UI).
                /// </summary>

                bool InteractiveLogon { get; }

                /// <summary>
                /// Get or set flag of the user's state (active == <c>true</c>, inactive = <c>false</c>).
                /// </summary>
       
                bool IsActive { get; }

                /// <summary>
                /// Get or set flag for the user whether he/she is deleted (<c>true</c>: deleted, <c>false</c> not deleted).
                /// </summary>
        
                bool IsDeleted { get; }

                /// <summary>
                /// Get or set the user specific language - either neutral ('en') or specific language ('en-ca').
                /// If no language is set, use the default/browser language.
                /// </summary>
       
                string UserLanguage { get; }

                /// <summary>
                /// Get flag whether the user has a profile picture assigned.
                /// </summary>

                bool HasProfilePicture { get; }

                /// <summary>
                /// Get the date time when the user profile picture was added.
                /// </summary>

                DateTime? ProfilePictureDateAdded { get; }

                /// <summary>
                /// Get the organization which the user is a member of.
                /// </summary>

                INavPage Organization { get; }

                /// <summary>
                /// Get the optional profile picture of the user.
                /// </summary>
        
                IUserProfilePicture ProfilePicture { get; }

                /// <summary>
                /// Get or set the super user flag (<c>true</c>: the user can do god-like everything, <c>false</c>: normal user).
                /// Note: This is a workaround until roles are implemented in the system.
                /// </summary>
       
                bool IsSuperUser { get; }

                /// <summary>
                /// The assigned max. permission level the user can access or set permissions for.
                /// <example>
                /// A user with level 50 can access items with permission levels 0-50.
                /// The user can also set permissions to items with max. level of 50
                /// </example>
                /// </summary>
        
                int PermissionLevel { get; }

                /// <summary>
                /// Id used for organizational level login, will be used in combination with OrgPath.
                /// </summary>

                string OrganizationLoginId { get; }

                /// <summary>
                /// Date and time of the last login.
                /// If <c>null</c> the user hasn't logged in ever.
                /// This information is used as read-only and will not be used at service level for any purpose.
                /// </summary>

                DateTime? LastLogin { get; }

                /// <summary>
                /// Time zone string the user has chosen to display content.
                /// This is also used for notifications.
                /// </summary>

                string TimeZone { get; }

                /// <summary>
                /// Whether the user can view reports.
                /// </summary>

                bool CanViewReports { get; }

                /// <summary>
                /// First failed login time
                /// </summary>

                DateTime? FirstFailedLoginTime { get; }

                /// <summary>
                /// Next available login time
                /// </summary>

                DateTime? NextAvailableLoginTime { get; }

                /// <summary>
                /// Locked out
                /// </summary>

                bool LockedOut { get; }

                /// <summary>
                /// Current failed attempts
                /// </summary>

                int CurrentFailedAttempts { get; }

                /// <summary>
                /// Forces user to change the password
                /// </summary>

                bool ForcePasswordChange { get; }

                #region CPMS

                /// <summary>
                /// CPMS field (html)
                /// </summary>

                string About { get; }
        
                /// <summary>
                /// CPMS field (html)
                /// </summary>

                string PastExperience { get; }

                /// <summary>
                /// CPMS field
                /// </summary>

                bool IsMentor { get; }

                #endregion

                #endregion

                #region Public Properties

                /// <summary>
                /// Returns the set of ALL (Active or otherwise) custom profile fields for this user (for all 
                /// organizations that they belong to)
                /// </summary>
        
                IQueryable<IOrgProfileValue> OrgProfileValues { get; }

                /// <summary>
                /// Returns the set of Active custom profile fields for this user (for all 
                /// organizations that they belong to)
                /// </summary>

                IQueryable<IOrgProfileValue> ActiveOrgProfileValues { get; }

                /// <summary>
                /// The set of cookies for this user.
                /// </summary>

                IQueryable<ICookieItem> Cookies { get; }

                /// <summary>
                /// The set of tokens issued for forgotten passwords.
                /// </summary>

                IQueryable<IForgotPasswordToken> Tokens { get; }

                #region CPMS

                /// <summary>
                /// Return all Bindings where User is a Supervisor
                /// </summary>

                IEnumerable<IUserSupervisorBinding> SupervisorBindings { get; }

                /// <summary>
                /// Return all Bindings where User is a Supervisee
                /// </summary>

                IEnumerable<IUserSupervisorBinding> SuperviseeBindings { get; }

                /// <summary>
                /// Return all Bindings where User is a Mentor
                /// </summary>

                IEnumerable<IUserMentorBinding> MentorBindings { get; }

                /// <summary>
                /// Return all Bindings where User is a Mentee
                /// </summary>

                IEnumerable<IUserMentorBinding> MenteeBindings { get; }

                #endregion

                #endregion*/
    }

    /// <summary>
    /// User-Supervisor Binding
    /// </summary>
    /// <remarks>
    /// Join table to bind 2 UserItems representing Supervisor-Supervisee relationship
    /// </remarks>

    public interface IUserSupervisorBinding : IValidate
    {
        /// <summary>
        /// Set Supervisor and Supervisee Binding Objects
        /// </summary>
        void SetBinding(IUser supervisor, IUser supervisee);

        IUser Supervisor { get; }
        IUser Supervisee { get; }
    }

    /// <summary>
    /// User-Mentor Binding
    /// </summary>
    /// <remarks>
    /// Join table to bind 2 UserItems representing Mentor-Mentee Relationship
    /// </remarks>

    /*  public interface IUserMentorBinding : IValidate
      {
          /// <summary>
          /// Set Mentor and Mentee Binding Objects
          /// </summary>
          void SetBinding(IUser mentor, IUser mentee);

          IUser Mentor { get; }
          IUser Mentee { get; }
      }*/
}

