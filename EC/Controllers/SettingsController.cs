using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Data.Entity.Migrations;

using EC.Constants;
using EC.Common.Util;
using EC.Utils;
using EC.Localization;

namespace EC.Controllers
{
    //[EC.Utils.AuthFilter]
    public class SettingsController : BaseController
    {

        private readonly CompanyModel companyModel = new CompanyModel();
        private SettingsModel SettingsModel = new SettingsModel();
        // GET: Settings
        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return RedirectToAction("Login", "Service", new { returnUrl = "/Settings/Index" });
            }
                

            int user_id = user.id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            UserModel um = new UserModel(user_id);
            Company ecModelCompany = new Company();
            um.listDepartments = ecModelCompany.CompanyDepartments(user.company_id, 1, true);
            ViewBag.um = um;
            ViewBag.page_subtitle = LocalizationGetter.GetString("Settings", is_cc);
            ViewBag.user_id = user_id;
            um._user.user_permissions_approve_case_closure = um._user.user_permissions_approve_case_closure == null ? 2 : um._user.user_permissions_approve_case_closure;
            um._user.user_permissions_change_settings = um._user.user_permissions_change_settings == null ? 2 : um._user.user_permissions_change_settings;
            // my profile
            return View(um._user);
        }

        // GET: Settings
        public ActionResult Company()
        {
            //int user_id = 2;

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            int user_id = user.id;

            UserModel um = new UserModel(user_id);
            ViewBag.page_subtitle = LocalizationGetter.GetString("Settings", is_cc);
            ViewBag.um = um;

            //int company_id = 2;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            ViewBag.user_id = user_id;
            int company_id = user.company_id;
            CompanyModel cm = new CompanyModel(company_id);
            ViewBag.CA = cm.getCA(company_id, 2);
            ViewBag.cm = cm;
            ViewBag.LocationExtendeds = new SelectList(db.location_cc_extended.OrderBy(x => x.description_en).ToList(), "id", "description_en");
            ViewBag.Company_root_cases_behavioral = db.company_root_cases_behavioral.Where(x => x.status_id == 2 & x.company_id == company_id).OrderBy(x => x.name_en).ToList();
            ViewBag.Company_root_cases_external = db.company_root_cases_external.Where(x => x.status_id == 2 & x.company_id == company_id).OrderBy(x => x.name_en).ToList();
            ViewBag.Company_root_cases_organizational = db.company_root_cases_organizational.Where(x => x.status_id == 2 & x.company_id == company_id).OrderBy(x => x.name_en).ToList();

            // company profile
            return View(cm._company);
        }
        // GET: Settings
        public ActionResult Mediators(bool all = false)
        {
            //int user_id = 2;
            ViewBag.All = all;

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            int user_id = user.id;

            UserModel um = new UserModel(user_id);
            int company_id = um._user.company_id;
            CompanyModel cm = new CompanyModel(company_id);

            ViewBag.page_subtitle = LocalizationGetter.GetString("Settings", is_cc);

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion


            ViewBag.user_id = user_id;
            ViewBag.um = um;
            ViewBag.cm = cm;
            List<user> all_users = cm.AllMediators(cm._company.id, false, null);
            all_users = all_users.Where(x => all || x.status_id != 1).ToList();

            List<string> _company_user_emails = new List<string>();
            _company_user_emails = (db.user.Where(t => ((t.company_id == company_id) && (t.role_id != ECLevelConstants.level_informant))).Select(t => t.email.Trim().ToLower())).ToList();

            List<int> _company_user_ids = new List<int>();
            _company_user_ids = (db.user.Where(t => ((t.company_id == company_id) && (t.role_id != ECLevelConstants.level_informant))).Select(t => t.id)).ToList();


            List<invitation> company_invitations = db.invitation.Where(t => ((!_company_user_emails.Contains(t.email.ToLower().Trim()) && (t.used_flag == 0) && (_company_user_ids.Contains(t.sent_by_user_id))))).ToList();

            foreach (invitation _inv in company_invitations)
            {
                user _user = new user();
                _user.id = 0;
                _user.role_id = 0;
                _user.first_nm = _inv.email;
                _user.email = _inv.email;
                _user.last_nm = "";
                _user.location_nm = "";
                _user.photo_path = "";
                _user.title_ds = "";
                _user.notepad_tx = LocalizationGetter.GetString("Awaiting_registration", is_cc);
                all_users.Add(_user);
            }
            ViewBag.AllUser = all_users;

            return View();
        }

        [HttpPost]
        public ActionResult Mediators(company company)
        {
            var cm = db.company.FirstOrDefault(x => x.id == company.id);
            cm.cc_campus_alert_manager_email = company.cc_campus_alert_manager_email;
            cm.cc_campus_alert_manager_first_name = company.cc_campus_alert_manager_first_name;
            cm.cc_campus_alert_manager_last_name = company.cc_campus_alert_manager_last_name;
            cm.cc_campus_alert_manager_phone = company.cc_campus_alert_manager_phone;

        /*    cm.cc_daily_crime_log_manager_email = company.cc_daily_crime_log_manager_email;
            cm.cc_daily_crime_log_manager_first_name = company.cc_daily_crime_log_manager_first_name;
            cm.cc_daily_crime_log_manager_last_name = company.cc_daily_crime_log_manager_last_name;
            cm.cc_daily_crime_log_manager_phone = company.cc_daily_crime_log_manager_phone;
            */
            db.SaveChanges();

            return RedirectToAction("Mediators");
        }

        // GET: Settings
        public ActionResult User(int? id)
        {
            user _user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (_user == null || _user.id == 0)
                return RedirectToAction("Login", "Service");

            ViewBag.MediatorId = id;
            ViewBag.user_id = _user.id;


            return View();

            //int user_id = 2;
            if (id == null)
            {
                return RedirectToAction("Index", "Settings");
            }
            if (_user == null || _user.id == 0)
                return RedirectToAction("Login", "Service");

            if (_user.role_id == 8)
            {
                return RedirectToAction("Login", "Service");
            }

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion


            if (_user.id == id.Value)
                return RedirectToAction("Index", "Settings");

            int user_id = id.Value;
            UserModel viewd_user = new UserModel(user_id);
            int company_id = viewd_user._user.company_id;
            CompanyModel cm = new CompanyModel(company_id);
            UserModel um = new UserModel(_user.id);

            ViewBag.page_subtitle = LocalizationGetter.GetString("Settings", is_cc);

            if ((viewd_user._user.role_id < 4) || (viewd_user._user.role_id > 7))
                return RedirectToAction("Login", "Service");


            ViewBag.um = um;
            ViewBag.cm = cm;
            ViewBag.user_id = _user.id;
            ViewBag.viewd_user = viewd_user;
            bool can_edit = false;

            if ((_user.role_id > 3) && (_user.role_id < 8))
            {
                if (viewd_user._user.company_id != _user.company_id)
                    return RedirectToAction("Login", "Service");
            }

            if (_user.role_id == 7 || _user.role_id == 6)
            {
                can_edit = false;
            }
            if (_user.role_id == 1 || _user.role_id == 2 || _user.role_id == 3 || _user.role_id == 4 || _user.role_id == 5)
            {
                can_edit = true;
            }
            if (_user.company_department_id == null || _user.company_department_id == 0)
            {
                viewd_user.selectedDepartment = LocalizationGetter.GetString("Other", is_cc);
            }
            else
            {
                viewd_user.selectedDepartment = new Department(_user.company_department_id.GetValueOrDefault(), 1).department_nm;
            }
            if (can_edit)
                ViewBag.ce = 1;
            else
                ViewBag.ce = 0;

            return View(viewd_user._user);
        }

        [HttpPost]
        public object User(int id, HttpPostedFileWrapper _file)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
            {
                return new
                {
                    ok = false,
                    message = "Authorization needed",
                };
            }

            var fi = new System.IO.FileInfo(_file.FileName);
            if (".png,.jpg,.jpeg,*.gif,*.bmp,".IndexOf(fi.Extension.ToLower()) == -1)
            {
                return new
                {
                    ok = false,
                    message = "Not valid extension",
                };
            }

            //UserModel um = new UserModel(user.id);
            UserModel um = new UserModel(id); //Selected user

            CompanyModel cm = new CompanyModel(um._user.company_id);

            var folder = Server.MapPath(String.Format("~/Upload/Company/{0}/users", cm._company.guid));
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            var file = String.Format("{0}\\{1}{2}", folder, um._user.guid, fi.Extension);
            _file.SaveAs(file);

            ImageUtils.MakeSquarePhoto(file);

            var dbUser = db.user.FirstOrDefault(x => x.id == id);
            //var url = String.Format("~/Upload/Company/{0}/users/{1}{2}", cm._company.guid, um._user.guid, fi.Extension);
            var url = System.Configuration.ConfigurationManager.AppSettings["SiteRoot"];

            var is_cloud = System.Configuration.ConfigurationManager.AppSettings["IsCloud"];
            var ext = System.Configuration.ConfigurationManager.AppSettings["ImageExtensionRoot"];
            if (is_cloud != null && is_cloud.ToString() == "1" && ext != null && !string.IsNullOrWhiteSpace(ext))
              url = url + ext;

            url = url ?? Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            url += String.Format("/Upload/Company/{0}/users/{1}{2}", cm._company.guid, um._user.guid, fi.Extension);

            dbUser.photo_path = url;
            db.SaveChanges();

            return url;
        }
        // GET: Settings
        public ActionResult Cases()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            int user_id = user.id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion



            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            ViewBag.page_subtitle = LocalizationGetter.GetString("Settings", is_cc);
            ViewBag.user_id = user_id;
            ViewBag.companyId = user.company_id;

            CompanyModel cm = new CompanyModel(um._user.company_id);
            company _comp = cm._company;
            return View(_comp);
        }
        //[Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult addNewfunction()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            SettingsViewModel settings = new SettingsViewModel();
            string flag = "false";
            if (Request.IsAjaxRequest())
            {
                settings.companyId = Convert.ToInt32(Request.QueryString["companyId"]);
                settings.userId = Convert.ToInt32(Request.QueryString["userId"]);
                settings.newSetting = Request.QueryString["newSetting"];
                settings.data = Request.QueryString["data"];

                if (settings.userId == user.id && settings.companyId == user.company_id)
                {
                    flag = SettingsModel.setNewItem(settings);
                }

            }
            if (Request.QueryString["partial"] == "BlockRootCauses")
            {
                return Content(BlockRootCauses());
            }
            if (Request.QueryString["partial"] == "Location")
            {
                return Content(LocationPartial(flag));
            }
            return Content(flag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser([Bind(Include = "id,first_nm,last_nm,title_ds,email,company_department_id,user_permissions_approve_case_closure,user_permissions_change_settings,company_location_id")] user _user)
        {
            if (!(_user.company_location_id.HasValue) || (_user.company_location_id == 0))
            {
                ModelState.AddModelError("company_location_id", LocalizationGetter.GetString("Location", is_cc));
            }
            if (ModelState.IsValid)
            {
                using (var db1 = new ECEntities())
                {
                    user _updateuser = db1.user.Where(item => item.id == _user.id).FirstOrDefault();

                    _updateuser.first_nm = _user.first_nm;
                    _updateuser.last_nm = _user.last_nm;
                    _updateuser.title_ds = _user.title_ds;
                    _updateuser.company_location_id = _user.company_location_id;
                    //_updateuser.notepad_tx = _user.notepad_tx;
                    _updateuser.email = _user.email;
                    //_updateuser.notification_messages_actions_flag = _user.notification_messages_actions_flag;
                    //_updateuser.notification_new_reports_flag = _user.notification_new_reports_flag;
                    _updateuser.company_department_id = _user.company_department_id;
                    _updateuser.user_permissions_approve_case_closure = _user.user_permissions_approve_case_closure;
                    _updateuser.user_permissions_change_settings = _user.user_permissions_change_settings;
                    db1.SaveChanges();
                    return RedirectToAction("Index");
                }

                //TODO: Save your model and redirect 
                // _user.Save();

                //     return RedirectToAction("Index");
            }

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            int user_id = user.id;

            UserModel um = new UserModel(user_id);
            Company ecModelCompany = new Company();
            um.listDepartments = ecModelCompany.CompanyDepartments(user.company_id, 1, true);
            ViewBag.um = um;
            ViewBag.page_subtitle = LocalizationGetter.GetString("Settings", is_cc);
            ViewBag.user_id = user_id;
            um._user.user_permissions_approve_case_closure = um._user.user_permissions_approve_case_closure == null ? 2 : um._user.user_permissions_approve_case_closure;
            um._user.user_permissions_change_settings = um._user.user_permissions_change_settings == null ? 2 : um._user.user_permissions_change_settings;
            return View("Index", _user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMediator([Bind(Include = "id,status_id,role_id")] user _user)
        {
            user session_user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (session_user == null || session_user.id == 0)
                return RedirectToAction("Login", "Service");

            if (ModelState.IsValid)
            {
                using (var db1 = new ECEntities())
                {
                    user _updateuser = db1.user.Where(item => item.id == _user.id).FirstOrDefault();
                    _updateuser.status_id = _user.status_id;
                    _updateuser.role_id = _user.role_id;

                    bool _status_change = false;
                    if ((_user.status_id == 2) && (_updateuser.status_id != 2))
                    {
                        _status_change = true;
                    }

                    bool _role_change = false;

                    if (_user.role_id != _updateuser.role_id)
                    {
                        _role_change = true;
                    }

                    CompanyModel cm = new CompanyModel(_user.company_id);

                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

                    if (_role_change)
                    {
                        if ((_updateuser.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_updateuser.email.Trim()))
                        {
                            string new_role = "";
                            user_role new_roledb = db.user_role.Where(t => t.id == _user.role_id).FirstOrDefault();
                            if (new_roledb != null)
                                new_role = new_roledb.role_en;

                            eb.MediatorRoleChange(_updateuser.first_nm, _updateuser.last_nm, session_user.first_nm, session_user.last_nm, new_role);
                            emailNotificationModel.SaveEmailBeforeSend(session_user.id, _user.id, _user.company_id, _updateuser.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                            LocalizationGetter.GetString("Email_Title_MediatorRoleChanged", is_cc), eb.Body, false, 42);
                        }
                    }
                    if (_status_change)
                    {
                        if ((_updateuser.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_updateuser.email.Trim()))
                        {
                            string new_status = "";
                            status new_roledb = db.status.Where(t => t.id == _user.status_id).FirstOrDefault();
                            if (new_status != null)
                                new_status = new_roledb.status_en;

                            eb.MediatorStatusChange(_updateuser.first_nm, _updateuser.last_nm, session_user.first_nm, session_user.last_nm, new_status);
                            emailNotificationModel.SaveEmailBeforeSend(session_user.id, _user.id, _user.company_id, _updateuser.email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                            LocalizationGetter.GetString("Email_Title_MediatorStatusChanged", is_cc) , eb.Body, false, 43);
                        }
                    }


                    db1.SaveChanges();
                    return RedirectToAction("User", new { id = _user.id });
                }
            }
            return RedirectToAction("User", new { id = _user.id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCompany(company _company)
        {
            if (ModelState.IsValid)
            {
                //TODO: Save your model and redirect 
                // _user.Save();
                return RedirectToAction("Index");
            }
            return View(_company);
        }

        public bool UpdateDelays()
        {
            try
            {
                user session_user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
                if (session_user == null || session_user.id == 0)
                {
                    return false;
                }

                if (session_user.role_id != ECLevelConstants.level_supervising_mediator)
                {
                    return false;
                }

                int company_id = Convert.ToInt16(Request["company_id"]);

                int step1_delayMin = Convert.ToInt16(Request["step1_delayMin"]);
                int step2_delayMin = Convert.ToInt16(Request["step2_delayMin"]);
                int step3_delayMin = Convert.ToInt16(Request["step3_delayMin"]);
                int step4_delayMin = Convert.ToInt16(Request["step4_delayMin"]);


                company company_item = db.company.Where(item => (item.id == company_id)).FirstOrDefault();

                company_item.step1_delay = step1_delayMin;
                company_item.step2_delay = step2_delayMin;
                company_item.step3_delay = step3_delayMin;
                company_item.step4_delay = step4_delayMin;

                db.company.AddOrUpdate(company_item);
                db.SaveChanges();

                return true;
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                Console.WriteLine("Delays were not updated");
                return false;
            }
        }

        public bool DeleteLocation()
        {
            int user_id = Convert.ToInt16(Request["user_id"]);
            int company_id = Convert.ToInt16(Request["company_id"]);

            try
            {
                user user = db.user.Where(item => (item.id == user_id)).FirstOrDefault();
                if (user.role_id == 4 || user.role_id == 5)
                {
                    company company_item = db.company.Where(item => (item.id == company_id)).FirstOrDefault();

                    db.company.AddOrUpdate(company_item);
                    db.SaveChanges();

                    return true;
                }
            }
            catch (FormatException ex)
            {
                logger.Error(ex.ToString());
                Console.WriteLine("Some exception catched.");
            }

            return false;
        }
        public string AddLogoCompany()
        {
            string result = "false";
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return result;
            
            HttpPostedFileBase photo = Request.Files["_file"];
            string MediatorIdParams = Request.Params["MediatorId"];
            //int MediatorId = 0;
            int user_id = user.id;
            if (MediatorIdParams != null)
            {
                try
                {
                    user_id = Convert.ToInt32(MediatorIdParams);
                } catch(FormatException ex)
                {
                    return ex.Message;
                }
            }

            
            UserModel um = new UserModel(user_id);
            int company_id = um._user.company_id;
            var cm = new CompanyModel(company_id);
            /**/

            try
            {
                if (photo.ContentLength > 0 && company_id > 0)
                {
                    if (Request.Form["from"] != null && Request.Form["from"] != String.Empty)
                    {
                        string temp = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                        string UploadedDirectory = "Upload\\" + Request.Form["from"] + "Logo";
                        string pathLogo = temp + "//" + "Upload//" + Request.Form["from"] + "Logo";
                        //UploadedDirectory = temp + "\\" + UploadedDirectory;
                        string Root = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                        string UploadTarget = Root + UploadedDirectory + @"\";
                        bool tempResult = false;
                        if (Request.Form["from"] == "Company")
                        {
                            var fileName = company_id + "_" + DateTime.Now.Ticks + System.IO.Path.GetExtension(photo.FileName);
                            string path = @"\" + UploadedDirectory + @"\" + fileName;
                            //result = "/Upload/" + Request.Form["from"] + "Logo/" + fileName;
                            pathLogo += "/" + fileName;
                            CompanyModel model = new CompanyModel();
                            tempResult = model.addLogoCompany(company_id, pathLogo);
                            if (tempResult)
                            {
                                photo.SaveAs(UploadTarget + fileName);
                            }
                            result = pathLogo;
                        }
                        if (Request.Form["from"] == "User")
                        {
                            var fi = new System.IO.FileInfo(photo.FileName);
                            var folder = Server.MapPath(String.Format("~/Upload/Company/{0}/users", cm._company.guid));
                            if (!System.IO.Directory.Exists(folder))
                            {
                                System.IO.Directory.CreateDirectory(folder);
                            }
                            var file = String.Format("{0}\\{1}{2}", folder, um._user.guid, fi.Extension);
                            photo.SaveAs(file);

                            ImageUtils.MakeSquarePhoto(file);

                            var url = System.Configuration.ConfigurationManager.AppSettings["SiteRoot"];
                            url = url ?? Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                            var is_cloud = System.Configuration.ConfigurationManager.AppSettings["IsCloud"];
                            var ext = System.Configuration.ConfigurationManager.AppSettings["ImageExtensionRoot"];
                            if (is_cloud != null && is_cloud.ToString() == "1" && ext != null && !string.IsNullOrWhiteSpace(ext))
                              url = url + ext;

                            url += String.Format("/Upload/Company/{0}/users/{1}{2}", cm._company.guid, um._user.guid, fi.Extension);

                            UserModel model = new UserModel();
                            tempResult = model.updateLogoUser(user_id, url);
                            result = url;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result = ex.ToString();
            }
            return result;
        }

        public string InviteMediator(string email)
        {
            email = email.ToLower().Trim();

            user _user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (_user == null || _user.id == 0)
                return LocalizationGetter.GetString("EmptyData", is_cc);

            if (_user.role_id == 8)
            {
                return LocalizationGetter.GetString("EmptyData", is_cc);
            }

            if (string.IsNullOrEmpty(email))
            {
                return LocalizationGetter.GetString("EmptyData", is_cc);
            }
            if (!m_EmailHelper.IsValidEmail(email))
            {
                return LocalizationGetter.GetString("EmailInvalid", is_cc);
            }

            UserModel um = new UserModel(_user.id);

            List<string> _company_user_emails = new List<string>();
            _company_user_emails = (db.user.Where(t => ((t.company_id == um._user.company_id) && (t.role_id != ECLevelConstants.level_informant))).Select(t => t.email.Trim().ToLower())).ToList();

            List<int> _company_user_allowed = new List<int>();
            _company_user_allowed = (db.user.Where(t => ((t.company_id == um._user.company_id) && (t.role_id != ECLevelConstants.level_informant))).Select(t => t.id)).ToList();

            if (_company_user_emails.Contains(email))
                return LocalizationGetter.GetString("MediatorAlreadyRegistered", is_cc) + "!";

            if ((db.invitation.Any(t => ((t.email.ToLower().Trim() == email) && (t.used_flag == 1) && (_company_user_allowed.Contains(t.sent_by_user_id))))) || (db.user.Any(u => ((u.email.ToLower().Trim() == email.ToLower().Trim() && u.role_id != 8)))))
                return LocalizationGetter.GetString("MediatorAlreadyRegistered", is_cc);

            if (db.invitation.Any(t => ((t.email.ToLower().Trim() == email) && (_company_user_allowed.Contains(t.sent_by_user_id)))))
                return LocalizationGetter.GetString("AlreadyInvited", is_cc);


            string generated_code = StringUtil.RandomString(6);
            // create invitation in db

            invitation _invitation = new invitation();
            _invitation.code = generated_code;
            _invitation.comment = "";
            _invitation.created_on = DateTime.Now;
            _invitation.email = email.Trim().ToLower();
            _invitation.sent_by_user_id = _user.id;
            _invitation.used_flag = 0;

            db.invitation.AddOrUpdate(_invitation);
            db.SaveChanges();
            // add _invitation to db

            // send email with code to email address


            if ((email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(email.Trim()))
            {
                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

                CompanyModel cm = new CompanyModel(_user.company_id);
                eb.MediatorInvited(_user.first_nm, _user.last_nm, _user.first_nm, _user.last_nm, cm._company.company_nm, generated_code,
                    DomainUtil.GetSubdomainLink(Request.Url.AbsoluteUri.ToLower(),
                    Request.Url.AbsoluteUri.ToLower()) + "/new/?code=" + generated_code + "&email=" + email);
                emailNotificationModel.SaveEmailBeforeSend(_user.id, 0, 0, email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                   LocalizationGetter.GetString("Email_Title_MediatorInvited", is_cc), eb.Body, false, 41);
            }

            return LocalizationGetter.GetString("_Completed", is_cc).ToLower();
        }
        public string resendInvitation(string email)
        {
            email = email.ToLower().Trim();

            user _user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (_user == null || _user.id == 0)
                return LocalizationGetter.GetString("EmptyData", is_cc);

            if (_user.role_id == 8)
            {
                return LocalizationGetter.GetString("EmptyData", is_cc);
            }

            if (string.IsNullOrEmpty(email))
            {
                return LocalizationGetter.GetString("EmptyData", is_cc);
            }
            if (!m_EmailHelper.IsValidEmail(email))
            {
                return LocalizationGetter.GetString("EmailInvalid", is_cc);
            }

            return emailNotificationModel.resendInvitation(email, is_cc, Request, _user);
        }
        public ActionResult Password()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            int user_id = user.id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            ViewBag.page_subtitle = LocalizationGetter.GetString("Settings", is_cc);
            ViewBag.user_id = user_id;
            return View(um._user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(string oldPass, string newPass, string confPass)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");
            int user_id = user.id;
            ViewBag.status = SettingsModel.IsValidPass(oldPass, newPass, confPass, user_id);
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            ViewBag.page_subtitle = LocalizationGetter.GetString("Settings", is_cc);
            ViewBag.user_id = user_id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            return View("Password", um._user);
        }

        public ActionResult casesHeared()
        {
            return PartialView("~/Views/Settings/partial/casesHeared.cshtml");
        }

        public ActionResult Languages()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service", new { returnUrl = Url.Action("Languages", "Settings") });

            UserModel um = new UserModel(user.id);
            ViewBag.um = um;
            ViewBag.user_id = user.id;

            return View();
        }

        public string BlockRootCauses()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return "";

            UserModel um = new UserModel(user.id);
            bool is_valid_mediator = false;
            var role_id = um._user.role_id;
            if (role_id == ECLevelConstants.level_supervising_mediator)
            {
                is_valid_mediator = true;
            }
            if (um._user.user_permissions_change_settings == 1)
            {
                is_valid_mediator = true;
            }

            ViewBag.Company_root_cases_behavioral = db.company_root_cases_behavioral.Where(x => x.status_id == 2 & x.company_id == um._user.company_id).OrderBy(x => x.name_en).ToList();
            ViewBag.Company_root_cases_external = db.company_root_cases_external.Where(x => x.status_id == 2 & x.company_id == um._user.company_id).OrderBy(x => x.name_en).ToList();
            ViewBag.Company_root_cases_organizational = db.company_root_cases_organizational.Where(x => x.status_id == 2 & x.company_id == um._user.company_id).OrderBy(x => x.name_en).ToList();
            ViewBag.is_valid_mediator = is_valid_mediator;

            return this.RenderPartialView("~/Views/Settings/partial/blockRootCauses.cshtml", null, ViewData);
        }

        public string LocationPartial(string locationId)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return "";

            UserModel um = new UserModel(user.id);
            bool is_valid_mediator = false;
            var role_id = um._user.role_id;
            if (role_id == ECLevelConstants.level_supervising_mediator)
            {
                is_valid_mediator = true;
            }
            if (um._user.user_permissions_change_settings == 1)
            {
                is_valid_mediator = true;
            }

            ViewBag.is_valid_mediator = is_valid_mediator;
            ViewBag.is_cc = is_cc;
            ViewBag.LocationExtendeds = new SelectList(db.location_cc_extended.OrderBy(x => x.description_en).ToList(), "id", "description_en");

            int id;
            if (int.TryParse(locationId, out id))
            {
                var location = db.company_location.FirstOrDefault(x => x.id == id);
                return this.RenderPartialView("~/Views/Shared/Partial/_SettingsCompanyLocationTemplate.cshtml", location, ViewData);
            }

            return "";
        }

        public ActionResult CaseRouting()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service", new { returnUrl = Url.Action("Languages", "Settings") });

            UserModel um = new UserModel(user.id);
            ViewBag.um = um;
            ViewBag.user_id = user.id;

            return View();
        }
    }
}