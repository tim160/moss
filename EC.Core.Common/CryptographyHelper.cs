using System;
using EC.Errors.CommonExceptions;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    [SingletonType]
    [RegisterAsType(typeof(ICryptographyHelper))]

    public class CryptographyHelper : ICryptographyHelper
    {
        /// <summary>
        /// Will encrypt <paramref name="plainPassword"/> with an optional <paramref name="salt"/> with the current user the service is running.
        /// To decrypt the password the service must run as the same user when it has been encrypted. 
        /// </summary>
        /// <param name="plainPassword">Plain password to encrypt</param>
        /// <param name="salt">Optional: Encryption salt. Set <c>null</c> not to use any salt for the encryption</param>
        /// <returns>Return the encrypted password (as base64 string)</returns>
        /// <exception cref="CantEncryptPasswordException">If an error occurred during encryption</exception>
        
        public string EncryptPassword(string plainPassword, string salt = null)
        {
            try
            {
                var plainPwdCharArray = plainPassword.ToCharArray();
                var plainPwdBytes = System.Text.Encoding.Default.GetBytes(plainPwdCharArray, 0, plainPwdCharArray.Length);
                
                byte[] saltBytes = null;
                if (salt != null)
                {
                    var saltCharArray = salt.ToCharArray();
                    saltBytes = System.Text.Encoding.Default.GetBytes(saltCharArray, 0, saltCharArray.Length);
                }
                var encryptedPwdByteArray = System.Security.Cryptography.ProtectedData.Protect(plainPwdBytes, saltBytes, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                var encryptedPwd = Convert.ToBase64String(encryptedPwdByteArray);
                return encryptedPwd;
            }
            catch (Exception ex)
            {
                throw new CantEncryptPasswordException("Error encrypting password.", ex);
            }
        }

        /// <summary>
        /// Will decrypt the passed in encrypted password (and optional <paramref name="salt"/>).
        /// The service must be running as the same user who encrypted the password.
        /// </summary>
        /// <param name="encryptedPwd">Encrypted string (must be a base64 string) from <see cref="EncryptPassword"/></param>
        /// <param name="salt">Optional: Salt used to encrypt the password. Set <c>null</c> is no salt was used during the encryption</param>
        /// <returns>Plain decrypted password</returns>
        /// <exception cref="CantDecryptPasswordException">If the password couldn't be decrypted</exception>
        
        public string DecryptPassword(string encryptedPwd, string salt = null)
        {
            try
            {
                var encryptedPwdBytes = Convert.FromBase64String(encryptedPwd);
                byte[] saltBytes = null;
                if (salt != null)
                {
                    var saltCharArray = salt.ToCharArray();
                    saltBytes = System.Text.Encoding.Default.GetBytes(saltCharArray, 0, saltCharArray.Length);
                }
                var plainPwdByteArray = System.Security.Cryptography.ProtectedData.Unprotect(encryptedPwdBytes, saltBytes, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                var encoding = System.Text.Encoding.Default;
                var plainTextPass = encoding.GetString(plainPwdByteArray);
                return plainTextPass;
            }
            catch (Exception ex)
            {
                throw new CantDecryptPasswordException("Error decrypting password. The current user and salt must be the same as the one encrypting the password.", ex);
            }
        }
    }
}
