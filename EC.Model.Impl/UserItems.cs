using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Model.Interfaces;
using EC.Constants;

namespace EC.Model.Impl
{
    class UserItems : IUser
    {

        public void SetPasswordHash(string newPasswordHash)
        {
            if (Password == newPasswordHash) { return; }
            Password = newPasswordHash;
        }

        /// <summary>
        /// Validates the password base on organization password rules and hashes and sets the password.
        /// </summary>
        /// <param name="newPassword"></param>

        public void SetPasswordAndValidate(string newPassword)
        {
            if (Password != null && BCrypt.Net.BCrypt.Verify(newPassword, Password)) { return; }
         //   var hasComplexityRule = m_OrganizationHelper.HasPasswordComplexityRuleSet(efOrganization) || m_OrganizationHelper.HasPasswordHistoryRuleSet(efOrganization);
        //    if (hasComplexityRule) { m_OrganizationHelper.ValidatePasswordComplexityAndHistory(efOrganization, newPassword, this); }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, PasswordConstants.ENCRYPTION_WORKLOAD);
            Password = passwordHash;
        }


        /// <summary>
        /// Hashes the password and sets it, without any validation
        /// </summary>
        /// <param name="newPassword"></param>

        public void SetPasswordNoValidation(string newPassword)
        {
            if (Password != null && BCrypt.Net.BCrypt.Verify(newPassword, Password)) { return; }
            Password = BCrypt.Net.BCrypt.HashPassword(newPassword, EncryptionConstants.ENCRYPTION_WORKLOAD);
        }

        // ------------------------------- Mapped Scalars --------------------------------------

        public virtual string Password { get; protected set; }
    }
}
