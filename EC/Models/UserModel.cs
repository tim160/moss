using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Configuration;

using EC.Models.Database;
using EC.Models;
using EC.Models.ECModel;
using EC.Model.Impl;
using EC.Model.Interfaces;
using EC.Constants;
using EC.Models.ViewModels;
using EC.Models.ViewModel;
using EC.Utils;
using System.Threading.Tasks;
using EC.Controllers.API;
using EC.Localization;

namespace EC.Models
{
    public class UserModel : BaseModel
    {
        UserItems ui = new UserItems();
        public EmailNotificationModel emailNotificationModel = new EmailNotificationModel();

        #region Properties
        public int ID
        { get; set; }
        public List<Department> listDepartments { get; set; }
        public string selectedDepartment { get; set; }
        public user _user;

        public int GetReportIDForReporter()
        {
            int report_id = 0;
            try
            {
                user _user = db.user.FirstOrDefault(item => item.id == ID);
                if (_user.role_id == 8)
                {
                    report _report = db.report.FirstOrDefault(item => item.reporter_user_id == ID);
                    report_id = _report.id;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return report_id;

        }

        public string _location_string
        {
            get
            {
                string location_nm = "";
                if ((_user != null) && (_user.id != 0))
                {
                    CompanyModel cm = new CompanyModel(_user.company_id);
                    company_location _loc;

                    if ((_user.company_location_id.HasValue) || (db.company_location.Any(item => item.company_id == cm._company.id)))
                    {
                        if (_user.company_location_id.HasValue)
                        {
                            _loc = db.company_location.FirstOrDefault(item => item.id == _user.company_location_id.Value && item.company_id == _user.company_id);
                            if (_loc != null)
                                location_nm = _loc.location_en;
                        }
                        else
                        {
                            _loc = db.company_location.FirstOrDefault(item => item.company_id == cm._company.id);
                            if (_loc != null)
                                location_nm = _loc.location_en;
                        }
                    }
                }

                return location_nm.Trim() != "" ? location_nm.Trim() : "Not Specified";
            }
        }
        #endregion

        public string GetDepartmentString()
        {
            if ((_user != null) && (_user.id != 0))
            {
                var d = db.company_department.FirstOrDefault(x => x.id == _user.company_department_id && x.company_id == _user.company_id);
                return d == null ? "" : d.department_en;
            }
            return "";
        }

        public UserModel()
        {

        }

        public UserModel(int user_id)
        {
            ID = user_id;
            _user = GetById(ID);
        }


        #region User Constructor
        public user GetById(int id)
        {
            ID = id;
            if (id != 0)
            {
                user _user = db.user.FirstOrDefault(item => item.id == id);
                // ui.SetUserDetails(_user.id, _user.password, _user.login_nm);
                return db.user.FirstOrDefault(item => item.id == id);
            }
            else
            {
                return null;
            }
        }

        public LoginUserViewModel Login(string login, string password, bool is_cc)
        {
            LoginUserViewModel loginUser = new LoginUserViewModel();
            /*   var hash = PasswordUtils.GetHash(password);
               var validate = PasswordUtils.Validate(password, hash);

               user _user = db.user.FirstOrDefault(item => item.login_nm.Trim() == login && item.password == hash); //By hash
               //_user = _user ?? db.user.FirstOrDefault(item => item.login_nm.Trim() == login && item.password.Trim() == password); //By password

               //user _user = db.user.FirstOrDefault(item => item.login_nm.Trim() == login && item.password.Trim() == password && item.status_id == 2);
               */
            user _user = db.user.FirstOrDefault(
                item => item.login_nm.Trim() == login &&
                item.password.Trim() == password);
            //_user = db.user.FirstOrDefault(item => item.login_nm.Trim() == login);
            if (_user != null)
            {
                if (_user.number_of_attempts == 5 && _user.last_tried_login_dt > DateTime.Now.AddHours(-1))
                {
                    loginUser.ErrorMessage = "AccountLocked";
                    return loginUser;
                }
                if (_user.number_of_attempts == 5 && _user.last_tried_login_dt < DateTime.Now.AddHours(-1))
                {
                    _user.number_of_attempts = 0;
                    db.SaveChanges();
                }
            }


            if (_user == null)
            {
                var userWhoHasNoAccess = db.user.FirstOrDefault(item => item.login_nm.Trim() == login);
                if (userWhoHasNoAccess != null)
                {
                    if (userWhoHasNoAccess.last_tried_login_dt > DateTime.Now.AddHours(-1) && userWhoHasNoAccess.number_of_attempts < 5)
                    {
                        userWhoHasNoAccess.number_of_attempts++;
                        userWhoHasNoAccess.last_tried_login_dt = DateTime.Now;
                        db.SaveChanges();
                    }
                    else if (userWhoHasNoAccess.last_tried_login_dt < DateTime.Now.AddHours(-1) && userWhoHasNoAccess.number_of_attempts < 5)
                    {
                        userWhoHasNoAccess.number_of_attempts++;
                        userWhoHasNoAccess.last_tried_login_dt = DateTime.Now;
                        db.SaveChanges();
                    }
                    else if (userWhoHasNoAccess.last_tried_login_dt > DateTime.Now.AddHours(-1) && userWhoHasNoAccess.number_of_attempts == 5)
                    {
                        userWhoHasNoAccess.last_tried_login_dt = DateTime.Now;
                        db.SaveChanges();
                        loginUser.ErrorMessage = "AccountLocked";
                        emailNotificationModel.SaveEmailBeforeSend(0, 0, 0, System.Configuration.ConfigurationManager.AppSettings["BCCEmail"],
                            System.Configuration.ConfigurationManager.AppSettings["emailFrom"], is_cc.ToString(), "failed login attempts",
                            "user loginName = " + userWhoHasNoAccess.login_nm + " had failed login attempts in a row", false, 0);
                        return loginUser;
                    }
                    else if (userWhoHasNoAccess.last_tried_login_dt < DateTime.Now.AddHours(-1) && userWhoHasNoAccess.number_of_attempts == 5)
                    {
                        userWhoHasNoAccess.last_tried_login_dt = DateTime.Now;
                        userWhoHasNoAccess.number_of_attempts = 0;

                        db.SaveChanges();
                    }
                }
                return null;
            }
            if ((_user.status_id != 2) & (_user.last_login_dt != null))
            {
                return null;
            }

            if (_user.role_id != ECLevelConstants.level_trainer)
            {
                company _company = db.company.FirstOrDefault(item => item.id == _user.company_id);

                if (_company == null || (_company.status_id != 2 && _company.status_id != 3 && _user.role_id != 8))
                {
                    //  company is not active or pending, means = inactive
                    return null;
                }
            }

            if (_user != null)
            {
                /////// ui.SetUserDetails(_user.id, _user.password, _user.login_nm);
                //////////         bool is_valid_pass = ui.VerifyPassword(password);
                // uncomment when database would be updated
                ////////        if (is_valid_pass)
                {
                    if (_user.last_login_dt.HasValue)
                    {
                        _user.previous_login_dt = _user.last_login_dt;

                    }
                    _user.last_tried_login_dt = DateTime.Now;
                    _user.number_of_attempts = 0;
                    _user.last_login_dt = DateTime.Now;

                    if ((new int[] { 4, 5, 6, 10 }.Contains(_user.role_id)) && (_user.status_id == 3)) //Pending
                    {
                        _user.status_id = 2;
                    }
                    db.SaveChanges();
                    loginUser.user = _user;
                    return loginUser;

                    //  _user.save();
                }
            }
            return null;
        }


        public user Add(user user)
        {
            db.user.Add(user);
            db.SaveChanges();
            return user;
        }
        #endregion

        /// <summary>
        /// list of all reports user can access.
        /// </summary>
        /// <param name="user_id">just pass id of user to look for.</param>
        /// <param name="company_id">pass null or 0 if you need all reports. Pass company_id if user_role=1,2,3 is looking for specific company</param>
        /// <param name="flag">0 - all reports, 1 - active, 2 - completed, 3 - spam, 4 - pending only, 5 - closed</param>
        /// <returns></returns>
        public List<report> ReportsSearch(int? company_id, int flag)
        {
            List<report> all_reports = new List<report>();
            List<report> reports = new List<report>();

            #region Cases based on user level
            if ((_user != null) && (_user.id != 0))
            {
                // if user is from Ec - we can show all reports
                if ((_user.role_id == 1) || (_user.role_id == 2) || (_user.role_id == 3))
                {
                    if ((company_id.HasValue) && (company_id.Value != 0))
                        all_reports = (db.report.Where(item => (item.company_id == company_id.Value))).ToList();
                    else
                        all_reports = (db.report).ToList();

                }
                // if user is top mediator - we can show all reports from company where user is not involved
                if ((_user.role_id == 4) || (_user.role_id == 5))
                {

                    CompanyModel cm = new CompanyModel(_user.company_id);
                    List<company> availableCompanies = cm.AdditionalCompanies();

                    List<int> involved_report_ids = (db.report_mediator_involved.Where(item => (item.mediator_id == _user.id)).Select(item => item.report_id)).ToList();
                    if (availableCompanies.Count > 1)
                    {
                        List<int> availableCompaniesID = availableCompanies.Select(t => t.id).ToList().Distinct().ToList();
                        all_reports = (db.report.Where(item => ((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id))))).ToList();

                    }
                    else
                        all_reports = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id))))).ToList();
                }
                // if user is regular mediator - we show only reports where he is assigned and hide all others
                if ((_user.role_id == 6))
                {
                    List<int> assigned_report_ids = (db.report_mediator_assigned.Where(item => ((item.mediator_id == _user.id) && (item.status_id == 2))).Select(item => item.report_id)).ToList();
                    all_reports = (db.report.Where(item => assigned_report_ids.Contains(item.id))).ToList();
                }
                // if user is legal - kick him away))
                if ((_user.role_id == 7))
                {
                    return all_reports;
                }
                // if user is reporter - we can only show his report
                if ((_user.role_id == 8))
                {
                    all_reports = (db.report.Where(item => item.reporter_user_id == _user.id)).ToList();
                }
            }
            #endregion

            if (all_reports.Count > 0)
            {
                ReportModel temp_rm = new ReportModel();
                if (flag == 1)
                {
                    // active only
                    foreach (report _temp in all_reports)
                    {
                        //     rm = ReportModel(_temp.id);
                        //   temp_status = new ReportModel(_temp.id)._investigation_status;

                        temp_rm = new ReportModel(_temp.id);
                        if ((!temp_rm.IsSpamScreen()) && (!temp_rm.IsPendingScreen()) && (!temp_rm.IsClosedScreen()) && (!temp_rm.IsCompletedScreen()))
                            //  if ((temp_status == 3) || (temp_status == 4) || (temp_status == 5))
                            reports.Add(_temp);
                    }
                }
                else if (flag == 2)
                {
                    // closed
                    foreach (report _temp in all_reports)
                    {
                        if (new ReportModel(_temp.id).IsCompletedScreen())
                            reports.Add(_temp);

                        //  if (new ReportModel(_temp.id)._investigation_status == 6)
                        //      reports.Add(_temp);
                    }
                }
                else if (flag == 3)
                {
                    // spam
                    foreach (report _temp in all_reports)
                    {
                        if (new ReportModel(_temp.id).IsSpamScreen())
                            reports.Add(_temp);
                        //  if (new ReportModel(_temp.id)._investigation_status == 7)
                        ///     reports.Add(_temp);
                    }
                }
                else if (flag == 4)
                {
                    // pending
                    foreach (report _temp in all_reports)
                    {
                        if (new ReportModel(_temp.id).IsPendingScreen())
                            reports.Add(_temp);
                        //   if ((new ReportModel(_temp.id)._investigation_status == 1) || (new ReportModel(_temp.id)._investigation_status == 2))
                        //       reports.Add(_temp);
                    }
                }
                else if (flag == 5)
                {
                    // pending
                    foreach (report _temp in all_reports)
                    {
                        if (new ReportModel(_temp.id).IsClosedScreen())
                            reports.Add(_temp);
                        //   if ((new ReportModel(_temp.id)._investigation_status == 1) || (new ReportModel(_temp.id)._investigation_status == 2))
                        //       reports.Add(_temp);
                    }
                }
                else
                {
                    reports = all_reports;
                }
            }
            else
                reports = all_reports;

            return reports;
        }

        /// <summary>
        /// returns id's of reports user can access
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public List<int> GetReportIds(int? report_id)
        {
            List<int> all_report_ids = new List<int>();

            // if user is top mediator - we can show all reports from company where user is not involved
            if ((_user != null) && (_user.id != 0))
            {
                #region List of reports for the user
                if ((report_id.HasValue) && (report_id != 0))
                {
                    // if user is top mediator - we can show all reports from company where user is not involved
                    if ((_user.role_id == 4) || (_user.role_id == 5))
                    {
                        List<int> involved_report_ids = (db.report_mediator_involved.Where(item => (item.mediator_id == _user.id)).Select(item => item.report_id)).ToList();

                        CompanyModel cm = new CompanyModel(_user.company_id);
                        List<company> availableCompanies = cm.AdditionalCompanies();

                        if (availableCompanies.Count > 1)
                        {
                            List<int> availableCompaniesID = availableCompanies.Select(t => t.id).ToList().Distinct().ToList();

                            all_report_ids = (db.report.Where(item => (((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id)) && (item.id == report_id)))).Select(item => item.id)).ToList();

                        }
                        else
                            all_report_ids = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id)) && (item.id == report_id))).Select(item => item.id)).ToList();
                    }
                    // if user is regular mediator - we show only reports where he is assigned and hide all others
                    if ((_user.role_id == 6))
                    {
                        List<int> assigned_report_ids = (db.report_mediator_assigned.Where(item => (item.mediator_id == _user.id) && (item.status_id == 2) && (item.report_id == report_id)).Select(item => item.report_id)).ToList();
                        all_report_ids = (db.report.Where(item => assigned_report_ids.Contains(item.id)).Select(item => item.id)).ToList();
                    }
                    if ((_user.role_id == 8))
                    {
                        all_report_ids = (db.report.Where(item => (item.reporter_user_id == _user.id) && (item.id == report_id)).Select(item => item.id)).ToList();
                    }
                }
                else
                {
                    // if user is top mediator - we can show all reports from company where user is not involved
                    if ((_user.role_id == 4) || (_user.role_id == 5))
                    {
                        List<int> involved_report_ids = (db.report_mediator_involved.Where(item => (item.mediator_id == _user.id)).Select(item => item.report_id)).ToList();

                        CompanyModel cm = new CompanyModel(_user.company_id);
                        List<company> availableCompanies = cm.AdditionalCompanies();

                        if (availableCompanies.Count > 1)
                        {
                            List<int> availableCompaniesID = availableCompanies.Select(t => t.id).ToList().Distinct().ToList();
                            all_report_ids = (db.report.Where(item => (((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id))))).Select(item => item.id)).ToList();

                        }
                        else
                            all_report_ids = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id)))).Select(item => item.id)).ToList();
                    }
                    // if user is regular mediator/ legal counsil - we show only reports where he is assigned and hide all others
                    if ((_user.role_id == 6) || (_user.role_id == 7))
                    {
                        List<int> assigned_report_ids = (db.report_mediator_assigned.Where(item => ((item.mediator_id == _user.id) && (item.status_id == 2))).Select(item => item.report_id)).ToList();
                        all_report_ids = (db.report.Where(item => assigned_report_ids.Contains(item.id)).Select(item => item.id)).ToList();
                    }
                    if ((_user.role_id == 8))
                    {
                        all_report_ids = (db.report.Where(item => item.reporter_user_id == _user.id).Select(item => item.id)).ToList();
                    }
                }
                #endregion
            }

            return all_report_ids;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="company_id"></param>
        /// <param name="flag">0 - all reports, 1 - active, 2 - closed, 3 - spam, 4 - pending only</param>
        /// <returns></returns>
        public List<report> UnreadReport(int? company_id, int flag)
        {
            List<report> all_reports = ReportsSearch(company_id, flag);
            List<report> unread_report = new List<report>();
            report temp_report;

            for (int i = all_reports.Count - 1; i >= 0; i--)

            //      for (int i = 0; i < all_reports.Count; i++)
            {
                temp_report = all_reports[i];
                //      if (unread_report.Count < 3)
                {
                    if (db.report_user_read.Where(item => ((item.user_id == ID) && (item.report_id == temp_report.id))).Count() == 0)
                        unread_report.Add(temp_report);
                }
            }

            return unread_report;
        }


        /// <summary>
        ///  https://projects.invisionapp.com/d/#/console/2453713/75211865/preview
        /// </summary>
        /// <param name="user_id">user_id</param>
        /// <param name="task_status">0 - all tasks, 1 - active, 2 - competed</param>
        /// <returns></returns>
        public List<task> UserTasks(int task_status, int? report_id, bool is_user_only)
        {
            List<int> all_report_ids = new List<int>();
            List<task> all_tasks = new List<task>();
            all_report_ids = GetReportIds(report_id);

            if ((report_id.HasValue) && (report_id.Value != 0))
            {
                if (task_status == 0)
                    all_tasks = db.task.Where(item => item.assigned_to == ID && item.report_id == report_id.Value).ToList();
                if (task_status == 1)
                    all_tasks = db.task.Where(item => item.assigned_to == ID && item.is_completed == false && item.report_id == report_id.Value).ToList();
                if (task_status == 2)
                    all_tasks = db.task.Where(item => item.assigned_to == ID && item.is_completed == true && item.report_id == report_id.Value).ToList();
            }
            else
            {
                if (is_user_only)
                {
                    if (task_status == 0)
                        all_tasks = db.task.Where(item => item.assigned_to == ID && all_report_ids.Contains(item.report_id)).ToList();
                    if (task_status == 1)
                        all_tasks = db.task.Where(item => item.assigned_to == ID && item.is_completed == false && all_report_ids.Contains(item.report_id)).ToList();
                    if (task_status == 2)
                        all_tasks = db.task.Where(item => item.assigned_to == ID && item.is_completed == true && all_report_ids.Contains(item.report_id)).ToList();

                }
                else
                {
                    if (task_status == 0)
                        all_tasks = db.task.Where(item => ((all_report_ids.Contains(item.report_id)))).ToList();
                    if (task_status == 1)
                        all_tasks = db.task.Where(item => ((all_report_ids.Contains(item.report_id)) && item.is_completed == false)).ToList();
                    if (task_status == 2)
                        all_tasks = db.task.Where(item => ((all_report_ids.Contains(item.report_id)) && item.is_completed == true)).ToList();
                }

            }

            return all_tasks;

        }

        /// <summary>
        /// ETO krasnie krujki vozle TASKS((neprichitannie tasks). esli report_id = 0(null), to togda v samom verhnem menu. esli report_id = chislu, to vnutri reporta
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="report_id">if report_id = null or 0, its total for this user</param>
        /// <param name="status_id">if status_id = 0 - all tasks, or can be just active/inactive</param>
        /// 
        /// <returns></returns>
        public int UnreadTasksQuantity(int? report_id, bool is_user_only, int status_id)
        {
            int tasks_quantity = 0;

            List<int> all_task_ids = new List<int>();
            List<int> all_report_ids = new List<int>();

            List<int> read_task_ids = new List<int>();
            List<int> unread_task_ids = new List<int>();

            if ((_user != null) && (_user.id != 0))
            {
                all_report_ids = GetReportIds(report_id);

                #region Got All tasks_id's for current user
                // old function - replaced with the new call below. check if correct

                #endregion
                /*
                                if ((_user.role_id == 4) || (_user.role_id == 5) || (_user.role_id == 6) || (_user.role_id == 7))
                                    all_task_ids = UserTasks(status_id, null, is_user_only).Select(t => t.id).ToList();

                                read_task_ids = (db.task_user_read.Where(item => (all_task_ids.Contains(item.task_id) && (item.user_id == ID))).Select(item => item.task_id)).ToList();

                                //all_message_ids
                                //   unread_message_ids = all_message_ids.Except(read_message_ids);
                                unread_task_ids = all_task_ids.Where(id => !read_task_ids.Contains(id)).ToList();
                                tasks_quantity = unread_task_ids.Count;
                                */

                List<task> tasks = UserTasks(status_id, report_id, is_user_only);
                List<TaskExtended> list_tsk = new List<TaskExtended>();
                foreach (task _task in tasks)
                {
                    TaskExtended tsk = new TaskExtended(_task.id, this.ID);

                    if (!tsk.IsRead())
                        tasks_quantity++;

                }
            }

            return tasks_quantity;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="report_id">null or 0 - all reports</param>
        /// <param name="thread_id">0 - all messages, 1 - reporter thread, 2 - mediators thread, 3 - privilege thread</param>
        /// <returns></returns>
        public int Unread_Messages_Quantity(int? report_id, int thread_id)
        {
            int quantity = 0;
            List<message> all_messages = UserMessages(report_id, thread_id);
            MessageExtended _extended;
            foreach (message _message in all_messages)
            {
                _extended = new MessageExtended(_message.id, ID);

                // is unread
                if (!_extended.IsRead())
                    quantity++;
            }
            return quantity;
        }


        /// <summary>
        /// Returns all messages for user ( all reports or for specific report)
        /// </summary>
        /// <param name="report_id">if report_id = null or 0, its total messages for this user</param>
        /// <param name="thread_id">0 - all messages, 1 - reporter thread, 2 - mediators thread, 3 - privilege thread</param>
        /// <returns></returns>
        public List<message> UserMessages(int? report_id, int thread_id)
        {
            List<message> all_messages = new List<message>();
            List<int> all_report_ids = new List<int>();

            if ((_user != null) && (_user.id != 0))
            {
                all_report_ids = GetReportIds(report_id);

                #region Got All messages for current user
                if ((_user.role_id == 8) || (thread_id == 1))
                {
                    // reporter can see only messages with reporter_access == 1 
                    all_messages = (db.message.Where(item => (all_report_ids.Contains(item.report_id) && (item.reporter_access == 1)))).ToList();
                }
                else if ((_user.role_id != 7) && (thread_id == 3))
                {
                    // messages to mediator from legal
                    all_messages = (db.message.Where(item => (all_report_ids.Contains(item.report_id) && (item.reporter_access == 3) && ((item.sent_to_id == _user.id) || (item.sender_id == _user.id))))).ToList();
                }
                else if (_user.role_id == 7)
                {
                    // to legal messages
                    all_messages = (db.message.Where(item => (all_report_ids.Contains(item.report_id) && (item.reporter_access == 3)))).ToList();
                }
                else if (thread_id == 0)
                {
                    // all messages
                    all_messages = (db.message.Where(item => (all_report_ids.Contains(item.report_id)))).ToList();
                }
                else
                {
                    all_messages = (db.message.Where(item => (all_report_ids.Contains(item.report_id) && (item.reporter_access == 2)))).ToList();
                }
                #endregion

            }
            return all_messages;
        }


        public int CaseTasksQuantity(int? report_id)
        {
            int tasks_quantity = 0;

            List<int> all_task_ids = new List<int>();
            List<int> all_report_ids = new List<int>();

            if ((_user != null) && (_user.id != 0))
            {
                all_report_ids = GetReportIds(report_id);

                #region Got All tasks_id's for current user
                if ((_user.role_id == 4) || (_user.role_id == 5) || (_user.role_id == 6))
                    tasks_quantity = (db.task.Where(item => (all_report_ids.Contains(item.report_id) && (item.assigned_to == _user.id)))).ToList().Count;
                #endregion
            }

            return tasks_quantity;
        }

        public int CaseMessagesQuantity(int? report_id)
        {
            List<message> all_messages_reporter = (UserMessages(report_id, 1).Where(item => item.sender_id == ID)).ToList();
            List<message> all_messages_mediator = (UserMessages(report_id, 2).Where(item => item.sender_id == ID)).ToList();
            //       List<message> all_messages_privilege = (UserMessages(report_id, 3).Where(item => item.sender_id == ID)).ToList();
            return all_messages_reporter.Count + all_messages_mediator.Count;
        }

        public int CaseActionsQuantityNoCheck(int? report_id)
        {
            int actions_quantity = 0;

            List<int> all_report_ids = new List<int>();

            if ((_user != null) && (_user.id != 0))
            {
                if (report_id.HasValue)
                    all_report_ids.Add(report_id.Value);
                else
                    all_report_ids = GetReportIds(report_id);

                #region Got All action's for current user
                actions_quantity = (db.report_log.Where(item => (all_report_ids.Contains(item.report_id) && (item.user_id == _user.id)))).ToList().Count;
                #endregion
            }

            return actions_quantity;
        }


        public bool AddToMediators(int mediator_id, int report_id)
        {
            List<report_mediator_assigned> mediators =
                db.report_mediator_assigned.Where(item => ((item.report_id == report_id) && (item.mediator_id == mediator_id))).ToList();

            try
            {
                if (mediators != null && mediators.Count > 0)
                {
                    mediators.First().status_id = 2;
                    db.report_mediator_assigned.AddOrUpdate(mediators.First());
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    report_mediator_assigned result = new report_mediator_assigned()
                    {
                        report_id = report_id,
                        mediator_id = mediator_id,
                        assigned_dt = DateTime.Now,
                        status_id = 2,
                        last_update_dt = DateTime.Now,
                        user_id = 1
                    };

                    db.report_mediator_assigned.Add(result);
                }

                db.SaveChanges();
                return true;
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                Console.WriteLine("Meditor didn't add");
                return false;
            }
        }

        public bool RemoveMediator(int mediator_id, int report_id)
        {
            List<report_mediator_assigned> mediators =
                db.report_mediator_assigned.Where(item => ((item.report_id == report_id) && (item.mediator_id == mediator_id))).ToList();
            try
            {
                if (mediators != null && mediators.Count > 0)
                {
                    mediators.First().status_id = 1;
                    db.report_mediator_assigned.AddOrUpdate(mediators.First());
                }
                else
                {
                    report_mediator_assigned result = new report_mediator_assigned()
                    {
                        user_id = 1,
                        mediator_id = mediator_id,
                        status_id = 1,
                        last_update_dt = DateTime.Now,
                        report_id = report_id
                    };

                    db.report_mediator_assigned.Add(result);
                }

                db.SaveChanges();
                return true;
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                Console.WriteLine("Meditor didn't removed");
                return false;
            }
        }

        public bool PromoteCase(int report_id, int mediator_id, string description, int new_status, int? reason_id, int sign_off_mediator_id)
        {
            try
            {
                int case_closure_reason_id = 0;
                if (reason_id.HasValue)
                    case_closure_reason_id = reason_id.Value;

                if ((case_closure_reason_id == 0) && (new_status == 9))
                {
                    var ps = db.report_investigation_status.OrderByDescending(x => x.id).FirstOrDefault(x => x.report_id == report_id);
                    if ((ps != null) && (ps.case_closure_reason_id.HasValue))
                    {
                        case_closure_reason_id = ps.case_closure_reason_id.Value;
                    }
                }

                //  outcome_message = outcome,   outcome acts like a case_closure_report. We would need to change this in future

                report_investigation_status report_investigation_status;

                report_investigation_status = new report_investigation_status()
                {
                    report_id = report_id,
                    investigation_status_id = new_status,
                    created_date = DateTime.Now,
                    user_id = mediator_id,
                    description = description,
                    outcome_message = "",
                    case_closure_reason_id = case_closure_reason_id
                };

                db.report_investigation_status.Add(report_investigation_status);

                report _report = db.report.Where(item => (item.id == report_id)).FirstOrDefault();
                _report.status_id = new_status;
                _report.last_update_dt = DateTime.Now;
                _report.user_id = mediator_id;
                _report.cc_crime_statistics_category_id = null;

                db.SaveChanges();


                return true;
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                Console.WriteLine("Case wasn't resolved. Ex:" + ex.Data);
                return false;
            }
        }
        public bool updateLogoUser(int user_id, string path)
        {
            bool result = false;
            if (user_id > 0 && path != null && path != string.Empty)
            {
                user user = db.user.FirstOrDefault(item => (item.id == user_id));
                user.photo_path = path;
                db.user.AddOrUpdate(user);
                db.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool CanEditUserProfiles
        {
            get
            {
                if (_user == null)
                {
                    return false;
                }
                return _user.role_id == 5 || _user.user_permissions_change_settings == 1;
            }
        }

        public UsersReportIDsViewModel GetAllUserReportIdsLists()
        {
            UsersReportIDsViewModel vm = new UsersReportIDsViewModel();
            List<int> all_reports_id = new List<int>();
            List<int> statuses_match_all_report_id = new List<int>();
            CompanyModel cm = new CompanyModel(_user.company_id);
            List<company> availableCompanies = cm.AdditionalCompanies();

            if ((_user.role_id == 4) || (_user.role_id == 5))
            {
                var involved_report_ids = (db.report_mediator_involved.Where(item => (item.mediator_id == _user.id)).Select(item => item.report_id));

                if (availableCompanies.Count > 1)
                {
                    List<int> availableCompaniesID = availableCompanies.Select(t => t.id).ToList().Distinct().ToList();

                    vm.all_report_ids = (db.report.Where(item => ((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id)))).Select(item => item.id)).ToList();
                    vm.all_active_report_ids = (db.report.Where(item => ((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Investigation))).Select(item => item.id)).ToList();
                    vm.all_completed_report_ids = (db.report.Where(item => ((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Completed))).Select(item => item.id)).ToList();
                    vm.all_spam_report_ids = (db.report.Where(item => ((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Spam))).Select(item => item.id)).ToList();
                    vm.all_closed_report_ids = (db.report.Where(item => ((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Closed))).Select(item => item.id)).ToList();
                    vm.all_pending_report_ids = (db.report.Where(item => ((availableCompaniesID.Contains(item.company_id)) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Pending || item.status_id == (int)CaseStatusConstants.CaseStatusValues.Review))).Select(item => item.id)).ToList();
                }
                else
                {
                    vm.all_report_ids = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id)))).Select(item => item.id)).ToList();
                    vm.all_active_report_ids = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Investigation))).Select(item => item.id)).ToList();
                    vm.all_completed_report_ids = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Completed))).Select(item => item.id)).ToList();
                    vm.all_spam_report_ids = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Spam))).Select(item => item.id)).ToList();
                    vm.all_closed_report_ids = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Closed))).Select(item => item.id)).ToList();
                    vm.all_pending_report_ids = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Pending || item.status_id == (int)CaseStatusConstants.CaseStatusValues.Review))).Select(item => item.id)).ToList();

                }
            }
            // if user is regular mediator/ legal counsil - we show only reports where he is assigned and hide all others

            if ((_user.role_id == 6) || (_user.role_id == 7))
            {
                var assigned_report_ids = (db.report_mediator_assigned.Where(item => ((item.mediator_id == _user.id) && (item.status_id == 2))).Select(item => item.report_id));

                vm.all_report_ids = (db.report.Where(item => ((assigned_report_ids.Contains(item.id)))).Select(item => item.id)).ToList();
                vm.all_active_report_ids = (db.report.Where(item => ((assigned_report_ids.Contains(item.id) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Investigation)))).Select(item => item.id)).ToList();
                vm.all_completed_report_ids = (db.report.Where(item => ((assigned_report_ids.Contains(item.id) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Completed)))).Select(item => item.id)).ToList();
                vm.all_spam_report_ids = (db.report.Where(item => ((assigned_report_ids.Contains(item.id) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Spam)))).Select(item => item.id)).ToList();
                vm.all_closed_report_ids = (db.report.Where(item => ((assigned_report_ids.Contains(item.id) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Closed)))).Select(item => item.id)).ToList();
                vm.all_pending_report_ids = (db.report.Where(item => ((assigned_report_ids.Contains(item.id)) && (item.status_id == (int)CaseStatusConstants.CaseStatusValues.Pending || item.status_id == (int)CaseStatusConstants.CaseStatusValues.Review))).Select(item => item.id)).ToList();
            }

            return vm;

        }


        public List<CasePreviewViewModel> ReportPreviews(List<int> report_ids, string investigation_status, int delay_allowed, bool is_cc)
        {
            var severities = db.severity.Select(z => new { id = z.id, severity_en = z.severity_en });
            var colors = db.color.Select(z => new { id = z.id, color_code = z.color_code });
            IEnumerable<int> top_mediator_ids = db.user.Where(item => (item.company_id == _user.company_id) && (item.role_id == 4 || item.role_id == 5)).Select(t => t.id);
            string base_url = ConfigurationManager.AppSettings["SiteRoot"];
            string _no_photo_path = base_url + "/Content/Icons/noPhoto.png";
            if (report_ids != null)
            {
                report_ids = report_ids.OrderByDescending(t => t).ToList();
            }
            //var reports = report_ids.Select(x => new CasePreviewViewModel(x, user.id)).ToList();
            var reports = new List<CasePreviewViewModel>();
            if (report_ids != null)
            {
                reports = report_ids
                    .Select(x =>
                    {
                        var rm = new ReportModel(x);
                        var access_mediators = rm.MediatorsWhoHasAccessToReportQuick(top_mediator_ids);
                        return new CasePreviewViewModel
                        {
                            current_status = investigation_status,

                            report_id = rm._report.id,
                            company_id = rm._report.company_id,

                            case_number = rm._report.display_name,
                            case_dt_s = rm._report.reported_dt.Ticks,
                            cc_is_life_threating = rm._report.cc_is_life_threating,
                            total_days = Math.Floor((DateTime.Now.Date - rm._report.reported_dt.Date).TotalDays),

                            location = rm._reportStringModel.LocationString(),
                            case_secondary_types = rm._reportStringModel.SecondaryTypeString(),
                            days_left = rm.GetThisStepDaysLeft(delay_allowed),

                            reported_dt = rm._reportStringModel.ReportedDateString(),
                    //  case_dt = rm.IncidentDateString(),

                    tasks_number = rm.ReportTasksCount(0).ToString(),
                            messages_number = rm.UserMessagesCountNotSecure(ID, 0).ToString(),


                            mediators = (access_mediators == null || access_mediators.Count == 0) ? null : access_mediators.Select(z => new
                            {
                                id = z.id,
                                first_nm = z.first_nm,
                                last_nm = z.last_nm,
                                photo_path = string.IsNullOrWhiteSpace(z.photo_path) ? _no_photo_path : z.photo_path,
                        //photo_path = glb.Photo_Path_String(z.photo_path, 1, 5),
                        is_owner = z.is_owner
                            }),
                            case_color_code = (rm._report.report_color_id == 0) ? colors.Where(item => item.id == 1).FirstOrDefault().color_code : colors.Where(item => item.id == rm._report.report_color_id).FirstOrDefault().color_code,
                            severity_s = !rm._report.severity_id.HasValue ? LocalizationGetter.GetString("Unspecified").ToUpper() : severities.FirstOrDefault(z => z.id == rm._report.severity_id).severity_en,
                            severity_id = !rm._report.severity_id.HasValue ? 0 : rm._report.severity_id.Value,
                            under_status_message = rm._reportStringModel.DaysLeftClosedSpamMessage(delay_allowed),
                            last_update_dt = m_DateTimeHelper.ConvertDateToShortString(rm._report.last_update_dt),
                            agentName = rm._report.agent_id > 0 ? db.user.Find(rm._report.agent_id).first_nm : ""
                        };

                    }).ToList();
            }
            return reports;
        }


    }
}