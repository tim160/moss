using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Castle.Core.Logging;
using Castle.Windsor;
using EC.Constants;
using EC.Common.Errors.CommonExceptions;
using EC.Common.Interfaces;
using EC.Service.Interfaces;

namespace EC.Web.ECWebDav
{
    public class ECBasic : IDisposable
    {
        /// <summary>
        /// Authenticate user against MarineLMS user service.
        /// </summary>
        /// <param name="userName">This can only be username/email.  OrgId login will not work with webdav.</param>
        /// <param name="password"></param>
        /// <returns></returns>

        protected bool AuthenticateUser(string userName, string password)
        {
            IUserServiceSecure userServiceSecure = null;
            IManageRequestContexts rContext = null;
            IAdminService adminService = null;

            try
            {
                //We use an empty orgPath because only username and email login via webdav are supported.
                string currentUrl = HttpContext.Current.Request.Url.Host;
                adminService = this.GetContainer().Resolve<IAdminService>();

                var customDNS = adminService.GetAllCustomDNS().FirstOrDefault(z => z.IncomingDNS.ToLower().Equals(currentUrl.ToLower()));
                if (customDNS != null && customDNS.OrgPath == null) throw new Exception("Org path is null");

                var orgPath = "";
                if (customDNS != null)
                {
                    orgPath = customDNS.OrgPath;
                }

                userServiceSecure = this.GetContainer().Resolve<IUserServiceSecure>();
                var isUserValid = userServiceSecure.ValidateUserWithLoginId(userName, password, orgPath);

                rContext = this.GetContainer().Resolve<IManageRequestContexts>();
                rContext.GetContext().CurrentUserId = null; // Reset user request context.
                if (isUserValid)
                {
                    // Set user to request context to an actual Id...
                    var user = userServiceSecure.GetUserByLoginId(userName, null, true, orgPath);
                    rContext.GetContext().CurrentUserId = user.Id;
                    rContext.GetContext().UserTimeZone = user.TimeZone;
                    rContext.GetContext().UserLanguage = user.UserLanguage;
                }

                return isUserValid;
            }
            catch (FaultException<NotFoundFault>)
            {
                // User can't be found... this occurs quite frequently, so we won't log it
                // this.LogInfo(string.Format("{0}: User not found: '{1}'", this.logPrefix, nff.Detail.Name));
                return false;
            }
            catch (Exception ex)
            {
                this.LogError(string.Format("{0}: User authentication failed. Reason: {1}", this.LogPrefix, ex.ToString()));
                return false;
            }
            finally
            {
                // Release user service instance...
                if (userServiceSecure != null)
                {
                    try
                    {
                        this.GetContainer().Release(userServiceSecure);
                    }
                    catch (Exception ex)
                    {
                        this.LogError(string.Format("{0}AuthenticateUser: Error releasing the user service: {1}", this.LogPrefix, ex.ToString()));
                    }
                }
                if (adminService != null)
                {
                    try
                    {
                        this.GetContainer().Release(adminService);
                    }
                    catch (Exception ex)
                    {
                        this.LogError(string.Format("{0}AuthenticateUser: Error releasing the admin service: {1}", this.LogPrefix, ex.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Check capability CanManageFiles for the current user and the path.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns></returns>

        protected bool CheckCapabilityForPathAndUser(string path)
        {
            IContentServiceSecure contentService = null;
            try
            {
                contentService = this.GetContainer().Resolve<IContentServiceSecure>();

                if (string.IsNullOrWhiteSpace(path)) { return false; }
                return contentService.CheckCapabilityByPath(path, CapabilityConstants.CAN_MANAGE_FILES);
            }
            catch (Exception ex)
            {
                this.LogError(string.Format("{0}: Path validation for the user failed. Reason: {1}", this.LogPrefix, ex.ToString()));
                return false;
            }
            finally
            {
                if (contentService != null)
                {
                    try
                    {
                        this.GetContainer().Release(contentService);
                    }
                    catch (Exception ex)
                    {
                        this.LogError(string.Format("{0}ValidatePathForUser: Error releasing the content service: {1}", this.LogPrefix, ex.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Get path to validate against from the request.
        /// </summary>
        /// <param name="request">http request</param>
        /// <returns></returns>

        protected string GetPathFromRequest(HttpRequest request)
        {
            string path = null;
            string tempPath = request.AppRelativeCurrentExecutionFilePath.Replace("~/", string.Empty);

            if (tempPath.Length == 0)
            {
                return null;
            }

            int slashPosition = tempPath.IndexOf('/');
            if (slashPosition == -1)
            {
                // This should be the path...
                path = tempPath;
            }
            else
            {
                path = tempPath.Substring(0, slashPosition);
            }

            if (path != null)
            {
                var webdavHelper = this.GetContainer().Resolve<IWebDavHelper>();
                try
                {
                    // Decode virtual path...
                    return webdavHelper.GetPathFromVirtualDirectoryName(path);
                }
                finally
                {
                    if (webdavHelper != null) { this.GetContainer().Release(webdavHelper); }
                }
            }

            return null;
        }

        /// <summary>
        /// Dispose kernel-resolved objects.
        /// </summary>

        public void Dispose()
        {
            if (Logger != null)
            {
                this.GetContainer().Release(Logger);
                Logger = null;
            }
        }


        /// <summary>
        /// Log an information level message.
        /// </summary>
        /// <param name="msg"></param>

        protected void LogInfo(string msg)
        {
            try
            {
                var log = this.GetLogger();
                if (log != null)
                {
                    log.Info(msg);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine(string.Format("Can't log info to logging file. Message: {0}", msg));
            }
        }

        /// <summary>
        /// Log an debug level message.
        /// </summary>
        /// <param name="msg"></param>

        protected void LogDebug(string msg)
        {
            try
            {
                var log = this.GetLogger();
                if (log != null)
                {
                    log.Debug(msg);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine(string.Format("Can't log debug info to logging file. Message: {0}", msg));
            }
        }

        /// <summary>
        /// Log an warn level message.
        /// </summary>
        /// <param name="msg"></param>

        protected void LogWarn(string msg)
        {
            try
            {
                var log = this.GetLogger();
                if (log != null)
                {
                    log.Warn(msg);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine(string.Format("Can't log warn info to logging file. Message: {0}", msg));
            }
        }

        /// <summary>
        /// Log an error level message (to logger and debug).
        /// </summary>
        /// <param name="msg"></param>

        protected void LogError(string msg)
        {
            Debug.WriteLine(msg);
            try
            {
                var log = this.GetLogger();
                if (log != null)
                {
                    log.Error(msg);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine(string.Format("Can't log error to logging file. Message: {0}", msg));
            }
        }

        /// <summary>
        /// Get Windsor Castle container instance from global application.
        /// </summary>
        /// <returns>Return Windsor Castle container</returns>

        protected IWindsorContainer GetContainer()
        {
            return Global.container;
        }

        /// <summary>
        /// Get logger instance.
        /// </summary>
        /// <returns></returns>

        protected ILogger GetLogger()
        {
            if (Logger == null)
            {
                Logger = this.GetContainer().Resolve<ILogger>();
            }
            return Logger;
        }

        public ECBasic(string logPrefix)
        {
            this.LogPrefix = logPrefix;
        }


        protected string LogPrefix { get; private set; }

        private static ILogger Logger { get; set; }

    }
}
