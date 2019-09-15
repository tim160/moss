using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using EC.Constants;
using EC.Models;
using EC.Models.Database;

namespace EC.Controllers.API
{

    public class SettingsCompanyClientController : BaseApiController
    {
        public class Filter
        {
            public int newClientId { get; set; }
        }
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public async Task<object> Post([FromBody]Filter filter)
        {
            user user = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return HttpStatusCode.InternalServerError;
            }
            var userCompany = DB.company.Where(c => c.id == user.company_id).FirstOrDefault();
            if (userCompany != null)
            {
                userCompany.client_id = filter.newClientId;
                DB.SaveChanges();
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.OK;
        }
    }
}