using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using System.Web.Mvc;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Utils.Auth;
using EC.Models;
using EC.Controllers.Utils;
using EC.Controllers.utils;
using log4net;
using EC.Constants;
using EC.Common.Util;
using EC.Models.DataObjects;

namespace EC.Controllers
{
    public class IndexController : BaseController
    {
        private readonly CompanyModel companyModel = new CompanyModel();
        //
        // GET: /Index/
        //[EcAuthorized("admin, moderator, mediator")] //раскоментировать когда будет использоваться авторизация! и в BaseController
        public ActionResult Index(string submit)
        {
            if (!String.IsNullOrEmpty(submit))
            {
                var cmp = db.company.FirstOrDefault(x => x.company_short_name != null && x.company_short_name.ToLower() == submit.ToLower());
                if (cmp != null)
                {
                    return RedirectToAction("Disclaimer", "Service", new { id = cmp.company_code });
                }
            }
            RedirectToAction("Login", "Service");

            //////////ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            /////        logger.Info("info message to logger");
            ///      logger.Error("error message to logger");


            ///// Session.Clear();
            //   System.Configuration.Configuration rootWebConfig1 =
            //      System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
            //   System.Configuration.KeyValueConfigurationElement customSetting =
            //          rootWebConfig1.AppSettings.Settings["DisableWebMessage"];

            //    UserModel um = new UserModel(977);
            // int t = um.Unread_Messages_Quantity(0, 1) + um.Unread_Messages_Quantity(0, 2) + um.Unread_Messages_Quantity(0, 3);
            //  for (int i = 0; i < db.company.Count(); i++)
            //{
            ///    foreach (var item in db.user)
            //  {
            ////    item.guid = Guid.NewGuid();


            //   }
            ////   db.SaveChanges();
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


            //       string temp = LocalizationGetter.GetString(m_CultureInfo, "_Completed", LocalizationGetter.GetString(m_CultureInfo, "_Completed"));
            //     temp = LocalizationGetter.GetString(m_CultureInfo, "_Completed");

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


                     /*List<company> list = companyModel.GeCompaniesWithStatus();
                     List<SearchCompanyDto> searchCompanyDto = new List<SearchCompanyDto>();
                     if (!CurrentURL.Contains("registration"))
                     {
                         foreach (var item in list)
                         {
                             SearchCompanyDto searchCompany = new SearchCompanyDto();
                             searchCompany.label = item.company_nm;
                             searchCompany.value = item.company_code;
                             searchCompanyDto.Add(searchCompany);
                         }
                     }

                     ViewBag.listCompanies = list;
                     //JSON goes v View
                     var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     EcAuthorizedAttribute attr = new EcAuthorizedAttribute();

                     ViewBag.newListComp = serializer.Serialize(searchCompanyDto.ToArray());*/

            return RedirectToAction("Login", "Service");
                //return View();
            }
           ///// return RedirectToAction("Login", "Service");
        }

        [AllowAnonymous]
        public JsonResult SeekCompany(string term)
        {
            var list = companyModel.GeCompaniesWithStatusAndTerm(term).Select(x => new {
                label = x.company_nm,
                value = x.company_code
            });

            return Json(list, JsonRequestBehavior.AllowGet);
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
            return RedirectToAction("Login", "Service");
           /// ViewBag.LogoPath = DomainUtil.LogoBaseUrl(Request.Url.AbsoluteUri.ToLower());

            ///return View();
        }

        public ActionResult Start()
        {
            return RedirectToAction("Login", "Service");
            Session.Clear();

        
            

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion
            List<company> list = companyModel.GeCompaniesWithStatus();
            List<SearchCompanyDto> searchCompanyDto = new List<SearchCompanyDto>();

            ViewBag.LogoPath = DomainUtil.LogoBaseUrl(Request.Url.AbsoluteUri.ToLower());
            if (Request.Url.AbsoluteUri.ToLower().Contains("report"))
            {
                ViewBag.LogoPath = "";
            }
            string CurrentURL = Request.Url.AbsoluteUri.ToLower();
            //        if (!CurrentURL.Contains("report"))
            {
                foreach (var item in list)
                {
                    SearchCompanyDto searchCompany = new SearchCompanyDto();
                    searchCompany.label = item.company_nm;
                    searchCompany.value = item.company_code;
                    searchCompanyDto.Add(searchCompany);
                }
            }

            ViewBag.listCompanies = list;
            //JSON goes v View
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            EcAuthorizedAttribute attr = new EcAuthorizedAttribute();

            ViewBag.newListComp = serializer.Serialize(searchCompanyDto.ToArray());
            ViewBag.CurrentURL = CurrentURL;
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
            Session[ECGlobalConstants.CurrentUserMarcker] = user;
        }

        private void SingOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// returns commpanies' list
        /// </summary>
        /// <param name="lookup"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CompanyLookup(string lookup)
        {
            List<SearchCompanyDto> searchCompanyDto = new List<SearchCompanyDto>();
            List<company> list = companyModel.GeCompaniesWithStatus(lookup);
            foreach (var item in list)
            {
                SearchCompanyDto searchCompany = new SearchCompanyDto();
                searchCompany.label = item.company_code;
                searchCompany.value = item.company_nm;
                searchCompanyDto.Add(searchCompany);
            }
            return Json(searchCompanyDto);
        }
    }
}
