
namespace EC.Common.Interfaces
{
    public interface ICultureHelper
    {
        /// <summary>
        /// Will return culture info for <paramref name="cultureString"/>. If <paramref name="cultureString"/> is null or empty or can't be converted to a valid culture string then 
        /// we will return the default culture for "en-US"
        /// </summary>
        /// <param name="cultureString"></param>
        /// <returns>System.Globalization.CultureInfo</returns>
        
        System.Globalization.CultureInfo GetCulture(string cultureString);

        /// <summary>
        /// Set thread culture and UI culture to user's optional user language (e.g. en-uk or even custom languages).
        /// </summary>
        /// <remarks>
        /// If the user language doesn't exist we don't change the thread culture or UI culture.
        /// No exception is thrown.
        /// </remarks>
        /// <param name="userLanguage">Optional: user language</param>
        
        void SetThreadCultureFromUserLanguage(string userLanguage);
    }
}
