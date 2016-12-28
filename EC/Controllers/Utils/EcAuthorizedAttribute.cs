using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using EC.Controllers;
using EC.Controllers.utils;
using EC.Models.Database;
using WebGrease.Css.Extensions;
using EC.Models;
using EC.Constants;


namespace EC.Utils.Auth
{
    public class EcAuthorizedAttribute : AuthorizeAttribute
    {
        private List<string> allowedRoles;

        public EcAuthorizedAttribute()
        {

        }

        public EcAuthorizedAttribute(string roles)
        {

            allowedRoles = roles.Split(',').Select(p => p.Trim()).ToList();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            user user = (user)httpContext.Session[ECSessionConstants.CurrentUserMarcker];
            if (user == null)
            {
                user = AuthHelper.GetCookies(httpContext);
            }
            var q =  UserIdentifyModel.inst.GetUserRole(user).ToLower();
            return user != null && allowedRoles.Any(role => role == UserIdentifyModel.inst.GetUserRole(user).ToLower());
        }

    }
}