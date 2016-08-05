//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//
//namespace EC.Utils.Auth
//{
//    public class AuthHttpModule : IHttpModule
//    {
//        public static CookiesAuthentication auth = new CookiesAuthentication();
//        public void Init(HttpApplication context)
//        {
//            context.AuthenticateRequest += new EventHandler(this.Authenticate);
//        }
//
//        private void Authenticate(Object source, EventArgs e)
//        {
//            HttpApplication app = (HttpApplication)source;
//            HttpContext context = app.Context;
//
//           
//            auth.HttpContext = context;
//            context.User = auth.CurrentUser;
//        }
//
//        public void Dispose()
//        {
//        }
//    }
//
//   
//}