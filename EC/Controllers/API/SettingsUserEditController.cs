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
using EC.App_LocalResources;

namespace EC.Controllers.API
{
    public class SettingsUserEditController : BaseApiController
    {

        [HttpGet]
        public object Get(int id)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            var model = DB.user.FirstOrDefault(x => x.id == id);

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
                    user_permissions_approve_case_closure = model == null ? 0 : model.user_permissions_approve_case_closure,
                    user_permissions_change_settings = model == null ? 0 : model.user_permissions_change_settings,
                    status_id = model.status_id,
                }
            };
        }

        [HttpPost]
        public object Post(user model)
        {
            user curUser = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            var user = DB.user.FirstOrDefault(x => x.id == model.id);
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

                int language_id = 1;
                int location_id = 0;
                List<company_location> company_locations = DB.company_location.Where(t => ((t.status_id == 2))).OrderBy(t => t.id).ToList();
                if (company_locations.Count > 0)
                    location_id = company_locations[0].id;

                user = new user();
                user.first_nm = model.first_nm.Trim();
                user.last_nm = model.last_nm.Trim();
                user.company_id = curUser.company_id;
                user.role_id = model.role_id;
                //user.status_id = 2;
                user.status_id = model.status_id;
                user.login_nm = glb.GenerateLoginName(user.first_nm, user.last_nm);
                user.password = glb.GeneretedPassword().Trim();
                user.photo_path = "";
                user.email = model.email.Trim();
                user.phone = "";
                user.preferred_contact_method_id = 1;
                user.title_ds = model.title_ds.Trim();
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
                user.user_permissions_approve_case_closure = model.user_permissions_approve_case_closure;
                user.user_permissions_change_settings = model.user_permissions_change_settings;
                DB.user.Add(user);
            }
            DB.SaveChanges();

            return new
            {
                result = 0
            };
        }
    }
}