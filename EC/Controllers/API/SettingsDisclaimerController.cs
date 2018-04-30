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
using EC.Common.Interfaces;
using System.Threading.Tasks;

namespace EC.Controllers.API
{
    public class SettingsDisclaimerController : BaseApiController
    {
        [HttpGet]
        [Route("api/SettingsDisclaimer/Get")]
        public object Get()
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            var model = new
            {
                company_disclamer_page = DB.company_disclamer_page
                    .Include(x => x.user)
                    .Include(x => x.user1)
                    .FirstOrDefault(x => x.company_id == user.company_id),
                company_disclamer_page_date = "",
                company_disclamer_page_date1 = "",

                company_disclamer_uploads = DB.company_disclamer_uploads
                    .Where(x => x.company_id == user.company_id)
                    .ToList(),
            };

            var dt = DateTime.Now;
            var dt1 = DateTime.Now;
            if (model.company_disclamer_page != null)
            {
                model.company_disclamer_page.user = new user
                {
                    first_nm = model.company_disclamer_page.user1.first_nm,
                    last_nm = model.company_disclamer_page.user1.last_nm,
                };
                model.company_disclamer_page.user1 = new user
                {
                    first_nm = model.company_disclamer_page.user.first_nm,
                    last_nm = model.company_disclamer_page.user.last_nm,
                };
                if (model.company_disclamer_page.message_to_employees_created_dt.HasValue)
                {
                    dt = model.company_disclamer_page.message_to_employees_created_dt.Value;
                }
                if (model.company_disclamer_page.message_about_guidelines_created_dt.HasValue)
                {
                    dt1 = model.company_disclamer_page.message_about_guidelines_created_dt.Value;
                }
            }

            IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();

            model.company_disclamer_uploads.ForEach(x =>
            {
                if (x.user != null)
                {
                    x.user = new user
                    {
                        first_nm = x.user.first_nm,
                        last_nm = x.user.last_nm,
                    };
                }
                if (x.user1 != null)
                {
                    x.user1 = new user
                    {
                        first_nm = x.user1.first_nm,
                        last_nm = x.user1.last_nm,
                    };
                }
            });

            return new
            {
                company_disclamer_page = model.company_disclamer_page,
                company_disclamer_page_date = dt.Day.ToString() + " " + m_DateTimeHelper.GetShortMonth(dt.Month) + " " + dt.Year.ToString(),
                company_disclamer_page_date1 = dt1.Day.ToString() + " " + m_DateTimeHelper.GetShortMonth(dt1.Month) + " " + dt1.Year.ToString(),

                company_disclamer_uploads = model.company_disclamer_uploads,
                company_disclamer_uploads_dt = model.company_disclamer_uploads.Select(x => x.created_dt.Value.Day.ToString() + " " + m_DateTimeHelper.GetShortMonth(x.created_dt.Value.Month) + " " + x.created_dt.Value.Year.ToString())
            };
        }

        public class SaveModel
        {
            public string Message { get; set; }
        }

        [HttpPost]
        [Route("api/SettingsDisclaimer/Save1")]
        public object Save1(SaveModel message)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            var model = DB.company_disclamer_page
                    .FirstOrDefault(x => x.company_id == user.company_id);

            if (model == null)
            {
                model = new company_disclamer_page();
                model.company_id = user.company_id;
                DB.company_disclamer_page.Add(model);
            }
            model.message_to_employees = message.Message;
            model.message_to_employees_created_dt = DateTime.Now;
            model.message_to_employees_user_id = user.id;
            DB.SaveChanges();

            return new
            {
            };
        }

        [HttpPost]
        [Route("api/SettingsDisclaimer/Save2")]
        public object Save2(SaveModel message)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            var model = DB.company_disclamer_page
                    .FirstOrDefault(x => x.company_id == user.company_id);

            if (model == null)
            {
                model = new company_disclamer_page();
                model.company_id = user.company_id;
                DB.company_disclamer_page.Add(model);
            }
            model.message_about_guidelines = message.Message;
            model.message_about_guidelines_created_dt = DateTime.Now;
            model.message_about_guidelines_user_id = user.id;
            DB.SaveChanges();

            return new
            {
            };
        }

        [HttpPost]
        [Route("api/SettingsDisclaimer/Upload")]
        public object Upload()
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }
            var company = DB.company.FirstOrDefault(x => x.id == user.company_id);

            string root = HttpContext.Current.Server.MapPath($"~/Upload/Company/{company.guid}/Disclaimers/");
            if (!System.IO.Directory.Exists(root))
            {
                System.IO.Directory.CreateDirectory(root);
            }

            foreach (var fileId in HttpContext.Current.Request.Files.AllKeys)
            {
                var file = HttpContext.Current.Request.Files[fileId];
                var fi = new System.IO.FileInfo(file.FileName);

                var id = Guid.NewGuid();
                file.SaveAs($"{root}{id}{fi.Extension}");

                var fileDB = new company_disclamer_uploads
                {
                    company_id = user.company_id,
                    created_by_user_id = user.id,
                    created_dt = DateTime.Now,
                    display_ext = fi.Extension,
                    display_name = fi.Name,
                    file_path = $"/Upload/Company/{company.guid}/Disclaimers/{id}{fi.Extension}",
                    last_update_dt = DateTime.Now,
                    last_update_user_id = user.id,
                    status_id = 2,
                };

                DB.company_disclamer_uploads.Add(fileDB);
                DB.SaveChanges();
            }

            return new
            {
            };
        }

        [HttpPost]
        [Route("api/SettingsDisclaimer/DeleteFile")]
        public object DeleteFile(company_disclamer_uploads file)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            var fileDB = DB.company_disclamer_uploads.FirstOrDefault(x => x.company_id == user.company_id && x.Id == file.Id);
            DB.company_disclamer_uploads.Remove(fileDB);
            DB.SaveChanges();

            return new
            {
            };
        }

        [HttpPost]
        [Route("api/SettingsDisclaimer/SaveFile")]
        public object SaveFile(company_disclamer_uploads file)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return null;
            }

            var fileDB = DB.company_disclamer_uploads.FirstOrDefault(x => x.company_id == user.company_id && x.Id == file.Id);
            fileDB.Description = file.Description;
            DB.SaveChanges();

            return new
            {
            };
        }
    }
}