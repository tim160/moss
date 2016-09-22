using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Model.Interfaces;
using EC.Constants;

namespace EC.Model.Impl
{
    public class UserItems : IUser
    {

        public void SetPasswordHash(string newPasswordHash)
        {
            if (Password + PasswordConstants.PASSWORD_SALT == newPasswordHash) { return; }
            Password = newPasswordHash + PasswordConstants.PASSWORD_SALT;
        }

        /// <summary>
        /// Validates the password base on organization password rules and hashes and sets the password.
        /// </summary>
        /// <param name="newPassword"></param>

        public void SetPasswordAndValidate(string newPassword)
        {
            if (Password != null && BCrypt.Net.BCrypt.Verify(newPassword + PasswordConstants.PASSWORD_SALT, Password)) { return; }
         //   var hasComplexityRule = m_OrganizationHelper.HasPasswordComplexityRuleSet(efOrganization) || m_OrganizationHelper.HasPasswordHistoryRuleSet(efOrganization);
        //    if (hasComplexityRule) { m_OrganizationHelper.ValidatePasswordComplexityAndHistory(efOrganization, newPassword, this); }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword + PasswordConstants.PASSWORD_SALT, PasswordConstants.ENCRYPTION_WORKLOAD);
            Password = passwordHash;
        }


        /// <summary>
        /// Hashes the password and sets it, without any validation
        /// </summary>
        /// <param name="newPassword"></param>

        public void SetPasswordNoValidation(string newPassword)
        {
            if (Password != null && BCrypt.Net.BCrypt.Verify(newPassword + PasswordConstants.PASSWORD_SALT, Password)) { return; }
            Password = BCrypt.Net.BCrypt.HashPassword(newPassword + PasswordConstants.PASSWORD_SALT, PasswordConstants.ENCRYPTION_WORKLOAD);
        }


        /// <summary>
        /// Verify Password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password + PasswordConstants.PASSWORD_SALT, Password);
        }

        /// <summary>
        /// Validate Password and check it against password lockout rules and lock user if necessary.
        /// </summary>
        /// <remarks>
        /// SuperUsers and non-interactive users are excempted from the lockout rules.
        /// </remarks>
        /// <returns>Return <c>true</c> if logon was valid. Return <c>false</c> if the user is either locked out or the password was wrong</returns>
        public bool ValidatePasswordAndUpdateLockoutState(string password)
        {
            bool isValid = VerifyPassword(password);
            /*    if (IsSuperUser || !InteractiveLogon)
                {
                    // SuperUsers and non-interactive users don't apply to the lockout rules...
                    return isValid;
                }

                if (isValid && !LockedOut)
                {
                    ResetFailedLoginTime();
                    return true;
                }

                if (isValid && LockedOut && NextAvailableLoginTime < DateTime.UtcNow)
                {
                    ResetFailedLoginTime();
                    return true;
                }
                if (isValid) { return false; }

                var passwordLockoutWindow = m_OrganizationHelper.PasswordLockoutWindow(efOrganization, this);
                if (FirstFailedLoginTime == null)
                {
                    SetFirstFailedLoginTimeAndNextAvailableLoginTime(DateTime.UtcNow, passwordLockoutWindow);
                    SetCurrentFailedAttempts(1);
                    return false;
                }

                if (FirstFailedLoginTime != null && NextAvailableLoginTime < DateTime.UtcNow)
                {
                    SetFirstFailedLoginTimeAndNextAvailableLoginTime(DateTime.UtcNow, passwordLockoutWindow);
                    SetCurrentFailedAttempts(1);
                }
                else
                {
                    SetCurrentFailedAttempts(CurrentFailedAttempts + 1);
                }

                bool needToBeLocked = m_OrganizationHelper.PasswordNeedToBeLocked(efOrganization, this);
                if (needToBeLocked) { SetLockout(true); }*/
            return false;
        }



        // ------------------------------- Mapped Scalars --------------------------------------

        public virtual string Password { get; protected set; }
    }
}
