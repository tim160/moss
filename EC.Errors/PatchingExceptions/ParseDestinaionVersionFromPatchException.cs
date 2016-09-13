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

    public class ParseDestinaionVersionFromPatchException : FaultableException<ParseDestinaionVersionFromPatchFault>
    {
        public override ParseDestinaionVersionFromPatchFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new ParseDestinaionVersionFromPatchFault(Message, reqPath, userInfo);
            f.PatchPath = PatchPath;
            return f;
        }

        public ParseDestinaionVersionFromPatchException(String patchPath, Exception innerException = null)
            : base(string.Format("Could not parse int destination version number from patch path at '{0}'"), innerException)
        {
            PatchPath = patchPath;
        }

        public String PatchPath { get; set; }
    }

    /// <summary>
    /// If user is not allowed to take an exam because they have already taken it.
    /// </summary>

    [DataContract]
    public class ParseDestinaionVersionFromPatchFault : BasicFault
    {
        public ParseDestinaionVersionFromPatchFault(string msg, string path, CurrentUserInfo userInfo)
            : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public String PatchPath { get; set; }

    }

}
