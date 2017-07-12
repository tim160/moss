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
    public class EmployeeAwarenessController : BaseAipController
    {

        [HttpGet]
        public object Get()
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            var posters = DB.poster.AsNoTracking().ToList();
            var messages = DB.message_posters.AsNoTracking().ToList();

            try
            {
                var m = new
                {

                    posters = DB.poster
                        .ToList()
                        .Select(x => new {
                            poster = x,
                            message = messages.FirstOrDefault(z => z.id == x.poster_message_posters_id),
                            posterCategoryNames = DB.poster_industry_posters.Where(z => z.poster_id == x.id).ToList()
                        })
                        .ToList(),

                    categories = DB.industry_posters.ToList(),

                    messages = messages,

                    avaibleFormats = new[] {
                        new { Id = 1, Name = GlobalRes.AvailableFormat_1 },
                        new { Id = 2, Name = GlobalRes.AvailableFormat_2 },
                        new { Id = 3, Name = GlobalRes.AvailableFormat_3 },
                    },
                };

                return ResponseObject2Json(m);
            }
            catch(Exception exc)
            {
                return "EXC! " + exc.Message;
            }
        }
    }
}