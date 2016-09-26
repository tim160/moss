using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    public interface IServiceValidator
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
        
        void VerifyServiceRunningAsUser(string username,string domainname);

    }
}
