using EC.Constants;
using System.Web;
using System.Web.Http;
using EC.Models.Database;
using System;
using log4net;
using EC.Common.Util;
using EC.Models;

namespace EC.Controllers.API
{
    public class SettingsGlobalLogoController : BaseApiController
    {
        public ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string GLOBAL_LOGO_PATH = "~/Content/Icons";
        private const string GLOBAL_LOGO_FILE_NAME = "logo.png";

        [HttpPost]
        public object Post()
        {
            string result = "false";
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0 || user.role_id > ECLevelConstants.level_escalation_mediator)
                return result;

            var photo = System.Web.HttpContext.Current.Request.Files["_file"];

            try
            {
                if (photo.ContentLength > 0 && user.role_id == ECLevelConstants.level_supervising_mediator)
                {
                    if (!System.IO.Directory.Exists(GLOBAL_LOGO_PATH))
                    {
                        var folder = HttpContext.Current.Server.MapPath(GLOBAL_LOGO_PATH + "\\" + GLOBAL_LOGO_FILE_NAME);
                        photo.SaveAs(folder);
                        SettingsModel settingsModel = new SettingsModel();
                        settingsModel.updateIconPath(user.id, folder);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.ToString());
                result = ex.ToString();
            }
            return true;
        }
    }
}