using System;
using System.Collections.Generic;
using System.Linq;
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
    public class EmployeeAwarenessController : ApiController
    {

        [HttpGet]
        public object Get()
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            var um = new UserModel(user.id);
            var cm = new CompanyModel(um._user.company_id);
            var posters = cm.GetAllPosters();

            return new {
                posters = posters,
                categories = posters.SelectMany(x => x.posterCategoryNames).DistinctBy(x => x.Id).OrderBy(x => x.posterCategoryName),
                messages = new [] {
                    new { Id = 1, Name = GlobalRes.Message_1 },
                    new { Id = 2, Name = GlobalRes.Message_2 },
                    new { Id = 3, Name = GlobalRes.Message_3 },
                },
                avaibleFormats = new[] {
                    new { Id = 1, Name = GlobalRes.AvailableFormat_1 },
                    new { Id = 2, Name = GlobalRes.AvailableFormat_2 },
                    new { Id = 3, Name = GlobalRes.AvailableFormat_3 },
                },
                
            };
        }
    }
}