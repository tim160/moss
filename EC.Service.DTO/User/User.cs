using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EC.Common.Base;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO implementations for the user service.
    /// </summary>
    [DataContract]
    public class User : ITrim
    {
        public virtual void TrimStringProperties()
        {
            ContactEmail = ContactEmail.TrimOrDefault();
            DisplayName = DisplayName.TrimOrDefault();
            DisplayNameWitUserName = DisplayNameWitUserName.TrimOrDefault();
            EmailAddress = EmailAddress.TrimOrDefault();
            FirstName = FirstName.TrimOrDefault();
            HomePath = HomePath.TrimOrDefault();
            LastName = LastName.TrimOrDefault();
            OrganizationLoginId = OrganizationLoginId.TrimOrDefault();
            OrgPath = OrgPath.TrimOrDefault();
            PhoneNumber = PhoneNumber.TrimOrDefault();
            UserLanguage = UserLanguage.TrimOrDefault();
            UserName = UserName.TrimOrDefault();
            TimeZone = TimeZone.TrimOrDefault();
        }

        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// User name will be generated automatically from first and last name and email address.
        /// </summary>

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the contact email or default.
        /// </summary>
        /// <value>
        /// The contact email or default.
        /// </value>
        
        [DataMember]
        public string ContactEmailOrDefault { get; set; }

        /// <summary>
        /// Email address must be unique throughout the system.
        /// </summary>

        [DataMember]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Contact email does NOT need to be unique.
        /// </summary>
        /// <remarks>
        /// The assembler sets this field using the logic in UserItem.cs:GetContactEmail()
        /// </remarks>

        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// Optional phone number.
        /// </summary>

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string HomePath { get; set; }

        /// <summary>
        /// Whether the user is allowed to logon via UI.
        /// </summary>

        [DataMember]
        public bool InteractiveLogon { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsSuperUser { get; set; }

        [DataMember]
        public string UserLanguage { get; set; }  
        
        [DataMember]
        public byte[] ProfilePicture { get; set; }

        [DataMember]
        public string DisplayNameWitUserName { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the date the profile picture was added.
        /// Null if there is no profile picture for this user
        /// </summary>
        /// <value>
        /// The profile picture date added.
        /// </value>
        
        [DataMember]
        public DateTime? ProfilePictureDateAdded { get; set; }

        /// <summary>
        /// Flag whether the user has a profile picture.
        /// </summary>

        [DataMember]
        public bool HasProfilePicture { get; set; }

        [DataMember]
        public int PermissionLevel { get; set; }

        [DataMember]
        public bool CanViewReports { get; set; }

        /// <summary>
        /// Id used for organizational level login, will be used in combination with OrgPath.
        /// </summary>

        [DataMember]
        public string OrganizationLoginId { get; set; }

        /// <summary>
        /// Path that associates user with an organization. Used with OrganizationLoginId to provide organizational level login.
        /// </summary>

        [DataMember]
        public string OrgPath { get; set; }


        /// <summary>
        /// Date and time of the last login.
        /// If <c>null</c> the user hasn't logged in ever.
        /// This information is used as read-only and will not be used at service level for any purpose.
        /// </summary>

        [DataMember]
        public DateTime? LastLogin { get; set; }

        [DataMember]
        public string TimeZone { get; set; }

        [DataMember]
        public List<OrgProfileValue> OrgProfileValues { get; set; }

        /// <summary>
        /// List of registrations for the user for a specific path.
        /// </summary>
        /// <remarks>
        /// This is only filled in with a detail level.
        /// Only service entry points GetStudentsWithRegistrationsByPath provides the capability to 
        /// fill in this property.
        /// </remarks>

        ////////////[DataMember]
        ////////////public List<Registration> Registrations { get; set; }

        [DataMember]
        public bool lockedOut { get; set; }

        [DataMember]
        public bool ForcePasswordChange { get; set; }

        /// <summary>
        /// List of Users who Supervise this User
        /// </summary>

        [DataMember]
        public List<User> Supervisors { get; set; } 

        /// <summary>
        /// List of Users that are Supervised by this User
        /// </summary>

        [DataMember]
        public List<User> Supervisees { get; set; }

        /// <summary>
        /// List of Users who Mentor this User
        /// </summary>

        [DataMember]
        public List<User> Mentors { get; set; } 

        /// <summary>
        /// List of Users that are Mentored by this User
        /// </summary>

        [DataMember]
        public List<User> Mentees { get; set; } 
    }

    /// <summary>
    /// Cookie item which contains all necessary information for a remember me login.
    /// </summary>
    [DataContract]
    public class CookieItem
    {
        /// <summary>
        /// User name (= logon name).
        /// </summary>
        
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// Series Id which remains the same for this cookie.
        /// </summary>
        
        [DataMember]
        public string SeriesId { get; set; }

        /// <summary>
        /// One time token Id which is exchanged for every remember me login.
        /// </summary>

        [DataMember]
        public string TokenId { get; set; }

        /// <summary>
        /// Expiry date of the cookie.
        /// </summary>

        [DataMember]
        public DateTime ExpiryDate { get; set; }
    }

    /// <summary>
    /// User OrgProfile which contains the latest DateCreated for each OrgProfile Field/Value pair
    /// </summary>
    [DataContract]
    public class OrgProfileValueAudit
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string OrgProfileField { get; set; }

        [DataMember]
        public string OrgProfileValue { get; set; }

        [DataMember]
        public DateTime DateCreated { get; set; }
    }

    /// <summary>
    /// Data bag to get all success, warn and error messages during a CSV user import.
    /// </summary>
    [DataContract]
    public class CsvUserImportState
    {
        [DataMember]
        public List<string> SuccessMessages { get; set; }
        
        [DataMember]
        public List<string> WarnMessages { get; set; }
        
        [DataMember]
        public List<string> ErrorMessages { get; set; }
    }

    /// <summary>
    /// Sorting organizational user columns
    /// </summary>
    [DataContract]
    public enum OrganizationalUserSortEnum
    {
        [EnumMember]
        FirstName = 0,

        [EnumMember]
        LastName = 1,
        
        [EnumMember]
        LoginId = 2,
        
        [EnumMember]
        Email = 3,
        
        [EnumMember]
        IsAdmin = 4,
        
        [EnumMember]
        LastLogin = 5,

        [EnumMember]
        IsLocked = 6
    }
}
