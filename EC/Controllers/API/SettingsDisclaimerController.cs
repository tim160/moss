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
            IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();
            var dt = model.company_disclamer_page.message_to_employees_created_dt.Value;
            var dt1 = model.company_disclamer_page.message_about_guidelines_created_dt.Value;

            return new
            {
                company_disclamer_page = model.company_disclamer_page,
                company_disclamer_page_date = dt.Day.ToString() + " " + m_DateTimeHelper.GetShortMonth(dt.Month) + " " + dt.Year.ToString(),
                company_disclamer_page_date1 = dt1.Day.ToString() + " " + m_DateTimeHelper.GetShortMonth(dt1.Month) + " " + dt1.Year.ToString(),

                company_disclamer_uploads = model.company_disclamer_uploads,
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
    }
}