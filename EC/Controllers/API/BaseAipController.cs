using EC.Models.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace EC.Controllers.API
{
    public class BaseAipController : ApiController
    {
        private ECEntities db = null;
        internal ECEntities DB
        {
            get
            {
                if (db == null)
                {
                    db = new ECEntities();
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;
                }

                return db;
            }
        }

        internal HttpResponseMessage ResponseObject2Json(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None
                });

            return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }
    }
}
