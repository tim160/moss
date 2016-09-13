using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// If an upload token is invalid (file for the token doesn't exist).
    /// </summary>

    public class InvalidUploadTokenException : FaultableException<InvalidUploadTokenFault>
    {
        public override InvalidUploadTokenFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new InvalidUploadTokenFault(Message, reqPath, userInfo);
            f.UploadTokenId = UploadTokenId;
            return f;
        }

        public Guid UploadTokenId { get; set; }
    }

    /// <summary>
    /// If a file referenced with an upload token doesn't exist.
    /// </summary>

    [DataContract]
    public class InvalidUploadTokenFault : BasicFault
    {
        public InvalidUploadTokenFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Time the user has to wait.
        /// </summary>

        [DataMember]
        public Guid UploadTokenId { get; set; }
    }
}
