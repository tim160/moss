using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    [DataContract]
    public class OrgInfo
    {
        /// <summary>
        /// copyright string from org attribute
        /// </summary>

        [DataMember]
        public string Copyright { get; set; }

        /// <summary>
        /// Feedback email address pulled from org attribute
        /// </summary>

        [DataMember]
        public string Feedback { get; set; }

        /// <summary>
        /// Where the user will be redirected to once they logout
        /// </summary>        
        [DataMember]
        public string LogoutRedirect { get; set; }

        /// <summary>
        /// True if org on path has OrgId logon
        /// </summary>

        [DataMember]
        public bool HasOrgIdLogon { get; set; }

        /// <summary>
        /// Path to org where org data was found
        /// </summary>

        [DataMember]
        public string OrgPath { get; set; }

        /// <summary>
        /// Name of the organization
        /// </summary>
        
        [DataMember]
        public string OrgName {get; set;}

        /// <summary>
        /// Optional custom DNS entries for the organization.
        /// Empty if no custom DNS entries exist.
        /// </summary>

        [DataMember]
        public List<CustomDNS> CustomDNS { get; set; }

        [DataMember]
        public bool HasPasswordExpiryRuleSet { get; set; }

        [DataMember]
        public bool HasPasswordLockoutRuleSet { get; set; }

    }
}
