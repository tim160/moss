using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using EC.Models;
using EC.Models.Database;
using EC.Constants;

namespace EC.Controllers.utils
{
    public class AuthHelper
    {
        public static void SetCookies(user user, HttpContextBase httpContext)
        {
      /*      var ticket = new FormsAuthenticationTicket(
               1,
               user.login_nm,
               DateTime.Now,
               DateTime.Now.Add (FormsAuthentication.Timeout),
               true,
               string.Empty,
               FormsAuthentication.FormsCookiePath);
            var encTicket = FormsAuthentication.Encrypt(ticket);
            var AuthCookie = new HttpCookie(ECGlobalConstants.AuthUserCookies)
            {
                Value = encTicket,
                Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
            };
            httpContext.Response.Cookies.Set(AuthCookie);
            */
        }

 
    }
}