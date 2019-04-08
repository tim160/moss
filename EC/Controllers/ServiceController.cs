using EC.App_LocalResources;
using EC.Common.Util;
using EC.Constants;
using EC.Localization;
using EC.Models;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Business.Actions;
using EC.Utils;
using System.Globalization;

namespace EC.Controllers
{
    public class ServiceController : BaseController
    {
        public class LoginViewModel
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string HostUrl { get; set; }
        }

        private readonly UserModel userModel = new UserModel();

        // GET: Service
        public ActionResult Login(string host_url)
        {
            foreach(var user in db.user.Where(x => !x.password.EndsWith("=")).ToList())
            {
                user.password = PasswordUtils.GetHash(user.password);
            }
            db.SaveChanges();

            Session.Clear();
            return View($"Login{(is_cc ? "-CC" : "")}", new LoginViewModel { HostUrl = host_url });
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            return DoLogin(model, returnUrl, "Login");
        }

        [HttpPost]
        public ActionResult CheckStatus(LoginViewModel model, string returnUrl)
        {
            Session.Clear();
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

                    Session[ECGlobalConstants.CurrentUserMarcker] = user;
                    Session["userName"] = user.login_nm;
                    Session["userId"] = user.id;

                    if (!String.IsNullOrEmpty(model.HostUrl))
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

        public ActionResult Disclaimer(string id, string companyCode)
        {
            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Disclaimer", new { id = companyCode });
            }
            var c = db.company.FirstOrDefault(x => x.company_code == id);
            if (c == null)
            {
                return RedirectToAction("Index", "Index");
            }
            ViewBag.CID = id;
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

                                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
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
                ViewBag.email = email;
                ViewBag.token = token;
                ViewBag.Error = "Error: token mismatch. Please contact our customer support";
                return View($"Restore{(is_cc ? "-CC" : "")}");
                //return RedirectToAction("Login", "Service");
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
        public ActionResult Scheduler1(int? param1, int? varInfoId)
        {
 
            using (var db = new ECEntities())
            {
                var dt = new DateTime(2018, 06, 14);
                var reports = db.report.Where(x => x.reported_dt >= dt.Date).ToList();
                ReportModel rm = new ReportModel();
                string email = "";
                Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(is_cc);
                Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

                foreach (var _report in reports)
                {
                    rm = new ReportModel(_report.id);
                    if (
                        (rm.GetThisStepDaysLeft() <= 0) 
                        && (rm._last_investigation_status().investigation_status_id != 7) 
                        && (rm._last_investigation_status().investigation_status_id != 9)
                        )
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
                                        //em.Send(email, "Case Management Deadline is past due", eb.Body, true);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }

                var everyHourEmail = "timur160@gmail.com ";
                ActionResultExtended emailResult = null;
                foreach (var varinfo in db.var_info.Where(x => !x.registered_dt.HasValue || (varInfoId.HasValue && x.id == varInfoId)).ToList())
                {
                    try
                    {
                        //Every hours
                        eb.UserNotCompleteRegistration_Email((body) =>
                        {
                            return body;
                        });
                        //em.Send(everyHourEmail, "User not complete registration", eb.Body, true);

                        var invitation = db.company_invitation.FirstOrDefault(x => x.invitation_code == varinfo.invitation_code);
                        company company = null;
                        if (invitation != null)
                        {
                            company = db.company.FirstOrDefault(x => x.id == invitation.created_by_company_id);
                        }

                        //017-2
                        //trigger the email 4 hours after signup to company
                        if ((varinfo.CompanyNotified != true && (DateTime.Now - varinfo.created_dt).TotalHours >= 4) || (varinfo.id == varInfoId))
                        {
                            if (company != null)
                            {
                                eb.VarAfter4HoursAfterSignUp((body) =>
                                {
                                    body = body.Replace("[Referral Partner Name]", company.contact_nm);
                                    body = body.Replace("[Referenced Code]", varinfo.invitation_code);
                                    body = body.Replace("[First Name]", varinfo.first_nm);
                                    body = body.Replace("[Last Name]", varinfo.last_nm);
                                    body = body.Replace("[Company Name]", varinfo.company_nm);
                                    body = body.Replace("[Phone]", varinfo.phone);
                                    body = body.Replace("[Email]", varinfo.email);
                                    body = body.Replace("[Number of Employees]", $"{varinfo.employee_no}");

                                    return body;
                                });

                                emailResult = em.Send(everyHourEmail, "Employee Confidential prospect follow-up needed", eb.Body, true);
                                if (emailResult.ReturnCode == ReturnCode.Success)
                                {
                                    varinfo.CompanyNotified = true;
                                    db.SaveChanges();
                                }
                            }
                        }

                        //017-3
                        //trigger the email 4 hours after signup to sales
                        if ((varinfo.SalesNotified != true && (DateTime.Now - varinfo.created_dt).TotalHours >= 4) || (varinfo.id == varInfoId))
                        {
                            if (company != null)
                            {
                                eb.VarAfter4HoursAfterSignUpToSales((body) =>
                                {
                                    body = body.Replace("[First Name]", varinfo.first_nm);
                                    body = body.Replace("[Last Name]", varinfo.last_nm);
                                    body = body.Replace("[Company Name]", varinfo.company_nm);
                                    body = body.Replace("[Phone]", varinfo.phone);
                                    body = body.Replace("[Email]", varinfo.email);
                                    body = body.Replace("[Number of Employees]", $"{varinfo.employee_no}");
                                    body = body.Replace("[Referral Partner Code]", varinfo.invitation_code);
                                    body = body.Replace("[Referral Partner Names]", company.contact_nm);
                                    return body;
                                });

                                emailResult = em.Send("sales@employeeconfidential.com", "Employee Confidential prospect follow-up", eb.Body, true);
                                if (emailResult.ReturnCode == ReturnCode.Success)
                                {
                                    varinfo.SalesNotified = true;
                                    db.SaveChanges();
                                }
                            }
                        }

                        //017-4
                        //trigger the email 24 hours after signup to User
                        if ((varinfo.User24HNotified != true && (DateTime.Now - varinfo.created_dt).TotalHours >= 24) || (varinfo.id == varInfoId))
                        {
                            eb.VarAfter24HoursAfterSignUpToUser((body) =>
                            {
                                body = body.Replace("[Prospect First Name]", varinfo.first_nm);
                                body = body.Replace("[Phone]", varinfo.phone);
                                body = body.Replace("[Email]", varinfo.email);
                                body = body.Replace("[Referral Partner Company Name]", company.company_nm);
                                body = body.Replace("[Referral Partner contact name]", company.contact_nm);
                                return body;
                            });

                            emailResult = em.Send(varinfo.email, "Employee Confidential prospect follow-up", eb.Body, true);
                            if (emailResult.ReturnCode == ReturnCode.Success)
                            {
                                varinfo.SalesNotified = true;
                                db.SaveChanges();
                            }
                        }

                        //017-5
                        //trigger the email 3 weeks after signup to User
                        if ((varinfo.User24HNotified != true && (DateTime.Now - varinfo.created_dt).TotalHours >= 24) || (varinfo.id == varInfoId))
                        {
                            eb.VarAfter3WeekAfterSignUpToUser((body) =>
                            {
                                body = body.Replace("[Prospect First Name]", varinfo.first_nm);
                                body = body.Replace("[Phone]", varinfo.phone);
                                body = body.Replace("[Email]", varinfo.email);
                                body = body.Replace("[Referral Partner Company Name]", company.company_nm);
                                body = body.Replace("[Referral Partner contact name]", company.contact_nm);
                                return body;
                            });

                            emailResult = em.Send(varinfo.email, "Employee Confidential prospect follow-up", eb.Body, true);
                            if (emailResult.ReturnCode == ReturnCode.Success)
                            {
                                varinfo.SalesNotified = true;
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception exc)
                    {

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


    //[HttpPost]
    //EC.Windows.Task.Scheduler.exe http://localhost:8093/Service/Scheduler2?param1=197
    public ActionResult Scheduler2(int? param1)
    {

      using (var db = new ECEntities())
      {
        //logger.Info("Scheduler, is_cc" + is_cc.ToString());
        var unsend_emails = db.email.Where(x => x.is_sent == false).ToList();
        Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(is_cc);
        Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
        
      
        foreach (var _email in unsend_emails)
        {
          ActionResultExtended emailResult = em.Send(_email.To, _email.Title, _email.Body, true);
         
          //em.Send(email, "Case Management Deadline is past due", eb.Body, true);
          if (emailResult.ReturnCode == ReturnCode.Success)
          {
            _email.is_sent = true;
            _email.sent_dt = DateTime.Now;
            db.SaveChanges();
          }
        }
      }


      return new JsonResult
      {
        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        Data = new
        {
          ok = true,
        }
      };

    }

    /// <summary>
    /// Run this method each hour
    /// </summary>
    /// <param name="param1"></param>
    /// <returns></returns>
 /*   public ActionResult Scheduler_1HR(int? param1)
    {
      if (param1 == 1)
      {
        using (var db = new ECEntities())
        {
          //logger.Info("Scheduler, is_cc" + is_cc.ToString());
          var unsend_emails = db.email.Where(x => x.is_sent == false).ToList();
          Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(is_cc);
          Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());


          foreach (var _email in unsend_emails)
          {
            ActionResultExtended emailResult = em.Send(_email.To, _email.Title, _email.Body, true);

            //em.Send(email, "Case Management Deadline is past due", eb.Body, true);
            if (emailResult.ReturnCode == ReturnCode.Success)
            {
              _email.is_sent = true;
              _email.sent_dt = DateTime.Now;
              db.SaveChanges();
            }
          }
        }
      }

      return new JsonResult
      {
        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        Data = new
        {
          ok = true,
        }
      };

    }

    /// <summary>
    /// Run this method each 4 hours
    /// </summary>
    /// <param name="param1"></param>
    public ActionResult Scheduler_4HR(int? param1)
    {
      if (param1 == 4)
      {
        using (var db = new ECEntities())
        {
          //logger.Info("Scheduler, is_cc" + is_cc.ToString());
          var unsend_emails = db.email.Where(x => x.is_sent == false).ToList();
          Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(is_cc);
          Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());


          var everyHourEmail = "timur160@gmail.com ";
          ActionResultExtended emailResult = null;
          foreach (var varinfo in db.var_info.Where(x => !x.registered_dt.HasValue || (varInfoId.HasValue && x.id == varInfoId)).ToList())
          {
            try
            {
              
              var invitation = db.company_invitation.FirstOrDefault(x => x.invitation_code == varinfo.invitation_code);
              company company = null;
              if (invitation != null)
              {
                company = db.company.FirstOrDefault(x => x.id == invitation.created_by_company_id);
              }

              //017-2
              //trigger the email 4 hours after signup to company
              if ((varinfo.CompanyNotified != true && (DateTime.Now - varinfo.created_dt).TotalHours >= 4) || (varinfo.id == varInfoId))
              {
                if (company != null)
                {
                  eb.VarAfter4HoursAfterSignUp((body) =>
                  {
                    body = body.Replace("[Referral Partner Name]", company.contact_nm);
                    body = body.Replace("[Referenced Code]", varinfo.invitation_code);
                    body = body.Replace("[First Name]", varinfo.first_nm);
                    body = body.Replace("[Last Name]", varinfo.last_nm);
                    body = body.Replace("[Company Name]", varinfo.company_nm);
                    body = body.Replace("[Phone]", varinfo.phone);
                    body = body.Replace("[Email]", varinfo.email);
                    body = body.Replace("[Number of Employees]", $"{varinfo.employee_no}");

                    return body;
                  });

                  emailResult = em.Send(everyHourEmail, "Employee Confidential prospect follow-up needed", eb.Body, true);
                  if (emailResult.ReturnCode == ReturnCode.Success)
                  {
                    varinfo.CompanyNotified = true;
                    db.SaveChanges();
                  }
                }
              }

              //017-3
              //trigger the email 4 hours after signup to sales
              if ((varinfo.SalesNotified != true && (DateTime.Now - varinfo.created_dt).TotalHours >= 4) || (varinfo.id == varInfoId))
              {
                if (company != null)
                {
                  eb.VarAfter4HoursAfterSignUpToSales((body) =>
                  {
                    body = body.Replace("[First Name]", varinfo.first_nm);
                    body = body.Replace("[Last Name]", varinfo.last_nm);
                    body = body.Replace("[Company Name]", varinfo.company_nm);
                    body = body.Replace("[Phone]", varinfo.phone);
                    body = body.Replace("[Email]", varinfo.email);
                    body = body.Replace("[Number of Employees]", $"{varinfo.employee_no}");
                    body = body.Replace("[Referral Partner Code]", varinfo.invitation_code);
                    body = body.Replace("[Referral Partner Names]", company.contact_nm);
                    return body;
                  });

                  emailResult = em.Send("sales@employeeconfidential.com", "Employee Confidential prospect follow-up", eb.Body, true);
                  if (emailResult.ReturnCode == ReturnCode.Success)
                  {
                    varinfo.SalesNotified = true;
                    db.SaveChanges();
                  }
                }
              }

               
              

             
            }
            catch (Exception exc)
            {

            }
          }
        }
      }

      return new JsonResult
      {
        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        Data = new
        {
          ok = true,
        }
      };
    }

    /// <summary>
    /// Run this method each 24 hours
    /// </summary>
    /// <param name="param1"></param>
    public ActionResult Scheduler_24HR(int? param1)
    {
      if (param1 == 24)
      {
        using (var db = new ECEntities())
        {
          //logger.Info("Scheduler, is_cc" + is_cc.ToString());
          var unsend_emails = db.email.Where(x => x.is_sent == false).ToList();
          Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(is_cc);
          Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());


          foreach (var _email in unsend_emails)
          {
            ActionResultExtended emailResult = em.Send(_email.To, _email.Title, _email.Body, true);

            //em.Send(email, "Case Management Deadline is past due", eb.Body, true);
            if (emailResult.ReturnCode == ReturnCode.Success)
            {
              _email.is_sent = true;
              _email.sent_dt = DateTime.Now;
              db.SaveChanges();
            }
          }
        }
      }

      return new JsonResult
      {
        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        Data = new
        {
          ok = true,
        }
      };

    }
    */
    public ActionResult FreshDeskSSO()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login");

            return Redirect(FreshDesk.GetSsoUrl(Server,
               System.Configuration.ConfigurationManager.AppSettings["FreshDeskSite"],
               System.Configuration.ConfigurationManager.AppSettings["FreshDeskSecret"],
               user.login_nm, user.email));
        }
    }
}