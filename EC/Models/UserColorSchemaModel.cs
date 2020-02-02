using EC.Constants;
using EC.Models.Database;
using System.Linq;
using System.Web.Configuration;

namespace EC.Models
{
    public class UserColorSchemaModel : BaseModel
    {
        private company company { get; set; }
        public global_settings global_Setting { get; set; }
        public UserColorSchemaModel(int? companyid)
        {
            if (companyid > 0)
            {
                company = db.company.Find(companyid);
                if (company != null)
                {
                    global_Setting = db.global_settings.Where(glb => glb.client_id == company.client_id).FirstOrDefault();
                    if (global_Setting != null)
                    {
                        if (string.IsNullOrWhiteSpace(global_Setting.header_color_code))
                        {
                            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings[ECGlobalConstants.APP_SETTING_HEADER_COLOR]))
                            {
                                global_Setting.header_color_code = WebConfigurationManager.AppSettings[ECGlobalConstants.APP_SETTING_HEADER_COLOR];
                            }
                        }
                        if (string.IsNullOrWhiteSpace(global_Setting.header_links_color_code))
                        {
                            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings[ECGlobalConstants.APP_SETTING_HEADER_COLOR_LINK]))
                            {
                                global_Setting.header_links_color_code = WebConfigurationManager.AppSettings[ECGlobalConstants.APP_SETTING_HEADER_COLOR_LINK];
                            }
                        }
                    } else
                    {
                        this.global_Setting = new global_settings();
                    }
                }
            }
            else
            {
                global_Setting = new global_settings();
                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings[ECGlobalConstants.APP_SETTING_HEADER_COLOR]))
                {
                    global_Setting.header_color_code = WebConfigurationManager.AppSettings[ECGlobalConstants.APP_SETTING_HEADER_COLOR];
                }
                if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings[ECGlobalConstants.APP_SETTING_HEADER_COLOR_LINK]))
                {
                    global_Setting.header_links_color_code = WebConfigurationManager.AppSettings[ECGlobalConstants.APP_SETTING_HEADER_COLOR_LINK];
                }
            }
        }
    }
}