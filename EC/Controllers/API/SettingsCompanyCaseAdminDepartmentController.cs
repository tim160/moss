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
using EC.Models.ViewModel;
using EC.Common.Interfaces;

namespace EC.Controllers.API
{
    public class SettingsCompanyCaseAdminDepartmentController : BaseApiController
    {
        public class Filter
        {
            public int ReportFlag { get; set; }
        }
        public class PostModel
        {
            public string AddName { get; set; }

            public int? DeleteId { get; set; }
        }

        [HttpGet]
        public object Get()
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            UserModel um = new UserModel(user.id);

            var case_admin_departments = DB.company_case_admin_department
                    .Where(x => x.status_id == 2 & x.company_id == user.company_id);

            var m = new
            {
                case_admin_departments = case_admin_departments,

            };
            return ResponseObject2Json(m);
        }

        [HttpPost]
        public object Post(PostModel model)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            UserModel um = new UserModel(user.id);

            company_case_admin_department item;
            if (model.DeleteId.HasValue)
            {
                item = DB.company_case_admin_department.FirstOrDefault(x => x.id == model.DeleteId);
                if (item != null)
                {
                    item.status_id = 1;
                    DB.SaveChanges();
                }
                return Get();
            }

            item = DB.company_case_admin_department
                .FirstOrDefault(x => x.client_id == 1 && x.company_id == user.company_id && x.name_en.ToLower().Trim() == model.AddName.ToLower().Trim());

            if (item != null)
            {
                item.status_id = 2;
                DB.SaveChanges();
            }
            else
            {

                item = new company_case_admin_department
                {
                    company_id = user.company_id,
                    client_id = 1,
                    status_id = 2,
                    name_en = model.AddName.ToLower().Trim(),
                    name_es = "",
                    name_fr = "",
                    name_ar = "",
                    name_ru = "",
                };
                DB.company_case_admin_department.Add(item);
                DB.SaveChanges();
            }

            return Get();
        }
    }
}