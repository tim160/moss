using System.Web;
using System.Web.Http;
using EC.Constants;
using EC.Models;
using EC.Models.Database;


namespace EC.Controllers.API
{
    public class SettingsGlobalColorController : BaseApiController
    {
        public class Filter
        {
            public string header_color_code { get; set; }
            public string header_links_color_code { get; set; }
        }
        [HttpPost]
        public object Post([FromBody] Filter filter)
        {
            string result = "false";
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0 || user.role_id > ECLevelConstants.level_escalation_mediator)
                return result;

            if (string.IsNullOrWhiteSpace(filter.header_color_code) || string.IsNullOrWhiteSpace(filter.header_links_color_code))
            {
                return result;
            }

            var settingModel = new SettingsModel();
            var company = DB.company.Find(user.company_id);
            return settingModel.updateColorGlobalSettings(company.client_id, filter.header_color_code, filter.header_links_color_code);
        }
    }
}