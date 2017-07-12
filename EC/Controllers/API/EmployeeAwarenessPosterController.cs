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
    public class EmployeeAwarenessPosterController : ApiController
    {

        [HttpGet]
        public object Get(int id)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            return new {
                mainImage = Url.Content("~/Content/img/employeeAwarenessPoster.jpg")
            };
        }

        [HttpPost]
        public object Post(int size, int logo)
        {
            return null;
        }
   }
}