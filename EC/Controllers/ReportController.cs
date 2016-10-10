using System;
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
using EC.Controllers.utils;


namespace EC.Controllers
{
    public class ReportController : BaseController
    {
        private readonly UserModel userModel = UserModel.inst;
        private readonly CompanyModel companyModel = CompanyModel.inst;
        private readonly ReportModel reportModel = ReportModel.inst;
        //
        // GET: /Report/
        [HttpGet]
        public ActionResult New(string companyCode)
        {
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

                if(reportModel.isCustomIncidentTypes(ViewBag.currentCompanyId)) {
                    /*custom types*/
                    ViewBag.secondary_type_mandatory = reportModel.getCompanySecondaryType(ViewBag.currentCompanyId);
                } else
                {
                    /*default*/
                    ViewBag.secondary_type_mandatory = reportModel.getSecondaryTypeMandatory();
                }

                //ViewBag.currentCompanySubmitted = CompanyModel.inst.GetById(id).company_nm;
                ViewBag.currentCompanySubmitted = currentCompany.company_nm;
                ViewBag.currentCompany = currentCompany.company_nm;
                //ViewBag.country = currentCompany.address.country.country_nm;
                ViewBag.locations = HtmlDataHelper.MakeSelect(companyModel.Locations(id), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
                ViewBag.managament = companyModel.getManagamentKnow();
                ViewBag.frequencies = HtmlDataHelper.MakeSelect(companyModel.getFrequencies(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("description")));
                List<country> arr = companyModel.getCountries();
                ViewBag.countries = HtmlDataHelper.MakeSelect(arr, item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.country_nm.ToString()));
                ViewBag.countriesDescription = arr;
                ViewBag.reportedOutsides = companyModel.getReportedOutside();
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
                ViewBag.departments = HtmlDataHelper.MakeSelect(companyModel.CompanyDepartmentsActive(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("department")));

                ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.LocationsOfIncident(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

                // ViewBag.departments2 = currentCompany.company_department.ToList();
                ViewBag.departments2 = companyModel.CompanyDepartmentsActive(id).ToList();

                ViewBag.locationsOfIncident2 = companyModel.LocationsOfIncident(id).ToList();

                ViewBag.injury_damage = companyModel.GetInjuryDamages().ToList();

                ViewBag.supervisingMediators = companyModel.AllSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));
                ViewBag.nonSupervisingMediators = companyModel.AllNonSupervisingMediators(id, true); // HtmlDataHelper.MakeSelect(companyModel.AllSupervisingMediators(id, true).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.last_nm));

                // company logo
                string companyLogo = currentCompany.path_en;
                string path = System.IO.Directory.GetCurrentDirectory();
                if (!System.IO.Directory.Exists(path + companyLogo))
                {
                    ViewBag.companylogo = companyLogo;
                } else
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
            model.Process(Request.Form, Request.Files);
            var currentReport = reportModel.AddReport(model);
            ViewBag.CaseNumber = currentReport.display_name;//Request.Form["caseNumber"];model.caseNumber
            if (currentReport.user_id > 0)
            {
                var user = companyModel.GetUser(currentReport.user_id);
                ViewBag.UserId = user.id;
                model.userName = ViewBag.Login = user.login_nm;
                model.password = ViewBag.Password = user.password;
                model.userEmail = ViewBag.Email = user.email;
                SignIn(user);
                GlobalFunctions glb = new GlobalFunctions();
                glb.UpdateReportLog(user.id, 2, currentReport.id, "", null, "");
                glb.UpdateReportLog(user.id, 28, currentReport.id, App_LocalResources.GlobalRes._Started, null, "");
            }
            ReportViewModel rvm = new ReportViewModel();
            rvm.Merge(currentReport);
            ViewBag.companylogo = companyModel._company.path_en;
            ReportSubmit submit = new ReportSubmit();
            submit.merge(rvm, companyModel, reportModel, model);
            return View("CaseSubmitted", submit);
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
          var currentCompany = CompanyModel.inst.GetById(id);
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
            ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.LocationsOfIncident(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
            
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
            var currentCompany = CompanyModel.inst.GetById(id);
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
            ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.LocationsOfIncident(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

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
            var currentCompany = CompanyModel.inst.GetById(id);
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
            ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.LocationsOfIncident(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

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
            var currentCompany = CompanyModel.inst.GetById(id);
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
            ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.LocationsOfIncident(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));

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
            AuthHelper.SetCookies(user, HttpContext);
            Session[Constants.CurrentUserMarcker] = user;
        }

        public List<anonymity> GetAnon(int company_id, int country_id)
        {
            List<anonymity> list_anon = null;
            if (company_id != null && country_id != null)
            {
                var currentCompany = CompanyModel.inst.GetById(company_id);
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
	}

}