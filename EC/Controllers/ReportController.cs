﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.DynamicData.ModelProviders;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EC.Controllers.Utils;
using EC.Models;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Controllers.ViewModel;
using EC.Constants;
using EC.Common.Base;
using EC.Localization;
using Rotativa.MVC;

namespace EC.Controllers
{
    public class ReportController : BaseController
    {
        //  UserModel userModel = new UserModel();
        //    CompanyModel companyModel = new CompanyModel();
        //   ReportModel reportModel = new ReportModel();

        private readonly UserModel userModel = UserModel.inst;
        private readonly CompanyModel companyModel = new CompanyModel();
        private readonly ReportModel reportModel = new ReportModel();

        //
        // GET: /Report/
        [HttpGet]
        public ActionResult New(string companyCode)
        {
            ///    private readonly UserModel userModel = UserModel.inst;
            ///    

            ///    reportModel = new ReportModel();
            ///     userModel = new UserModel();
            ////   companyModel = new CompanyModel();
            ModelState.Clear();

            int id = 0;
            if (companyCode != null)
            {
                id = companyModel.GetCompanyByCode(companyCode);
                if (id == 0)
                {
                    return RedirectToAction("Index", "Index");
                    // zmachit kod v baze ne nashl
                    ViewBag.currentCompanySubmitted = companyCode;
                    ViewBag.currentCompany = "";
                    id = 1;
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


                //    if(id == 1 ) - nado pokazat message reporteru iz psd
                //         reporter - file report no company
                ////screen - http://invis.io/QK2VGQNAW
                ////PSD - http://invis.io/a/G91HFKB8NJ4ZV


                CompanyModel model = new CompanyModel(id);
                company currentCompany = model.GetById(id);
                //company currentCompany = CompanyModel.inst.GetById(id);
                ViewBag.currentCompanyId = currentCompany.id;

                /*caseInformation*/

                if (reportModel.isCustomIncidentTypes(ViewBag.currentCompanyId))
                {
                    /*custom types*/
                    List<company_secondary_type> list = reportModel.getCompanySecondaryType(ViewBag.currentCompanyId);
                    ViewBag.secondary_type_mandatory = list.Where(t => t.status_id == 2).OrderBy(x => x.secondary_type_en).ToList();
                    ViewBag.CustomSecondaryType = true;
                }
                else
                {
                    /*default*/
                    ViewBag.secondary_type_mandatory = reportModel.getSecondaryTypeMandatory().Where(t => t.status_id == 2).OrderBy(x => x.secondary_type_en).ToList();
                    ViewBag.CustomSecondaryType = false;
                }

                //ViewBag.currentCompanySubmitted = CompanyModel.inst.GetById(id).company_nm;
                ViewBag.currentCompanySubmitted = currentCompany.company_nm;
                ViewBag.currentCompany = currentCompany.company_nm;
                //ViewBag.country = currentCompany.address.country.country_nm;
                ViewBag.locations = HtmlDataHelper.MakeSelect(companyModel.Locations(id).Where(t => t.status_id == 2).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
                ViewBag.managament = companyModel.getManagamentKnow();
                ViewBag.frequencies = HtmlDataHelper.MakeSelect(companyModel.getFrequencies(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("description")));
                List<country> arr = companyModel.getCountries();
                ViewBag.countries = HtmlDataHelper.MakeSelect(arr, item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.country_nm.ToString()));
                ViewBag.countriesDescription = arr;
                ViewBag.reportedOutsides = companyModel.getReportedOutside();
                List<role_in_report> roleInReport = db.role_in_report.ToList();
                List<SelectListItem> selectedRoleInReport = new List<SelectListItem>();
                /*put other*/
                SelectListItem temp = new SelectListItem { Text = App_LocalResources.GlobalRes.Other, Value = "0", Selected = true };
                selectedRoleInReport.Add(temp);
                foreach (var item in roleInReport)
                {
                    SelectListItem role = new SelectListItem { Text = item.role_en, Value = item.id.ToString() };
                    selectedRoleInReport.Add(role);
                }
                ViewBag.selectedRoleInReport = selectedRoleInReport;
                List<anonymity> list_anon = companyModel.GetAnonymities(id, 0);
                foreach (anonymity _anon in list_anon)
                {
                    _anon.anonymity_company_en = string.Format(_anon.anonymity_company_en, currentCompany.company_nm);
                    _anon.anonymity_ds_en = string.Format(_anon.anonymity_ds_en, currentCompany.company_nm);
                    _anon.anonymity_en = string.Format(_anon.anonymity_en, currentCompany.company_nm);


                }
                ViewBag.anonimity = list_anon;
                /*Relationship to company*/
                List<company_relationship> relationship = reportModel.getCustomRelationshipCompany(ViewBag.currentCompanyId);
                if (relationship.Count > 0)
                {
                    //loadCustom
                    //get other
                    //company_relationship other = companyModel.getOtherRelationship();
                    //relationship.Add(other);
                    ViewBag.relationship = relationship;
                }
                else
                {
                    ViewBag.relationship = companyModel.getRelationships();
                }


                //  ViewBag.departments = HtmlDataHelper.MakeSelect(currentCompany.company_department.ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("department")));
                var departmentsActive = companyModel.CompanyDepartmentsActive(id).ToList();

                company_department empty = new company_department();
                empty.department_en = App_LocalResources.GlobalRes.notListed;
                empty.id = 0;
                departmentsActive.Add(empty);
                ViewBag.departments = HtmlDataHelper.MakeSelect(departmentsActive, item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("department")));


                ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.Locations(id).Where(t => t.status_id == 2).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

                // ViewBag.departments2 = currentCompany.company_department.ToList();
                ViewBag.departments2 = companyModel.CompanyDepartmentsActive(id).ToList();

                ViewBag.locationsOfIncident2 = companyModel.Locations(id).Where(t => t.status_id == 2).ToList();

                ViewBag.injury_damage = companyModel.GetInjuryDamages().ToList();

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
                //
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
            companyModel.ID = model.currentCompanyId;
            model.Process(Request.Form, Request.Files);
            string password;
            var currentReport = reportModel.AddReport(model, out password);
            ReportSubmit submit = new ReportSubmit();
            submit.result = new ReportModelResult();
            submit.result.StatusCode = currentReport.StatusCode;
            submit.result.ErrorMessage = currentReport.ErrorMessage;
            ViewBag.companylogo = companyModel._company.path_en;
            if (currentReport.StatusCode == 200)
            {
                ViewBag.CaseNumber = currentReport.report.display_name;//Request.Form["caseNumber"];model.caseNumber
                if (currentReport.report.user_id > 0)
                {
                    var user = companyModel.GetUser(currentReport.report.user_id);
                    ViewBag.UserId = user.id;
                    /*model.userName = */
                    ViewBag.Login = user.login_nm;
                    /*model.password = */
                    ViewBag.Password = password;
                    /*model.userEmail = */
                    ViewBag.Email = user.email;
                    SignIn(user);
                    GlobalFunctions glb = new GlobalFunctions();
                    glb.UpdateReportLog(user.id, 2, currentReport.report.id, "", null, "");
                    glb.UpdateReportLog(user.id, 28, currentReport.report.id, App_LocalResources.GlobalRes._Started, null, "");
                }
                //ReportViewModel rvm = new ReportViewModel();
                //rvm.Merge(currentReport.report);
                //submit.merge(rvm, companyModel, reportModel, model);
                ViewBag.ReportModel = new ReportModel(currentReport.report.id);


                #region SendEmail To Admins
                ReportModel rm = new ReportModel(currentReport.report.id);

                Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(is_cc);
                Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                bool has_involved = false;
                List<string> to = new List<string>();
                List<string> cc = new List<string>();
                if (rm.InvolvedMediatorsUserList().Count > 0)
                    has_involved = true;

                string body = "";
                string title = LocalizationGetter.GetString("Email_Title_NewCase", is_cc);
                if (has_involved)
                    title = LocalizationGetter.GetString("Email_Title_NewCaseInvolved", is_cc);
                foreach (var _user in rm.MediatorsWhoHasAccessToReport().Where(t => t.role_id != ECLevelConstants.level_escalation_mediator).ToList())
                {
                    eb = new Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                    to = new List<string>();
                    to.Add(_user.email.Trim());
                    if (has_involved)
                        eb.NewCaseInvolved(_user.first_nm, _user.last_nm, rm._report.display_name);
                    else
                        eb.NewCase(_user.first_nm, _user.last_nm, rm._report.display_name);

                    body = eb.Body;
                    em.Send(to, cc, title, body, true);
                }
                #endregion
            } else
            {
                ViewBag.UserId = 0;
                ViewBag.Login = "";
                ViewBag.Password = "";
                ViewBag.Email = "";
                ViewBag.ReportModel = new ReportModel();
                ViewBag.CaseNumber = 0;
                ViewBag.company_code = companyModel._company.company_code;
            }

            return View("CaseSubmitted");
        }

        // case details
        public ActionResult Details(int case_id = 1, int user_id = 2)
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion



            int id = 1;
            var currentCompany = new CompanyModel().GetById(id);
            ViewBag.currentCompany = currentCompany.company_nm;
            ViewBag.currentCompanyId = currentCompany.id;
            ViewBag.currentCompanySubmitted = currentCompany.company_nm;

            //ViewBag.country = currentCompany.address.country.country_nm;
            ViewBag.locations = HtmlDataHelper.MakeSelect(companyModel.Locations(id), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
            ViewBag.managament = companyModel.getManagamentKnow();
            ViewBag.frequencies = HtmlDataHelper.MakeSelect(companyModel.getFrequencies(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("description")));
            List<country> arr = companyModel.getCountries();
            ViewBag.countries = HtmlDataHelper.MakeSelect(arr, item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.country_nm.ToString()));
            ViewBag.countriesDescription = arr;
            ViewBag.anonimity = companyModel.GetAnonymities(id, 0);
            ViewBag.relationship = companyModel.getRelationships();
            ViewBag.departments = HtmlDataHelper.MakeSelect(currentCompany.company_department.ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("department")));
            ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.Locations(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

            ViewBag.injury_damage = companyModel.GetInjuryDamages().ToList();

            ViewBag.supervisingMediators = companyModel.AllSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));
            ViewBag.nonSupervisingMediators = companyModel.AllNonSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));

            return View();
        }

        // messages in the case
        public ActionResult Messages(int case_id = 1, int user_id = 2)
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion



            int id = 1;
            var currentCompany = new CompanyModel().GetById(id);
            ViewBag.currentCompany = currentCompany.company_nm;
            ViewBag.currentCompanyId = currentCompany.id;
            ViewBag.currentCompanySubmitted = currentCompany.company_nm;

            //ViewBag.country = currentCompany.address.country.country_nm;
            ViewBag.locations = HtmlDataHelper.MakeSelect(companyModel.Locations(id), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
            ViewBag.managament = companyModel.getManagamentKnow();
            ViewBag.frequencies = HtmlDataHelper.MakeSelect(companyModel.getFrequencies(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("description")));
            List<country> arr = companyModel.getCountries();
            ViewBag.countries = HtmlDataHelper.MakeSelect(arr, item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.country_nm.ToString()));
            ViewBag.countriesDescription = arr;
            ViewBag.anonimity = companyModel.GetAnonymities(id, 0);
            ViewBag.relationship = companyModel.getRelationships();
            ViewBag.departments = HtmlDataHelper.MakeSelect(currentCompany.company_department.ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("department")));
            ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.Locations(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

            ViewBag.injury_damage = companyModel.GetInjuryDamages().ToList();

            ViewBag.supervisingMediators = companyModel.AllSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));
            ViewBag.nonSupervisingMediators = companyModel.AllNonSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));

            return View();
        }

        // tasks in the case
        public ActionResult Tasks(int case_id = 1, int user_id = 2)
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion


            int id = 1;
            var currentCompany = new CompanyModel().GetById(id);
            ViewBag.currentCompany = currentCompany.company_nm;
            ViewBag.currentCompanyId = currentCompany.id;
            ViewBag.currentCompanySubmitted = currentCompany.company_nm;

            //ViewBag.country = currentCompany.address.country.country_nm;
            ViewBag.locations = HtmlDataHelper.MakeSelect(companyModel.Locations(id), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
            ViewBag.managament = companyModel.getManagamentKnow();
            ViewBag.frequencies = HtmlDataHelper.MakeSelect(companyModel.getFrequencies(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("description")));
            List<country> arr = companyModel.getCountries();
            ViewBag.countries = HtmlDataHelper.MakeSelect(arr, item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.country_nm.ToString()));
            ViewBag.countriesDescription = arr;
            ViewBag.anonimity = companyModel.GetAnonymities(id, 0);
            ViewBag.relationship = companyModel.getRelationships();
            ViewBag.departments = HtmlDataHelper.MakeSelect(currentCompany.company_department.ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("department")));
            ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.Locations(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

            ViewBag.injury_damage = companyModel.GetInjuryDamages().ToList();

            ViewBag.supervisingMediators = companyModel.AllSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));
            ViewBag.nonSupervisingMediators = companyModel.AllNonSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));

            return View();
        }

        // messages for reporter in the case
        public ActionResult MessagesWithReporter(int case_id = 1, int user_id = 2)
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion


            int id = 1;
            var currentCompany = new CompanyModel().GetById(id);
            ViewBag.currentCompany = currentCompany.company_nm;
            ViewBag.currentCompanyId = currentCompany.id;
            ViewBag.currentCompanySubmitted = currentCompany.company_nm;

            //ViewBag.country = currentCompany.address.country.country_nm;
            ViewBag.locations = HtmlDataHelper.MakeSelect(companyModel.Locations(id), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
            ViewBag.managament = companyModel.getManagamentKnow();
            ViewBag.frequencies = HtmlDataHelper.MakeSelect(companyModel.getFrequencies(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("description")));
            List<country> arr = companyModel.getCountries();
            ViewBag.countries = HtmlDataHelper.MakeSelect(arr, item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.country_nm.ToString()));
            ViewBag.countriesDescription = arr;
            ViewBag.anonimity = companyModel.GetAnonymities(id, 0);
            ViewBag.relationship = companyModel.getRelationships();
            ViewBag.departments = HtmlDataHelper.MakeSelect(currentCompany.company_department.ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("department")));
            ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.Locations(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

            ViewBag.injury_damage = companyModel.GetInjuryDamages().ToList();

            ViewBag.supervisingMediators = companyModel.AllSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));
            ViewBag.nonSupervisingMediators = companyModel.AllNonSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));

            return View();
        }

        public ActionResult SaveLoginChanges()
        {
            int user_Id = Convert.ToInt16(Request["userId"]);
            string pass = Request["pass"];
            string result = reportModel.SaveLoginChanges(user_Id, pass);
            JsonResult json = new JsonResult();
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
    }
}