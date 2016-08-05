using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using EC.Business.Entities;
using EC.Common.Util;
using EC.Business.Actions;
using EC.Framework.Data;
using EC.Common;
using System.Linq;

namespace EC.Business
{
    public interface IConfigRepository
    {
        ConfigSetting GetConfigSetting(Guid objectKey);
        ConfigSetting[] GetConfigSettings();
        ConfigSetting GetConfigSetting(string name);
        ActionResult UpdateConfigSettings(ConfigSetting[] settings);
    }

    public class ConfigRepository :  IConfigRepository
    {

        /// <summary>
        /// Gets all ConfigSettings.
        /// </summary>
        public ConfigSetting[] GetConfigSettings()
        {
          //  return GetEntities(typeof(ConfigSetting)) as ConfigSetting[];
            return null;

        }

        /// <summary>
        /// Gets a ConfigSetting by key.
        /// </summary>
        public ConfigSetting GetConfigSetting(Guid objectKey)
        {
         //   return GetEntity(typeof(ConfigSetting), objectKey) as ConfigSetting;
            return null;

        }

        public ConfigSetting GetConfigSetting(string name)
        {
       //     var settings = GetEntities(typeof(ConfigSetting), "name", name) as ConfigSetting[];
        //    if (settings.Length > 0)
       //     {
         //       return settings[0];
         //   }
            return null;
        }

        public ActionResult UpdateConfigSettings(ConfigSetting[] settings)
        {
            ActionResult actionResult = new ActionResult();


            return actionResult;
        }
    }
}
