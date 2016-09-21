using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EC.Common.Base;
using EC.Common.Interfaces;
using EC.Errors;
using EC.Errors.CommonExceptions;

namespace EC.Core.Common
{
    [SingletonType]
    [RegisterAsType(typeof(IEmailAddressHelper))]
    
    public class EmailAddressHelper : IEmailAddressHelper
    {
        /// <summary>
        /// Check if the email has the correct format.
        /// </summary>
        /// <param name="emailAddress">Email address to check</param>
        /// <param name="throwException">Flag whether to throw an exception if the address is wrong or return <c>false</c> instead of throwing an exception.</param>
        /// <returns>Return <c>true</c> if correct email address format. Return <c>false</c> if not a valid email address and <c>throwException</c> is <c>false</c>.</returns>
        /// <exception cref="EmailFormatException">If email address has a wrong format and <c>throwException</c> is <c>true</c>.</exception>

        public bool CheckEmailAddressFormat(string emailAddress, bool throwException = true)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                if (throwException)
                {
                    throw new EmailFormatException("No email address.", null);
                }
                else
                {
                    return false;
                }
            }

            // The email address must be of a valid format...
            // Source for regular expression: http://www.regular-expressions.info/email.html
            //Regex rx = new Regex("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", RegexOptions.CultureInvariant);
            //Regex rx = new Regex(".+@.+\\..+", RegexOptions.CultureInvariant);
            Regex rx = new Regex(".+@+", RegexOptions.CultureInvariant);
            var m = rx.Match(emailAddress);
            if (!m.Success)
            {
                if (throwException)
                {
                    throw new EmailFormatException("Wrong email address format.", emailAddress);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
