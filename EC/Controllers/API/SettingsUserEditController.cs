using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

using EC.Models;
using EC.Models.Database;
using EC.Constants;
using EC.Core.Common;

using EC.Utils;

namespace EC.Controllers.API
{
    public class SettingsUserEditController : BaseApiController
    {

        [HttpGet]
        public object Get(int id)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            var model = DB.user.FirstOrDefault(x => x.id == id);
            var um = new UserModel(user.id);

            model = model ?? new user
            {
                user_permissions_approve_case_closure = 2,
                user_permissions_change_settings = 2,
                role_id = 6,
                status_id = 3,
            };

            return new
            {
                departments = DB.company_department.Where(x => x.company_id == user.company_id && x.status_id == 2).OrderBy(x => x.department_en).ToList(),
                locations = DB.company_location.Where(x => x.company_id == user.company_id && x.status_id == 2).OrderBy(x => x.location_en).ToList(),
                model = new
                {
                    first_nm = model == null ? "" : model.first_nm,
                    last_nm = model == null ? "" : model.last_nm,
                    title_ds = model == null ? "" : model.title_ds,
                    email = model == null ? "" : model.email,
                    role = model == null ? 5 : model.role_id,
                    departmentId = model == null ? 0 : model.company_department_id,
                    locationId = model == null ? 0 : model.company_location_id,
                    user_permissions_approve_case_closure = model == null || (model.user_permissions_approve_case_closure == null || model.user_permissions_approve_case_closure == 0) ? 2 : model.user_permissions_approve_case_closure,
                    user_permissions_change_settings = model == null || (model.user_permissions_change_settings == null || model.user_permissions_change_settings == 0) ? 2 : model.user_permissions_change_settings,
                    status_id = model == null ? 3 : model.status_id,
                    photo_path = model.photo_path,
                },
                user = new
                {
                    role = user.role_id,
                    CanEditUserProfiles = um.CanEditUserProfiles,
                },
            };
        }

        [HttpPost]
        public object Post(user model)
        {
            user sessionUser = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            var user = DB.user.FirstOrDefault(x => x.id == model.id);

            //Change only own settings
            //Can not add
            if ((sessionUser.role_id != 5) & (user == null || sessionUser.id != user.id))
            {
                return new
                {
                    result = 0,
                    ok = true,
                };
            }

            if (user != null)
            {
                user.first_nm = model.first_nm;
                user.last_nm = model.last_nm;
                user.title_ds = model.title_ds;
                user.email = model.email;
                user.role_id = model.role_id;
                user.company_department_id = model.company_department_id;
                user.company_location_id = model.company_location_id;
                user.user_permissions_approve_case_closure = model.user_permissions_approve_case_closure;
                user.user_permissions_change_settings = model.user_permissions_change_settings;
                user.status_id = model.status_id;
            }
            else
            {
                var glb = new GlobalFunctions();
                var generateModel = new GenerateRecordsModel();

                if (DB.user.Any(x => x.email.ToLower() == model.email.ToLower() && x.company_id == sessionUser.company_id && (x.role_id == ECLevelConstants.level_escalation_mediator || x.role_id == ECLevelConstants.level_mediator || x.role_id == ECLevelConstants.level_supervising_mediator)))
                {
                    return new
                    {
                        ok = false,
                        message = $"User with email '{model.email}' already exists!",
                    };
                }

                int language_id = 1;
                int location_id = model.company_location_id ?? 0;
                if (model.company_location_id == 0)
                {
                    List<company_location> company_locations = DB.company_location.Where(t => t.company_id == sessionUser.company_id && t.status_id == 2).OrderBy(t => t.id).ToList();
                    if (company_locations.Count > 0)
                        location_id = company_locations[0].id;
                }

                user = new user();
                user.first_nm = model.first_nm.Trim();
                user.last_nm = model.last_nm.Trim();
                user.company_id = sessionUser.company_id;
                if (sessionUser.role_id == 5)
                {
                    user.role_id = model.role_id;
                    user.status_id = model.status_id;
                    user.user_permissions_approve_case_closure = model.user_permissions_approve_case_closure;
                    user.user_permissions_change_settings = model.user_permissions_change_settings;
                }
                else
                {
                    //Activate if pendind
                    user.status_id = user.status_id == 3 ? 2 : user.status_id;
                }
                user.login_nm = generateModel.GenerateLoginName(user.first_nm, user.last_nm);
                var password = generateModel.GeneretedPassword().Trim();
                user.password = PasswordUtils.GetHash(password);
                user.photo_path = "";
                user.email = model.email.Trim();
                user.phone = "";
                user.preferred_contact_method_id = 1;
                user.title_ds = model.title_ds?.Trim();
                user.employee_no = "";
                user.company_department_id = model.company_department_id;
                user.question_ds = "";
                user.answer_ds = "";
                user.previous_login_dt = DateTime.Now;
                user.previous_login_dt = null;
                user.last_update_dt = DateTime.Now;
                user.user_id = 1;
                user.preferred_email_language_id = language_id;
                user.notification_messages_actions_flag = 1;
                user.notification_new_reports_flag = 1;
                user.notification_marketing_flag = 1;
                user.notification_summary_period = 1;
                user.company_location_id = location_id;
                user.location_nm = "";
                user.sign_in_code = null;
                user.guid = Guid.NewGuid();
                DB.user.Add(user);
                DB.SaveChanges();

                var company = DB.company.FirstOrDefault(x => x.id == sessionUser.company_id);
                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                eb.NewMediator(
                    $"{sessionUser.first_nm} {sessionUser.last_nm}",
                    $"{company.company_nm}",
                    HttpContext.Current.Request.Url.AbsoluteUri.ToLower(),
                    HttpContext.Current.Request.Url.AbsoluteUri.ToLower(),
                    $"{user.login_nm}",
                    $"{password}");
                string body = eb.Body;
                glb.SaveEmailBeforeSend(sessionUser.id, user.id, sessionUser.company_id, model.email, System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", "You have been added as a Case Administrator", body, false, 0);

            }
            DB.SaveChanges();

            return new
            {
                result = 0,
                ok = true,
            };
        }
    }
}