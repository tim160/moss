
namespace EC.Common.Interfaces
{
    public interface ICryptographyHelper
    {
        /// <summary>
        /// Will encrypt <paramref name="plainPassword"/> with an optional <paramref name="salt"/> with the current user the service is running.
        /// To decrypt the password the service must run as the same user when it has been encrypted. 
        /// </summary>
        /// <param name="plainPassword">Plain password to encrypt</param>
        /// <param name="salt">Optional: Encryption salt. Set <c>null</c> not to use any salt for the encryption</param>
        /// <returns>Return the encrypted password (as base64 string)</returns>
        /// <exception cref="CantEncryptPasswordException">If an error occurred during encryption</exception>

        string EncryptPassword(string plainPassword, string salt = null);

        /// <summary>
        /// Will decrypt the passed in encrypted password (and optional <paramref name="salt"/>).
        /// The service must be running as the same user who encrypted the password.
        /// </summary>
        /// <param name="encryptedPwd">Encrypted string (must be a base64 compatible string) from <see cref="EncryptPassword"/></param>
        /// <param name="salt">Optional: Salt used to encrypt the password. Set <c>null</c> is no salt was used during the encryption</param>
        /// <returns>Plain decrypted password</returns>
        /// <exception cref="CantDecryptPasswordException">If the password couldn't be decrypted</exception>

        string DecryptPassword(string encryptedPwd, string salt = null);

        
    }
}
