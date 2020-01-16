using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EC.Common.Util;
using EC.Common.Util.Authentication;
using EC.Common.Util.Models.API.v1.Token;
using EC.Constants;
using EC.Controllers.API;
using EC.Localization;
using EC.Models;
using EC.Models.Auth;
using EC.Services;
using EC.Utils;
using EC.Utils.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Tokens;
using static EC.Controllers.ServiceController;

namespace EC.Controllers
{
    public class OauthController : BaseController
    {
        private readonly MemoryCacheService memoryCacheService = new MemoryCacheService();
        private readonly UserModel userModel = new UserModel();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TokenLogin(string token)
        {
            var decode = Decode(token, key);
            var hash = decode.Claims.First(claim => claim.Type == "hash").Value;

            var userId = memoryCacheService.GetUser(hash);

            var _user = userModel.GetById(int.Parse(userId));
            if(_user != null)
            {
                LoginViewModel model = new LoginViewModel() { Login = _user.login_nm, Password = _user.password, Email = _user.email, HostUrl = "" };
                Session.Clear();
                return DoLoginToUser(model, null, "CheckStatus", false);
            }
            //if (!string.IsNullOrEmpty(userId))
            //{
            //    var user = UserManager.FindById(userId);
            //    SignInManager.SignIn(user, true, false);

            //    return RedirectToAction("Index", "Home");
            //}

            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You are not authorized to access.");
        }
        private ActionResult DoLoginToUser(LoginViewModel _user, string returnUrl, string view, bool is_sso = false)
        {
            Session.Clear();
            Session[ECSessionConstants.SessionIsSSO] = "0";
            if (is_sso)
                Session[ECSessionConstants.SessionIsSSO] = "1";
            if (DomainUtil.IsSubdomain(Request.Url.AbsoluteUri.ToLower()))
            {


                if (!String.IsNullOrEmpty(_user.Login))
                {

                    var loginUser = userModel.Login(_user.Login, _user.Password, is_cc);


                    if (loginUser == null || loginUser.user == null)
                    {
                        if (loginUser != null && loginUser.ErrorMessage != null)
                        {

                            ModelState.AddModelError("accountIsLocked", LocalizationGetter.GetString("AccountLocked", is_cc));
                        }
                        else
                        {
                            ModelState.AddModelError("PasswordError", "Password");
                        }

                        return View($"{view}{(is_cc ? "-CC" : "")}", _user);
                    }
                    var user = loginUser.user;
                    if (user.role_id == 10)
                    {
                        Session["id_agent"] = user.id;
                        return RedirectToAction("report", "service");
                    }

                    Session[ECGlobalConstants.CurrentUserMarcker] = user;
                    Session["userName"] = user.login_nm;
                    Session["userId"] = user.id;

                    if (!String.IsNullOrEmpty(_user.HostUrl))
                    {
                        return Redirect(FreshDesk.GetSsoUrl(Server,
                            System.Configuration.ConfigurationManager.AppSettings["FreshDeskSite"],
                            System.Configuration.ConfigurationManager.AppSettings["FreshDeskSecret"],
                            user.login_nm, user.email));
                    }

                    if (user.role_id == ECLevelConstants.level_informant)
                    {
                        return RedirectToAction("Index", "ReporterDashboard");
                    }

                    if (user.role_id == ECLevelConstants.level_trainer)
                    {
                        return RedirectToAction("Index", "Trainer");
                    }

                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    if (user.last_login_dt == null && user.role_id != ECLevelConstants.level_informant)
                    {
                        return RedirectToAction("Index", "Settings");
                    }

                    if (user.role_id == ECLevelConstants.level_escalation_mediator)
                    {
                        return RedirectToAction("Index", "Cases", new { mode = "completed" });
                    }
                    return RedirectToAction("Index", "Cases");
                }
            }

            ModelState.AddModelError("PasswordError", "Password");
            _user.Password = "";

            return View($"{view}{(is_cc ? "-CC" : "")}", _user);
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Token(TokenModel model)
        {
            if (model?.userId == null)
            {
                return new JsonResult()
                {
                    Data = new { errMsg = "error" },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
            }

            string guid = Guid.NewGuid().ToString();
            var token = GenerateToken(model.userId, guid);

            memoryCacheService.SetToken(guid, model.userId);

            return new JsonResult()
            {
                Data = new { token_type = "Bearer", access_token = token },
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }

        static string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";

        private static string GenerateToken(string user_id, string guid)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //  var tokenHandler = new Microsoft.IdentityModel.Tokens.TokenHandler();
            var now = DateTime.UtcNow;
            var symmetricKey = Convert.FromBase64String(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim( "user_id", user_id),
                new Claim( "hash", guid),
            }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }



        public JwtSecurityToken Decode(string token, string key, bool verify = true)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricKey = Convert.FromBase64String(key);
            var tokenValidationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,

                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(symmetricKey)
            };

            var tokenSec = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return tokenSec;
        }






















        //static string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
        //static string companyID = "123456";


        //[AllowAnonymous]
        //[HttpPost]
        //[Route]
        //public async Task<IHttpActionResult> Token([FromBody]TokenRequestModel tokenRequest)
        //{
        //  if (CheckCompany(tokenRequest.CompanyId, tokenRequest.SecretKey))
        //  {
        //    return ApiOk<string>(JwtManager.GenerateToken(tokenRequest.CompanyId));
        //  }

        //  throw new HttpResponseException(HttpStatusCode.Unauthorized);
        //}


        //private bool CheckCompany(string companyId, string secretKey)
        //{
        //  // TODO: should check in the database, merge with another CheckCompany
        //  if ((companyId == companyID) && (secretKey == key))
        //    return true;
        //  return false;
        //}


        /*  [HttpPost]
              public ActionResult Token(string jwt)
              {

            //{
                "access_token": < Your Token >,
           "token_type": "bearer",
           "expires_in": 7200,
           "created_at": 1429931390
            //}
            return View();
                }*/

    }
}