using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.PatchingExceptions
{
    /// <summary>
    /// If user is not allowed to take an exam because they have already taken it.
    /// </summary>

    public class InstallerPatchValidationException : FaultableException<InstallerPatchValidationFault>
    {
        public override InstallerPatchValidationFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new InstallerPatchValidationFault(Message, reqPath, userInfo);
            return f;
        }

        public InstallerPatchValidationException(String patchPath, Exception innerException = null)
            : base(string.Format("Could not parse int destination version number from patch path at '{0}'", patchPath), innerException)
        {
        }

    }

    /// <summary>
    /// If user is not allowed to take an exam because they have already taken it.
    /// </summary>

    [DataContract]
    public class InstallerPatchValidationFault : BasicFault
    {
        public InstallerPatchValidationFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }


    }

}
