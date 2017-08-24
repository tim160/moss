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
    public class SettingsCompanyRoutingController : BaseApiController
    {
        public class Filter
        {
            public int ReportFlag { get; set; }
        }
        public class PostModel
        {
            public company_case_routing Model { get; set; }

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

            var departments = DB.company_case_admin_department.Where(x => x.company_id == user.company_id && x.status_id == 2).ToList();
            var users = DB.user.Where(x => x.company_id == user.company_id && x.status_id == 2 && x.role_id == 6).ToList();
            var scopes = DB.scope.ToList();
            var items = DB.company_case_routing.Where(x => x.company_id == user.company_id).ToList();
            var types = DB.company_secondary_type.Where(x => x.company_id == user.company_id && x.status_id == 2).ToList();
            types.ForEach(x =>
            {
                var item = items.FirstOrDefault(z => z.company_secondary_type_id == x.id);
                if (item == null)
                {
                    items.Add(new company_case_routing
                    {
                        id = 0,
                        client_id = 1,
                        status_id = 2,
                        company_secondary_type_id = x.id,
                        company_case_admin_department_id = 0,
                        user_id = 0,
                        scope_id = 0,
                    });
                }
            });

            var m = new
            {
                types = types,
                departments = departments,
                users = users,
                scopes = scopes,
                items = items,
            };
            return ResponseObject2Json(m);
        }

        [HttpPost]
        public object Post(PostModel model)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0 || model.Model == null)
            {
                return Get();
            }

            UserModel um = new UserModel(user.id);

            company_case_routing item;
            /*if (model.DeleteId.HasValue)
            {
                item = DB.company_case_routing.FirstOrDefault(x => x.id == model.DeleteId);
                if (item != null)
                {
                    item.status_id = 1;
                    DB.SaveChanges();
                }
                return Get();
            }*/

            item = DB.company_case_routing
                .FirstOrDefault(x => x.client_id == 1 && x.company_id == user.company_id && x.company_secondary_type_id == model.Model.company_secondary_type_id);

            if (item != null)
            {
                item.company_case_admin_department_id = model.Model.company_case_admin_department_id;
                item.user_id = model.Model.user_id;
                item.scope_id = model.Model.scope_id;
                DB.SaveChanges();
            }
            else
            {

                item = new company_case_routing
                {
                    company_id = user.company_id,
                    client_id = 1,
                    status_id = 2,
                    company_case_admin_department_id = model.Model.company_case_admin_department_id,
                    user_id = model.Model.user_id,
                    scope_id = model.Model.scope_id,
                    company_secondary_type_id = model.Model.company_secondary_type_id,
                };
                DB.company_case_routing.Add(item);
                DB.SaveChanges();
            }

            return Get();
        }
    }
}