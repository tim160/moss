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
using EC.App_LocalResources;

namespace EC.Controllers
{

    public class SettingsController : BaseController
    {
        private readonly CompanyModel companyModel = CompanyModel.inst;
        private SettingsModel SettingsModel = new SettingsModel();
        // GET: Settings
        public ActionResult Index()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          

            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            ViewBag.page_subtitle = GlobalRes.Settings;
            ViewBag.user_id = user_id;
            // my profile
            return View(um._user);
        }

        // GET: Settings
        public ActionResult Company()
        {
            //int user_id = 2;

            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            UserModel um = new UserModel(user_id);
            ViewBag.page_subtitle = GlobalRes.Settings;
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
            // company profile
            return View(cm._company);
        }
        // GET: Settings
        public ActionResult Mediators()
        {
            //int user_id = 2;

            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            UserModel um = new UserModel(user_id);
            int company_id = um._user.company_id;
            CompanyModel cm = new CompanyModel(company_id);

            ViewBag.page_subtitle = GlobalRes.Settings;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            ViewBag.user_id = user_id;
            ViewBag.um = um;
            ViewBag.cm = cm;

            return View();
        }

        // GET: Settings
        public ActionResult User(int? id)
        {
            //int user_id = 2;
            if (id == null)
            {
                return RedirectToAction("Index", "Settings");
            }
            user _user = (user)Session[Constants.CurrentUserMarcker];
            if (_user == null || _user.id == 0)
                return RedirectToAction("Index", "Account");

            if (_user.role_id == 8)
            {
                return RedirectToAction("Index", "Account");
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

            ViewBag.page_subtitle = GlobalRes.Settings;

            if ((viewd_user._user.role_id < 4) || (viewd_user._user.role_id > 7))
                return RedirectToAction("Index", "Account");


            ViewBag.um = um;
            ViewBag.cm = cm;
            ViewBag.user_id = _user.id;
            ViewBag.viewd_user = viewd_user;
            bool can_edit = false;

            if ((_user.role_id > 3) && (_user.role_id < 8))
            {
                if (viewd_user._user.company_id != _user.company_id)
                    return RedirectToAction("Index", "Account");
            }

            if (_user.role_id == 7 || _user.role_id == 6)
            {
                can_edit = false;
            }
            if (_user.role_id == 1 || _user.role_id == 2 || _user.role_id == 3 || _user.role_id == 4 || _user.role_id == 5)
            {
                can_edit = true;
            }

            if (can_edit)
                ViewBag.ce = 1;
            else
                ViewBag.ce = 0;

            return View(viewd_user._user);
        }
        // GET: Settings
        public ActionResult Cases()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion



            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            ViewBag.page_subtitle = GlobalRes.Settings;
            ViewBag.user_id = user_id;
            ViewBag.companyId = user.company_id;

            CompanyModel cm = new CompanyModel(um._user.company_id);
            company _comp = cm._company;
            return View(_comp);
        }
        public string addNewfunction()
        {
            SettingsViewModel settings = new SettingsViewModel();
            string flag = "false";
            if (Request.IsAjaxRequest())
            {
                settings.companyId = Convert.ToInt32(Request.QueryString["companyId"]);
                settings.userId = Convert.ToInt32(Request.QueryString["userId"]);
                settings.newSetting = Request.QueryString["newSetting"];
                settings.data = Request.QueryString["data"];
                flag = SettingsModel.setNewItem(settings);
            }
            return flag;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser([Bind(Include = "id,status_id,role_id,first_nm,last_nm,title_ds,notepad_tx,email,notification_messages_actions_flag,notification_new_reports_flag")] user _user)
        {
            if (ModelState.IsValid)
            {
                using (var db1 = new ECEntities())
                {
                    user _updateuser = db1.user.Where(item => item.id == _user.id).FirstOrDefault();

                    _updateuser.first_nm = _user.first_nm;
                    _updateuser.last_nm = _user.last_nm;
                    _updateuser.title_ds = _user.title_ds;
                    _updateuser.notepad_tx = _user.notepad_tx;
                    _updateuser.email = _user.email;
                    _updateuser.notification_messages_actions_flag = _user.notification_messages_actions_flag;
                    _updateuser.notification_new_reports_flag = _user.notification_new_reports_flag;

                    db1.SaveChanges();
                    return RedirectToAction("Index");
                }

                //TODO: Save your model and redirect 
                // _user.Save();

                //     return RedirectToAction("Index");
            }
            return View(_user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMediator([Bind(Include = "id,status_id,role_id")] user _user)
        {
            user session_user = (user)Session[Constants.CurrentUserMarcker];
            if (session_user == null || session_user.id == 0)
                return RedirectToAction("Index", "Account");

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

                    List<string> to = new List<string>();
                    List<string> cc = new List<string>();
                    List<string> bcc = new List<string>();

                    CompanyModel cm = new CompanyModel(_user.company_id);

                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                    string body = "";

                    if (_role_change)
                    {
                        if ((_updateuser.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_updateuser.email.Trim()))
                        {
                            to = new List<string>();
                            cc = new List<string>();
                            bcc = new List<string>();

                            to.Add(_updateuser.email.Trim());
                            ///     bcc.Add("timur160@hotmail.com");
                            string new_role = "";
                            user_role new_roledb = db.user_role.Where(t => t.id == _user.role_id).FirstOrDefault();
                            if (new_roledb != null)
                                new_role = new_roledb.role_en;

                            eb.MediatorRoleChange(_updateuser.first_nm, _updateuser.last_nm, session_user.first_nm, session_user.last_nm, new_role);
                            body = eb.Body;
                            em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_MediatorRoleChanged, body, true);
                        }
                    }
                    if (_status_change)
                    {
                        if ((_updateuser.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_updateuser.email.Trim()))
                        {
                            to = new List<string>();
                            cc = new List<string>();
                            bcc = new List<string>();

                            to.Add(_updateuser.email.Trim());
                            ///     bcc.Add("timur160@hotmail.com");

                            string new_status = "";
                            status new_roledb = db.status.Where(t => t.id == _user.status_id).FirstOrDefault();
                            if (new_status != null)
                                new_status = new_roledb.status_en;

                            eb.MediatorStatusChange(_updateuser.first_nm, _updateuser.last_nm, session_user.first_nm, session_user.last_nm, new_status);
                            body = eb.Body;
                            em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_MediatorStatusChanged, body, true);
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
                Console.WriteLine("Some exception catched.");
            }

            return false;
        }
        public string AddLogoCompany()
        {
            string result = "false";
            HttpPostedFileBase photo = Request.Files["_file"];
            /*попробую получить юзера из сессии и id компании*/
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return result;
            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            int company_id = um._user.company_id;
            /**/

            try
            {
                if (photo.ContentLength > 0 && company_id > 0)
                {
                    if (Request.Form["from"] != null && Request.Form["from"] != String.Empty)
                    {
                        string UploadedDirectory = "Upload\\" + Request.Form["from"] + "Logo";
                        string Root = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                        string UploadTarget = Root + UploadedDirectory + @"\";
                        bool tempResult = false;
                        if (Request.Form["from"] == "Company")
                        {
                            var fileName = company_id + "_" + DateTime.Now.Ticks + System.IO.Path.GetExtension(photo.FileName);
                            string path = @"\" + UploadedDirectory + @"\" + fileName;
                            result = "/Upload/" + Request.Form["from"] + "Logo/" + fileName;
                            CompanyModel model = new CompanyModel();
                            tempResult = model.addLogoCompany(company_id, result);
                            if (tempResult)
                            {
                                photo.SaveAs(UploadTarget + fileName);
                            }

                        }
                        if (Request.Form["from"] == "User")
                        {
                            var fileName = user_id + "_" + DateTime.Now.Ticks + System.IO.Path.GetExtension(photo.FileName);
                            result = "/Upload/" + Request.Form["from"] + "Logo/" + fileName;
                            UserModel model = new UserModel();
                            tempResult = model.updateLogoUser(user_id, result);
                            if (tempResult)
                            {
                                photo.SaveAs(UploadTarget + fileName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();

            }
            return result;
        }

        public string InviteMediator(string email)
        {
            email = email.ToLower().Trim();

            user _user = (user)Session[Constants.CurrentUserMarcker];
            if (_user == null || _user.id == 0)
                return App_LocalResources.GlobalRes.EmptyData;

            if (_user.role_id == 8)
            {
                return App_LocalResources.GlobalRes.EmptyData;
            }

            if (string.IsNullOrEmpty(email))
            {
                return App_LocalResources.GlobalRes.EmptyData;
            }
            if (!m_EmailHelper.IsValidEmail(email))
            {
                return App_LocalResources.GlobalRes.EmailInvalid;
            }

            UserModel um = new UserModel(_user.id);

            List<string> _company_user_emails = new List<string>();
            _company_user_emails = (db.user.Where(t => ((t.company_id == um._user.company_id) && (t.role_id != Constant.level_informant))).Select(t => t.email.Trim().ToLower())).ToList();

            List<int> _company_user_allowed = new List<int>();
            _company_user_allowed = (db.user.Where(t => ((t.company_id == um._user.company_id) && (t.role_id != Constant.level_informant))).Select(t => t.id)).ToList();

            if (_company_user_emails.Contains(email))
                return App_LocalResources.GlobalRes.MediatorAlreadyRegistered + "!";

            if ((db.invitation.Any(t => ((t.email.ToLower().Trim() == email) && (t.used_flag == 1) && (_company_user_allowed.Contains(t.sent_by_user_id))))) || (db.user.Any(u => ((u.email.ToLower().Trim() == email.ToLower().Trim() && u.role_id != 8)))))
                return App_LocalResources.GlobalRes.MediatorAlreadyRegistered;

            if (db.invitation.Any(t => ((t.email.ToLower().Trim() == email) && (_company_user_allowed.Contains(t.sent_by_user_id)))))
                return App_LocalResources.GlobalRes.AlreadyInvited;


            string generated_code = GlobalFunctions.RandomString(6);
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
                List<string> to = new List<string>();
                List<string> cc = new List<string>();
                List<string> bcc = new List<string>();

                to.Add(email.Trim());
                ///     bcc.Add("timur160@hotmail.com");

                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

                CompanyModel cm = new CompanyModel(_user.company_id);

                eb.MediatorInvited(_user.first_nm, _user.last_nm, _user.first_nm, _user.last_nm, cm._company.company_nm, generated_code, "http://stark.employeeconfidential.com/new/?code=" + generated_code + "&email=" + email);
                string body = eb.Body;
                em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_MediatorInvited, body, true);

            }

            return App_LocalResources.GlobalRes._Completed.ToLower();
        }
        public ActionResult Password()
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");

            int user_id = user.id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          

            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            ViewBag.page_subtitle = GlobalRes.Settings;
            ViewBag.user_id = user_id;
            return View(um._user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(string oldPass, string newPass, string confPass)
        {
            user user = (user)Session[Constants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");
            int user_id = user.id;
            ViewBag.status = SettingsModel.IsValidPass(oldPass, newPass, confPass, user_id);
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;
            ViewBag.page_subtitle = GlobalRes.Settings;
            ViewBag.user_id = user_id;

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            return View("Password", um._user);
        }

    }
}