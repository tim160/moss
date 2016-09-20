using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using System.Web.Mvc;
using EC.Models.Database;
using EC.Models.DataObjects;
using EC.Models.ECModel;
using EC.Utils.Auth;
using EC.Models;
using EC.Controllers.Utils;
using EC.Controllers.utils;
using Resources = EC.Localization.Resources;


namespace EC.Controllers
{
    public class IndexController : BaseController
    {
        private readonly CompanyModel companyModel = CompanyModel.inst;
        //
        // GET: /Index/
        //[EcAuthorized("admin, moderator, mediator")] //раскоментировать когда будет использоваться авторизация! и в BaseController
        public ActionResult Index()
        {
            Session.Clear();
            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
            System.Configuration.KeyValueConfigurationElement customSetting =
                    rootWebConfig1.AppSettings.Settings["DisableWebMessage"];


 /*
    //       how to send email ( from newrequest@ec.com)
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            to.Add("timur160@gmail.com");
            cc.Add("timur160@hotmail.com");

            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1,1);
            eb.CaseReopened("Alex", "Stone", "case_1111", "Kate", "Milton");
            string body = eb.Body;
            em.Send(to, cc, "Case Reopened", body, true);
          //  em.Send(to, cc, "Case Reopened", body, true, 'email_from_if_needed');
            
          */


     //       string temp = Resources.GetString(m_CultureInfo, "_Completed", Resources.GetString(m_CultureInfo, "_Completed"));
       //     temp = Resources.GetString(m_CultureInfo, "_Completed");

            string CurrentURL = Request.Url.AbsoluteUri.ToLower();
            if (CurrentURL.Contains("stark."))
            {
                return RedirectToAction("Index", "Index/Page");
            }
            else if (CurrentURL.Contains("cai."))
            {
                return RedirectToAction("Index", "Index/Start");
            }
            else
            {
                #region EC-CC Viewbag
                ViewBag.is_cc = is_cc;
                string cc_ext = "";
                if (is_cc) cc_ext = "_cc";
                ViewBag.cc_extension = cc_ext;
                #endregion

          
                List<company> list = companyModel.GeCompaniesWithStatus();
                List<SearchCompanyDto> searchCompanyDto = new List<SearchCompanyDto>();
                if (!CurrentURL.Contains("registration"))
                {
                    foreach (var item in list)
                    {
                        SearchCompanyDto searchCompany = new SearchCompanyDto();
                        searchCompany.value = item.company_code;
                        searchCompany.label = item.company_nm;
                        searchCompanyDto.Add(searchCompany);
                    }
                }

                ViewBag.listCompanies = list;
                //JSON goes v View
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                EcAuthorizedAttribute attr = new EcAuthorizedAttribute();

                ViewBag.newListComp = serializer.Serialize(searchCompanyDto.ToArray());
                return View();
            }
        }

        public ActionResult Page()
        {
            Session.Clear();
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            return View();
        }

        public ActionResult Start()
        {
            Session.Clear();
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion
            List<company> list = companyModel.GeCompaniesWithStatus();
            List<SearchCompanyDto> searchCompanyDto = new List<SearchCompanyDto>();

            string CurrentURL = Request.Url.AbsoluteUri.ToLower();
            if (!CurrentURL.Contains("registration"))
            {
                foreach (var item in list)
                {
                    SearchCompanyDto searchCompany = new SearchCompanyDto();
                    searchCompany.value = item.company_code;
                    searchCompany.label = item.company_nm;
                    searchCompanyDto.Add(searchCompany);
                }
            }

            ViewBag.listCompanies = list;
            //JSON goes v View
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            EcAuthorizedAttribute attr = new EcAuthorizedAttribute();

            ViewBag.newListComp = serializer.Serialize(searchCompanyDto.ToArray());

            return View();
        }
       // [EcAuthorized("admin, moderator")]
        public ActionResult Test()
        {
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            //var user = SessionManager.inst.User.email;
            return View("Index");
        }

        private void SignIn(user user)
        {
            AuthHelper.SetCookies(user, HttpContext);
            Session[Constants.CurrentUserMarcker] = user;
        }

        private void SingOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
        }
    }
}
