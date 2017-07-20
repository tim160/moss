using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace EC.Localization
{
    public class LocalizationGetter
    {
        private static Dictionary<Assembly, ResourceManager> resourceManagers = new Dictionary<Assembly, ResourceManager>();

        #region PROPERTIES
        private static CultureInfo m_Culture = CultureInfo.CurrentCulture;
        public static CultureInfo Culture
        {
            get
            {
                return m_Culture;
            }
            set
            {
                m_Culture = value;
            }
        }
        #endregion PROPERTIES

        #region METHODS
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetString(string key, bool is_cc = false)
        {
            Assembly ca = Assembly.GetCallingAssembly();
            return GetString(key, m_Culture, ca, is_cc);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="culture">The culture info.</param>
        /// <param name="key">The key.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string key, bool is_cc = false)
        {
            Assembly ca = Assembly.GetCallingAssembly();
            return GetString(key, culture, ca, is_cc);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="culture">The culture info.</param>
        /// <param name="callingAssembly">The calling assembly.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <returns></returns>
        private static string GetString(string key, CultureInfo culture, Assembly callingAssembly, bool is_cc = false)
        {
            try
            {
                ResourceManager rm = GetResourceManager(callingAssembly);
                if (rm == null) return key;
                if (is_cc) { key = key + "_CC"; }

                string value = rm.GetString(key, culture);
                if (value != null)
                    return value;
                else
                    return key;
            }
            catch (Exception)
            {
                return "[" + key + ": Failed to retrieve resource string]";
            }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <returns></returns>
        public static string GetString(string key, object value, bool is_cc = false)
        {
            Assembly ca = Assembly.GetCallingAssembly();
            string valuestring = GetString(key, m_Culture, ca, is_cc);
            if (value != null)
                valuestring = String.Format(valuestring, value);
            return valuestring;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="value">The assembly to reference.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <returns></returns>
        public static string GetString(string key, object value, Assembly referenceAssembly, bool is_cc = false)
        {
            Assembly ca = referenceAssembly;
            string valuestring = GetString(key, m_Culture, ca, is_cc);
            if (value != null)
                valuestring = String.Format(valuestring, value);
            return valuestring;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="culture">The culture info.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string key, object value, bool is_cc = false)
        {
            Assembly ca = Assembly.GetCallingAssembly();
            string valuestring = GetString(key, culture, ca, is_cc);
            if (value != null)
                valuestring = String.Format(valuestring, value);
            return valuestring;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static string GetString(string key, bool is_cc = false, params string[] values)
        {
            Assembly ca = Assembly.GetCallingAssembly();
            string valuestring = GetString(key, m_Culture, ca, is_cc);
            if (values != null)
                valuestring = String.Format(valuestring, values);
            return valuestring;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="culture">The culture info.</param>
        /// <param name="key">The key.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <param name="value">The values.</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string key, bool is_cc = false, params string[] values)
        {
            try
            {
                Assembly ca = Assembly.GetCallingAssembly();
                string valuestring = GetString(key, culture, ca, is_cc);
                if (values != null)
                    valuestring = String.Format(valuestring, values);
                return valuestring;
            }
            catch (Exception)
            {
                return "Failed to retrieve formatted resource string for " + key;
            }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <param name="paramList">The param list.</param>
        /// <returns></returns>
        public static string GetString(string key, object[] paramList, bool is_cc = false)
        {
            Assembly ca = Assembly.GetCallingAssembly();
            string valuestring = GetString(key, m_Culture, ca, is_cc);
            if (paramList != null)
                valuestring = String.Format(valuestring, paramList);
            return valuestring;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="culture">The culture info.</param>
        /// <param name="key">The key.</param>
        /// <param name="paramList">The param list.</param>
        /// <param name="is_cc">Boolean is Campus Confidential or Employee Confidential; false by default</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string key, object[] paramList, bool is_cc = false)
        {
            Assembly ca = Assembly.GetCallingAssembly();
            string valuestring = GetString(key, culture, ca, is_cc);
            if (paramList != null)
                valuestring = String.Format(valuestring, paramList);
            return valuestring;
        }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        /// <param name="callingAssembly">The calling assembly.</param>
        /// <returns></returns>
        private static ResourceManager GetResourceManager(Assembly callingAssembly)
        {
            ResourceManager rm = null;
            if (!resourceManagers.ContainsKey(callingAssembly))
            {
                string[] resourceList = callingAssembly.GetManifestResourceNames();
                string baseName = "";
                foreach (string resource in resourceList)
                {
                    string lowerResource = resource.ToLower();
                    if (lowerResource.IndexOf("localization") >= 0)
                    {
                        int extOffset = lowerResource.IndexOf(".resources");
                        baseName = resource.Substring(0, extOffset);
                        break;
                    }
                }

                if (baseName.Length > 0)
                {
                    rm = new ResourceManager(baseName, callingAssembly);
                    resourceManagers.Add(callingAssembly, rm);
                }
            }
            else
            {
                rm = (ResourceManager)resourceManagers[callingAssembly];
            }

            return rm;
        }
        #endregion METHODS
    }
}
