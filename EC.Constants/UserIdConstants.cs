using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// This class contains all Ids for static users in the system (i.e. anonymous user).
    /// </summary>

    public static class UserIdConstants
    {
        public static readonly Guid STEFAN_USER_ID = Guid.Parse("6BB2ABF6-F081-4D0D-BBB8-B75DA3C5ADEA");

        public static readonly Guid SHAN_USER_ID = Guid.Parse("8A864E4B-9785-4F15-BAD6-F95580C72D57");

        public static readonly Guid NIC_USER_ID = Guid.Parse("4282C8EA-864F-4FB0-BE57-2772387F19A5");

        public static readonly Guid MURRAY_USER_ID = Guid.Parse("9D534A83-6630-4AA1-8D75-BA2E6695EB40");

        public static readonly Guid TERRY_USER_ID = Guid.Parse("9C1F429B-29EB-4102-B84F-BA394FED3998");

        public static readonly Guid ANONYMOUS_USER_ID = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public static readonly Guid SYSTEM_USER_ID = Guid.Parse("00000000-0000-0000-0000-000000000002");

        public static readonly Guid UNKNOWN_USER_ID = Guid.Parse("00000000-0000-0000-0000-000000000003");

        /// <summary>
        /// Mapping of (userId, userItemAuditId).
        /// </summary>
        /// <remarks>
        /// Every constant user Id has a corresponding user item audit Id
        /// </remarks>

        public static readonly IDictionary<Guid, Guid> UserIdAuditIdMapping = new Dictionary<Guid, Guid>() 
        {
            { UserIdConstants.STEFAN_USER_ID, Guid.Parse("656F923A-DC88-11E3-8354-9651563C3E58") },
            { UserIdConstants.SHAN_USER_ID, Guid.Parse("656F9260-DC88-11E3-8354-9651563C3E58") },
            { UserIdConstants.NIC_USER_ID, Guid.Parse("656F84E6-DC88-11E3-8354-9651563C3E58") },
            { UserIdConstants.MURRAY_USER_ID, Guid.Parse("656F923E-DC88-11E3-8354-9651563C3E58") },
            { UserIdConstants.TERRY_USER_ID, Guid.Parse("656F9240-DC88-11E3-8354-9651563C3E58") },
            { UserIdConstants.ANONYMOUS_USER_ID, Guid.Parse("656F84E0-DC88-11E3-8354-9651563C3E58") },
            { UserIdConstants.SYSTEM_USER_ID, Guid.Parse("656F84E2-DC88-11E3-8354-9651563C3E58") },
            { UserIdConstants.UNKNOWN_USER_ID, Guid.Parse("656F84E4-DC88-11E3-8354-9651563C3E58") }
        };

        /// <summary>
        /// Get all constant user Ids from the static class (with reflection).
        /// </summary>
        /// <remarks>
        /// All static field values of type Guid are read with reflection and returned as list.
        /// </remarks>
        /// <returns>Return list of constant user Ids form <c>UserIdConstants</c></returns>

        public static IList<Guid> All()
        {
            IList<Guid> result = new List<Guid>();
            var t = typeof(UserIdConstants);
            var allFields = t.GetFields().Where(p => p.FieldType == typeof(Guid)).ToList();
            foreach (var field in allFields)
            {
                result.Add((Guid)field.GetValue(null));
            }
            return result;
        }
    }
}
