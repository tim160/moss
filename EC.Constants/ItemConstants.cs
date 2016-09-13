using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// Constants relating to items stored in the EC.
    /// </summary>

    public static class ItemConstants
    {
        /// <summary>
        /// String representation of the GUID for the root link.
        /// </summary>

        public static readonly Guid RootLinkGuid = Guid.Parse("d421de82-eb2d-4194-aa8f-089c76a1f10e");

        /// <summary>
        /// String representation of the GUID for the root NavPage.
        /// </summary>

        public static readonly Guid RootNavPageGuid = Guid.Parse("3c8fa3f7-152b-4810-a6a9-fd109926ef72");

        /// <summary>
        /// String representation of the GUID for the silent registration policy.
        /// </summary>

        public static readonly Guid SilentRegistrationPolicyGuid = Guid.Parse("1E0B2E4E-988A-11E2-8C1F-F30997C1E54C");

        /// <summary>
        /// String representation of the GUID for the agree to registration policy.
        /// </summary>

        public static readonly Guid AggreeTermsRegistrationPolicyGuid = Guid.Parse("1E0B2E4F-988A-11E2-8C1F-F30997C1E54C");

        /// <summary>
        /// String representation of the root path ('/Root').
        /// </summary>

        public const string RootPath = "/Root";
    }
}
