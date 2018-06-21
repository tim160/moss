using EC.App_LocalResources;
using EC.Common.Util;
using EC.Constants;
using EC.Controllers.utils;
using EC.Localization;
using EC.Models;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Business.Actions;

namespace EC.Controllers
{
    public class ServiceController : BaseController
    {
        public class LoginViewModel
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        private readonly UserModel userModel = UserModel.inst;

        // GET: Service
        public ActionResult Login()
        {
            return View($"Login{(is_cc ? "-CC" : "")}", new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            return DoLogin(model, returnUrl, "Login");
        }

        [HttpPost]
        public ActionResult CheckStatus(LoginViewModel model, string returnUrl)
        {
            return DoLogin(model, returnUrl, "CheckStatus");
        }

        private ActionResult DoLogin(LoginViewModel model, string returnUrl, string view)
        {
            Session.Clear();
            GlobalFunctions glb = new GlobalFunctions();

            if (DomainUtil.IsSubdomain(Request.Url.AbsoluteUri.ToLower()))
            {
                if (!String.IsNullOrEmpty(model.Login))
                {
                    var user = userModel.Login(model.Login, model.Password);
                    if (user == null)
                    {
                        ModelState.AddModelError("PasswordError", "Password");
                        return View($"{view}{(is_cc ? "-CC" : "")}", model);
                    }

                    AuthHelper.SetCookies(user, HttpContext);
                    Session[ECGlobalConstants.CurrentUserMarcker] = user;

                    Session["userName"] = "";
                    Session["userId"] = user.id;

                    if (user.role_id == ECLevelConstants.level_informant)
                    {
                        return RedirectToAction("Index", "ReporterDashboard");
                    }

                    if (returnUrl != null)
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
            model.Password = "";

            return View($"{view}{(is_cc ? "-CC" : "")}", model);
        }

        public ActionResult Report()
        {
            return View($"Report{(is_cc ? "-CC" : "")}");
        }

        public ActionResult Disclaimer(string companyCode)
        {
            var c = db.company.FirstOrDefault(x => x.company_code == companyCode);
            return View($"Disclaimer{(is_cc ? "-CC" : "")}", new CompanyModel(c.id));
        }

        public ActionResult CheckStatus()
        {
            return View($"CheckStatus{(is_cc ? "-CC" : "")}", new LoginViewModel());
        }

        public ActionResult ForgetPassword()
        {
            Session.Clear();
            return View($"ForgetPassword{(is_cc ? "-CC" : "")}");
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
                                eb.ForgetPasswordNew(Request.Url.AbsoluteUri.ToLower(), email, password_token);
                                string body = eb.Body;
                                em.Send(to, cc, App_LocalResources.GlobalRes.ChangePasswordRequest, body, true);

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
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

        LoginModel loginModel = new LoginModel();
        public ActionResult Restore(string email, string token)
        {
            if ((loginModel.restorePass(token, email).ToLower().Trim() == App_LocalResources.GlobalRes.Success.ToLower().Trim()))
            {
                Session.Clear();
                ViewBag.email = email;
                ViewBag.token = token;
                return View($"Restore{(is_cc ? "-CC" : "")}");
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
            if (result.ToLower() != "success")
            {
                ViewBag.error = result;
                return View("Restore");
            }
            else if (result.ToLower() == "success")
            {
                ViewBag.email = email;
                ViewBag.token = token;
            }
            return View($"RestorePass{(is_cc ? "-CC" : "")}");
        }

        [HttpPost]
        public ActionResult setNewPass(string email, string token, string password, string confirmPassword)
        {
            ViewBag.error = loginModel.setNewPass(email, token, password, confirmPassword);
            ViewBag.redirect = "true";
            return RedirectToAction("Login", "Service");// RedirectToAction("Company", "Login");
        }

        //[HttpPost]
        //EC.Windows.Task.Scheduler.exe http://localhost:8093/Service/Scheduler1?param1=197
        public ActionResult Scheduler1(int param1)
        {
            //Allow sec
            /*if (this.Request.UserHostAddress != "")
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        ok = false,
                    }
                };
            }*/

            using (var db = new ECEntities())
            {
                var dt = new DateTime(2018, 06, 14);
                var reports = db.report.Where(x => x.reported_dt >= dt.Date).ToList();
                ReportModel rm = new ReportModel();
                string email = "";
                Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement();
                Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

                foreach (var _report in reports)
                {
                    rm = new ReportModel(_report.id);
                    if (rm.GetThisStepDaysLeft() <= 0)
                    {
                        eb.Scheduler1(rm._report.display_name);
                        // days are exceeded - reminder never sent - need to send reminder
                        List<user> listOwners = rm.ReportOwnersUserList();
                      
                        foreach (var user in rm.MediatorsWhoHasAccessToReport())
                        {
                            if (listOwners.Contains(user) || user.role_id == ECLevelConstants.level_supervising_mediator)
                            {
                                email = user.email;
                               // email = "timur160@hotmail.com";
                                if ((email != null) && (email.Length > 0))
                                {
                                    try
                                    {
                                        em.Send(email, "Case Management Deadline is past due", eb.Body, true);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new {
                    ok = true,
                }
            };
        }
    }
}