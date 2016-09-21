using Castle.Core.Logging;
using Castle.MicroKernel;
using EC.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Errors.CommonExceptions;

namespace EC.Core.Common
{
    /// <summary>
    /// Return settings read from app.config (or appropriate defaults)
    /// </summary>

    [SingletonType]
    [RegisterAsType(typeof(IServiceValidator))]

    public class ServiceValidator : IServiceValidator
    {
        /// <summary>
        /// Used to verify if service is running as current user.
        /// <remarks>
        /// In Release: Will log an error if service is not runing as passed in user
        /// In Debug: Will DBC assert if service is not runing as passed in user
        /// </remarks>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="domainname"></param>

        public void VerifyServiceRunningAsUser(string username, string domainname)
        {
            System.Threading.Thread.Sleep(10000);
            var userWithDomain = BuildUserWithDomainName(username, domainname);
            var user =  System.Security.Principal.WindowsIdentity.GetCurrent();
            if (!CompareUserWithDomainToIdentity(userWithDomain,user))
            {
                var message = string.Format("Service is not running as correct user {0}. Service is currently runnign as {1}", userWithDomain, user.Name);
                logger.Error(message);
#if DEBUG
                DBC.Assert(false,message);
#endif
            }
            else
            {
                logger.InfoFormat("Service is running as correct user {0}",userWithDomain);
            }
        }


        /// <summary>
        /// Build domain\username string
        /// </summary>
        /// <param name="username"></param>
        /// <param name="domainname"></param>
        /// <returns></returns>
        private string BuildUserWithDomainName(string username, string domainname)
        {
            return string.Format(@"{0}\{1}", domainname, username);
        }

        /// <summary>
        /// compare domain\username to passed in identity 
        /// </summary>
        /// <param name="userWithDomain"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool CompareUserWithDomainToIdentity(string userWithDomain, System.Security.Principal.WindowsIdentity user)
        {
            if (userWithDomain.ToLower() != user.Name.ToLower())
            {
                return false;
            }

            return true;
        }

         /// <summary>
        /// IoC object creation entry point.
        /// </summary>

        public ServiceValidator(IKernel k, ILogger l)
        {
            logger = l;
            Kernel = k;
        }

        /// <summary>
        /// Allows this component to be Resolved in a using block and properly released
        /// when the using block completes.
        /// </summary>

        public void Dispose()
        {
            if (Disposing) { return; }
            Disposing = true;
            Kernel.ReleaseComponent(this);
        }

        // ------------------------------- private state ------------------------------------------

        private IKernel Kernel = null;
        private ILogger logger = null;
        private bool Disposing = false;
    }
}
