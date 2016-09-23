using System;
using EC.Errors.FileExceptions;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// An IoC friendly wrapper around AppSettings.
    /// </summary>

    public interface IAppSettings : IDisposable
    {
        /// <summary>
        /// Return max number of bars on bar graph
        /// </summary>

        int GraphMaxBars { get; }

        /// <summary>
        /// Return the Max Received Message Size for 
        /// sending to and from the Web and Service
        /// </summary>

        int MaxReceivedMessageSize { get; }

        /// <summary>
        /// Send timeout of the WCF proxy/service in minutes
        /// </summary>
        /// <remarks>
        /// Default value is 180 min. (3h).
        /// </remarks>

        int WCFSendTimeoutInMinutes { get; }

        /// <summary>
        /// Receive timeout of the WCF proxy/service in minutes
        /// </summary>
        /// <remarks>
        /// Default value is 180 min. (3h).
        /// </remarks>

        int WCFReceiveTimeoutInMinutes { get; }

        /// <summary>
        /// Return the app setting for the network port we should be listening on for internal services (between service and web).
        /// </summary>

        int NetworkPort { get; }

        /// <summary>
        /// Return the app setting for the network port we should listening on external services (i.e. ValidationService and ExternalSyncService).
        /// </summary>

        int PublicNetworkPort { get; }

        /// <summary>
        /// Return the app setting for the network default domain.
        /// Prefix 'http(s)://' is included.
        /// </summary>

        string NetworkDefaultDomain { get; }

        /// <summary>
        /// Return the app setting for whether the WCF Service endpoints 
        /// should be hosted on HTTP only
        /// </summary>

        bool WCFHttpOnly { get; }

        /// <summary>
        /// Return the app setting for whether the WCF Service enpoint 
        /// should be configured to enable sharing
        /// </summary>
        
        bool WCFEnablePortSharing { get; }

        /// <summary>
        /// A folder on disk where content from questions and answers will be stored
        /// (using a FileCollection).
        /// If the directory doesn't exist - try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        string QuestionContentPath { get; }

        /// <summary>
        /// Get default file system path for the file collections to store content.
        /// If the directory doesn't exist - try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        string FileCollectionContentPath { get; }

        /// <summary>
        /// Temporary directory for uploaded files in the core service.
        /// If the directory doesn't exist - try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        string TemporaryCoreSvcUploadPath { get; }

        /// <summary>
        /// Temporary directory for uploaded files in the core web.
        /// If the directory doesn't exist - try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        string TemporaryCoreWebUploadPath { get; }

        /// <summary>
        /// Path where custom notification template files (cshtml) are stored.
        /// If the directory doesn't exist - try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        string CustomNotificationTemplatePath { get; }

        /// <summary>
        /// Return the app setting for the lifestyle used for IRequestContext (we need
        /// to use different lifestyles depending on whether we are in the front end
        /// code, back end code, or the test client.
        /// </summary>

        AppSettingsConstants.CodeContext RequestContextStyle { get; }

        /// <summary>
        /// Return whether the application should be using the DropCreateDatabaseAlways DB
        /// initializer or the MigrateDatabaseToLatestVersion DB initializer.
        /// <remarks>
        /// NOTE: For debugging/development only.
        /// </remarks>
        /// </summary>

        bool CreateDBIfNeeded { get; }

        /// <summary>
        /// Indicates whether the permissions cache should be disabled. Note that this 
        /// works by preventing the creating of new cache entries, so to completely
        /// disable the cache you must also empty the PermissionsCacheEntries table in
        /// the DB.
        /// </summary>
        /// <remarks>
        /// NOTE: For debugging/development only.
        /// </remarks>

        bool DisablePermissionsCache { get; }

        /// <summary>
        /// Endpoint for the secure user service
        /// </summary>

        string UserServiceSecureEndPoint { get; }

        /// <summary>
        /// Endpoint for the secure cpms service
        /// </summary>

        string CPMSServiceSecureEndPoint { get; }

        /// <summary>
        /// Endpoint for the admin cpms service
        /// </summary>

        string CPMSServiceAdminEndPoint { get; }

        /// <summary>
        /// Endpoint for the admin user service
        /// </summary>

        string UserServiceAdminEndPoint { get; }

        /// <summary>
        /// Endpoint for the secure content service
        /// </summary>

        string ContentServiceSecureEndPoint { get; }

        /// <summary>
        /// Endpoint for the admin content service
        /// </summary>

        string ContentServiceAdminEndPoint { get; }

        /// <summary>
        /// Endpoint for the secure registration service
        /// </summary>

        string RegistrationServiceSecureEndPoint { get; }

        /// <summary>
        /// Endpoint for the admin registration service
        /// </summary>

        string RegistrationServiceAdminEndPoint { get; }

        /// <summary>
        /// Endpoint for the secure audit service
        /// </summary>

        string AuditServiceSecureEndPoint { get; }

        /// <summary>
        /// Endpoint for the admin audit service
        /// </summary>

        string AuditServiceAdminEndPoint { get; }

        /// <summary>
        /// Endpoint for the admin service
        /// </summary>

        string AdminServiceEndPoint { get; }

        /// <summary>
        /// Endpoint for the CSMART Extension service
        /// </summary>

        string CSMARTExtensionServiceEndPoint { get; }

        /// <summary>
        /// Endpoint for the CSMART Integration service
        /// </summary>

        string CSMARTIntegrationEndPoint { get; }

        /// <summary>
        /// Endppoint for CUK Integration Service
        /// </summary>
        string CUKIntegrationEndPoint { get; }

        /// <summary>
        /// Endpoint for admin sync service.
        /// </summary>

        string SyncServiceAdminEndPoint { get; }

        /// <summary>
        /// Endpoint for secure sync service.
        /// </summary>

        string SyncServiceSecureEndPoint { get; }

        /// <summary>
        /// Endpoint for external sync service (this is a secure end point with AuthorizationEndpointBehavior).
        /// </summary>

        string ExternalSyncServiceEndPoint { get; }

        /// <summary>
        /// Endpoint for validation service (this is a secure endpoint with AuthorizationEndpointBehavior).
        /// </summary>

        string ValidationServiceEndpoint { get; }

        /// <summary>
        /// Name of the extensions service, used in the extensions web project to start the service if it is not already running.
        /// </summary>

        string ExtensionsServiceName { get; }

        /// <summary>
        /// Name of the core service, used in the core web project to start the core service if it is not already running
        /// </summary>

        string CoreServiceName { get; }

        /// <summary>
        /// Name of the core notification service, used in the extension web project
        /// </summary>

        string NotificationServiceAdminEndPoint { get; }

        /// <summary>
        /// The maximum email error count to allow before setting the item as processed.
        /// </summary>
        /// <remarks>
        /// The email notification channel will try to re-send emails indefinitely. 
        /// Instead, it should keep an error count for each item and discard them if there are too many errors.
        /// </remarks>

        int NotificationMaxEmailErrors { get; }

        /// <summary>
        /// The length of the interval (ms) for how often the notification service will run.
        /// </summary>
        /// <remarks>
        /// The notification manager pushes the notification into the dependent channels. The channels themselves have
        /// an interval to run - CoreNotificationChannelInterval.
        /// </remarks>

        int CoreNotificationInterval { get; }

        /// <summary>
        /// Length of the interval (ms) for how often the notification channels will run.
        /// </summary>
        /// <remarks>
        /// The notification manager pushes the notification into the dependent channels. The channels themselves have
        /// an interval to run - CoreNotificationChannelInterval.
        /// </remarks>

        int CoreNotificationChannelInterval { get; }

        /// <summary>
        /// The length of interval (ms) for how often the deferred action manager will run.
        /// </summary>

        int CoreDeferredActionInterval { get; }

        /// <summary>
        /// Set batch size for Deferred Actions
        /// </summary>

        int CoreDeferredActionBatchSize { get; }

        /// <summary>
        /// Defer a thread start on start-up.
        /// This time (ms) will prevent a thread to start it's regular work.
        /// </summary>
        /// <remarks>
        /// This should be used if the thread needs to wait until all DB migrations have been applied.
        /// </remarks>

        int ThreadStartDeferred { get; }

        /// <summary>
        /// Sleep/delay (ms) before the report warm up thread warms up the reports (it warms it up only once).
        /// Set to negative to prevent the warmup from running.
        /// </summary>

        int WarmUpDelay { get; }

        /// <summary>
        /// Used to set the extra service startup time, Can be configured so the service doesn't timeout on startup.
        /// </summary>

        int ExtraServiceStartupTime { get; }

        /// <summary>
        /// If true, this indicates that the service will sleep for the give number seconds when it is started. This sleep
        /// occurs just after the IoC container has been set up, but before DB initialization or any other
        /// startup code. This is normally used only for debugging - the sleep gives one time to attach a debugger to
        /// the process.
        /// </summary>

        int DelayedStart { get; }

        /// <summary>
        /// A folder on disk where RTA packages will be extracted to for archival purposes.
        /// If the directory doesn't exist - we try to create it.
        /// </summary>

        string PackagesDirectoryPath { get; }

        /// <summary>
        /// A folder on disk where RTA package content will be extracted to, for the purpose of applying the package.
        /// If the directory doesn't exist - we try to create it.
        /// </summary>

        string ExtractedPackagesWorkingDirectoryPath { get; }

        /// <summary>
        /// Directory used for a base directory to export sync packages.
        /// This Directory should only be used for this purpose because there is a background thread in the
        /// CoreService that will clean out old packages from it.
        /// </summary>

        string ExportDirectory { get; }

        /// <summary>
        /// Directory contains different versions of software installation packages.  Only the main server will use this directory. RTAs will have no need for it.
        /// </summary>

        string SoftwareVersionsDirectory { get; }

        /// <summary>
        /// Get the site Id .
        /// </summary>

        Guid SiteId { get; }

        /// <summary>
        /// Get the Main site Id.
        /// </summary>

        Guid MainSiteId { get; }

        /// <summary>
        /// Get flag whether this is an RTA- or a Main-installation - IsRTA is not actually in App.config/Web.config anymore - its derived from
        /// SiteId and MainSiteId 
        /// </summary>

        bool IsRTA { get; }

        /// <summary>
        /// Enable or disable Encryption for RTA mode when right hand menu is not available to do so.
        /// This is also used whether the automatic sync should encrypt its sync packages.
        /// </summary>

        bool EncryptSyncPackages { get; }

        /// <summary>
        /// Main site name used for RTA syncing. Main uses to generate package with name. RTA uses to validate that package came from correct main.
        /// </summary>

        string MainSiteName { get; }

        /// <summary>
        /// Path to folder containing certificates
        /// </summary>

        string CertificateTemplatePath { get; }

        /// <summary>
        /// Forces the RTA commit handler (which stops certain types from persisting on an RTA) off so it will
        /// not run, even on an RTA.
        /// </summary>

        bool ForceRTACommitHandlerOff { get; }

        /// <summary>
        /// Enables the service to run as executable (must be set to <c>false</c>
        /// in order to run as installed service).
        /// </summary>
        /// <remarks>
        /// Return <c>false</c> if configuration is missing.
        /// </remarks>

        bool RunServiceAsExecutable { get; }

        /// <summary>
        /// Get the current base patching version.
        /// <remarks>
        /// Only once an RTA has aknowledged that they are on this version can they using patches for sotware updates  
        /// </remarks>         
        /// </summary>

        int BasePatchingVersion { get; }

        /// <summary>
        /// Get the patch working directory
        /// <remarks>
        /// Only used on RTA's. This folder is used to patch the instller to the latest version 
        /// </remarks>         
        /// </summary>

        string PatchWD { get; }

        /// <summary>
        /// Get the packages directory
        /// <remarks>
        /// Only used on deployed builds (Live,Demo,Test). This is where installer patches will be save to
        /// </remarks>         
        /// </summary>

        string PatchesDir { get; }

        /// <summary>
        /// Install key used for installing on an RTA
        /// </summary>

        string RTAInstallerName { get; }

        /// <summary>
        /// RTA mode installer string 
        /// </summary>

        string RTAInstallMode { get; }

        /// <summary>
        /// Installer script name
        /// </summary>

        string InstallerScriptName { get; }


        /// <summary>
        /// 64bit powershell.exe path.
        /// </summary>

        string PowershellExePath { get; }

        /// <summary>
        /// True if password in config is encrypted
        /// </summary>

        bool HasEncryptedDBPassword { get; }

        /// <summary>
        /// Name user for DB connection string
        /// </summary>
        string DBUserName { get; }

        /// <summary>
        /// Users password for DB connection string, could be encrypted (HasEncryptedDBPassword) 
        /// </summary>
        string DBUserPassword { get; }

        /// <summary>
        /// Server name used in DB connection string
        /// </summary>
        string DBServerName { get; }

        /// <summary>
        /// Name of Database for DB connection string
        /// </summary>
        string DBName { get; }

        /// <summary>
        /// Flag whether to use Integrated Security for the DB connection (defaults to false)
        /// </summary>
        bool DBIntegratedSecurity { get; }

        /// <summary>
        /// Database command timeout in seconds (300 seconds, or 5 minutes, by default).
        /// </summary>
        int DBCommandTimeout { get; }

        /// <summary>
        /// User name the service should be running on
        /// </summary>

        string ServiceLogOnUserName { get; }

        /// <summary>
        /// Domain name the service should be running on
        /// </summary>

        string ServiceLoggOnDomainName { get; }

        /// <summary>
        /// If ture service will verify that it is running as Service.LogOnDomainName\Service.LogOnUserName. If false service doens't check what it is running as
        /// </summary>

        bool ServiceRunningAsLoggedOnUser { get; }

        /// <summary>
        /// Time interval in seconds to check whether we should sync an RTA (this is not the actual sync interval - that is determined via SiteInfo property).
        /// </summary>
        /// <remarks>
        /// This is the inverval for the background thread to run the loop/wakeup from sleep and do work.
        /// If it's not time to sync yet - go back to sleep again.
        /// Default value if no AppSetting found: 300s.
        /// </remarks>

        int AutoSyncBackgroundThreadCheckInterval { get; }

        /// <summary>
        /// Time (in seconds) to back off the network if 1 network failure occurs.
        /// </summary>
        /// <remarks>
        /// Default if no AppSetting found: 15s
        /// </remarks>

        int AutoSyncNetworkBackOffDelay1 { get; }

        /// <summary>
        ///  Time (in seconds) to back off the network if 2 network failures occur.
        /// </summary>
        /// <remarks>
        /// Default if no AppSetting found: 60s
        /// </remarks>

        int AutoSyncNetworkBackOffDelay2 { get; }

        /// <summary>
        /// Time (in seconds) to back off the network if 3 network failures occur .
        /// </summary>
        /// <remarks>
        /// Default if no AppSetting found: 300s
        /// </remarks>

        int AutoSyncNetworkBackOffDelay3 { get; }

        /// <summary>
        /// Time (in seconds) to back off the network if 4+ network failures occur.
        /// </summary>
        /// <remarks>
        /// Default if no AppSetting found: 3600s
        /// </remarks>

        int AutoSyncNetworkBackOffDelayMax { get; }

        /// <summary>
        /// Threshold of task and authentication errors to cancel the sync task (if (task + authentication errors) >= threshold - cancel the task).
        /// </summary>

        int AutoSyncErrorThreshold { get; }
        
        /// <summary>
        /// Time to wait for Main to run org and course rules before we generate the next MainToRta sync package in seconds.
        /// </summary>
        /// <remarks>
        /// This is a bandage implementation. 
        /// The proper implementation should check that the deferred actions caused by the applied sync package have run.
        /// This means a migration to extend the deferred action to contain the sync package Id to identify the deferred actions
        /// to check. 
        /// With multiple RTAs there might be always outstanding deferred actions for rules to run and we need to know which actions
        /// have been generated by which sync package.
        /// </remarks>

        int AutoSyncWaitForMainToRunRules { get; }
        
        /// <summary>
        /// Chunk size to up/download a package piece in bytes.
        /// </summary>
        /// <remarks>
        /// This will be adjusted if the chunk size is larger than the max bandwidth specified for this RTA.
        /// That means that the MaxBandWidth will be the upper limit to up/download a package piece.
        /// </remarks>
        
        int AutoSyncMaxTransferChunkSize { get; }


        /// <summary>
        /// The timeout/expiry time (in seconds) of an authentication token.
        /// </summary>

        int AuthenticationTokenTimeout { get; }

        /// <summary>
        /// Path to log4net dir, used to clean out old logs
        /// </summary>

        string CleanLog4NetDirPath { get; }

        /// <summary>
        /// Path to IIS logs dir, used to clean out old logs
        /// </summary>

        string CleanIISLogsDirPath { get; }

        /// <summary>
        /// Configure the Root log level at runtime
        /// </summary>

        string RootLogLevel { get; }

        /// <summary>
        /// Configure the Trace log level at runtime
        /// </summary>

        string TraceLogLevel { get; }


        /// <summary>
        /// Configure the Trace log output file path
        /// </summary>
        
        string TraceLogPath { get; }

        /// <summary>
        /// Configure the Event log level at runtime
        /// </summary>

        string EventLogLevel { get; }

        /// <summary>
        /// Configure the Email log level at runtime
        /// </summary>

        string EmailLogLevel { get; }

        /// <summary>
        /// Configure the Debugger log level at runtime
        /// </summary>

        string DebuggerLogLevel { get; }

        /// <summary>
        /// Configure the Stats log level at runtime
        /// </summary>

        string StatsLogLevel { get; }

        /// <summary>
        /// Configure the Stats log output file path
        /// </summary>

        string StatsLogPath { get; }

        /// <summary>
        /// Email Override
        /// </summary>
        string EmailOverride { get; }

        /// <summary>
        /// Return the app setting for the number of hours that we should add to SyncDate to get out of paranoia to sync a little more content (in seconds).
        /// </summary>

        int FudgeDuration { get; }

        /// <summary>
        /// Set email subject length
        /// </summary>
        /// 

        int SetSubjectLength { get; }



        /// <summary>
        /// Return suppressed IPs to add IP filtering for logging
        /// </summary>

        string LoggingSuppressedIPs { get; }

        /// <summary>
        /// Boolean parameter to show if we would like to generate sync package with/without software
        /// </summary>

        bool SyncNoSoftware { get; }


        /// <summary>
        /// Allow all user to logon to root
        /// </summary>

        bool AllowAllUsersAtRoot { get; }

        /// <summary>
        /// The timeout after which the UI gives up on applying a sync package, in ms.
        /// </summary>

        int SyncPackageUITimeOut { get; }

        /// <summary>
        /// The maximum number of certificates that user can download in one try
        /// </summary>

        int CertificatesMaxDownload { get; }

        /// <summary>
        /// If MonotonicClock is MaxSkipForwardSeconds older than the current time, log and hault.
        /// </summary>
        int MaxSkipForward { get; }

        /// <summary>
        /// MonotonicClockManager logs an error when the MonotonicClock WarnBackwardSeconds is older than the current time.
        /// </summary>
        int WarnBackward { get; }

        /// <summary>
        /// Every ClockMaintenanceInterval milliseconds the ClockTimeStamp is updated by MonotonicClockManager.
        /// </summary>
        int ClockMaintenanceInterval { get; }

        /// <summary>
        /// Records in ClockTimeStamp that are older than this value will be deleted by MonotonicClockManager.
        /// </summary>
        int MaxClockTimeStamp { get; }

        /// <summary>
        /// Used just by devs to make things easier to manage.
        /// </summary>
        bool ClockCheckOverride { get; }

        /// <summary>
        /// Records in ClockTimeStamp that are older than this value will be deleted by MonotonicClockManager.
        /// </summary>
        bool ForceClock { get; }
    }

    /// <summary>
    /// AppSettings key values and other constants.
    /// </summary>

    public class AppSettingsConstants
    {
        public const string GraphMaxBars = "Graph.MaxBars";                                                     //The maximum number of bar on bar graph
        public const string NetworkPortOpt = "Network.Port";                                                    // NetworkPort()
        public const string NetworkPortPublic = "Network.Port.Public";                                          // Public network port for external services.
        public const string QuestionRepoPath = "Paths.QuestionContentRepository";                               // Location of question content 
        public const string FileCollectionContentPath = "Paths.FileCollectionDefault";                          // File system path for the file collections.
        public const string TemporaryCoreSvcUploadPath = "Paths.CoreSvc.TempFiles";                             // Temporary directory for uploaded files of the core service.
        public const string TemporaryCoreWebUploadPath = "Paths.CoreWeb.TempFiles";                             // Temporary directory for uploaded files of the core web.
        public const string CustomNotificationTemplatePath = "Paths.CustomNotificationTemplates";               // Absolute path where custom notification templates are stored.
        public const string NetworkSecureDomain = "Network.SecureDomain";                                       // Secure domain used for WebDav access when a user is not accessing the site from a secure domain. Contains prefix 'https://'.
        public const string NetworkDefaultDomain = "Network.DefaultDomain";                                     /// Non-secure default domain used for generating URLs (e.g. to content). It contains the prefix 'http://'
        public const string WCFHttpOnly = "Core.WCFHttpOnly";                                                   // Whether to host services from Http Only
        public const string WCFEnablePortSharing = "Core.WCFEnablePortSharing";                                 // Whether to configure services to use port sharing
        public const string MaxReceivedMessageSize = "Core.MaxReceivedMessageSize";                             // The Max Size transmitted between the Service and Web
        public const string WCFSendTimeoutInMinutes = "WCF.SendTimeout.Minutes";                                // Send timeout of the WCF proxy/service in minutes
        public const string WCFReceiveTimeoutInMinutes = "WCF.ReceiveTimeout.Minutes";                          // Receive timeout of the WCF proxy/service in minutes
        public const string CreateDBIfNeeded = "Core.CreateDB";                                                 // If present, use the CreateDatabaseIfNotExists initializer
        public const string RequestContextStyleOpt = "Core.RequestContext";                                     // RequestContextStyle()
        public const string DisablePermissionsCache = "Core.DisablePermissionsCache";                           // If present, turn off permissions caching 
        public const string ContentServiceSecureEndPoint = "Network.ContentServiceSecureEndPoint";              // Content service secure end point
        public const string ContentServiceAdminEndPoint = "Network.ContentServiceAdminEndPoint";                // Content admin secure end point
        public const string UserServiceSecureEndPoint = "Network.UserServiceSecureEndPoint";                    // User service secure end point
        public const string UserServiceAdminEndPoint = "Network.UserServiceAdminEndPoint";                      // User service admin end point
        public const string CPMSServiceSecureEndPoint = "Network.CPMSServiceSecureEndPoint";                    // CPMS service secure end point
        public const string CPMSServiceAdminEndPoint = "Network.CPMSServiceAdminEndPoint";                      // CPMS service admin end point
        public const string RegistrationServiceSecureEndPoint = "Network.RegistrationServiceSecureEndPoint";    // Registration service secure end point
        public const string RegistrationServiceAdminEndPoint = "Network.RegistrationServiceAdminEndPoint";      // Registration service admin end point
        public const string AuditServiceSecureEndPoint = "Network.AuditServiceSecureEndPoint";                  // Audit service secure end point
        public const string AuditServiceAdminEndPoint = "Network.AuditServiceAdminEndPoint";                    // Audit service admin end point
        public const string AdminServiceEndPoint = "Network.AdminServiceEndPoint";                              // Admin service endpoint
        public const string CSMARTExtensionServiceEndPoint = "Network.CSMARTExtensionsServiceEndPoint";         // CSMART Extension Service end point used in the Extensions Web Project
        public const string CSMARTIntegrationEndPoint = "Network.CSMARTIntegrationEndPoint";                    // CSMART Integration Service end point used in the Extensions Web Project
        public const string CUKIntegrationEndPoint = "Network.CUKIntegrationEndPoint";                          // CUK Integration Service end point
        public const string SyncServiceAdminEndpoint = "Network.SyncServiceAdminEndPoint";                      // Sync service admin end point
        public const string SyncServiceSecureEndpoint = "Network.SyncServiceSecureEndPoint";                    // Sync service secure end point
        public const string ExternalSyncServiceEndpoint = "Network.Public.ExternalSyncServiceEndpoint";         // External sync service end point (this is the secure end point since it is publicly available)
        public const string ValidationServiceEndpoint = "Network.Public.ValidationServiceEndpoint";             // Validation service end point as external service (secure endpoint publicly available).
        public const string ExtensionsServiceName = "Ext.ExtensionsServiceName";                                // Name of the extensions service, used in the extensions web project to start the service if it is not already running.
        public const string CoreServiceName = "Core.CoreServiceName";                                           // Name of the core service, used in the core web project to start the core service if it is not already running
        public const string NotificationServiceAdminEndPoint = "Network.NotificationServiceAdminEndPoint";      // Name of the core notification service, used in the extension web project to connect to the service
        public const string NotificationMaxEmailErrors = "Core.Notifications.MaxEmailErrors";                   // The maximum email error count to allow before setting the item as processed.
        public const string NotificationInterval = "Core.NotificationInterval";                                 // The length of the interval (ms) for how often the notification service will run.
        public const string NotificationChannelInterval = "Core.NotificationChannelInterval";                   // The length of the interval (ms) for how often the notification channels will run.
        public const string DeferredActionInterval = "Core.DeferredActionInterval";                             // The length of the interval (ms) for how often the deferred action manager will run.
        public const string DeferredActionBatchSize = "Core.DeferredActionBatchSize";                           // The number of deferred actions processed in one go in the deferred action manager.
        public const string ThreadStartDeferred = "Core.ThreadStartDeferred";                                   // The amount of ms to sleep before a thread does its normal work.
        public const string WarmUpDelay = "Core.WarmUpDelay";                                                   // The amount of ms to sleep/delay before the report warm up thread warms up the reports (it warms it up only once).
        public const string ExtraServiceStartupTime = "Service.ExtraStartupTime";                               // Make the service request additional start time from the SCM
        public const string DelayedStart = "Service.DelayedStartTime";                                          // Sleep time in main program before DB initialization
        public const string PackagesDirectoryPath = "Paths.PackagesDirPath";                                    // Path to the packages directory.  RTA will unzip new packages to this location
        public const string ExtractedPackagesWorkingDirectoryPath = "Paths.ExtractedPackagesWorkingDirPath";    // A folder on disk where RTA package content will be extracted to, for the purpose of applying the package.
        public const string ExportDirectory = "Paths.Exports";                                                  // Path to the exports directory. Used to generate syn packages
        public const string SoftwareVersionsDirectory = "Paths.SoftwareVersions";                               // Path to the software versions directory.   
        public const string CertificateTemplatePath = "Paths.CertificateTemplates";                             // Path to the certificates template directory
        public const string SiteId = "Sync.SiteId";                                                             // (Optional) Site Id for an RTA/main site.
        public const string MainSiteId = "Sync.MainSiteId";                                                     // Site Id for an main site.
        public const string MainSiteName = "Sync.MainSiteName";                                                 // Main site name, used to make sure sync packages are coming from the correct place.
        public const string SyncServiceName = "Sync.SyncServicename";                                           // Name of sync service used in web to start sync service
        public const string EncryptionSyncPackages = "Sync.EncryptSyncPackages";                                // Allows to avoid encryption when syncing in RTA mode, when the right hand menu does not exist. This flag is also used for automatic sync.
        public const string ForceRTACommitHandlerOff = "Sync.ForceRTACommitHandlerOff";                         // Forces the RTA commit handler (which stops certain types from persisting on an RTA) off so it will not run, even on an RTA.
        public const string RunServiceAsExecutable = "Debug.RunServiceAsExecutable";                            // Enable running the service as executable (use with care)
        public const string BasePatchingVersion = "Sync.BasePatchingVersion";                                   //Only once an RTA has aknowledged that they are on this version can they using patches for sotware updates                   
        public const string RTAPatchWD = "Sync.PatchWD";                                                        //Only used on RTA's. This folder is used to patch the instller to the latest version
        public const string PatchesDir = "Sync.Patches";                                                        //Only used on deployed builds (Live,Demo,Test). This is where installer patches will be save to
        public const string RTAInstallerName = "RTA.InstallName";                                               //Install key used for installing on an RTA
        public const string RTAInstallMode = "RTA.RTAInstallMode";                                              //RTA mode installer string 
        public const string InstallerScriptName = "RTA.InstallerScriptName";                                    //Installer script name
        public const string PowershellExePath = "RTA.PowershellExePath";                                        //Used on an RTA to run the installer, needs to be path to 64bit powershell.exe
        public const string DBName = "Connection.DBName";                                                       //DBName used to create connection string
        public const string DBIntegratedSecurity = "Connection.IntegratedSecurity";                             //Allow DB to connect with integrated security
        public const string DBUserName = "Connection.UserName";                                                 //User name for connection string
        public const string DBUserPassword = "Connection.UserPassword";                                         //User password for connection string
        public const string DBServerName = "Connection.DataSource";                                             //DataSource/Server name for connection string 
        public const string DBHasEncryptedPassword = "Connection.HasEncryptedDBPassword";                       //True if DB password needs to decrypted, false to use a plain text password
        public const string DBCommandTimeout = "Connection.DBCommandTimeout";                                   //Time in seconds for a DB command timeout
        public const string AutoSyncBackgroundThreadCheckInterval = "Sync.Automatic.BackgroundThreadCheckInterval";  //Time interval in seconds to check whether we should sync an RTA (this is not the actual sync interval - that is determined via SiteInfo property).
        public const string AutoSyncNetworkBackOffDelay1 = "Sync.Automatic.NetworkBackOffDelay1";               //Time to back off the network if 1 network failure occurs (in seconds).
        public const string AutoSyncNetworkBackOffDelay2 = "Sync.Automatic.NetworkBackOffDelay2";               //Time to back off the network if 2 network failures occur (in seconds).
        public const string AutoSyncNetworkBackOffDelay3 = "Sync.Automatic.NetworkBackOffDelay3";               //Time to back off the network if 3 network failures occur (in seconds).
        public const string AutoSyncNetworkBackOffDelayMax = "Sync.Automatic.NetworkBackOffDelayMax";           //Time to back off the network if 4+ network failures occur (in seconds).
        public const string AutoSyncErrorThreshold = "Sync.Automatic.ErrorThreshold";                           //Threshold before an auto-sync task is cancelled (sum of task errors and authentication errors).
        public const string AutoSyncWaitForMainToRunRules = "Sync.Automatic.WaitForMainToRunRules";             //Time to wait for all rules (org/course) to run after applying an RtaToMain sync package. That is the time before we generate the next MainToRta package in auto-sync.
        public const string AutoSyncMaxTransferChunkSize = "Sync.Automatic.MaxTransferChunkSize";               //Max. chunk of a sync package up/downloaded within one service call during the automatic sync.
        public const string AuthenticationTokenTimeout = "Core.AuthenticationTokenTimeout";                     //Timeout of an authentication token in seconds
        public const string ServiceLogOnUserName = "Service.LogOnUserName";                                     //User the service is running as. User on RTAs to run as administrator.
        public const string ServiceLogOnUserDomain = "Service.LogOnDomainName";                                 //Domain the serice is running as. Must be domain if service is logged on as one or PC Name if it is not
        public const string ServiceRunAsLoggedOnUser = "Service.RunAsLoggedOnUser";                             //If ture service will verify that it is running as Service.LogOnDomainName\Service.LogOnUserName. If false service doens't check what it is running as
        public const string CleanLog4NetDirPath = "Clean.Log4NetDirPath";                                       //Log4Net path to clean out
        public const string CleanIISLogsDirPath = "Clean.IISLogsDirPath";                                       //IISLogsPath to clean out 
        public const string RootLogLevel = "Logging.RootLogLevel";                                              //Root (log4net) log level
        public const string TraceLogLevel = "Logging.TraceLogLevel";                                            //Trace (file) log level
        public const string TraceLogPath = "Logging.TraceLogPath";                                              //Trace (file) log file output path
        public const string EventLogLevel = "Logging.EventLogLevel";                                            //Event (windows) log level
        public const string EmailLogLevel = "Logging.EmailLogLevel";                                            //Email (smtp) log level
        public const string DebuggerLogLevel = "Logging.DebuggerLogLevel";                                      //Debugger (visual studio) log level
        public const string StatsLogLevel = "Logging.StatsLogLevel";                                            //Stats (file) log level
        public const string StatsLogPath = "Logging.StatsLogPath";                                              //Stats (file) log file output path
        public const string EmailOverride = "Core.EmailOverride";                                               //Email redirected to specific email address
        public enum CodeContext { web, server, client };                                                        //Possible values for RequestContextStyle
        public const string FudgeDuration = "Sync.ExportFudgeTime";                                             //Fudges the date out of paranoia to sync a little more content.
        public const string SetSubjectLength = "Core.SetSubjectLength";                                         //Set subject length of email to a specific number
        public const string LoggingSuppressedIP = "Logging.SuppressedIP";                                       //Suppressed IPs to add IP filtering for logging (comma separated, .255 for wildcard)
        public const string SyncNoSoftware = "Sync.NoSoftware";                                                 //Boolean parameter to show if we would like to generate sync package with/without software
        public const string MaxRTAClockDifference = "Sync.MaxRTAClockDifference";                               //Time difference allowed between Main and RTA.
        public const string SyncPackageUITimeOut = "SyncPackage.TimeOut";                                       //The timeout after which the UI gives up on applying a sync package
        public const string AllowAllUsersAtRoot = "Dev.AllowAllUsersAtRoot";                                    //Allow all user to login to the root
        public const string CertificatesMaxDownload = "CertificatesDownload.MaxNumber";                         //Max number for downloading certificates
        public const string MaxSkipForward = "MonotonicClock.MaxSkipForward";                                   //If MonotonicClock is MaxSkipForwardSeconds older than the current time, log and hault.
        public const string WarnBackward = "MonotonicClock.WarnBackward";                                       //MonotonicClockManager logs an error when the MonotonicClock WarnBackwardSeconds is older than the current time.
        public const string ClockMaintenanceInterval = "MonotonicClock.ClockMaintenanceInterval";                                             //Every ClockPing milliseconds the ClockTimeStamp is updated by MonotonicClockManager.
        public const string MaxClockTimeStamp = "MonotonicClock.MaxClockTimeStamp";                             //Records in ClockTimeStamp that are older than this value will be deleted by MonotonicClockManager.
        public const string ForceClock = "MonotonicClock.ForceClock";                                           //Used on an RTA that has been turned off too long or otherwise has clock issues
        public const string ClockCheckOverride = "MonotonicClock.ClockCheckOverride";                           //Used just by devs to make things easier to manage without ForceClock.
    }
}
