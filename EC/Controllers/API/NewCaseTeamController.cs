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
using EC.Models.App.Case;

namespace EC.Controllers.API
{
    public class NewCaseTeamController : BaseApiController
    {
        public class Filter
        {
            public int? ReportFlag { get; set; }
            public int? Report_id { get; set; }
            public string NewMessage { get; set; }
        }

        [HttpGet]
        public object Get(int id)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            UserModel um = new UserModel(user.id);
            var rm = new ReportModel(id);

            var m = new
            {
                involved_mediators_user_list = rm._involved_mediators_user_list,
                mediators_whoHasAccess_toReport = rm._mediators_whoHasAccess_toReport,
                available_toAssign_mediators = rm._available_toAssign_mediators,
                currentInfo = um._user,
            };

            return ResponseObject2Json(m);
        }

        [HttpPost]
        public object Post(Filter filter)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            var m = new
            {
            };

            return ResponseObject2Json(m);
        }
    }
}