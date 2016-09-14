using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// Thrown when the system is not configured correctly (DB schema issues,
    /// base configuration not loaded, etc).
    /// </summary>

    public class ConfigurationException : FaultableException<ConfigurationFault>
    {
        public override ConfigurationFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new ConfigurationFault(Message, path, userInfo);
            return f;
        }

        public ConfigurationException(string msg, Exception innerException = null) : base(msg, innerException)
        {
        }
    }

    /// <summary>
    /// Thrown when there is no root content item. Normally caused by failure to 
    /// load the base configuration.
    /// </summary>

    public class NoRootException : FaultableException<ConfigurationFault>
    {
        public override ConfigurationFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var result = new ConfigurationFault(Message, path, userInfo);
            return result;
        }

        public NoRootException() : base("No root page found in DB")
        {
        }
    }

    /// <summary>
    /// Thrown when the system appears to be configured incorrectly.
    /// </summary>

    [DataContract]
    public class ConfigurationFault : BasicFault
    {
        public ConfigurationFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }
    }
}
