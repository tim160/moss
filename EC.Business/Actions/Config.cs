using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Configuration; 
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Web;
using EC.Framework.Logger;
using System.Threading;
using EC.Business.Entities;
using EC.Framework.Data;
using EC.Common.Util;
using System.Data;
using System.Globalization;


namespace EC.Business.Actions
{
    public sealed class Config
    {
        #region Member(s) and Property(s)
        public event EventHandler ConfigSettingsUpdated; 
  //      static readonly ICustomLog Log = CustomLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        CultureInfo m_CultureInfo = new CultureInfo("en-US");
        private static Config instance = null;
        private Dictionary<string, ConfigSetting> configSettingItems = new Dictionary<string, ConfigSetting>();
        private ManualResetEvent m_ReinitWaitEvent = new ManualResetEvent(true);
        private string m_BackupFolder;

        static IConfigRepository _ConfigRepository;

        public static IConfigRepository ConfigRepository
        {
            get
            {
                if (_ConfigRepository == null)
                    _ConfigRepository = new ConfigRepository();
                return _ConfigRepository;
            }
            set
            {
                _ConfigRepository = value;
            }
        }
    
        public static Config Instance
        {
            get 
            {
                if (instance == null)
                    instance = new Config();
                return instance; 
            }
        }

        public string BackupFolder
        {
            get
            {
                if (m_BackupFolder == null)
                    return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, @"..\backup\");
                else
                    return m_BackupFolder;
            }
            set
            {
                m_BackupFolder = value;
            }
        }
        #endregion

        #region Constructor(s)
        private Config()
        {
            ConfigSetting[] dbConfigs = ConfigRepository.GetConfigSettings();

            if (dbConfigs != null)
            {
                foreach (ConfigSetting setting in dbConfigs)
                {
                    configSettingItems.Add(setting.Name, setting);
                }
       //         Log.Info("Config Settings loaded!");
            }
        }
        #endregion

        #region DB Read
        /// <summary>
        /// Gets a ConfigSetting by key.
        /// </summary>
    //    public ConfigSetting GetConfigSetting(Guid objectKey)
   //     {
    //        return ConfigRepository.GetConfigSetting(objectKey);
   //     }

        /// <summary>
        /// Gets all Config Settings.
        /// </summary>
   //     public ConfigSetting[] GetConfigSettings()
     //   {
   //         return ConfigRepository.GetConfigSettings();
   //     }
        #endregion



        #region Method(s)

      
        public string GetConfig(string key)
        {
            m_ReinitWaitEvent.WaitOne();
            if (configSettingItems.ContainsKey(key))
            {
                return configSettingItems[key].Value;
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetConfig(string key, string defaultValue)
        {
            m_ReinitWaitEvent.WaitOne();
            if (configSettingItems.ContainsKey(key))
            {
                return configSettingItems[key].Value;
            }
            else
            {
      //          Log.WarnFormat("{0} not found in config, defaulting to {1}", key, defaultValue);
            }
            return defaultValue; 
        }

        public int GetConfig(string key, int defaultValue)
        {
            m_ReinitWaitEvent.WaitOne();
            if (configSettingItems.ContainsKey(key))
            {
                int result = 0;
                if (Int32.TryParse(configSettingItems[key].Value, out result))
                {
                    return result;
                }
                else
                {
      //              Log.WarnFormat("{0} ({1}) in config has invalid value, defaulting to {2}",
        //                configSettingItems[key].Value, key, defaultValue);
                    return defaultValue;
                }
            }
            else
            {
 //               Log.WarnFormat("{0} not found in config, defaulting to {1}", key, defaultValue); 
            }
            return defaultValue;
        }

        public bool GetConfig(string key, bool defaultValue)
        {
            m_ReinitWaitEvent.WaitOne();
            bool configValue = defaultValue;
            if (configSettingItems.ContainsKey(key))
            {
                try
                {
                    configValue = bool.Parse(configSettingItems[key].Value);
                }
                catch
                {
        //            Log.WarnFormat("{0} in config has invalid value, default to {1}", key, defaultValue);
                }
            }
            else
            {
        //        Log.WarnFormat("{0} not found in config defaulting to {1}", key, defaultValue);
            }
            return configValue;
        }

        public decimal GetConfig(string key, decimal defaultValue)
        {
            m_ReinitWaitEvent.WaitOne();
            if (configSettingItems.ContainsKey(key))
            {
                decimal result = 0;
                if (decimal.TryParse(configSettingItems[key].Value, out result))
                    return result;
                else
                {
       //             Log.WarnFormat("{0} ({1}) in config has invalid value, defaulting to {2}",
       //                 configSettingItems[key].Value, key, defaultValue);
                    return defaultValue;
                }
            }
            else
            {
        //        Log.WarnFormat("{0} not found in config, defaulting to {1}", key, defaultValue);
            }
            return defaultValue;
        }

        public ConfigSetting GetDetailedConfig(ConfigSetting setting)
        {
            m_ReinitWaitEvent.WaitOne();

            ConfigSetting storedSetting = null;

            if (configSettingItems.ContainsKey(setting.Name))
            {
                storedSetting = configSettingItems[setting.Name];

                if (storedSetting != null)
                {
                    storedSetting.Value = setting.Value;
                }
                else
                {
        //            Log.WarnFormat("{0} in config dictionary has invalid value", setting.Name);
                }
            }
            else
            {
      //          Log.WarnFormat("{0} not found in config dictionary", setting.Name);
            }

            return storedSetting;
        }
        #endregion
    }

    public class Configuration : EC.Business.Abstract.IConfig
    {
        #region IConfig Members

        public string GetConfig(string key)
        {
            return Config.Instance.GetConfig(key);
        }

        public string GetConfig(string key, string defaultValue)
        {
            return Config.Instance.GetConfig(key, defaultValue);
        }

        public int GetConfig(string key, int defaultValue)
        {
            return Config.Instance.GetConfig(key, defaultValue);
        }

        public bool GetConfig(string key, bool defaultValue)
        {
            return Config.Instance.GetConfig(key, defaultValue);
        }

        public decimal GetConfig(string key, decimal defaultValue)
        {
            return Config.Instance.GetConfig(key, defaultValue);
        }

        #endregion
    }

}
