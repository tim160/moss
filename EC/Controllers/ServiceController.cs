
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;

using EC.Business.Actions.Email;
using EC.Common.Util;
using EC.Constants;
using EC.Localization;
using EC.Models;
using EC.Models.Database;
using EC.Utils;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace EC.Controllers
{
    public class ServiceController : BaseController
    {
        public class LoginViewModel
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string HostUrl { get; set; }
            public string Email { get; set; }
        }

        private readonly UserModel userModel = new UserModel();
        private const string DEFAULT_LANGUAGE = "en-US";
        private static string FullNameLanguage = "English";
        // GET: Service
        public ActionResult Login(string host_url)
        {
            UserColorSchemaModel userColorSchema = new UserColorSchemaModel(null);
            ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
            ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
            ViewBag.is_sso_domain = is_sso_domain;
            //foreach (var user in db.user.Where(x => !x.password.EndsWith("=")).ToList())
            //{
            //       user.password = PasswordUtils.GetHash(user.password);
            //}
            //       db.SaveChanges();

            Session.Clear();

            if (is_sso_domain)
            {
                CompanyModel cm = new CompanyModel(3136);
                userColorSchema = new UserColorSchemaModel(cm.ID);
                ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
                ViewBag.clientLogo = cm.companyClientLogo();
                ViewBag.LogoCompany = cm.getLogoCompany(cm.ID);
            }

            return View($"Login{(is_cc ? "-CC" : "")}", new LoginViewModel { HostUrl = host_url });
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            UserColorSchemaModel userColorSchema = new UserColorSchemaModel(null);
            ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
            ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
            ViewBag.is_sso_domain = is_sso_domain;

            if (is_sso_domain)
            {
                CompanyModel cm = new CompanyModel(3136);
                userColorSchema = new UserColorSchemaModel(cm.ID);
                ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
                ViewBag.clientLogo = cm.companyClientLogo();
                ViewBag.LogoCompany = cm.getLogoCompany(cm.ID);
            }
            return DoLogin(model, returnUrl, "Login", false);
        }



        [HttpPost]
        public ActionResult CheckStatus(LoginViewModel model, string returnUrl)
        {
            Session.Clear();
            UserColorSchemaModel userColorSchema = new UserColorSchemaModel(null);
            ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
            ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
            ViewBag.is_sso_domain = is_sso_domain;

            if (is_sso_domain)
            {
                CompanyModel cm = new CompanyModel(3136);
                userColorSchema = new UserColorSchemaModel(cm.ID);
                ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
                ViewBag.clientLogo = cm.companyClientLogo();
                ViewBag.LogoCompany = cm.getLogoCompany(cm.ID);
            }
            return DoLogin(model, returnUrl, "CheckStatus", false);
        }

        private ActionResult DoLogin(LoginViewModel model, string returnUrl, string view, bool is_sso = false)
        {
            Session.Clear();
            Session[ECSessionConstants.SessionIsSSO] = "0";
            if (is_sso)
                Session[ECSessionConstants.SessionIsSSO] = "1";
            ///      if (DomainUtil.IsSubdomain(Request.Url.AbsoluteUri.ToLower()))
            {


                if (!String.IsNullOrEmpty(model.Login))
                {

                    var loginUser = userModel.Login(model.Login, model.Password, is_cc);


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

                        return View($"{view}{(is_cc ? "-CC" : "")}", model);
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
                        UserColorSchemaModel userColorSchema = new UserColorSchemaModel(user.company_id);

                        Session[ECGlobalConstants.APP_SETTING_HEADER_COLOR] = userColorSchema.global_Setting.header_color_code;
                        Session[ECGlobalConstants.APP_SETTING_HEADER_COLOR_LINK] = userColorSchema.global_Setting.header_links_color_code;

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
            ViewBag.DEFAULT_LANGUAGE = DEFAULT_LANGUAGE;
            ViewBag.fullNameLanguage = FullNameLanguage;
            ViewBag.is_sso_domain = is_sso_domain;

            UserColorSchemaModel userColorSchema = new UserColorSchemaModel(null);
            if (is_sso_domain)
            {
                CompanyModel cm = new CompanyModel(3136);
                userColorSchema = new UserColorSchemaModel(cm.ID);
                ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
                ViewBag.clientLogo = cm.companyClientLogo();
                ViewBag.LogoCompany = cm.getLogoCompany(cm.ID);
            }
            else
            {
                ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
            }

            return View($"Report{(is_cc ? "-CC" : "")}");
        }

        public ActionResult Disclaimer(string id, string companyCode)
        {

            ViewBag.is_sso_domain = is_sso_domain;
            var selectedCompany = GetCompanyModel(id, companyCode);
            if (selectedCompany == null)
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                UserColorSchemaModel userColorSchema = new UserColorSchemaModel(selectedCompany.ID);
                ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
                CompanyModel cm = new CompanyModel(selectedCompany.ID);
                ViewBag.clientLogo = cm.companyClientLogo();
            }
            return View($"Disclaimer{(is_cc ? "-CC" : "")}", selectedCompany);
        }

        private void SetSelectedCulture(string lang)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(lang);
                LocalizationGetter.Culture = Thread.CurrentThread.CurrentCulture;
            }
            catch (Exception)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(DEFAULT_LANGUAGE);
            }
        }
        private CompanyModel GetCompanyModel(string id, string companyCode)
        {

            var getDBEntityModel = new GetDBEntityModel();

            int company_id = 0;
            if (String.IsNullOrEmpty(id))
            {
                company_id = getDBEntityModel.GetCompanyByCode(companyCode);
            }
            else
            {
                company_id = getDBEntityModel.GetCompanyByCode(id);
            }
            var selectedCompanyModel = new CompanyModel(company_id);
            return selectedCompanyModel;
        }

        public ActionResult CheckStatus()
        {
            ViewBag.fullNameLanguage = FullNameLanguage;
            ViewBag.is_sso_domain = is_sso_domain;
            UserColorSchemaModel userColorSchema = new UserColorSchemaModel(null);
            if (is_sso_domain)
            {
                CompanyModel cm = new CompanyModel(3136);
                userColorSchema = new UserColorSchemaModel(cm.ID);
                ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
                ViewBag.clientLogo = cm.companyClientLogo();
                ViewBag.LogoCompany = cm.getLogoCompany(cm.ID);
            }
            else
            {
                ViewBag.header_color_code = userColorSchema.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColorSchema.global_Setting.header_links_color_code;
            }



            return View($"CheckStatus{(is_cc ? "-CC" : "")}", new LoginViewModel());
        }

        public ActionResult ForgetPassword()
        {
            Session.Clear();
            return View($"ForgetPassword{(is_cc ? "-CC" : "")}");
        }
        public ActionResult ForgetLogin()
        {
            Session.Clear();
            return View($"ForgetLogin{(is_cc ? "-CC" : "")}");
        }
        public string SendLogin(string email)
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
                            #region Email Sending

                            EmailManagement em = new EmailManagement(is_cc);
                            EmailBody eb = new EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                            eb.forgetLogin(_user.login_nm);

                            string body = eb.Body;

                            emailNotificationModel.SaveEmailBeforeSend(0, _user.id, _user.company_id, _user.email.Trim(),
                                System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                                LocalizationGetter.GetString("ForgotLogin", is_cc), body, false, 53);
                            #endregion
                            return LocalizationGetter.GetString("Success", is_cc).ToLower();
                        }
                        return LocalizationGetter.GetString("NoUserFound", is_cc);
                    }
                    return LocalizationGetter.GetString("NoUserFound", is_cc);
                }
                else
                    return LocalizationGetter.GetString("EmailInvalid", is_cc);
            }
            else
                return LocalizationGetter.GetString("EnterYourEmail", is_cc);
        }
        public string Email(string email, string login)
        {
            if (email != null && email.Length > 0 && login != null && login.Length > 0)
            {
                if (m_EmailHelper.IsValidEmail(email.Trim()))
                {
                    ECEntities db = new ECEntities();
                    if (db.user.Any(t => (t.email.ToLower().Trim() == email.ToLower().Trim()) &&
                    (t.login_nm.ToLower().Trim() == login.ToLower().Trim()) && (t.role_id != 8)))
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

                                EmailManagement em = new EmailManagement(is_cc);
                                EmailBody eb = new EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                                eb.ForgetPasswordNew(Request.Url.AbsoluteUri.ToLower(), email, password_token);

                                string body = eb.Body;

                                emailNotificationModel.SaveEmailBeforeSend(0, _user.id, _user.company_id, _user.email.Trim(),
                                    System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                                    LocalizationGetter.GetString("ChangePasswordRequest", is_cc), body, false, 53);
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                                return ex.ToString();
                            }
                            return LocalizationGetter.GetString("Success", is_cc).ToLower();
                        }
                    }
                    return LocalizationGetter.GetString("NoUserFound", is_cc);
                }
                else
                    return LocalizationGetter.GetString("EmailInvalid", is_cc);
            }
            else
                return LocalizationGetter.GetString("EnterYourEmail", is_cc);
        }

        LoginModel loginModel = new LoginModel();
        public ActionResult Restore(string email, string token)
        {
            if ((loginModel.restorePass(token, email).ToLower().Trim() == LocalizationGetter.GetString("Success", is_cc).ToLower().Trim()))
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
                Data = new
                {
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
                var unsend_emails = db.email.Where(x => x.is_sent == false && (x.email_freeze_send_dt == null || x.email_freeze_send_dt < DateTime.Now)).ToList();
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

                // we want to send emails in case user did not bought the license during 24 hours
                #region 3 Hours Notification
                var fourHours = DateTime.Now.AddHours(-4);
                var unfinished_var = db.var_info.Where(x => x.is_registered == false && x.created_dt < fourHours && x.SalesNotified != true).ToList();

                foreach (var var_info in unfinished_var)
                {
                    if (var_info.invitation_code != null)
                    {
                        // means user is
                        var company_invitation = db.company_invitation.Where(t => t.invitation_code == var_info.invitation_code).FirstOrDefault();
                        if (company_invitation != null)
                        {
                            /// need to add partner_id and replace company_id with it.
                            var partner = db.partner.Where(t => t.id == company_invitation.created_by_company_id).FirstOrDefault();
                            if (partner != null)
                            {
                                EmailBody eb_partner = new EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                                eb_partner.VarAfter4HoursAfterSignUp((body) =>
                                {
                                    body = body.Replace("[Referral Partner Name]", partner.first_nm);
                                    body = body.Replace("[Referenced Code]", var_info.invitation_code);
                                    body = body.Replace("[First Name]", var_info.first_nm);
                                    body = body.Replace("[Last Name]", var_info.last_nm);
                                    body = body.Replace("[Company Name]", var_info.company_nm);
                                    body = body.Replace("[Phone]", var_info.phone);
                                    body = body.Replace("[Email]", var_info.email);
                                    body = body.Replace("[Number of Employees]", $"{var_info.employee_no}");

                                    return body;
                                });
                                emailNotificationModel.SaveEmailBeforeSend(2, 2, 2, partner.email, "employeeconfidential@employeeconfidential.com", null, "Employee Confidential prospect follow-up needed", eb_partner.Body, false, 69);
                            }

                            var partnerEC = db.partner.Where(t => t.partner_code == "EC").FirstOrDefault();
                            if (partnerEC != null)
                            {
                                EmailBody eb_EC = new EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                                eb_EC.VarAfter4HoursAfterSignUpToSales((body) =>
                                {
                                    body = body.Replace("[First Name]", var_info.first_nm);
                                    body = body.Replace("[Last Name]", var_info.last_nm);
                                    body = body.Replace("[Company Name]", var_info.company_nm);
                                    body = body.Replace("[Phone]", var_info.phone);
                                    body = body.Replace("[Email]", var_info.email);
                                    body = body.Replace("[Number of Employees]", $"{var_info.employee_no}");
                                    body = body.Replace("[Referral Partner Code]", var_info.invitation_code);
                                    body = body.Replace("[Referral Partner Names]", partner.first_nm + " " + partner.last_nm);
                                    return body;
                                });
                                emailNotificationModel.SaveEmailBeforeSend(2, 2, 2, partnerEC.email.Trim(), "employeeconfidential@employeeconfidential.com", null, "Employee Confidential prospect follow-up", eb_EC.Body, false, 72);
                            }
                            var_info.SalesNotified = true;
                            db.SaveChanges();

                        }
                    }
                }
                #endregion

                #region 24 Hours Notification
                var twentyfourHours = DateTime.Now.AddHours(-25);
                var unfinished24_var = db.var_info.Where(x => x.is_registered == false && x.created_dt < twentyfourHours && x.User24HNotified != true).ToList();

                foreach (var var_info in unfinished24_var)
                {
                    if (var_info.invitation_code != null)
                    {
                        // means user is
                        var company_invitation = db.company_invitation.Where(t => t.invitation_code == var_info.invitation_code).FirstOrDefault();
                        if (company_invitation != null)
                        {
                            /// need to add partner_id and replace company_id with it.
                            var partner = db.partner.Where(t => t.id == company_invitation.created_by_company_id).FirstOrDefault();

                            if (partner == null)
                                partner = db.partner.Where(t => t.partner_code == "EC").FirstOrDefault();
                            if (partner != null)
                            {
                                EmailBody eb_partner = new EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                                var data = $"{var_info.first_nm}|{var_info.last_nm}|{var_info.company_nm}|{var_info.phone}|{var_info.email}|{var_info.employee_no}|{var_info.invitation_code}|{0}";

                                string url = "https://employeeconfidential.com/video/" + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data));


                                eb_partner.VarAfter24HoursAfterSignUpToUser((body) =>
                                {
                                    body = body.Replace("[Prospect First Name]", var_info.first_nm);
                                    body = body.Replace("[Phone]", partner.phone);
                                    body = body.Replace("[Email]", partner.email);
                                    body = body.Replace("[Referral Partner Company Name]", partner.partner_name);
                                    body = body.Replace("[Referral Partner contact name]", partner.first_nm + " " + partner.last_nm);
                                    body = body.Replace("[CaseUrl]", url);


                                    return body;
                                });
                                emailNotificationModel.SaveEmailBeforeSend(2, 2, 2, var_info.email, "employeeconfidential@employeeconfidential.com", null, "Employee Confidential follow-up", eb_partner.Body, false, 70);
                            }

                        }
                        var_info.User24HNotified = true;
                        db.SaveChanges();

                    }
                }

                #endregion

                #region 3 weeks Notification
                var days21 = DateTime.Now.AddDays(-21);
                var unfinished21_var = db.var_info.Where(x => x.is_registered == false && x.created_dt < days21 && x.User3WNotified != true).ToList();

                foreach (var var_info in unfinished21_var)
                {
                    if (var_info.invitation_code != null)
                    {
                        // means user is
                        var company_invitation = db.company_invitation.Where(t => t.invitation_code == var_info.invitation_code).FirstOrDefault();
                        if (company_invitation != null)
                        {
                            /// need to add partner_id and replace company_id with it.
                            var partner = db.partner.Where(t => t.id == company_invitation.created_by_company_id).FirstOrDefault();

                            if (partner == null)
                                partner = db.partner.Where(t => t.partner_code == "EC").FirstOrDefault();
                            if (partner != null)
                            {
                                EmailBody eb_partner = new EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                                var data = $"{var_info.first_nm}|{var_info.last_nm}|{var_info.company_nm}|{var_info.phone}|{var_info.email}|{var_info.employee_no}|{var_info.invitation_code}|{0}";

                                string url = "https://employeeconfidential.com/video/" + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(data));


                                eb_partner.VarAfter3WeekAfterSignUpToUser((body) =>
                                {
                                    body = body.Replace("[Prospect First Name]", var_info.first_nm);
                                    body = body.Replace("[Phone]", partner.phone);
                                    body = body.Replace("[Email]", partner.email);
                                    body = body.Replace("[Referral Partner Company Name]", partner.partner_name);
                                    body = body.Replace("[Referral Partner contact name]", partner.first_nm + " " + partner.last_nm);
                                    body = body.Replace("[CaseUrl]", url);


                                    return body;
                                });
                                emailNotificationModel.SaveEmailBeforeSend(2, 2, 2, var_info.email, "employeeconfidential@employeeconfidential.com", null, "Employee Confidential follow-up", eb_partner.Body, false, 71);
                            }

                        }
                        var_info.User3WNotified = true;
                        db.SaveChanges();

                    }
                }

                #endregion

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
        public ActionResult ChangeCulture(string lang, string fullNameLanguage = "English")
        {
            string returnUrl = Request.UrlReferrer.AbsolutePath;
            FullNameLanguage = fullNameLanguage;
            this.SetSelectedCulture(lang);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
            return Redirect(returnUrl);
        }



        static string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";

        [HttpPost]
        public ActionResult LoginSso(string jwt)
        {

            Session.Clear();

            //     return View($"Login{(is_cc ? "-CC" : "")}", new LoginViewModel {});


            string tokenString = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(tokenString);
            var jwtToken = handler.ReadJwtToken(tokenString);

            var tokenS = handler.ReadToken(tokenString) as JwtSecurityToken;
            var user_id = tokenS.Claims.First(claim => claim.Type == "user_id").Value;


            var temp = jwtToken.SigningKey;
            var jwtTokenS = handler.ReadToken(tokenString) as JwtSecurityToken;

            string token = Decode(tokenString, key, true);
            if (!string.IsNullOrWhiteSpace(user_id))
            {
                var user = db.user.Where(t => t.id.ToString() == user_id).FirstOrDefault();
                if (user != null)
                {
                    LoginViewModel model = new LoginViewModel();
                    model.Login = user.login_nm;
                    model.Password = user.password;

                    return DoLogin(model, "", "Login", true);
                }

                Response.Write("1" + tokenString);
                Response.Flush();
            }
            else
            {
                Response.Write("whitespace");
                Response.Flush();
            }

            Response.Write("whitespace");
            Response.Flush();
            return View($"Login{(is_cc ? "-CC" : "")}", new LoginViewModel { });
        }

        public ActionResult LoginSsoString(string jwt)
        {

            Session.Clear();

            string tokenString = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(tokenString);
            var jwtToken = handler.ReadJwtToken(tokenString);

            var tokenS = handler.ReadToken(tokenString) as JwtSecurityToken;
            var user_id = tokenS.Claims.First(claim => claim.Type == "user_id").Value;


            var temp = jwtToken.SigningKey;
            var jwtTokenS = handler.ReadToken(tokenString) as JwtSecurityToken;

            string token = Decode(tokenString, key, true);
            if (!string.IsNullOrWhiteSpace(user_id))
            {
                var user = db.user.Where(t => t.id.ToString() == user_id).FirstOrDefault();
                if (user != null)
                {
                    LoginViewModel model = new LoginViewModel();
                    model.Login = user.login_nm;
                    model.Password = user.password;

                    return DoLogin(model, "", "Login", true);
                }

                Response.Write("1" + tokenString);
                Response.Flush();
            }
            else
            {
                Response.Write("whitespace");
                Response.Flush();
            }

            Response.Write("whitespace");
            Response.Flush();
            return View($"Login{(is_cc ? "-CC" : "")}", new LoginViewModel { });
        }
        public static string Decode(string token, string key, bool verify = true)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var tokenHandler = new JwtSecurityTokenHandler();
            //byte[] symmetricKey = Convert.FromBase64String(key);
            //byte[] symmetricKey = (Encoding.UTF8.GetBytes(key));// Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature
            var symmetricKey = Convert.FromBase64String(key);
            var tokenValidationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,

                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(symmetricKey)
            };

            //  Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = new 
            var tokenSec = tokenHandler.ReadToken(token) as Microsoft.IdentityModel.Tokens.SecurityToken;
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out tokenSec);
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}