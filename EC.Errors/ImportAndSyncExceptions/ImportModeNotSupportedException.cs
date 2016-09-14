using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Exception used to indicate that an import mode is not supported.
    /// </summary>

    public class ImportModeNotSupportedException : FaultableException<ImportModeNotSupportedFault>
    {
        public override ImportModeNotSupportedFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new ImportModeNotSupportedFault(Message, path, userInfo);
            return f;
        }

        public ImportModeNotSupportedException(string msg, Exception innerException = null) : base(msg, innerException)
        {
        }
    }

    /// <summary>
    /// Exception used to indicate that an import mode is not supported.
    /// </summary>

    [DataContract]
    public class ImportModeNotSupportedFault : BasicFault
    {
        public ImportModeNotSupportedFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
