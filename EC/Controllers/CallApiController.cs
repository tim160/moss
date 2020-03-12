using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EC.Controllers
{
    public class CallApiController : Controller
    {
        static string isURL = ConfigurationManager.AppSettings.Get("IdentityServerUrl").TrimEnd('/');
        static string apiUrl = ConfigurationManager.AppSettings.Get("ApiUrl").TrimEnd('/');

        public ActionResult Index()
        {
            return View();
        }

        // GET: CallApi/ClientCredentials
        public async Task<ActionResult> ClientCredentials()
        {
            var response = await GetTokenAsync();
            var result = await CallApi(response.AccessToken);

            ViewBag.Json = result;
            return View("ShowApiResult");
        }
        public async Task<ActionResult> UserCredentials()
        {
            var user = User as ClaimsPrincipal;
            var token = user.FindFirst("access_token").Value;
            var result = await CallApi(token);

            ViewBag.Json = result;
            return View("ShowApiResult");
        }
        private async Task<string> CallApi(string token)
        {
            var client = new HttpClient();
            client.SetBearerToken(token);

            var json = await client.GetStringAsync(apiUrl + "/identity");
            return JArray.Parse(json).ToString();
        }

        private async Task<TokenResponse> GetTokenAsync()
        {
            var client = new TokenClient(
                isURL + "/identity/connect/token",
                "mvc_service",
                "secret");

            return await client.RequestClientCredentialsAsync("sampleApi");
        }
    }
}