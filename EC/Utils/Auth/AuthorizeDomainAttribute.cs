using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Ajax.Utilities;

namespace EC.Utils.Auth
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            if (CheckDomain() || CheckHeader())
                return true;
            return false;
        }

        private bool CheckDomain()
        {
            var hostName = HttpContext.Current.Request.UrlReferrer?.Host ?? string.Empty;
            var allowedHosts = WebConfigurationManager.AppSettings["AllowedHosts"];

            if (string.IsNullOrEmpty(hostName) || string.IsNullOrEmpty(allowedHosts))
                return false;

            var allowedHostsList = allowedHosts.Split(';').Select(h => h.Trim());
            if (allowedHostsList.Contains(hostName))
            {
                return true;
            }

            return false;
        }

        private bool CheckHeader()
        {
            var authHeader = HttpContext.Current.Request.Headers["CustomAuthHeader"];
            var secret = WebConfigurationManager.AppSettings["AuthSecret"];

            if (string.IsNullOrEmpty(authHeader) || string.IsNullOrEmpty(secret))
                return false;
            if (authHeader == secret)
                return true;
            return false;
        }
    }
}