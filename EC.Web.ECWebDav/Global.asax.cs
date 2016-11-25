using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Castle.Core.Logging;
using Castle.Windsor;
using EC.Common.Base;
using EC.Core.Common;
using EC.Common.Interfaces;
using EC.Service.Interfaces;

namespace EC.Web.ECWebDav
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            WindowsEventLog.AddEvent("WebDav Application_Start()", 9999);
            IoCSetup.ForceLoad(typeof(EC.Core.Common.ForceLoad));
            BootstrapContainer();
            SetupServices();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Log the Error
            Exception ex = Server.GetLastError();
            ILogger logger = null;

            try
            {
                if (ex != null)
                {
                    string errMsg = string.Format("Unhandled top-level error: {0}", ex.ToString());
                    string innerErrMsg = ex.InnerException != null ? string.Format("Unhandled top-level error (inner): {0}", ex.InnerException.Message) : "No inner exception";

                    // Event logs...
                    WindowsEventLog.AddEvent(string.Format("WebDav Global.Application_Error(): {0}", errMsg), 9999);
                    if (ex.InnerException != null) { WindowsEventLog.AddEvent(string.Format("WebDav Global.Application_Error(): {0}", innerErrMsg), 9999); }

                    // File log...
                    logger = container.Resolve<ILogger>();
                    logger.Error(errMsg);
                    if (ex.InnerException != null) { logger.ErrorFormat("WebDav Global.Application_Error(): {0}", innerErrMsg); }
                }
                else
                {
                    WindowsEventLog.AddEvent("WebDav Global.Application_Error(): No exception found", 9999);
                }

            }
            finally { if (logger != null) { container.Release(logger); } }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            WindowsEventLog.AddEvent("WebDav Application_End()", 9999);
        }

        /// <summary>
        /// Initiates IOC
        /// </summary>
        /// <remarks>
        /// IoCSetupForceLoad() deals with the problem that the app is so loosely
        /// coupled that it won't bother loading some assemblies because it doesn't
        /// see direct references to this.
        /// </remarks>

        private void BootstrapContainer()
        {
            try
            {
                container = IoCSetup.Run("EC WebDav Application Starting", "EC-Web-Dav");
            }
            catch (Exception ex)
            {
                var msg = String.Format("EC WebDav Application Startup (BootstrapContainer) - Unexpected Exception [{0}]", ex.Message.ToString());
                WindowsEventLog.AddEvent(msg, WindowsEventLog.FatalError);
            }
        }

        /// <summary>
        /// Register WCF client proxies for the various MLS services.
        /// </summary>
        /// <remarks>
        /// NOTE: The binding parameters used here MUST agree with those used on
        /// the server side, otherwise you will get errors when attempting to make
        /// requests.
        /// </remarks>

        private void SetupServices()
        {
            using (IWCFHelper helper = container.Resolve<IWCFHelper>())
            using (IAppSettings settings = container.Resolve<IAppSettings>())
            {
                var sendTimeout = new TimeSpan(0, settings.WCFSendTimeoutInMinutes, 0);
                var receiveTimeout = new TimeSpan(0, settings.WCFReceiveTimeoutInMinutes, 0);

                var maxMsgSize = settings.MaxReceivedMessageSize;
                helper.RegisterWCFClientProxy<IContentServiceSecure>(settings.ContentServiceSecureEndPoint, true, maxMsgSize, sendTimeout, receiveTimeout);
                helper.RegisterWCFClientProxy<IContentServiceAdmin>(settings.ContentServiceAdminEndPoint, true, maxMsgSize, sendTimeout, receiveTimeout);
                helper.RegisterWCFClientProxy<IUserServiceSecure>(settings.UserServiceSecureEndPoint, true, maxMsgSize, sendTimeout, receiveTimeout);
                helper.RegisterWCFClientProxy<IAdminService>(settings.AdminServiceEndPoint, false, maxMsgSize, sendTimeout, receiveTimeout);
            }
        }

        // ------------------------------------ Local State ---------------------------------------

        public static IWindsorContainer container = null;
    }
}