using System;
using System.Globalization;
using System.Threading;
using EC.Constants;
using EC.Common.Interfaces;

namespace EC.Core.Common
{
    [SingletonType]
    [RegisterAsType(typeof(ICultureHelper))]
    public class CultureHelper : ICultureHelper
    {
        /// <summary>
        /// Will return culture info for <paramref name="cultureString" />. If <paramref name="cultureString" /> is null or empty or can't be converted to a valid culture string then
        /// we will return the default culture for "en-US"
        /// </summary>
        /// <param name="cultureString"></param>
        /// <returns>
        /// System.Globalization.CultureInfo
        /// </returns>
        public CultureInfo GetCulture(string cultureString)
        {
            CultureInfo culture = null;

            try
            {
                if (!string.IsNullOrEmpty(cultureString))
                {
                    culture = new CultureInfo(cultureString);
                }
            }
            catch (Exception)
            {
                // Do nothing but use default CultureInfo
            }

            if (culture == null)
            {
                culture = new CultureInfo(LanguageConstants.Default_Culture_String);
            }

            return culture;
        }

        /// <summary>
        /// Set thread culture and UI culture to user's optional user language (e.g. en-uk or even custom languages).
        /// </summary>
        /// <remarks>
        /// If the user language doesn't exist we don't change the thread culture or UI culture.
        /// No exception is thrown.
        /// </remarks>
        /// <param name="userLanguage">Optional: user language</param>
        public void SetThreadCultureFromUserLanguage(string userLanguage)
        {
            try
            {
                // Override thread culture if a user culture exists...
                if (!string.IsNullOrWhiteSpace(userLanguage))
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(userLanguage);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(userLanguage);
                }
            }
            catch (Exception)
            {
                // Nothing to change if the user language doesn't exist.
            }
        }
    }
}
