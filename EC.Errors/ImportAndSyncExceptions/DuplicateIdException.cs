using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.ImportAndSyncExceptions
{
    /// <summary>
    /// Exception thrown when a duplicate Id is found during import.
    /// </summary>

    public class DuplicateIdException : FaultableException<DuplicateIdFault>
    {
        public override DuplicateIdFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new DuplicateIdFault(Message, reqPath, userInfo);
            f.DuplicateId = DuplicateId;
            return f;
        }

        public DuplicateIdException(string msg, Guid id, Exception innerException = null) : base(string.IsNullOrEmpty(msg) ? "Duplicate Id" : msg, innerException)
        {
            DuplicateId = id;
        }

        /// <summary>
        /// Id which is a duplicate.
        /// </summary>

        public Guid DuplicateId { get; set; }
    }

    /// <summary>
    /// Fault thrown when a duplicate Id is found.
    /// </summary>

    [DataContract]
    public class DuplicateIdFault : BasicFault
    {
        public DuplicateIdFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid DuplicateId { get; set; }
    }
}
