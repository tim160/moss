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
    /// Exception if something is not initialized.
    /// </summary>

    public class NotInitializedException : FaultableException<NotInitializedFault>
    {
        public override NotInitializedFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NotInitializedFault(Message, reqPath, userInfo);
            return f;
        }

        public NotInitializedException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }

    [DataContract]
    public class NotInitializedFault : BasicFault
    {
        public NotInitializedFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
