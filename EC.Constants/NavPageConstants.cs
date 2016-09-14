using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Tag types for a NavPage.
    /// </summary>

    [DataContract]
    [Flags]

    public enum NavPageTagTypesEnum
    {
        /// <summary>
        /// No type set for the NavPage.
        /// </summary>

        [EnumMember]
        None = 1,

        /// <summary>
        /// Organization NavPage. This tags a root level NavPage for an organization.
        /// </summary>

        [EnumMember]
        Organization = 2,

        /// <summary>
        /// Course. This tag marks a NavPage that it could hold course offerings.
        /// </summary>

        [EnumMember]
        Course = 4
    }
}
