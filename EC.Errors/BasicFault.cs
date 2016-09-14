using EC.Errors.CommonExceptions;
using EC.Errors.FileExceptions;
using EC.Errors.ImportAndSyncExceptions;
using EC.Errors.LMSExceptions;
using EC.Errors.NotificationExceptions;
using EC.Errors.RegistrationExceptions;
using EC.Errors.UserExceptions;
using System.Runtime.Serialization;
using EC.Errors.Synchronization;

namespace EC.Errors
{
    /// <summary>
    /// Basic fault which is inherited by all other fault exceptions.
    /// </summary>

    [DataContract]
    [KnownType(typeof(AlreadyRegisteredFault))]
    [KnownType(typeof(OverlappingRegistrationFault))]
    [KnownType(typeof(ConfigurationParameterFault))]
    [KnownType(typeof(NotFoundFault))]
    [KnownType(typeof(AlreadyExistsFault))]
    [KnownType(typeof(AlreadySetFault))]
    [KnownType(typeof(ParameterValidationFault))]
    [KnownType(typeof(PasswordComplexityFault))]
    [KnownType(typeof(PasswordHistoryFault))]
    [KnownType(typeof(NullAttributeFault))]
    [KnownType(typeof(AuthenticationRequiredFault))]
    [KnownType(typeof(IsRequiredFault))]
    [KnownType(typeof(DeletedUserFault))]
    [KnownType(typeof(FileWriteFault))]
    [KnownType(typeof(DirectoryCreationFault))]
    [KnownType(typeof(DirectoryAlreadyExistsFault))]
    [KnownType(typeof(FileDeletionFault))]
    [KnownType(typeof(FileAlreadyExistsFault))]
    [KnownType(typeof(FileAccessFault))]
    [KnownType(typeof(PathCombineFault))]
    [KnownType(typeof(FileCopyFault))]
    [KnownType(typeof(DirectoryDoesNotExistFault))]
    [KnownType(typeof(EmailFormatFault))]
    [KnownType(typeof(EmailSendFault))]
    [KnownType(typeof(ContainsWhiteSpacesFault))]
    [KnownType(typeof(UnknownFault))]
    [KnownType(typeof(CannotCreateItemFault))]
    [KnownType(typeof(OrganizationLogonStateFault))]
    [KnownType(typeof(IllegalLogonCharacterFault))]
    [KnownType(typeof(PathFormatFault))]
    [KnownType(typeof(ConfigurationFault))]
    [KnownType(typeof(NotAuthorizedFault))]
    [KnownType(typeof(AttributeNotFoundFault))]
    [KnownType(typeof(NotANavPageFault))]
    [KnownType(typeof(NotACourseFault))]
    [KnownType(typeof(CookieTheftFault))]
    [KnownType(typeof(ForgotPasswordTokenExpiredFault))]
    [KnownType(typeof(RegistrationAgreementFailureFault))]
    [KnownType(typeof(ContentTypeMismatchFault))]
    [KnownType(typeof(InvalidCapabilityFault))]
    [KnownType(typeof(CapabilityNotFoundFault))]
    [KnownType(typeof(XMLFormatFault))]
    [KnownType(typeof(FileCollectionNotFoundFault))]
    [KnownType(typeof(WrongLinkTypeFault))]
    [KnownType(typeof(ItemInUseFault))]
    [KnownType(typeof(NotRegisteredInOfferingFault))]
    [KnownType(typeof(WrongCapabilityAttributeFormatFault))]
    [KnownType(typeof(AttributeFormatFault))]
    [KnownType(typeof(AlreadyOrgMemberFault))]
    [KnownType(typeof(AlreadyMemberFault))]
    [KnownType(typeof(FileNotCshtmlFault))]
    [KnownType(typeof(NotificationExpressionFault))]
    [KnownType(typeof(FolderOrFileNotFoundFault))]
    [KnownType(typeof(DataConsistencyFault))]
    [KnownType(typeof(NoUserNameFault))]
    [KnownType(typeof(InvalidCookieFault))]
    [KnownType(typeof(NavPageTagConflictFault))]
    [KnownType(typeof(InvalidReferenceFault))]
    [KnownType(typeof(ImportFault))]
    [KnownType(typeof(OrganizationNotFoundFault))]
    [KnownType(typeof(DuplicateIdFault))]
    [KnownType(typeof(StillConvertingFault))]
    [KnownType(typeof(WrongPackageFault))]
    [KnownType(typeof(PathsDoNotMatchFault))]
    [KnownType(typeof(PackageAlreadyAppliedFault))]
    [KnownType(typeof(PackageTooOldFault))]
    [KnownType(typeof(RelativePathFault))]
    [KnownType(typeof(ImportModeNotSupportedFault))]
    [KnownType(typeof(InvalidDateTimeFault))]
    [KnownType(typeof(SoftwareUpdateMissingFault))]
    [KnownType(typeof(ModelValidationFault))]
    [KnownType(typeof(LockFailedFault))]
    [KnownType(typeof(OutOfRangeFault))]
    [KnownType(typeof(WrongPackageSizeFault))]
    [KnownType(typeof(WrongTransferStateFault))]
    [KnownType(typeof(CantApplySyncPackageFault))]
    [KnownType(typeof(NoConnectionFault))]
    [KnownType(typeof(AuthenticationExpiredFault))]
    [KnownType(typeof(DuplicateGroupDefinitionFault))]
    [KnownType(typeof(UsersBelongToSubGroupsFault))]
    [KnownType(typeof(CantDecryptDBPasswordFault))]
    [KnownType(typeof(CantDecryptPasswordFault))]
    [KnownType(typeof(CantEncryptPasswordFault))]
    [KnownType(typeof(NotSupportedOnMainFault))]
    [KnownType(typeof(NotSupportedOnRtaFault))]
    [KnownType(typeof(NotAPathFault))]
    [KnownType(typeof(CannotDeleteFault))]

    public class BasicFault
    {
        public BasicFault(string msg, string reqPath, CurrentUserInfo userInfo)
        {
            Message = msg;
            RequestPath = reqPath;

            if (userInfo == null)
            {
                CurrentUserUserName = "*Anonymous*";
            }
            else
            {
                CurrentUserUserName = userInfo.UserName;
                CurrentUserEmail = userInfo.Email;
            }
        }

        public BasicFault() { }

        /// <summary>
        /// Error message.
        /// </summary>

        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Path of the element in question or where the error occurred. May be null
        /// for operations that do not use paths.
        /// </summary>

        [DataMember]
        public string RequestPath { get; set; }

        /// <summary>
        /// The current user when the fault occurred. Will be null if there is no 
        /// current user.
        /// </summary>
        
        [DataMember]
        public string CurrentUserUserName { get; set; }

        /// <summary>
        /// The current user's email. Will be null if there is no current user.
        /// </summary>
        
        [DataMember]
        public string CurrentUserEmail { get; set; }
    }

    /// <summary>
    /// Fault returned for an unexpected internal error
    /// </summary>

    [DataContract]
    public class UnknownFault : BasicFault
    {
        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string Type { get; set; }
    }
}
