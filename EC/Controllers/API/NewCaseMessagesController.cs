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
    public class NewCaseMessagesController : BaseApiController
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

            var mediators = new CaseMessagesModel(id, 2, user.id);
            var reporters = new CaseMessagesModel(id, 1, user.id);

            var m = new
            {
                mediators = mediators.AllMessagesList.ToList(),
                reporters = reporters.AllMessagesList.ToList(),
                currentUser = new
                {
                    photo_path = user.photo_path
                },
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

            var message = new message();
            message.sender_id = user.id;
            message.created_dt = DateTime.Now;
            message.ip_ds = "";
            message.subject_ds = "";
            message.reporter_access = 2;
            message.body_tx = filter.NewMessage;
            message.report_id = filter.Report_id.Value;
            DB.message.Add(message);
            DB.SaveChanges();

            var m = new
            {
                retsult = true,
            };

            return ResponseObject2Json(m);
        }
    }
}