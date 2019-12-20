using EC.Models.Database;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using EC.Common.Util;
using EC.Models;
using Newtonsoft.Json;

namespace EC.Controllers.API
{
    //[AuthFilterApi]
    public class BaseApiController : ApiController
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

        internal bool is_cc
        {
            

                   get
            {
                return DomainUtil.IsCC(HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
            }
        }
        public EmailNotificationModel emailNotificationModel = new EmailNotificationModel();
  }
}
