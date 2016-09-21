using System;
using System.Configuration;
using Castle.Core.Logging;
using Castle.MicroKernel;
using EC.Common.Base;
using EC.Errors.CommonExceptions;
using EC.Errors.FileExceptions;
using EC.Common.Interfaces;
using EC.Core.Common;

namespace EC.Core.Common
{
    /// <summary>
    /// Return settings read from app.config (or appropriate defaults)
    /// </summary>

    [SingletonType]
    [RegisterAsType(typeof(IAppSettings))]

    public class AppSettings : IAppSettings
    {
        public int GraphMaxBars
        {
            get
            {
                
                var s = ConfigurationManager.AppSettings[AppSettingsConstants.GraphMaxBars];
                if (s == null) { return 18; }

                int result = 18;

                if (!int.TryParse(s, out result))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.GraphMaxBars, s);
                    result = 18;
                    m_Logger.WarnFormat("AppSettings: use default max bars '{0}' for '{1}.'", s, AppSettingsConstants.GraphMaxBars);
                }
                return result;
            }
        }

        /// <summary>
        /// Send timeout of the WCF proxy/service in minutes
        /// </summary>
        /// <remarks>
        /// Default value is 180 min. (3h).
        /// </remarks>

        public int WCFSendTimeoutInMinutes
        {
            get
            {
                const int defaultResult = 180;
                int result = defaultResult;
                var s = ConfigurationManager.AppSettings[AppSettingsConstants.WCFSendTimeoutInMinutes];
                if (s == null) { return result; }

                if (!int.TryParse(s, out result))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.WCFSendTimeoutInMinutes, s);
                    result = defaultResult;
                    m_Logger.WarnFormat("AppSettings: use default {0} min. for '{1}.'", s, AppSettingsConstants.WCFSendTimeoutInMinutes);
                }
                return result;
            }
        }

        /// <summary>
        /// Receive timeout of the WCF proxy/service in minutes
        /// </summary>
        /// <remarks>
        /// Default value is 180 min. (3h).
        /// </remarks>

        public int WCFReceiveTimeoutInMinutes
        {
            get
            {
                const int defaultResult = 180;
                int result = defaultResult;
                var s = ConfigurationManager.AppSettings[AppSettingsConstants.WCFReceiveTimeoutInMinutes];
                if (s == null) { return result; }

                if (!int.TryParse(s, out result))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.WCFReceiveTimeoutInMinutes, s);
                    result = defaultResult;
                    m_Logger.WarnFormat("AppSettings: use default {0} min. for '{1}.'", s, AppSettingsConstants.WCFReceiveTimeoutInMinutes);
                }
                return result;
            }
        }

        /// <summary>
        /// Return whether the application should be using the DropCreateDatabaseAlways DB
        /// initializer or the MigrateDatabaseToLatestVersion DB initializer.
        /// </summary>

        public bool CreateDBIfNeeded
        {
            get
            {
                var s = ConfigurationManager.AppSettings[AppSettingsConstants.CreateDBIfNeeded];
                if (s == null) return false;
                bool result;

                if (!bool.TryParse(s, out result))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.CreateDBIfNeeded, s);
                    result = false;
                    m_Logger.WarnFormat("AppSettings: Using default migration-based initializer {0} = false.", AppSettingsConstants.CreateDBIfNeeded);
                }
                return result;
            }
        }
        /// <summary>
        /// Return the Max Received Message Size for 
        /// sending to and from the Web and Service
        /// </summary>

        public int MaxReceivedMessageSize
        {
            get
            {
                var s = ConfigurationManager.AppSettings[AppSettingsConstants.MaxReceivedMessageSize];
                if (s == null) return 2147483647;
                int size;

                if (!int.TryParse(s, out size))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.MaxReceivedMessageSize, s);
                    size = 8000;
                    m_Logger.WarnFormat("AppSettings: use default port '{0}' for '{1}.'", size, AppSettingsConstants.MaxReceivedMessageSize);
                }
                return size;
            }
        }


        /// <summary>
        /// Return the app setting for the network port we should be listening on.
        /// </summary>

        public int NetworkPort
        {
            get
            {
                var p = ConfigurationManager.AppSettings[AppSettingsConstants.NetworkPortOpt];
                if (p == null) return 8000;
                int port;

                if (!int.TryParse(p, out port))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.NetworkPortOpt, p);
                    port = 8000;
                    m_Logger.WarnFormat("AppSettings: use default port '{0}' for '{1}.'", port, AppSettingsConstants.NetworkPortOpt);
                }
                return port;
            }
        }

        /// <summary>
        /// Return the app setting for the number of hours that we should add to SyncDate to get out of paranoia to sync a little more content (in seconds).
        /// </summary>

        public int FudgeDuration
        {
            get
            {
                var s = ConfigurationManager.AppSettings[AppSettingsConstants.FudgeDuration];
                if (s == null) return -300;
                int seconds;

                if (!int.TryParse(s, out seconds))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.FudgeDuration, s);
                    seconds = -300;
                    m_Logger.WarnFormat("AppSettings: useing default fudge value '{0}' for '{1}.'", seconds, AppSettingsConstants.FudgeDuration);
                }
                return seconds;
            }
        }

        /// <summary>
        /// Return the app setting for the network port we should listening on external services (i.e. ValidationService and ExternalSyncService).
        /// </summary>

        public int PublicNetworkPort
        {
            get
            {
                var p = ConfigurationManager.AppSettings[AppSettingsConstants.NetworkPortPublic];
                if (p == null) return 8500;
                int port;

                if (!int.TryParse(p, out port))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.NetworkPortOpt, p);
                    port = 8500;
                    m_Logger.WarnFormat("AppSettings: use default port '{0}' for '{1}.'", port, AppSettingsConstants.NetworkPortOpt);
                }
                return port;
            }
        }

        /// <summary>
        /// Return the app setting for the network default domain.
        /// Prefix 'http://' is included.
        /// </summary>

        public string NetworkDefaultDomain
        {
            get
            {
                var dd = ConfigurationManager.AppSettings[AppSettingsConstants.NetworkDefaultDomain];
                if (string.IsNullOrWhiteSpace(dd))
                {
                    dd = "http://report.ec.com";
                    m_Logger.WarnFormat("AppSettings '{0}' is missing or not configured. Use network default domain '{1}'.", AppSettingsConstants.NetworkSecureDomain, dd);
                }
                return dd;
            }

        }


        /// <summary>
        /// Return the app setting for whether the WCF Service endpoints 
        /// should be hosted on HTTP only
        /// </summary>

        public bool WCFHttpOnly
        {
            get
            {
                var h = ConfigurationManager.AppSettings[AppSettingsConstants.WCFHttpOnly];
                if (h == null) return false;
                bool httpOnly = false;
                if (!bool.TryParse(h, out httpOnly))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.WCFHttpOnly, h);
                }
                return httpOnly;
            }
        }

        public bool WCFEnablePortSharing
        {
            get
            {
                var h = ConfigurationManager.AppSettings[AppSettingsConstants.WCFEnablePortSharing];
                if (h == null) return true;
                bool enablePortSharing = true;
                if (!bool.TryParse(h, out enablePortSharing))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.WCFEnablePortSharing, h);
                }
                return enablePortSharing;
            }
        }

        /// <summary>
        /// A folder on disk where content from questions and answers will be stored
        /// (using a FileCollection).
        /// If the directory doesn't exist - try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        public string QuestionContentPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings[AppSettingsConstants.QuestionRepoPath];
                if (path == null)
                {
                    path = "C:/MarineLMS/QuestionContent/";
                    m_Logger.WarnFormat("AppSettings '{0}' is missing or not configured. Use default path '{1}'.", AppSettingsConstants.QuestionRepoPath, path);
                }

                if (!m_FileAccess.DirectoryExists(path))
                {
                    // Create directory if it doesn't exist...
                    m_FileAccess.CreateDirectory(path);
                }
                return path;
            }
        }

        /// <summary>
        /// A folder on disk where RTA packages will be extracted to
        /// If the directory doesn't exist - we try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        public string PackagesDirectoryPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings[AppSettingsConstants.PackagesDirectoryPath];
                if (path == null)
                {
                    path = "C:/MarineLMS/Packages/";
                    m_Logger.WarnFormat("AppSettings '{0}' is missing or not configured. Use default path '{1}'.", AppSettingsConstants.PackagesDirectoryPath, path);
                }

                if (!m_FileAccess.DirectoryExists(path))
                {
                    // Create directory if it doesn't exist...
                    m_FileAccess.CreateDirectory(path);
                }
                return path;
            }
        }

        /// <summary>
        /// A folder on disk where RTA package content will be extracted to, for the purpose of applying the package.
        /// If the directory doesn't exist - we try to create it.
        /// </summary>
        public string ExtractedPackagesWorkingDirectoryPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings[AppSettingsConstants.ExtractedPackagesWorkingDirectoryPath];
                if (path == null)
                {
                    path = "C:/MarineLMS/ExtractedPackWD/";
                    m_Logger.WarnFormat("AppSettings '{0}' is missing or not configured. Use default path '{1}'.", AppSettingsConstants.ExtractedPackagesWorkingDirectoryPath, path);
                }

                if (!m_FileAccess.DirectoryExists(path))
                {
                    // Create directory if it doesn't exist...
                    m_FileAccess.CreateDirectory(path);
                }
                return path;
            }
        }


        /// <summary>
        /// Get default file system path for the file collections to store content.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        public string FileCollectionContentPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings[AppSettingsConstants.FileCollectionContentPath];
                if (path == null)
                {
                    path = "C:/MarineLMS/FileCollectionContent/";
                    m_Logger.WarnFormat("AppSettings '{0}' is missing or not configured. Use default path '{1}'.", AppSettingsConstants.FileCollectionContentPath, path);
                }

                if (!m_FileAccess.DirectoryExists(path))
                {
                    // Create directory if it doesn't exist...
                    m_FileAccess.CreateDirectory(path);
                }
                return path;
            }
        }

        /// <summary>
        /// Temporary directory for uploaded files in the core service.
        /// If the directory doesn't exist - try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        public string TemporaryCoreSvcUploadPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings[AppSettingsConstants.TemporaryCoreSvcUploadPath];
                if (path == null)
                {
                    path = "C:/MarineLMS/TempFilesCoreSvc/";
                    m_Logger.WarnFormat("AppSettings '{0}' is missing or not configured. Use default path '{1}'.", AppSettingsConstants.TemporaryCoreSvcUploadPath, path);
                }

                if (!m_FileAccess.DirectoryExists(path))
                {
                    // Create directory if it doesn't exist...
                    m_FileAccess.CreateDirectory(path);
                }
                return path;
            }
        }

        /// <summary>
        /// Temporary directory for uploaded files in the core web.
        /// If the directory doesn't exist - try to create it.
        /// </summary>
        /// <exception cref="DirectoryCreationException">If the missing directory can't be created - see inner exception for more details.</exception>

        public string TemporaryCoreWebUploadPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings[AppSettingsConstants.TemporaryCoreWebUploadPath];
                if (path == null)
                {
                    path = "C:/MarineLMS/TempFilesCoreWeb/";
                    m_Logger.WarnFormat("AppSettings '{0}' is missing or not configured. Use default path '{1}'.", AppSettingsConstants.TemporaryCoreWebUploadPath, path);
                }

                if (!m_FileAccess.DirectoryExists(path))
                {
                    // Create directory if it doesn't exist...
                    m_FileAccess.CreateDirectory(path);
                }
                return path;
            }
        }

        public string CustomNotificationTemplatePath
        {
            get
            {
                var path = ConfigurationManager.AppSettings[AppSettingsConstants.CustomNotificationTemplatePath];
                if (path == null)
                {
                    path = "C:/MarineLMS/CustomNotificationTemplates/";
                    m_Logger.WarnFormat("AppSettings '{0}' is missing or not configured. Use default path '{1}'", AppSettingsConstants.CustomNotificationTemplatePath, path);
                }

                if (!m_FileAccess.DirectoryExists(path))
                {
                    // Create directory if it doesn't exist...
                    m_FileAccess.CreateDirectory(path);
                }
                return path;
            }
        }

        /// <summary>
        /// Return the app setting for the lifestyle used for IRequestContext (we need
        /// to use different lifestyles depending on whether we are in the front end
        /// code, back end code, or the test client.
        /// </summary>

        public AppSettingsConstants.CodeContext RequestContextStyle
        {
            get
            {
                var settingDefault = "client";
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.RequestContextStyleOpt];
                if (string.IsNullOrEmpty(setting))
                {
                    setting = settingDefault;
                    m_Logger.Error("No appSetting for " + AppSettingsConstants.RequestContextStyleOpt + " could be found. You must set a value unless calling from powerhshell");
                }
                //DBC.NonNull(setting, String.Format("App.config: {0} is a required setting", AppSettingsConstants.RequestContextStyleOpt));
                AppSettingsConstants.CodeContext contextStyle;
                var rc = Enum.TryParse<AppSettingsConstants.CodeContext>(setting, out contextStyle);
                DBC.Assert(rc, string.Format("App.config: {0} is a required setting (parse error)", AppSettingsConstants.RequestContextStyleOpt));
                return contextStyle;
            }
        }

        /// <summary>
        /// Indicates whether the permissions cache should be disabled. Note that this 
        /// works by preventing the creating of new cache entries, so to completely
        /// disable the cache you must also empty the PermissionsCacheEntries table in
        /// the DB.
        /// </summary>
        /// <remarks>
        /// NOTE: For debugging/development only.
        /// </remarks>

        public bool DisablePermissionsCache
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DisablePermissionsCache];
                if (setting == null) return false;
                bool disableCache = false;
                if (!bool.TryParse(setting, out disableCache))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.DisablePermissionsCache, setting);
                }
                return disableCache;
            }
        }

        public string ContentServiceSecureEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ContentServiceSecureEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings ContentServiceSecureEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string ContentServiceAdminEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ContentServiceAdminEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings ContentServiceAdminEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string UserServiceSecureEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.UserServiceSecureEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings UserServiceSecureEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string CPMSServiceSecureEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CPMSServiceSecureEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings CPMSServiceSecureEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string CPMSServiceAdminEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CPMSServiceAdminEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings CPMSServiceAdminEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string UserServiceAdminEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.UserServiceAdminEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings UserServiceAdminEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string RegistrationServiceSecureEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.RegistrationServiceSecureEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings RegistrationServiceSecureEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string RegistrationServiceAdminEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.RegistrationServiceAdminEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings RegistrationServiceAdminEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string AuditServiceAdminEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AuditServiceAdminEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings AuditServiceAdminEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string AuditServiceSecureEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AuditServiceSecureEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings AuditServiceSecureEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string AdminServiceEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AdminServiceEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings AdminServiceEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string CSMARTExtensionServiceEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CSMARTExtensionServiceEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings CSMARTExtensionServiceEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string CSMARTIntegrationEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CSMARTIntegrationEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings CSMARTIntegrationEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string CUKIntegrationEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CUKIntegrationEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings CUKIntegrationEndPoint needs to be set.");
                }
                return setting;
            }
        }

        public string SyncServiceAdminEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.SyncServiceAdminEndpoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings SyncServiceAdminEndpoint needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Endpoint for secure sync service.
        /// </summary>

        public string SyncServiceSecureEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.SyncServiceSecureEndpoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings SyncServiceSecureEndpoint needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Endpoint for external sync service (this is a secure end point with AuthorizationEndpointBehavior).
        /// </summary>
        /// <remarks>
        /// This is only used by an RTA-service to bind to the external sync service on main core service as a client proxy.
        /// </remarks>

        public string ExternalSyncServiceEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ExternalSyncServiceEndpoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings ExternalSyncServiceEndpoint needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Endpoint for validation service (this is a secure endpoint with AuthorizationEndpointBehavior).
        /// </summary>
        /// <remarks>
        /// This is only used by an RTA-service to bind to the validation service on main core service as a client proxy.
        /// </remarks>

        public string ValidationServiceEndpoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ValidationServiceEndpoint];
                if (string.IsNullOrEmpty(setting))
                {
                    m_Logger.WarnFormat("AppSettings ValidationServiceEndpoint needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Name of the extensions service, used in the extensions web project to start the service if it is not already running.
        /// </summary>

        public string ExtensionsServiceName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ExtensionsServiceName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings ExtensionsServiceName needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Name of the core service, used in the core web project to start the core service if it is not already running
        /// </summary>

        public string CoreServiceName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CoreServiceName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings CoreServiceName needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Name of the core notification service, used in the extension web project 
        /// </summary>

        public string NotificationServiceAdminEndPoint
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.NotificationServiceAdminEndPoint];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings NotificationServiceName needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// The maximum email error count to allow before setting the item as processed.
        /// </summary>
        /// <remarks>
        /// The email notification channel will try to re-send emails indefinitely. 
        /// Instead, it should keep an error count for each item and discard them if there are too many errors.
        /// </remarks>

        public int NotificationMaxEmailErrors
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.NotificationMaxEmailErrors];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings Core.NotificationMaxEmailErrors needs to be set.");
                }

                const int defaultNotificationMaxEmailErrors = 10;
                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = defaultNotificationMaxEmailErrors;
                    m_Logger.WarnFormat("AppSettings Core.NotificationMaxEmailErrors could not be parsed, the default value of " + defaultNotificationMaxEmailErrors + " was used.");
                }
                return result;
            }
        }

        /// <summary>
        /// The length of the interval for how often the notification service will run, in milliseconds.
        /// </summary>
        /// /// <remarks>
        /// The notification manager pushes the notification into the dependent channels. The channels themselves have
        /// an interval to run - CoreNotificationChannelInterval.
        /// </remarks>

        public int CoreNotificationInterval
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.NotificationInterval];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings Core.NotificationInterval needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 30000;
                    m_Logger.WarnFormat("AppSettings Core.NotificationInterval could not be parsed, the default value of 30000 (30 seconds) was used.");
                }
                return result;
            }
        }

        /// <summary>
        /// The length of the interval for how often the notification channels will run, in milliseconds.
        /// </summary>
        /// <remarks>
        /// The notification manager pushes the notification into the dependent channels. The channels themselves have
        /// an interval to run - CoreNotificationChannelInterval.
        /// </remarks>

        public int CoreNotificationChannelInterval
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.NotificationChannelInterval];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings Core.NotificationChannelInterval needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 30000;
                    m_Logger.WarnFormat("AppSettings Core.NotificationChannelInterval could not be parsed, the default value of 30000 (30 seconds) was used.");
                }
                return result;
            }
        }

        public int CoreDeferredActionInterval
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DeferredActionInterval];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings Core.DeferredActionInterval needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 180000;
                    m_Logger.WarnFormat("AppSettings Core.DeferredActionInterval could not be parsed, the default value of 180,000 (3min) was used.");
                }
                return result;
            }
        }

        public int CoreDeferredActionBatchSize
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DeferredActionBatchSize];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings Core.DeferredActionBatchSize needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 100;
                    m_Logger.WarnFormat("AppSettings Core.DeferredActionBatchSize could not be parsed, the default value of 100 (actions) was used.");
                }
                return result;
            }
        }

        public int ThreadStartDeferred
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ThreadStartDeferred];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings Core.ThreadStartDeferred needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 180000;
                    m_Logger.WarnFormat("AppSettings Core.ThreadStartDeferred could not be parsed, the default value of 180,000 (3min) was used.");
                }
                return result;
            }
        }

        /// <summary>
        /// Time interval in seconds to check whether we should sync an RTA (this is not the actual sync interval - that is determined via SiteInfo property).
        /// </summary>
        /// <remarks>
        /// This is the inverval for the background thread to run the loop/wakeup from sleep and do work.
        /// If it's not time to sync yet - go back to sleep again.
        /// Default value if no AppSetting found: 300s.
        /// </remarks>

        public int AutoSyncBackgroundThreadCheckInterval
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AutoSyncBackgroundThreadCheckInterval];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.BackgroundThreadCheckInterval] needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 300;
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.BackgroundThreadCheckInterval] could not be parsed, the default value is  {0}sec (this will be automatically calculated from the SiteInfo.AutoSyncInterval) was used.", result);
                }
                return result;
            }
        }

        /// <summary>
        /// Time (in seconds) to back off the network if 1 network failure occurs.
        /// </summary>
        /// <remarks>
        /// Default if no AppSetting found: 15s
        /// </remarks>

        public int AutoSyncNetworkBackOffDelay1
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AutoSyncNetworkBackOffDelay1];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.NetworkBackOffDelay1] needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 15;
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.NetworkBackOffDelay1] could not be parsed, the default value is {0}sec (this will be automatically calculated from the SiteInfo.AutoSyncInterval) was used.", result);
                }
                return result;
            }
        }

        /// <summary>
        ///  Time (in seconds) to back off the network if 2 network failures occur.
        /// </summary>
        /// <remarks>
        /// Default if no AppSetting found: 60s
        /// </remarks>

        public int AutoSyncNetworkBackOffDelay2
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AutoSyncNetworkBackOffDelay2];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.NetworkBackOffDelay2] needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 60;
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.NetworkBackOffDelay2] could not be parsed, the default value is {0}sec (this will be automatically calculated from the SiteInfo.AutoSyncInterval) was used.", result);
                }
                return result;
            }
        }

        /// <summary>
        /// Time (in seconds) to back off the network if 3 network failures occur .
        /// </summary>
        /// <remarks>
        /// Default if no AppSetting found: 300s
        /// </remarks>     

        public int AutoSyncNetworkBackOffDelay3
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AutoSyncNetworkBackOffDelay3];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.NetworkBackOffDelay3] needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 300;
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.NetworkBackOffDelay3] could not be parsed, the default value is {0}sec (this will be automatically calculated from the SiteInfo.AutoSyncInterval) was used.", result);
                }
                return result;
            }
        }

        /// <summary>
        /// Time (in seconds) to back off the network if 4+ network failures occur.
        /// </summary>
        /// <remarks>
        /// Default if no AppSetting found: 3600s
        /// </remarks>

        public int AutoSyncNetworkBackOffDelayMax
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AutoSyncNetworkBackOffDelayMax];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.NetworkBackOffDelayMax] needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 3600;
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.NetworkBackOffDelayMax] could not be parsed, the default value is {0}sec (this will be automatically calculated from the SiteInfo.AutoSyncInterval) was used.", result);
                }
                return result;
            }
        }

        /// <summary>
        /// Threshold of task and authentication errors to cancel the sync task (if (task + authentication errors) >= threshold - cancel the task).
        /// </summary>

        public int AutoSyncErrorThreshold 
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AutoSyncErrorThreshold];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.ErrorThreshold] needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 10;
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.ErrorThreshold] could not be parsed, the default value is {0} used.", result);
                }
                return result;
            } 
        }

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

        public int AutoSyncWaitForMainToRunRules
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AutoSyncWaitForMainToRunRules];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.WaitForMainToRunRules] needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 120;
                    m_Logger.WarnFormat("AppSettings [Sync.Automatic.WaitForMainToRunRules] could not be parsed, the default value is {0} used.", result);
                }
                return result;
            }
        }

        /// <summary>
        /// Chunk size to up/download a package piece in bytes.
        /// </summary>
        /// <remarks>
        /// This will be adjusted if the chunk size is larger than the max bandwidth specified for this RTA.
        /// That means that the MaxBandWidth will be the upper limit to up/download a package piece.
        /// </remarks>

        public int AutoSyncMaxTransferChunkSize
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AutoSyncMaxTransferChunkSize];
                // Don't warn because it turns out we call this all the time and it fills the log.

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 100000;
                    // Don't warn because it turns out we call this all the time and it fills the log.
                }
                return result;
            }
        }

        /// <summary>
        /// The timeout/expiry time (in seconds) of an authentication token.
        /// </summary>

        public int AuthenticationTokenTimeout
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.AuthenticationTokenTimeout];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings [Core.AuthenticationTokenTimeout] needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 300;
                    m_Logger.WarnFormat("AppSettings [Core.AuthenticationTokenTimeout] could not be parsed, the default value of 300 (5min) was used.");
                }
                return result;
            }
        }

        /// <summary>
        /// Sleep/delay (ms) before the warm up thread warms up the reports/group cache (it warms it up only once).
        /// Set to negative to prevent the warmup from running.
        /// </summary>

        public int WarmUpDelay
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.WarmUpDelay];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings Core.WarmUpDelay needs to be set.");
                }

                int result;
                if (!int.TryParse(setting, out result))
                {
                    result = 60000;
                    m_Logger.WarnFormat("AppSettings Core.WarmUpDelay could not be parsed, the default value of {0} ms was used.", result);
                }
                return result;
            }
        }

        /// <summary>
        /// Used to set the extra service startup time, Can be configured so the service doesn't timeout on startup.
        /// </summary>

        public int ExtraServiceStartupTime
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ExtraServiceStartupTime];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings Service.ExtraStartupTime needs to be set.");
                }

                int result = 0;
                if (!int.TryParse(setting, out result))
                {
                    m_Logger.WarnFormat("AppSettings Service.ExtraStartupTime could not be parsed, the default value of 0ms used.");
                }
                return result;
            }
        }

        /// <summary>
        /// If true, this indicates that the service will sleep for the give number seconds when it is started. This sleep
        /// occurs just after the IoC container has been set up, but before DB initialization or any other
        /// startup code. This is normally used only for debugging - the sleep gives one time to attach a debugger to
        /// the process.
        /// </summary>

        public int DelayedStart
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DelayedStart];

                if (string.IsNullOrWhiteSpace(setting)) { return 0; }

                int result = 0;
                if (!int.TryParse(setting, out result))
                {
                    m_Logger.WarnFormat("AppSettings Service.DelayedStartTime could not be parsed, the default value of 0ms used.");
                }
                return result;
            }
        }

        /// <summary>
        /// path to the export directory used for exporting content 
        /// </summary>

        public string ExportDirectory
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ExportDirectory];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings ExportDirectory needs to be set.");
                }
                return setting;
            }
        }

        public string SoftwareVersionsDirectory
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.SoftwareVersionsDirectory];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings SoftwareVersions Directory path needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Path to folder containing certificates
        /// </summary>

        public string CertificateTemplatePath
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CertificateTemplatePath];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings Certificate Template path needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Forces the RTA commit handler (which stops certain types from persisting on an RTA) off so it will
        /// not run, even on an RTA.
        /// </summary>

        public bool ForceRTACommitHandlerOff
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ForceRTACommitHandlerOff];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.WarnFormat("AppSettings 'Sync.ForceRTACommitHandlerOff' is not set.");
                }

                bool result;
                if (!bool.TryParse(setting, out result))
                {
                    m_Logger.WarnFormat("AppSettings Sync.ForceRTACommitHandlerOff could not be parsed, the default value 'False' is used.");
                }
                return result;
            }
        }

        /// <summary>
        /// Get the site Id .
        /// </summary>

        public Guid SiteId
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.SiteId];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' is missing.", AppSettingsConstants.SiteId);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.SiteId), "Guid", AppSettingsConstants.SiteId, null);
                }
                try
                {
                    Guid parsedSiteId = Guid.Parse(setting);
                    return parsedSiteId;
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (not a Guid). Reason: {1}", AppSettingsConstants.SiteId, ex);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' has a wrong value (not a Guid).", AppSettingsConstants.SiteId), "Guid", AppSettingsConstants.SiteId, null, ex);
                }
            }
        }

        /// <summary>
        /// Get the Main site Id.
        /// </summary>

        public Guid MainSiteId
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.MainSiteId];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' is missing.", AppSettingsConstants.MainSiteId);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.MainSiteId), "Guid", AppSettingsConstants.MainSiteId, null);
                }
                try
                {
                    Guid parsedSiteId = Guid.Parse(setting);
                    return parsedSiteId;
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (not a Guid). Reason: {1}", AppSettingsConstants.MainSiteId, ex);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' has a wrong value (not a Guid).", AppSettingsConstants.MainSiteId), "Guid", AppSettingsConstants.MainSiteId, null, ex);
                }
            }
        }

        /// <summary>
        /// Get flag whether this is an RTA- or a Main-installation - IsRTA is not actually in App.config/Web.config anymore - its derived from
        /// SiteId and MainSiteId 
        /// </summary>

        public bool IsRTA
        {
            get
            {
                return SiteId != MainSiteId;
            }
        }

        /// <summary>
        /// Enable or disable Encryption for RTA mode when right hand menu is not available to do so.
        /// This is also used whether the automatic sync should encrypt its sync packages.
        /// </summary>

        public bool EncryptSyncPackages
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.EncryptionSyncPackages];
                // true if setting does not exist, otherwise its what appsetting is set to
                var encryption = string.IsNullOrWhiteSpace(setting) ? true : bool.Parse(setting);
                return encryption;
            }
        }

        /// <summary>
        /// Main site name used for RTA syncing. Main uses to generate package with name. RTA uses to validate that package came from correct main.
        /// </summary>

        public string MainSiteName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.MainSiteName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings MainSiteName needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Enables the service to run as executable (must be set to <c>false</c>
        /// in order to run as installed service).
        /// </summary>
        /// <remarks>
        /// Return <c>false</c> if configuration is missing.
        /// </remarks>

        public bool RunServiceAsExecutable
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.RunServiceAsExecutable];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' is missing - use 'false' as default.", AppSettingsConstants.RunServiceAsExecutable);
                    return false;
                }
                try
                {
                    bool runAsExecutable = bool.Parse(setting);
                    return runAsExecutable;
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (not a bool) - use 'false' as default. Reason: {1}", AppSettingsConstants.RunServiceAsExecutable, ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the current base patching version.
        /// <remarks>
        /// Only once an RTA has acknowledged that they are on this version can they using patches for software updates  
        /// </remarks>         
        /// </summary>

        public int BasePatchingVersion
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.BasePatchingVersion];
                if (string.IsNullOrWhiteSpace(setting)) { return -1; }

                int result = 0;
                if (!int.TryParse(setting, out result))
                {
                    m_Logger.WarnFormat("AppSettings Sync.BasePatchingVersion could not be parsed, the default value of -1 was used.");
                }
                return result;
            }
        }

        /// <summary>
        /// Get the patch working directory
        /// <remarks>
        /// Only used on RTA's. This folder is used to patch the instller to the latest version 
        /// </remarks>         
        /// </summary>

        public string PatchWD
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.RTAPatchWD];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings RTAPatchWD needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Get the packages directory
        /// <remarks>
        /// Only used on deployed builds (Live,Demo,Test). This is where installer patches will be save to
        /// </remarks>         
        /// </summary>

        public string PatchesDir
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.PatchesDir];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings PatchesDir needs to be set.");
                }
                return setting;
            }
        }


        /// <summary>
        /// Install key used for installing on an RTA
        /// </summary>

        public string RTAInstallerName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.RTAInstallerName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings RTAInstallerName needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// RTA mode installer string - set to string "IsRTA"
        /// </summary>

        public string RTAInstallMode
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.RTAInstallMode];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings RTAInstallMode needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// Installer script name
        /// </summary>

        public string InstallerScriptName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.InstallerScriptName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings InstallerScriptName needs to be set.");
                }
                return setting;
            }
        }


        /// <summary>
        /// 64bit powershell.exe path.
        /// </summary>

        public string PowershellExePath
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.PowershellExePath];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings PowershellExePath needs to be set.");
                }
                return setting;
            }
        }

        /// <summary>
        /// True if password in config is encrypted
        /// </summary>

        public bool HasEncryptedDBPassword
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DBHasEncryptedPassword];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' is missing.", AppSettingsConstants.DBHasEncryptedPassword);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.DBHasEncryptedPassword), "bool", AppSettingsConstants.DBHasEncryptedPassword, null);
                }
                try
                {
                    bool hasEncryptedDbPass = bool.Parse(setting);
                    return hasEncryptedDbPass;
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.DBHasEncryptedPassword, ex);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.DBHasEncryptedPassword), "bool", AppSettingsConstants.DBHasEncryptedPassword, null, ex);
                }
            }
        }

        /// <summary>
        /// Name of Database for DB connection string
        /// </summary>

        public string DBName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DBName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings {0} needs to be set.", AppSettingsConstants.DBName);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.DBName), "string", AppSettingsConstants.DBName, null);
                }
                return setting;
            }
        }

        /// <summary>
        /// Flag whether to use Integrated Security for the DB connection (defaults to false)
        /// </summary>

        public bool DBIntegratedSecurity
        {
            get
            {
                var result = false;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DBIntegratedSecurity];

                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    if (bool.TryParse(setting, out result) == false)
                    {
                        m_Logger.WarnFormat("AppSettings {0} is Not a Valid Boolean ({1})", AppSettingsConstants.DBIntegratedSecurity, setting);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Server namne used in DB connection string
        /// </summary>

        public string DBServerName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DBServerName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings {0} needs to be set.", AppSettingsConstants.DBServerName);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.DBServerName), "string", AppSettingsConstants.DBServerName, null);
                }
                return setting;
            }
        }

        /// <summary>
        /// Users password for DB connection string, could be encrypted (HasEncryptedDBPassword) 
        /// </summary>

        public string DBUserPassword
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DBUserPassword];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings {0} needs to be set.", AppSettingsConstants.DBUserPassword);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.DBUserPassword), "string", AppSettingsConstants.DBUserPassword, null);
                }
                return setting;
            }
        }


        /// <summary>
        /// Name user for DB connection string
        /// </summary>>

        public string DBUserName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DBUserName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings {0} needs to be set.", AppSettingsConstants.DBUserName);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.DBUserName), "string", AppSettingsConstants.DBUserName, null);
                }
                return setting;
            }
        }

        /// <summary>
        /// Database command timeout in seconds (300 seconds, or 5 minutes, by default).
        /// </summary>

        public int DBCommandTimeout
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DBCommandTimeout];
                int? timeout = setting.ParseToInt();

                if (timeout.HasValue && timeout.Value > 0) { return timeout.Value; }

                m_Logger.WarnFormat("AppSettings {0} needs to be set to a positive number of seconds.", AppSettingsConstants.DBCommandTimeout);
                return 300;
            }
        }

        /// <summary>
        /// User name service should be running as
        /// </summary>

        public string ServiceLogOnUserName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ServiceLogOnUserName];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings {0} needs to be set.", AppSettingsConstants.ServiceLogOnUserName);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.ServiceLogOnUserName), "string", AppSettingsConstants.ServiceLogOnUserName, null);
                }
                return setting;

            }
        }

        /// <summary>
        /// Domain name service should be running as
        /// </summary>

        public string ServiceLoggOnDomainName
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ServiceLogOnUserDomain];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings {0} needs to be set.", AppSettingsConstants.ServiceLogOnUserDomain);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.ServiceLogOnUserDomain), "string", AppSettingsConstants.ServiceLogOnUserDomain, null);
                }
                return setting;

            }
        }

        /// <summary>
        /// True if service should be running as logged on user ServiceLoggOnDomainName\ServiceLogOnUserName.
        /// </summary>

        public bool ServiceRunningAsLoggedOnUser
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.ServiceRunAsLoggedOnUser];

                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' is missing.", AppSettingsConstants.ServiceRunAsLoggedOnUser);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.ServiceRunAsLoggedOnUser), "bool", AppSettingsConstants.ServiceRunAsLoggedOnUser, null);
                }
                try
                {
                    bool hasEncryptedDbPass = bool.Parse(setting);
                    return hasEncryptedDbPass;
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.ServiceRunAsLoggedOnUser, ex);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.ServiceRunAsLoggedOnUser), "bool", AppSettingsConstants.ServiceRunAsLoggedOnUser, null, ex);
                }
            }
        }

        /// <summary>
        /// Path to log4net dir, used to clean out old logs
        /// </summary>

        public string CleanLog4NetDirPath
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CleanLog4NetDirPath];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings {0} needs to be set.", AppSettingsConstants.CleanLog4NetDirPath);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.CleanLog4NetDirPath), "string", AppSettingsConstants.CleanLog4NetDirPath, null);
                }
                return setting;
            }
        }

        /// <summary>
        /// Path to IIS logs dir, used to clean out old logs
        /// </summary>

        public string CleanIISLogsDirPath
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.CleanIISLogsDirPath];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    m_Logger.ErrorFormat("AppSettings {0} needs to be set.", AppSettingsConstants.CleanIISLogsDirPath);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.CleanIISLogsDirPath), "string", AppSettingsConstants.CleanIISLogsDirPath, null);
                }
                return setting;
            }
        }

        /// <summary>
        /// Configure the Root log level at runtime
        /// </summary>

        public string RootLogLevel
        {
            get
            {
                // an empty string represents a log level that will not be overridden
                var result = string.Empty;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.RootLogLevel];
                
                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    result = setting;
                }

                return result;
            }
        }

        /// <summary>
        /// Configure the Trace log level at runtime
        /// </summary>

        public string TraceLogLevel
        {
            get
            {
                // an empty string represents a log level that will not be overridden
                var result = string.Empty;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.TraceLogLevel];

                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    result = setting;
                }

                return result;
            }
        }

        /// <summary>
        /// Configure the Trace log file output path
        /// </summary>

        public string TraceLogPath
        {
            get
            {
                // an empty string represents using the log4netConfig.xml configured path
                var result = string.Empty;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.TraceLogPath];

                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    result = setting;
                }

                return result;
            }
        }

        /// <summary>
        /// Configure the Event log level at runtime
        /// </summary>

        public string EventLogLevel
        {
            get
            {
                // an empty string represents a log level that will not be overridden
                var result = string.Empty;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.EventLogLevel];

                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    result = setting;
                }

                return result;
            }
        }

        /// <summary>
        /// Configure the Email log level at runtime
        /// </summary>

        public string EmailLogLevel
        {
            get
            {
                // an empty string represents a log level that will not be overridden
                var result = string.Empty;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.EmailLogLevel];

                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    result = setting;
                }

                return result;
            }
        }

        /// <summary>
        /// Configure the Debugger log level at runtime
        /// </summary>

        public string DebuggerLogLevel
        {
            get
            {
                // an empty string represents a log level that will not be overridden
                var result = string.Empty;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.DebuggerLogLevel];

                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    result = setting;
                }

                return result;
            }
        }

        /// <summary>
        /// Configure the Stats log level at runtime
        /// </summary>

        public string StatsLogLevel
        {
            get
            {
                // an empty string represents a log level that will not be overridden
                var result = string.Empty;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.StatsLogLevel];

                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    result = setting;
                }

                return result;
            }
        }

        /// <summary>
        /// Configure the Stats log file output path
        /// </summary>

        public string StatsLogPath
        {
            get
            {
                // an empty string represents using the log4netConfig.xml configured path
                var result = string.Empty;

                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.StatsLogPath];

                if (string.IsNullOrWhiteSpace(setting) == false)
                {
                    result = setting;
                }

                return result;
            }
        }

        public string EmailOverride
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.EmailOverride];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    return null;
                }
                else
                {
                    return setting;
                }
            }

        }

        public int SetSubjectLength
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingsConstants.SetSubjectLength];
                if (string.IsNullOrWhiteSpace(setting)) { return 78; }

                int result = 78;
                if (!int.TryParse(setting, out result))
                {
                    m_Logger.WarnFormat("AppSettings Core.SetSubjectLength could not be parsed, the default value of -1 was used.");
                }
                return result;
            }
        }



        /// <summary>
        /// Suppressed IPs to add IP filtering for logging
        /// </summary>

        public string LoggingSuppressedIPs
        {
            get
            {
                var sIp = ConfigurationManager.AppSettings[AppSettingsConstants.LoggingSuppressedIP];
                return sIp;
            }
        }


        /// <summary>
        /// Boolean parameter to show if we would like to generate sync package with/without software
        /// </summary>

        public bool SyncNoSoftware
        {
            get
            {
                var sns = ConfigurationManager.AppSettings[AppSettingsConstants.SyncNoSoftware];

                if (string.IsNullOrWhiteSpace(sns))
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' is missing.", AppSettingsConstants.SyncNoSoftware);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' is missing.", AppSettingsConstants.SyncNoSoftware), "bool", AppSettingsConstants.SyncNoSoftware, null);
                }
                try
                {
                    bool syncNoSoftware = bool.Parse(sns);
                    return syncNoSoftware;
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.SyncNoSoftware, ex);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.SyncNoSoftware), "bool", AppSettingsConstants.SyncNoSoftware, null, ex);
                }
            }

        }

        public bool AllowAllUsersAtRoot
        {
            get
            {
                var allowUserAtRoor = ConfigurationManager.AppSettings[AppSettingsConstants.AllowAllUsersAtRoot];
                if (string.IsNullOrWhiteSpace(allowUserAtRoor))
                {
                    return false;
                }
                try
                {
                    bool allowAllUserAtRoor = bool.Parse(allowUserAtRoor);
                    return allowAllUserAtRoor;
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.AllowAllUsersAtRoot, ex);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.AllowAllUsersAtRoot), "bool", AppSettingsConstants.AllowAllUsersAtRoot, null, ex);
                }
            }
        }


        /// <summary>
        /// The timeout after which the UI gives up on applying a sync package, in ms.
        /// </summary>

        public int SyncPackageUITimeOut
        {
            get
            {
                var ms = ConfigurationManager.AppSettings[AppSettingsConstants.SyncPackageUITimeOut];
                if (ms == null) return 3600000;
                int milliseconds;

                if (!int.TryParse(ms, out milliseconds))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}) = {1}.", AppSettingsConstants.SyncPackageUITimeOut, ms);
                    milliseconds = 3600000;
                    m_Logger.WarnFormat("AppSettings: using default timeout value '{0}' for '{1}.'", milliseconds, AppSettingsConstants.SyncPackageUITimeOut);
                }
                return milliseconds;
            }
        }

        /// <summary>
        /// The maximum number of certificates that user can download in one try
        /// </summary>

        public int CertificatesMaxDownload
        {
            get
            {
                const int defaultMax = 100;
                var cc = ConfigurationManager.AppSettings[AppSettingsConstants.CertificatesMaxDownload];
                if (cc == null) return defaultMax;
                int certificatesCount;

                if (!int.TryParse(cc,out certificatesCount))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}(={1}.", AppSettingsConstants.CertificatesMaxDownload, cc);
                    certificatesCount = defaultMax;
                    m_Logger.WarnFormat("AppSettings: using default value '{0}' for '{1}'.", certificatesCount, AppSettingsConstants.CertificatesMaxDownload);
                }
                return certificatesCount;
            }
        }

        /// <summary>
        /// If MonotonicClock is MaxSkipForwardSeconds older than the current time, log and hault.
        /// </summary>
        public int MaxSkipForward
        {
            get
            {
                const int defaultMax = 72 * 60 * 60 * 1000;
                var ms = ConfigurationManager.AppSettings[AppSettingsConstants.MaxSkipForward];
                if (ms == null) return defaultMax;
                int maxSkipForward;

                if (!int.TryParse(ms, out maxSkipForward))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}(={1}.", AppSettingsConstants.MaxSkipForward, ms);
                    maxSkipForward = defaultMax;
                    m_Logger.WarnFormat("AppSettings: using default value '{0}' for '{1}'.", maxSkipForward, AppSettingsConstants.MaxSkipForward);
                }
                return maxSkipForward;
            }
        }


        /// <summary>
        /// MonotonicClockManager logs an error when the MonotonicClock WarnBackwardSeconds is older than the current time.
        /// </summary>
        public int WarnBackward
        {
            get
            {
                const int defaultMax = 1 * 60 * 60 * 1000;
                var ms = ConfigurationManager.AppSettings[AppSettingsConstants.WarnBackward];
                if (ms == null) return defaultMax;
                int warnBackward;

                if (!int.TryParse(ms, out warnBackward))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}(={1}.", AppSettingsConstants.WarnBackward, ms);
                    warnBackward = defaultMax;
                    m_Logger.WarnFormat("AppSettings: using default value '{0}' for '{1}'.", warnBackward, AppSettingsConstants.WarnBackward);
                }
                return warnBackward;
            }
        }

        /// <summary>
        /// Every ClockMaintenanceInterval milliseconds the ClockTimeStamp is updated by MonotonicClockManager.
        /// </summary>
        public int ClockMaintenanceInterval
        {
            get
            {
                const int defaultMax = 1 * 60 * 1000;
                var ms = ConfigurationManager.AppSettings[AppSettingsConstants.ClockMaintenanceInterval];
                if (ms == null) return defaultMax;
                int clockMaintenanceInterval;

                if (!int.TryParse(ms, out clockMaintenanceInterval))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}(={1}.", AppSettingsConstants.ClockMaintenanceInterval, ms);
                    clockMaintenanceInterval = defaultMax;
                    m_Logger.WarnFormat("AppSettings: using default value '{0}' for '{1}'.", clockMaintenanceInterval, AppSettingsConstants.ClockMaintenanceInterval);
                }
                return clockMaintenanceInterval;
            }
        }

        /// <summary>
        /// Records in ClockTimeStamp that are older than this value will be deleted by MonotonicClockManager.
        /// </summary>
        public int MaxClockTimeStamp
        {
            get
            {
                const int defaultMax = 10;
                var days = ConfigurationManager.AppSettings[AppSettingsConstants.MaxClockTimeStamp];
                if (days == null) return defaultMax;
                int maxClockTimeStamp;

                if (!int.TryParse(days, out maxClockTimeStamp))
                {
                    m_Logger.WarnFormat("AppSettings parse error ({0}(={1}.", AppSettingsConstants.MaxClockTimeStamp, days);
                    maxClockTimeStamp = defaultMax;
                    m_Logger.WarnFormat("AppSettings: using default value '{0}' for '{1}'.", maxClockTimeStamp, AppSettingsConstants.MaxClockTimeStamp);
                }
                return maxClockTimeStamp;
            }
        }

        /// <summary>
        /// Used just by devs to make things easier to manage.
        /// </summary>
        public bool ClockCheckOverride
        {
            get
            {
                var clockCheckOverride = ConfigurationManager.AppSettings[AppSettingsConstants.ClockCheckOverride];
                if (string.IsNullOrWhiteSpace(clockCheckOverride))
                {
                    return false;
                }
                try
                {
                    return bool.Parse(clockCheckOverride);
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.ClockCheckOverride, ex);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.ClockCheckOverride), "bool", AppSettingsConstants.ClockCheckOverride, null, ex);
                }
            }
        }

        /// <summary>
        /// Used on an RTA that has been turned off too long or otherwise has clock issues.
        /// </summary>
        public bool ForceClock
        {
            get
            {
                var forceClock = ConfigurationManager.AppSettings[AppSettingsConstants.ForceClock];
                if (string.IsNullOrWhiteSpace(forceClock))
                {
                    return false;
                }
                try
                {
                    return bool.Parse(forceClock);
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.ForceClock, ex);
                    throw new ConfigurationParameterException(string.Format("AppSetting '{0}' has a wrong value (could not parse as bool).", AppSettingsConstants.ForceClock), "bool", AppSettingsConstants.ForceClock, null, ex);
                }
            }
        }

        /// <summary>
        /// IoC object creation entry point.
        /// </summary>

        public AppSettings(IKernel k, ILogger l, IFileAccess fa)
        {
            m_Logger = l;
            m_FileAccess = fa;
            m_Kernel = k;
        }

        /// <summary>
        /// Allows this component to be Resolved in a using block and properly released
        /// when the using block completes.
        /// </summary>

        public void Dispose()
        {
            if (m_Disposing) { return; }
            m_Disposing = true;
            m_Kernel.ReleaseComponent(this);
        }



        // ------------------------------- private state ------------------------------------------

        private readonly IKernel m_Kernel = null;
        private readonly ILogger m_Logger = null;
        private readonly IFileAccess m_FileAccess = null;
        private bool m_Disposing = false;
    }
}
