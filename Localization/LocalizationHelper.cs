using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Globalization;
using System.Threading;

namespace EC.Localization
{
    public class LocalizationHelper
    {
        #region Parameter(s)
       
        public static CultureInfo Culture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
        }
        #endregion

        #region Method(s)
        
        /// <summary>
        /// Gets the string
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="culture">The culture info</param>
        /// <returns></returns>
        public static string GetString(string key, CultureInfo culture)
        {
            string value = LocalizationResource.ResourceManager.GetString(key, culture);
            if (value != null)
                return value;
            else
                return key;
        }

        /// <summary>
        /// Gets the string
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            return GetString(key, Culture);
        }

        /// <summary>
        /// Gets the string
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public static string GetString(string key, object value)
        {
            string valuestring = GetString(key, Culture);
            if (value != null)
                valuestring = String.Format(valuestring, value);
            return valuestring;
        }

        /// <summary>
        /// Gets the string
        /// </summary>
        /// <param name="culture">The culture info</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string key, object value)
        {
            string valuestring = GetString(key, culture);
            if (value != null)
                valuestring = String.Format(valuestring, value);
            return valuestring;
        }

        /// <summary>
        /// Gets the string
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The values</param>
        /// <returns></returns>
        public static string GetString(string key, params string[] values)
        {
            string valuestring = GetString(key, Culture);
            if (values != null)
                valuestring = String.Format(valuestring, values);
            return valuestring;
        }

        /// <summary>
        /// Gets the string
        /// </summary>
        /// <param name="culture">The culture info</param>
        /// <param name="key">The key</param>
        /// <param name="value">The values</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string key, params string[] values)
        {
            string valuestring = GetString(key, culture);
            if (values != null)
                valuestring = String.Format(valuestring, values);
            return valuestring;
        }

        /// <summary>
        /// Gets the string
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="paramList">The param list</param>
        /// <returns></returns>
        public static string GetString(string key, object[] paramList)
        {
            string valuestring = GetString(key, Culture);
            if (paramList != null)
                valuestring = String.Format(valuestring, paramList);
            return valuestring;
        }

        /// <summary>
        /// Gets the string
        /// </summary>
        /// <param name="culture">The culture info</param>
        /// <param name="key">The key</param>
        /// <param name="paramList">The param list</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string key, object[] paramList)
        {
            string valuestring = GetString(key, culture);
            if (paramList != null)
                valuestring = String.Format(valuestring, paramList);
            return valuestring;
        }
        #endregion
    }
}
