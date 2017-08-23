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
    public class SettingsCompanySecondaryTypeController : BaseApiController
    {
        public class Filter
        {
            public int ReportFlag { get; set; }
        }
        public class PostModel
        {
            public string AddName { get; set; }
            public int AddType { get; set; }

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

            var third_level_types = DB.company_third_level_type
                    .Where(x => x.status_id == 2 & x.company_id == user.company_id);

            var m = new
            {
                secondaryTypes = DB.company_secondary_type
                    .Where(x => x.status_id == 2 & x.company_id == user.company_id)
                    .OrderBy(x => x.secondary_type_en)
                    .ToList(),

                third_level_types = third_level_types,

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

            company_third_level_type item;
            if (model.DeleteId.HasValue)
            {
                item = DB.company_third_level_type.FirstOrDefault(x => x.id == model.DeleteId);
                if (item != null)
                {
                    item.status_id = 1;
                    DB.SaveChanges();
                }
                return Get();
            }

            item = DB.company_third_level_type
                .FirstOrDefault(x => x.client_id == 1 && x.company_id == user.company_id && x.third_level_type_name_en.ToLower().Trim() == model.AddName.ToLower().Trim() && x.company_secondary_type_id == model.AddType);

            if (item != null)
            {
                item.status_id = 2;
                item.company_secondary_type_id = model.AddType;
                DB.SaveChanges();
            }
            else
            {

                item = new company_third_level_type
                {
                    company_id = user.company_id,
                    client_id = 1,
                    status_id = 2,
                    third_level_type_name_en = model.AddName.ToLower().Trim(),
                    third_level_type_name_es = "",
                    third_level_type_name_fr = "",
                    company_secondary_type_id = model.AddType,
                };
                DB.company_third_level_type.Add(item);
                DB.SaveChanges();
            }

            return Get();
        }
    }
}