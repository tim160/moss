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
using System.IO;
using System.Net.Http.Headers;

namespace EC.Controllers.API
{
    public class EmployeeAwarenessPosterController : BaseApiController
    {
        public class Filter
        {
            public int posterId { get; set; }            
            public int type { get; set; }
            public int size { get; set; }
            public int logo1 { get; set; }
        }

        [HttpGet]
        public object Get(int id)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }

            var poster = DB.poster.FirstOrDefault(x => x.id == id);

            return new {
                //mainImage = Url.Content("~/Content/img/employeeAwarenessPoster.jpg")
                //mainImage = $"{poster.image_path}{poster.image_name}"
                mainImage = ($"/Upload/EAPdf/{poster.id}/1_short.jpg"),
            };
        }

        [HttpPost]
        public object Post([FromBody]Filter filter)
        {
            var poster = DB.poster.FirstOrDefault(x => x.id == filter.type);

            return new {
                file = ($"/Upload/EAPdf/{poster.id}/{filter.type}_{filter.size}_{filter.logo1}.pdf"),
                name = $"type:{filter.type} size:{filter.size} logo1:{filter.logo1}"
            };

        }
   }
}