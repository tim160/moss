using EC.Errors.CommonExceptions;

namespace EC.Common.Interfaces
{
    public interface IEmailAddressHelper
    {
        /// <summary>
        /// Check if the email has the correct format.
        /// </summary>
        /// <param name="emailAddress">Email address to check</param>
        /// <param name="throwException">Flag whether to throw an exception if the address is wrong or return <c>false</c> instead of throwing an exception.</param>
        /// <returns>Return <c>true</c> if correct email address format. Return <c>false</c> if not a valid email address and <c>throwException</c> is <c>false</c>.</returns>
        /// <exception cref="EmailFormatException">If email address has a wrong format and <c>throwException</c> is <c>true</c>.</exception>

        bool IsValidEmail(string emailAddress, bool throwException = true);
    }
}