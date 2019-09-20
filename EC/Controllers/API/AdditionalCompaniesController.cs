using EC.Constants;
using EC.Models;
using EC.Models.Database;
using System.Linq;
using System.Web.Http;

namespace EC.Controllers.API
{
    public class AdditionalCompaniesController : BaseApiController
    {
        public object Get([FromUri] int ?id)
        {
            user user = (user)System.Web.HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            if (user == null || user.id == 0)
            {
                return null;
            }
            CompanyModel cm = null;
            if (id.HasValue && id.Value != 0)
            {
                cm = new CompanyModel(id.Value);
            } else
            {
                cm = new CompanyModel(user.company_id);
            }
            var additionalCompanies = cm.AdditionalCompanies();

            return new
            {
                additionalCompanies = additionalCompanies.Distinct().Select(m => new { m.id, m.company_nm }).ToList()
            };
        }
    }
}