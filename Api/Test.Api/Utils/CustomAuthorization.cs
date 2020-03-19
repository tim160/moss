using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;

namespace TestApi.Utils
{
    public class CustomAuthorization : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,

                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    Message = "Authorization has been denied for this request."
                }), Encoding.UTF8, "application/json")
            };
        }
    }
}