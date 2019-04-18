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
using EC.Localization;

namespace EC.Controllers.API
{
    public class EmployeeAwarenessController : BaseApiController
    {

        [HttpGet]
        public object Get()
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            var posters = DB.poster.Where(t => t.status == 2).AsNoTracking().ToList();
            var messages = DB.message_posters.AsNoTracking().ToList();

            try
            {
                var m = new
                {

                    posters = DB.poster.Where(t => t.status == 2)
                        .ToList()
                        .Select(x => new {
                            poster = x,
                            message = messages.FirstOrDefault(z => z.id == x.poster_message_posters_id),
                            posterCategoryNames = DB.poster_industry_posters.Where(z => z.poster_id == x.id).ToList()
                        })
                        .ToList(),

                    categories = DB.industry_posters.Where(x => x.status == 2).ToList(),

                    messages = messages.Where(x => x.status == 2).ToList(),

                    languages = new[] {
                        new { Id = 1, Name = LocalizationGetter.GetString("English") },
                    },

                    avaibleFormats = new[] {
                        new { Id = 1, Name = LocalizationGetter.GetString("AvailableFormat_1")},
                      ///  new { Id = 2, Name = LocalizationGetter.GetString("AvailableFormat_2")},
                     ///   new { Id = 3, Name = LocalizationGetter.GetString("AvailableFormat_3")},
                    },
                };

                return ResponseObject2Json(m);
            }
            catch (Exception exc)
            {
                return "EXC! " + exc.Message;
            }
        }
    }
}