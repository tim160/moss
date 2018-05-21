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
using System.Threading.Tasks;
using System.Collections.Specialized;
using Newtonsoft.Json;

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
            public company_case_routing_location ModelLocation { get; set; }            

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

            //var departments = DB.company_case_admin_department.Where(x => x.company_id == user.company_id && x.status_id == 2).ToList();
            var departments = DB.company_department
                .AsNoTracking()
                .Where(x => x.company_id == user.company_id && x.status_id == 2)
                .ToList();

            var users = DB.user
                .AsNoTracking()
                .Where(x => x.company_id == user.company_id && x.status_id == 2 && x.role_id == 6)
                .ToList();

            var scopes = DB.scope
                .AsNoTracking()
                .ToList();

            var items = DB.company_case_routing
                .AsNoTracking()
                .Where(x => x.company_id == user.company_id)
                .ToList();

            var types = DB.company_secondary_type
                .AsNoTracking()
                .Where(x => x.company_id == user.company_id && x.status_id == 2)
                .ToList();

            var ids = items
                .Select(x => x.id)
                .ToList();

            var files = DB.company_case_routing_attachments
                .AsNoTracking()
                .Where(x => ids.Contains(x.company_case_routing_id) & x.status_id == 2)
                .ToList();

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

            var locations = DB.company_location
                .AsNoTracking()
                .Where(x => x.company_id == user.company_id && x.status_id == 2)
                .OrderBy(x => x.location_en)
                .ToList();

            ids = locations.Select(x => x.id).ToList();
            var locationItems = DB.company_case_routing_location
                .AsNoTracking()
                .Where(x => ids.Contains(x.company_location_id) & x.company_id == user.company_id)
                .ToList();

            locations.ForEach(x =>
            {
                var item = locationItems.FirstOrDefault(z => z.company_location_id == x.id);
                if (item == null)
                {
                    locationItems.Add(new company_case_routing_location
                    {
                        id = 0,
                        client_id = 1,
                        status_id = 2,
                        company_location_id = x.id,
                        user_id = 0,
                        scope_id = 0,
                        company_id = user.company_id,
                    });
                }
            });

            var cm = new CompanyModel(user.company_id);

            var m = new
            {
                types = types,
                departments = departments,
                users = cm.AllMediators(user.company_id, true, null).Select(x => new {
                    id = x.id,
                    first_nm = x.first_nm,
                    last_nm = x.last_nm,
                }),
                scopes = scopes,
                items = items,
                files = files,
                locations = locations,
                locationItems = locationItems,
            };
            return ResponseObject2Json(m);
        }

        [HttpPost]
        public async Task<object> Post()
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return Get();
            }

            company_case_routing model;
            string jsonContent;
            if (Request.Content.IsMimeMultipartContent())
            {
                var parts = await Request.Content.ReadAsMultipartAsync();

                jsonContent = parts.Contents[0].ReadAsStringAsync().Result;
                model = JsonConvert.DeserializeObject<company_case_routing>(jsonContent);

                var file = HttpContext.Current.Request.Files[0];

                CompanyModel cm = new CompanyModel(user.company_id);
                var dir = System.Web.Hosting.HostingEnvironment.MapPath(String.Format("~/upload/Company/{0}", cm._company.guid));
                var filename = String.Format("{0}_{1}{2}", user.id, DateTime.Now.Ticks, System.IO.Path.GetExtension(file.FileName));
                
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                file.SaveAs(String.Format("{0}\\{1}", dir, filename));

                if (model.id == 0)
                {
                    var item = DB.company_case_routing
                        .FirstOrDefault(x => x.client_id == 1 && x.company_id == user.company_id && x.company_secondary_type_id == model.company_secondary_type_id);
                    if (item == null)
                    {
                        item = new company_case_routing
                        {
                            company_id = user.company_id,
                            company_secondary_type_id = model.company_secondary_type_id,
                            user_id = 0,
                            client_id = 1,
                            status_id = 2,
                            company_case_admin_department_id = 0,
                            scope_id = 0,
                        };
                        DB.company_case_routing.Add(item);
                        DB.SaveChanges();
                    }
                    model.id = item.id;
                }

                var itemDb = new company_case_routing_attachments
                {
                    company_case_routing_id = model.id,
                    status_id = 2,
                    user_id = user.id,
                    upload_dt = DateTime.Now,
                    file_nm = file.FileName,
                    extension_nm = System.IO.Path.GetExtension(file.FileName), 
                    path_nm = String.Format("\\upload\\Company\\{0}\\{1}", cm._company.guid, filename),
                };
                DB.company_case_routing_attachments.Add(itemDb);
                DB.SaveChanges();

                return Get();
            }

            jsonContent = Request.Content.ReadAsStringAsync().Result;
            PostModel pModel = JsonConvert.DeserializeObject<PostModel>(jsonContent);

            UserModel um = new UserModel(user.id);
            if (pModel.DeleteId.HasValue)
            {
                var file = DB.company_case_routing_attachments.FirstOrDefault(x => x.id == pModel.DeleteId.Value);
                file.status_id = 1;
                DB.SaveChanges();
            }

            if (pModel.Model != null)
            {
                company_case_routing item;

                item = DB.company_case_routing
                    .FirstOrDefault(x => x.client_id == 1 && x.company_id == user.company_id && x.company_secondary_type_id == pModel.Model.company_secondary_type_id);

                if (item != null)
                {
                    item.company_case_admin_department_id = pModel.Model.company_case_admin_department_id;
                    item.user_id = pModel.Model.user_id;
                    item.scope_id = pModel.Model.scope_id;
                    DB.SaveChanges();
                }
                else
                {

                    item = new company_case_routing
                    {
                        company_id = user.company_id,
                        client_id = 1,
                        status_id = 2,
                        company_case_admin_department_id = pModel.Model.company_case_admin_department_id,
                        user_id = pModel.Model.user_id,
                        scope_id = pModel.Model.scope_id,
                        company_secondary_type_id = pModel.Model.company_secondary_type_id,
                    };
                    DB.company_case_routing.Add(item);
                    DB.SaveChanges();
                }
            }

            if (pModel.ModelLocation != null)
            {
                company_case_routing_location item;
                item = DB.company_case_routing_location
                    .FirstOrDefault(x => x.client_id == 1 && x.company_id == user.company_id && x.company_location_id == pModel.ModelLocation.company_location_id);

                if (item != null)
                {
                    item.company_location_id = pModel.ModelLocation.company_location_id;
                    item.user_id = pModel.ModelLocation.user_id;
                    item.scope_id = pModel.ModelLocation.scope_id;
                    DB.SaveChanges();
                }
                else
                {

                    item = new company_case_routing_location
                    {
                        company_id = user.company_id,
                        client_id = 1,
                        status_id = 2,
                        company_location_id = pModel.ModelLocation.company_location_id,
                        user_id = pModel.ModelLocation.user_id,
                        scope_id = pModel.ModelLocation.scope_id,
                    };
                    DB.company_case_routing_location.Add(item);
                    DB.SaveChanges();
                }
            }

            return Get();
        }
    }
}