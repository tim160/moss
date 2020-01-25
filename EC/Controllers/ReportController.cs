using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EC.Models;
using EC.Models.Database;
using EC.Controllers.ViewModel;
using EC.Constants;
using EC.Common.Base;
using EC.Localization;
using Rotativa;
using EC.Models.ViewModels;
using EC.Models.Culture;
using EC.Utils;
using EC.Common.Util;
using System.Web.Configuration;

namespace EC.Controllers
{
    public class ReportController : BaseController
    {
        private readonly CompanyModel companyModel = new CompanyModel();
        private readonly ReportModel reportModel = new ReportModel();

        // GET: /Report/
        [HttpGet]
        public ActionResult New(string companyCode)
        {
            ModelState.Clear();

            ViewBag.displayAngular = !CheckBrowser.detectOldIE(Request.Browser.Type);

            int id = 0;
            if (companyCode != null)
            {
                var getDBEntityModel = new GetDBEntityModel();

                id = getDBEntityModel.GetCompanyByCode(companyCode);
                if (id == 0)
                {
                    return RedirectToAction("Index", "Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "Index");
                // esli u nas net company_code, znachit reportera nado redirect na index/company_selector
            }

            if (id > 0)
            {

                #region EC-CC Viewbag
                ViewBag.is_cc = is_cc;
                string cc_ext = "";
                if (is_cc) cc_ext = "_cc";
                ViewBag.cc_extension = cc_ext;
        #endregion

                CompanyModel model = new CompanyModel(id);
                company currentCompany = model.GetById(id);
                GetDBEntityModel getDBEntityModel = new GetDBEntityModel();
                ViewBag.currentCompanyId = currentCompany.id;

                /*caseInformation*/
                SecondaryMandatoryCulture secondaryMandatoryCulture = new SecondaryMandatoryCulture(reportModel, currentCompany.id);
                if (model.IsCustomIncidentTypes())
                {
                    /*custom types*/
                    ViewBag.secondary_type_mandatory = secondaryMandatoryCulture.GetSecondaryMandCustom();
                    ViewBag.CustomSecondaryType = true;
                }
                else
                {
                    /*default*/
                    ViewBag.secondary_type_mandatory = secondaryMandatoryCulture.getSecondaryTypeMandatory();
                    ViewBag.CustomSecondaryType = false;
                }

                ViewBag.currentCompanySubmitted = currentCompany.company_nm;
                ViewBag.currentCompany = currentCompany.company_nm;
                ViewBag.locations = HtmlDataHelper.MakeSelect(companyModel.Locations(id).Where(t => t.status_id == 2).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
                ManagamentKnowCulture managamentKnowCulture = new ManagamentKnowCulture(companyModel);
                ViewBag.managament = managamentKnowCulture.GetManagamentKnowCulture();
                ViewBag.frequencies = HtmlDataHelper.MakeSelect(getDBEntityModel.getFrequencies(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("description")));
                List<country> arr = getDBEntityModel.getCountries();
                ViewBag.countries = HtmlDataHelper.MakeSelect(arr, item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.country_nm.ToString()));

                arr.ForEach(t => t.country_description = (string.IsNullOrWhiteSpace(t.country_cl)  ? LocalizationGetter.GetString("YesAnon", is_cc) : LocalizationGetter.GetString(t.country_cl.Trim(), is_cc)));
                arr.ForEach(t => t.country_description_en = (string.IsNullOrWhiteSpace(t.country_cl) ? LocalizationGetter.GetString("YesAnon", is_cc) : LocalizationGetter.GetString(t.country_cl.Trim(), is_cc)));
                arr.ForEach(t => t.country_description_es = (string.IsNullOrWhiteSpace(t.country_cl) ? LocalizationGetter.GetString("YesAnon", is_cc) : LocalizationGetter.GetString(t.country_cl.Trim(), is_cc)));

                ViewBag.countriesDescription = arr;
                ReportedOutsideCulture reportedOutsideCulture = new ReportedOutsideCulture(companyModel);
                ViewBag.reportedOutsides =reportedOutsideCulture.getReportedOutside();

                RoleInReportCulture roleInReportCulture = new RoleInReportCulture(db, is_cc);
                ViewBag.selectedRoleInReport = roleInReportCulture.getRoleInReportCultureSelect();

                AnonimityCulture viewModelAnonimity = new AnonimityCulture(currentCompany, companyModel);
                ViewBag.anonimity = viewModelAnonimity.getAnonimityCultrure();

                /*Relationship to company*/
                RelationshipCulture relationshipCulture = new RelationshipCulture(companyModel);

                CompanyModel cm = new CompanyModel(ViewBag.currentCompanyId);
                List<company_relationship> relationship = cm.GetCustomRelationshipCompany();
                if (relationship.Count > 0)
                {
                    //loadCustom
                    ViewBag.relationship = relationshipCulture.getOtherRelationshipCulture(reportModel, currentCompany.id);
                }
                else
                {
                    ViewBag.relationship = relationshipCulture.getRelationshipCulture();
                }

                DepartmentCulture departmentCulture = new DepartmentCulture(companyModel, currentCompany.id);
                ViewBag.departments = departmentCulture.GetDepartmentsCultureSelect();

                CompanyLocationCulture locationCulture = new CompanyLocationCulture(companyModel, currentCompany.id);
                ViewBag.locationsOfIncident = locationCulture.getLocationsCompanyCultureSelect();

                InjuryDamageCulture injuryDamageCulture = new InjuryDamageCulture(companyModel);
                ViewBag.injury_damage = injuryDamageCulture.getInjuryDamageCulture();

                ViewBag.supervisingMediators = companyModel.AllSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));
                ViewBag.nonSupervisingMediators = companyModel.AllNonSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));

