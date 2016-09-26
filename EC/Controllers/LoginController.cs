using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Models;
using EC.Models.Database;
using EC.Controllers.utils;
using EC.App_LocalResources;
using System.Text.RegularExpressions;
//using EC.Common.Util;

namespace EC.Controllers
{
    public class LoginController : BaseController
    {
        private readonly UserModel userModel = UserModel.inst;
        LoginModel loginModel = new LoginModel();
        // GET: Login
        [HttpGet]
        public ActionResult Index(string loginField, string loginPass)
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if(is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            Session.Clear();
            GlobalFunctions glb = new GlobalFunctions();
            if (glb.IsSubdomain(Request.Url.AbsoluteUri.ToLower()))
            {
                if (loginField != null && loginField.Length > 0)
                {
                    var user = userModel.Login(loginField, loginPass);
                    SignIn(user);
                    Session["userName"] = "";
                    Session["userId"] = user.id;
                    if (user.role_id == 8)
                    {
                        //   return RedirectToAction("Index", "ReporterDashboard");
                        return Redirect("~/ReporterDashboard");
                    }
                    return Redirect("~/Cases");
                    //  return RedirectToAction("Index", "Cases");


                }
                return RedirectToAction("Company", "Login");
            }
            else
            {
                ViewBag.login = loginField;
                ViewBag.pass = loginPass;
                return View();
            }
        }
        public ActionResult Company()
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion
            ViewBag.LogoCompany = new EC.Models.CompanyModel().getLogoCompany();
            ViewBag.LogoPath = glb.LogoBaseUrl(Request.Url.AbsoluteUri.ToLower());

            Session.Clear();
            return View();
        }

        public ActionResult ForgetPassword()
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion
            Session.Clear();
            return View();
        }

        public string Email(string email)
        {
            if (email != null && email.Length > 0)
            {
                if (m_EmailHelper.IsValidEmail(email.Trim()))
                {
                    ECEntities db = new ECEntities();
                    if (db.user.Any(t => (t.email.ToLower().Trim() == email.ToLower().Trim()) && (t.role_id != 8)))
                    {
                        user _user = (db.user.Where(t => t.email.ToLower().Trim() == email.ToLower().Trim())).First();
                        if (_user != null)
                        {
                            string password_token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

                            user_change_password _ucp = new user_change_password();
                            _ucp.password_token = password_token;
                            _ucp.user_id = _user.id;
                            _ucp.user_ip = "";
                            _ucp.password_updated = 0;
                            _ucp.created_on = DateTime.Today;

                            db.user_change_password.Add(_ucp);
                            try
                            {
                                db.SaveChanges();

                                #region Email Sending

                                List<string> to = new List<string>();
                                List<string> cc = new List<string>();
                                List<string> bcc = new List<string>();

                                to.Add(email.Trim());
                                ///     bcc.Add("timur160@hotmail.com");

                                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                                eb.ForgetPassword(Request.Url.AbsoluteUri.ToLower(), email, password_token);
                                string body = eb.Body;
                                em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_ForgetPassword, body, true);

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                return ex.ToString();
                            }
                            return GlobalRes.Success.ToLower();
                        }
                    }
                    return GlobalRes.NoUserFound;
                }
                else
                    return GlobalRes.EmailInvalid;
            }
            else
                return GlobalRes.EnterYourEmail;
        }


     
        private void SignIn(user user)
        {
            AuthHelper.SetCookies(user, HttpContext);
            Session[Constants.CurrentUserMarcker] = user;
        }


        public string Login(string login, string password)
        {
            Session.Clear();
            GlobalFunctions glb = new GlobalFunctions();

            if (glb.IsSubdomain(Request.Url.AbsoluteUri.ToLower()))
            {
                if (login != null && login.Length > 0)
                {
                    var user = userModel.Login(login, password);
                    SignIn(user);
                    Session["userName"] = "";
                    Session["userId"] = user.id;
                    if (user.role_id == 8)
                    {
                        //   return RedirectToAction("Index", "ReporterDashboard");
                        return "ReporterDashboard";
                    }
                    return "Cases";
                    //  return RedirectToAction("Index", "Cases");


                }
                //  return RedirectToAction("Company", "Login");
                // AuthHelper.SetCookies(user, HttpContext);
                ///   Session[Constants.CurrentUserMarcker] = user;
            }
            return "";
        }

        public ActionResult Restore(string email, string token)
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            if ((loginModel.restorePass(token, email).ToLower().Trim() == App_LocalResources.GlobalRes.Success.ToLower().Trim()))
            {
                Session.Clear();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpPost]
        public ActionResult RestorePass(string email, string token)
        {
            Session.Clear();
            
            string result = loginModel.restorePass(token, email);
            if (result.ToLower() != "success") {
                ViewBag.error = result;
                return View("Restore");
            }
            else if (result.ToLower() == "success")
            {
                ViewBag.email = email;
                ViewBag.token = token;
            }
            return View();
        }
        [HttpPost]
        public ActionResult setNewPass(string email, string token, string password, string confirmPassword)
        {
            ViewBag.error = loginModel.setNewPass(email, token, password, confirmPassword);
            ViewBag.redirect = "true";
            return View("Restore");
        }
    }
}
