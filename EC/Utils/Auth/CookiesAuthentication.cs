//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Web;
//using System.Web.Security;
//
//
//namespace EC.Utils.Auth
//{
//    public class CookiesAuthentication
//    {
//        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
//
//        private const string cookieName = "__AUTH_COOKIE";
//
//        public HttpContext HttpContext { get; set; }
//
//        public Repo Repository = new Repo();
//
//        #region IAuthentication Members
//
//        public User Login(string userName, string Password, bool isPersistent)
//        {
//            User retUser = Repository.Login(userName, Password);
//            if (retUser != null)
//            {
//                CreateCookie(userName, isPersistent);
//            }
//            return retUser;
//        }
//
//        public User Login(string userName)
//        {
//            User retUser = new User()
//            {
//                Login = "e",
//                Email = "email",
//                Password = "psw"
//            
//            };//Repository.Users.FirstOrDefault(p => string.Compare(p.Email, userName, true) == 0);
//            if (retUser != null)
//            {
//                CreateCookie(userName);
//            }
//            return retUser;
//        }
//
//        private void CreateCookie(string userName, bool isPersistent = false)
//        {
//            var ticket = new FormsAuthenticationTicket(
//                  1,
//                  userName,
//                  DateTime.Now,
//                  DateTime.Now.Add(FormsAuthentication.Timeout),
//                  isPersistent,
//                  string.Empty,
//                  FormsAuthentication.FormsCookiePath);
//
//            // Encrypt the ticket.
//            var encTicket = FormsAuthentication.Encrypt(ticket);
//
//            // Create the cookie.
//            var AuthCookie = new HttpCookie(cookieName)
//            {
//                Value = encTicket,
//                Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
//            };
//            HttpContext.Response.Cookies.Set(AuthCookie);
//        }
//
//        public void LogOut()
//        {
//            var httpCookie = HttpContext.Response.Cookies[cookieName];
//            if (httpCookie != null)
//            {
//                httpCookie.Value = string.Empty;
//            }
//        }
//
//        private IPrincipal currentUser;
//
//        public IPrincipal CurrentUser
//        {
//            get
//            {
//                if (currentUser == null)
//                {
//                    try
//                    {
//                        HttpCookie authCookie = HttpContext.Request.Cookies.Get(cookieName);
//                        if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
//                        {
//                            var ticket = FormsAuthentication.Decrypt(authCookie.Value);
//                            ticket.Name
//                            currentUser = new UserProvider(ticket.Name, Repository);
//                        }
//                        else
//                        {
//                            currentUser = new UserProvider(null, null);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        logger.Error("Failed authentication: " + ex.Message);
//                        currentUser = new UserProvider(null, null);
//                    }
//                }
//                return currentUser;
//            }
//        }
//        #endregion
//    }
//}