using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.MicroKernel;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    /// <summary>
    /// Utility class for interacting with Windows Services.
    /// </summary>
    
    [SingletonType]
    [RegisterAsType(typeof(IServiceProcessHelper))]

    public class ServiceProcessHelper : IServiceProcessHelper
    {

        //See for impersonation
        //http://stackoverflow.com/questions/916714/how-to-run-c-sharp-application-with-admin-creds

        /// <summary>
        /// Start the named service if it is not running already.
        /// <para>
        /// NOTE: Swallows (but logs) all exceptions.
        /// </para>
        /// </summary>
        /// <param name="serviceName">display name of the service</param>
        
        public void CheckService(string serviceName)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceName)) return;

                System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController();
                sc.ServiceName = serviceName;
                sc.MachineName = ".";   // for local.  use windows machine name here for a remote service

                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        // nothing to do, service is running.
                        break;
                    case ServiceControllerStatus.Stopped:
                        Logger.DebugFormat("{0} service is stopped. Attempting to start.", serviceName);
                        StartService(sc, serviceName);
                        break;
                    case ServiceControllerStatus.Paused:
                        Logger.DebugFormat("{0} service is paused.", serviceName);
                        break;
                    case ServiceControllerStatus.StopPending:
                        Logger.DebugFormat("{0} service is currently stop pending.", serviceName);
                        break;
                    case ServiceControllerStatus.StartPending:
                        Logger.DebugFormat("{0} service is currently start pending.", serviceName);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("CheckService - Unexpected exception", ex);
            }
        }

        /// <summary>
        /// Attempt to start the named service. NOTE: Catches, logs, and swallows exceptions.
        /// </summary>

        private void StartService(System.ServiceProcess.ServiceController sc,string serviceName)
        {
            try
            {
                sc.Start();

                TimeSpan ts = new TimeSpan(0, 0, 0, 3, 0); // 3 sec

                sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, ts);

                if (sc.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    Logger.Debug("{0} service started successfully.");
                }
                else
                {
                    Logger.Error("Failed to start {0} service.");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("StartService - Unexpected exception", ex);
            }
        }

        /// <summary>
        /// IoC object creation entry point.
        /// </summary>

        public ServiceProcessHelper(IKernel k, ILogger l)
        {
            Kernel = k;
            Logger = l;
        }

        protected IKernel Kernel = null;
        protected ILogger Logger = null;
    }
}
