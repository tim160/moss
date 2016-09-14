using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception used to indicate that an organization hasn't been found at a specific path.
    /// </summary>

    public class OrganizationNotFoundException : FaultableException<OrganizationNotFoundFault>
    {
        public override OrganizationNotFoundFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new OrganizationNotFoundFault(Message, path, userInfo);
            f.MissingOrganizationPath = MissingOrganizationPath;
            return f;
        }

        public OrganizationNotFoundException(string msg, string missingOrganizationPath, Exception innerException = null) : base(msg, innerException)
        {
            MissingOrganizationPath = missingOrganizationPath;
        }

        /// <summary>
        /// The missing organization path.
        /// </summary>

        public string MissingOrganizationPath { get; set; }
    }

    /// <summary>
    /// Exception used to indicate that an organization path hasn't been found.
    /// </summary>

    [DataContract]
    public class OrganizationNotFoundFault : BasicFault
    {
        public OrganizationNotFoundFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Missing organization path.
        /// </summary>

        [DataMember]
        public string MissingOrganizationPath { get; set; }
    }
}