                // company logo
                string companyLogo = currentCompany.path_en;
                string path = System.IO.Directory.GetCurrentDirectory();
                if (!System.IO.Directory.Exists(path + companyLogo))
                {
                    ViewBag.companylogo = companyLogo;
                }
                else
                {
                    ViewBag.companyLogo = null;
                }

                var userColors = new UserColorSchemaModel(currentCompany.id);
                ViewBag.header_color_code = userColors.global_Setting.header_color_code;
                ViewBag.header_links_color_code = userColors.global_Setting.header_links_color_code;
            }
            else
            {
                // esli u nas net company_code, znachit reportera nado redirect na index/company_selector
                return RedirectToAction("index", "Index");
            }

            return View();
        }
        [HttpPost]
        public ActionResult New(ReportViewModel model)
        {
            if (Session["id_agent"] != null)
            {
                model.agentId = (int)Session["id_agent"];
            }
            ViewBag.displayAngular = !CheckBrowser.detectOldIE(Request.Browser.Type);

            var cm = new CompanyModel(model.currentCompanyId);
            model.Process(Request.Form, Request.Files);
            string password;

            ReportAddModel report = new ReportAddModel();
            var currentReport = report.AddReport(model, out password);

            ReportSubmit submit = new ReportSubmit();
            submit.result = new ReportModelResult();
            submit.result.StatusCode = currentReport.StatusCode;
            submit.result.ErrorMessage = currentReport.ErrorMessage;
            ViewBag.companylogo = cm._company.path_en;


            if (currentReport.StatusCode == 200)
            {
                ViewBag.CaseNumber = currentReport.report.display_name;//Request.Form["caseNumber"];model.caseNumber
                if (currentReport.report.user_id > 0)
                {
                    UserModel um = new UserModel(currentReport.report.user_id);
                    var user = um._user;
                    ViewBag.UserId = user.id;
                    /*model.userName = */
                    ViewBag.Login = user.login_nm;
                    /*model.password = */
                    ViewBag.Password = password;
                    /*model.userEmail = */
                    ViewBag.Email = user.email;
                    SignIn(user);

                    base.logModel.UpdateReportLog(user.id, 2, currentReport.report.id, "", null, "");
                    base.logModel.UpdateReportLog(user.id, 28, currentReport.report.id, LocalizationGetter.GetString("_Started", is_cc), null, "");
                }
               

                #region SendEmail To Admins
                ReportModel rm = new ReportModel(currentReport.report.id);
                ViewBag.ReportModel = rm;

                Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(is_cc);
                Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                bool has_involved = false;
                if (rm.InvolvedMediatorsUserList().Count > 0)
                    has_involved = true;

                string body = "";
                string title = LocalizationGetter.GetString("Email_Title_NewCase", is_cc);
                if (has_involved)
                    title = LocalizationGetter.GetString("Email_Title_NewCaseInvolved", is_cc);
                foreach (var _user in rm.MediatorsWhoHasAccessToReport().Where(t => t.role_id != ECLevelConstants.level_escalation_mediator).ToList())
                {
                    eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                    int email_type = 0;
                    if (has_involved)
                    {
                        eb.NewCaseInvolved(_user.first_nm, _user.last_nm, rm._report.display_name);
                        email_type = 4;
                    }
                    else
                    {
                        eb.NewCase(_user.first_nm, _user.last_nm, rm._report.display_name);
                        email_type = 3;
                    }

                    body = eb.Body;
                    emailNotificationModel.SaveEmailBeforeSend(0, _user.id, cm._company.id, _user.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", title, body, false, email_type);
                }
                #endregion
            }
            else
            {
                ViewBag.UserId = 0;
                ViewBag.Login = "";
                ViewBag.Password = "";
                ViewBag.Email = "";
                ViewBag.ReportModel = new ReportModel();
                ViewBag.CaseNumber = 0;
                ViewBag.company_code = cm._company.company_code;
            }

            return View("CaseSubmitted");
        }

 
        [HttpPost]
        public ActionResult SaveLoginChanges(int userId, string pass)
        {
            JsonResult json = new JsonResult();
            string result = "false";

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                json.Data = result;
                return json;
            }

            
            if (user.id == userId)
            {
                LoginModel lm = new LoginModel();
                result = lm.SaveLoginChanges(userId, pass);
                if (result.ToLower() == "success")
                {
                    SignIn(db.user.Find(userId));
                }
            }
            
            json.Data = result;
            return json;
        }

        public void SignIn(user user)
        {
            Session[ECGlobalConstants.CurrentUserMarcker] = user;
        }

        public List<anonymity> GetAnon(int company_id, int country_id)
        {
            List<anonymity> list_anon = null;
            if (company_id != null && country_id != null)
            {
                var currentCompany = new CompanyModel().GetById(company_id);
                list_anon = companyModel.GetAnonymities(company_id, country_id);
                foreach (anonymity _anon in list_anon)
                {
                    _anon.anonymity_company_en = string.Format(_anon.anonymity_company_en, currentCompany.company_nm);
                    _anon.anonymity_ds_en = string.Format(_anon.anonymity_ds_en, currentCompany.company_nm);
                    _anon.anonymity_en = string.Format(_anon.anonymity_en, currentCompany.company_nm);
                }

            }
            return list_anon;
        }
        public JsonResult getAjaxCountry(int countryId)
        {
            JsonResult result_company = new JsonResult();
            //bool result = glb.isCompanyInUse(IdCountry);
            return result_company;
        }

        public ActionResult PrintToPdf(Guid id, bool pdf = true)
        {
            var report = db.report.FirstOrDefault(x => x.guid == id);
            var rm = new ReportModel(report.id);
            if (pdf)
            {
                var fn = $"Report to {rm.CompanyName()}";
                //return new ActionAsPdf("PrintToPdf", new { id = id, pdf = false }) { FileName = fn };
                return new ActionAsPdf("PrintToPdf", new { id = id, pdf = false }) {  };
            }

            ViewBag.Roles = db.role_in_report.ToList();
            return View(rm);
        }

        public ActionResult Company(string id, string backUrl)
        {
            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Report", "Service");
            }
            var c = db.company.FirstOrDefault(x => x.company_short_name != null && x.company_short_name.ToLower() == id.ToLower());
            if (c == null)
            {
                return RedirectToAction("Report", "Service");
            }
            return RedirectToAction("Disclaimer", "Service", new { id = c.company_code });
        }
        public ActionResult RedirectToReporterDashboard(int reportId)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            #region send email to user
            reportModel.ID = reportId;
            if (user != null)
            {
                if(reportModel._report != null && (reportModel._report.incident_anonymity_id == 2 || reportModel._report.incident_anonymity_id == 3))
                {
                    Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(is_cc);
                    Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

                    eb.ReporterNewCase(user.login_nm, user.password, reportModel._report.display_name);
                    emailNotificationModel.SaveEmailBeforeSend(0, user.id, user.company_id, user.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"],
                        "", LocalizationGetter.GetString("Email_Title_NewCase", is_cc), eb.Body, false, 30);
                }
            } else
            {
                return RedirectToAction("Login", "Service");
            }
            #endregion
            return RedirectToAction("Index", "ReporterDashboard");
        }
    }
}