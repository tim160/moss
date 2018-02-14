using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using EC;
namespace EC.Localization
{
    public class Resources
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
        /// <returns></returns>
        private static string GetString(string key, CultureInfo culture, Assembly callingAssembly, bool is_cc = false)
        {
            try
            {
                string temp_key = key;
                if (is_cc) { key = key + "_CC"; }
                string value = App_GlobalResources.Resources.ResourceManager.GetString(key);

                if (value != null)
                    return value;
                else
                {
                    if (is_cc)
                        value = App_GlobalResources.Resources.ResourceManager.GetString(temp_key);
                    if(value != null)
                        return value;
                    else
                        return key;
                }


                /*
                EC_App_LocalResources_GlobalRes_resources res = new EC_App_LocalResources_GlobalRes_resources();
                 
                ///       EC.Localization.EC_App_LocalResources_GlobalRes_resources.NewReportUp rm = new EC_App_LocalResources_GlobalRes_resources.ResourceManager();
              ////  value = Resources.ResourceManager.GetObject(fileName, Properties.Resources.Culture);
          ResourceManager rm = GetResourceManager(callingAssembly);




           /////     if (rm == null) return key;
                if(is_cc) { key = key + "_CC"; }
                string value = EC_App_LocalResources_GlobalRes_resources.ResourceManager.GetString(key);
            ///    string value = rm.GetString(key, culture);
                if (value != null)
                    return value;
                else
                    return key;*/
            }
            catch (Exception ex)
            {
                return key;// "[" + key + ": Failed to retrieve resource string]";
            }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
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
        /// <param name="value">The values.</param>
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