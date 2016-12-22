using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EC.Models.Database;
using EC.Models;
using EC.Models.ECModel;
using EC.Model.Impl;
using EC.Model.Interfaces;
using EC.Constants;


namespace EC.Models
{
    public class UserModel : BaseModel
    {
        public static readonly UserModel inst = new UserModel();
        GlobalFunctions glb = new GlobalFunctions();
        UserItems ui = new UserItems();

        #region Properties
        public int ID
        { get; set; }

        public user _user
        {
            get
            {
                return GetById(ID);
            }
        }

        public int _reporter_report_id
        {
            get
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

                }

                return report_id;
            }
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
                            _loc = db.company_location.FirstOrDefault(item => item.id == _user.company_location_id.Value);
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

                return location_nm.Trim();
            }
        }
        #endregion

        public UserModel()
        {

        }

        public UserModel(int user_id)
        {
            ID = user_id;
        }


        #region User Constructor
        public user GetById(int id)
        {
            ID = id;
            if (id != 0)
            {
                user _user = db.user.FirstOrDefault(item => item.id == id);
                ui.SetUserDetails(_user.id, _user.password, _user.login_nm);
                return db.user.FirstOrDefault(item => item.id == id);
            }
            else
            {
                return null;
            }
        }

        public user Login(string login, string password)
        {
            user _user = db.user.FirstOrDefault(item => item.login_nm == login && item.password == password);

            if (_user != null)
            {
                ui.SetUserDetails(_user.id, _user.password, _user.login_nm);
       //////////         bool is_valid_pass = ui.VerifyPassword(password);
                // uncomment when database would be updated
        ////////        if (is_valid_pass)
                {
                    if (_user.last_login_dt.HasValue)
                        _user.previous_login_dt = _user.last_login_dt;
                    _user.last_login_dt = DateTime.Now;
                    return _user;

                  //  _user.save();
                }
            }
            return null;
        }


        public user registration(string name, string pasword)
        {
            //    users.Add(name, new User() { Email = name, Login = name, Password = pasword });
            return null;
        }
        public user GetUserByEmail(string email)
        {
            return db.user.FirstOrDefault(item => item.email == email);
        }

        public user GetUserByLogin(string login)
        {
            user _user = db.user.FirstOrDefault(item => item.login_nm == login);
            ui.SetUserDetails(_user.id, _user.password, _user.login_nm);

            return _user;
        }

        public user Add(user user)
        {
            db.user.Add(user);
            db.SaveChanges();
            return user;
        }
        #endregion

        public List<user_role> GetAllRoles()
        {
            return db.user_role.ToList();
        }


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
                    List<int> involved_report_ids = (db.report_mediator_involved.Where(item => (item.mediator_id == _user.id)).Select(item => item.report_id)).ToList();
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
                        if ((!temp_rm.IsSpamScreen) && (!temp_rm.IsPendingScreen) && (!temp_rm.IsClosedScreen) && (!temp_rm.IsCompletedScreen))
                            //  if ((temp_status == 3) || (temp_status == 4) || (temp_status == 5))
                            reports.Add(_temp);
                    }
                }
                else if (flag == 2)
                {
                    // closed
                    foreach (report _temp in all_reports)
                    {
                        if (new ReportModel(_temp.id).IsCompletedScreen)
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
                        if (new ReportModel(_temp.id).IsSpamScreen)
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
                        if (new ReportModel(_temp.id).IsPendingScreen)
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
                        if (new ReportModel(_temp.id).IsClosedScreen)
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

        public List<int> ReportsSearchIds(int? company_id, int flag)
        {
            List<int> all_reports_id = new List<int>();
            List<report> reports = new List<report>();
            all_reports_id = GetReportIds(null);

            //    select rep_id from[Marine].[dbo].[testt]  z
            //where id in (select max(id) from[Marine].[dbo].[testt] z2 group by z2.rep_id) and z.status_id = 4
            var refGroupInvestigationStatuses = (from m in db.report_investigation_status
                                 group m by m.report_id into refGroup
                                 select refGroup.OrderByDescending(x =>x.id).FirstOrDefault());
            List<int> statuses_match_all_report_id = new List<int>();
            if (flag == 0)
            {
                statuses_match_all_report_id = (from m in refGroupInvestigationStatuses
                                                select m.report_id).ToList();
            }
            if (flag == 1)
            {
                //active
                statuses_match_all_report_id = (from m in refGroupInvestigationStatuses
                                                where m.investigation_status_id == ECGlobalConstants.investigation_status_investigation
                                                select m.report_id).ToList();
            }
            if (flag == 2)
            {
                // completed
                statuses_match_all_report_id = (from m in refGroupInvestigationStatuses
                                                where m.investigation_status_id == ECGlobalConstants.investigation_status_resolution || m.investigation_status_id == ECGlobalConstants.investigation_status_completed
                                                select m.report_id).ToList();
            }
            if (flag == 3)
            {
                //spam
                statuses_match_all_report_id = (from m in refGroupInvestigationStatuses
                                                where m.investigation_status_id == ECGlobalConstants.investigation_status_spam
                                                select m.report_id).ToList();
            }
            if (flag == 4)
            {
                //pending
                statuses_match_all_report_id = (from m in refGroupInvestigationStatuses
                                                where m.investigation_status_id == ECGlobalConstants.investigation_status_pending || m.investigation_status_id == ECGlobalConstants.investigation_status_review 
                                                select m.report_id).ToList();

                List<int> reports_with_history = (from m in refGroupInvestigationStatuses
                                                  where m.investigation_status_id > 0
                                                  select m.report_id).ToList();
                IEnumerable<int> excluded = all_reports_id.Except(reports_with_history);

                statuses_match_all_report_id.AddRange(excluded);

            }
            if (flag == 5)
            {
                //closed
                statuses_match_all_report_id = (from m in refGroupInvestigationStatuses
                                                where m.investigation_status_id == ECGlobalConstants.investigation_status_closed
                                               
                                                select m.report_id).ToList();
            }

            IEnumerable<int> both = all_reports_id.Intersect(statuses_match_all_report_id);
            //.Where(j => (j.investigation_status_id == 4))
            //var refGroupInvestigationStatuses = (from m in db.report_investigation_status



         /*   var query = report_investigation_status.GroupBy(p => p.report_id)
                  .Select(g => g.OrderByDescending(p => p.id).FirstOrDefault()
                   );
            var query1 = query.Select(decimal ) // where status = 4.
            */



            /*      from row in Posts
                  group row by row.type
                  into g
                  select new
                  {
                      Content = (from row2 in g orderby row2.date descending select row2.content).FirstOrDefault(),
                      Type = g.Key
                  }*/

            //https://msdn.microsoft.com/en-us/library/bb738512(v=vs.100).aspx
            // http://stackoverflow.com/questions/16273485/entity-framework-select-one-of-each-group-by-date

            ///
            /// IEnumerable<int> both = id1.Intersect(id2);

            ///     foreach (int id in both)
            ////         Console.WriteLine(id);

/*
            if (all_reports_id.Count > 0)
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
                        if ((!temp_rm.IsSpamScreen) && (!temp_rm.IsPendingScreen) && (!temp_rm.IsClosedScreen) && (!temp_rm.IsCompletedScreen))
                            //  if ((temp_status == 3) || (temp_status == 4) || (temp_status == 5))
                            reports.Add(_temp);
                    }
                }
                else if (flag == 2)
                {
                    

                    // closed
                    foreach (report _temp in all_reports)
                    {
                        if (new ReportModel(_temp.id).IsCompletedScreen)
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
                        if (new ReportModel(_temp.id).IsSpamScreen)
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
                        if (new ReportModel(_temp.id).IsPendingScreen)
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
                        if (new ReportModel(_temp.id).IsClosedScreen)
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
            */
            return both.ToList();
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

        public int UnreadReportQuantity(int? company_id, int flag)
        {
            int quantity = 0;
            List<report> all_reports = UnreadReport(company_id, flag);
            quantity = all_reports.Count;
            return quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="report_id"></param>
        /// <param name="flag">0 - all reports, 1 - active, 2 - closed, 3 - spam, 4 - pending only</param>
        /// <returns></returns>
        public List<int> ReportsSearchIds_bkp(int? company_id, int flag)
        {
            List<report> _reportsSearch = ReportsSearch(company_id, flag).ToList();

            List<int> search_report_ids = new List<int>();
            foreach (report _report in _reportsSearch)
            {
                search_report_ids.Add(_report.id);
            }

            return search_report_ids;
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

                if ((_user.role_id == 4) || (_user.role_id == 5) || (_user.role_id == 6) || (_user.role_id == 7))
                    all_task_ids = UserTasks(status_id, null, is_user_only).Where(x => x.created_by != _user.id).Select(t => t.id).ToList();

                read_task_ids = (db.task_user_read.Where(item => (all_task_ids.Contains(item.task_id) && (item.user_id == ID))).Select(item => item.task_id)).ToList();

                //all_message_ids
                //   unread_message_ids = all_message_ids.Except(read_message_ids);
                unread_task_ids = all_task_ids.Where(id => !read_task_ids.Contains(id)).ToList();
                tasks_quantity = unread_task_ids.Count;
            }

            return tasks_quantity;
        }


        /// <summary>
        ///  https://projects.invisionapp.com/d/#/console/2453713/75211865/preview
        /// </summary>
        /// <param name="task_status">0 - all tasks, 1 - active, 2 - competed</param>
        /// <returns></returns>
        public int UnreadActivityUserTaskQuantity(int? report_id, bool is_user_only)
        {
            int quantity = 0;
            List<task> all_tasks = UserTasks(0, report_id, is_user_only);
            TaskExtended _extended;
            foreach (task _task in all_tasks)
            {
                _extended = new TaskExtended(_task.id, ID);

                // is unread or has new actvity
                if ((!_extended.IsRead()) || (_extended.HasNewActivity()))
                    quantity++;
            }
            return quantity;
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

        public int CaseActionsQuantity(int? report_id)
        {
            int actions_quantity = 0;

            List<int> all_report_ids = new List<int>();

            if ((_user != null) && (_user.id != 0))
            {
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
                Console.WriteLine("Meditor didn't removed");
                return false;
            }
        }

        public bool CreateNewTask(NameValueCollection form, HttpFileCollectionBase files)
        {
            int mediator_id = Convert.ToInt16(form["user_id"]);
            int report_id = Convert.ToInt16(form["report_id"]);
            string taskName = form["taskName"];
            string taskDescription = form["taskDescription"];
            int assignTo = Convert.ToInt16(form["taskAssignTo"]);
            DateTime? dueDate = null;
            try
            {
                dueDate = DateTime.ParseExact(form["dueDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                dueDate = null;
            }

            string Root = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            string UploadedDirectory = "Upload";
            string UploadTarget = Root + UploadedDirectory + @"\";

            try
            {


                task newTask = new task()
                {
                    report_id = report_id,
                    title = taskName,
                    description = taskDescription,
                    assigned_to = assignTo,
                    is_completed = false,
                    due_date = dueDate,
                    created_on = DateTime.Now,
                    created_by = mediator_id
                };

                db.task.Add(newTask);
                db.SaveChanges();


                for (var i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    var fileName = DateTime.Now.Ticks + Path.GetExtension(file.FileName);
                    string path = @"\" + UploadedDirectory + @"\" + fileName;

                    attachment attach = new attachment()
                    {
                        report_id = null,
                        report_task_id = newTask.id,
                        status_id = 2,
                        path_nm = path,
                        file_nm = file.FileName,
                        extension_nm = Path.GetExtension(file.FileName),
                        effective_dt = System.DateTime.Now,
                        expiry_dt = System.DateTime.Now,
                        last_update_dt = System.DateTime.Now,
                        user_id = mediator_id
                    };

                    file.SaveAs(UploadTarget + fileName);
                    db.attachment.Add(attach);
                    db.SaveChanges();

                }

                glb.UpdateReportLog(mediator_id, 10, report_id, taskName, null, "");

                #region New Task - Email to Asignee
                UserModel um = new UserModel(assignTo);
                ReportModel _rm = new ReportModel(report_id);

                if ((um._user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(um._user.email.Trim()))
                {
                    List<string> to = new List<string>();
                    List<string> cc = new List<string>();
                    List<string> bcc = new List<string>();

                    to.Add(um._user.email.Trim());
                    ///     bcc.Add("timur160@hotmail.com");

                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                    eb.NewTask(um._user.first_nm, um._user.last_nm, _rm._report.display_name);
                    string body = eb.Body;
                    em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NewTask, body, true);
                }

                #endregion

                return true;
            }
            catch (System.Data.DataException ex)
            {
                Console.WriteLine("Task/Attachments didn't add");
                Console.WriteLine(ex.Data);
                return false;
            }
        }

        public bool CloseTask(int task_id, int mediator_id)
        {
            try
            {
                task completedTask = db.task.Where(item => (item.id == task_id)).FirstOrDefault();

                completedTask.is_completed = true;
                completedTask.completed_by = mediator_id;
                completedTask.completed_on = DateTime.Now;

                db.task.AddOrUpdate(completedTask);
                db.SaveChanges();

                TaskExtended tsk = new TaskExtended(task_id, mediator_id);
                glb.UpdateReportLog(mediator_id, 11, tsk.TaskReportID, tsk._task.title, null, "");


                return true;
            }
            catch (System.Data.DataException ex)
            {
                Console.WriteLine("Task wasn't close");
                Console.WriteLine(ex.Data);
                return false;
            }
        }

        public bool ResolveCase(int report_id, int mediator_id)
        {
            try
            {
                report_investigation_status report_investigation_status = new report_investigation_status()
                {
                    report_id = report_id,
                    investigation_status_id = 6,
                    created_date = DateTime.Now,
                    user_id = mediator_id,
                    description = ""
                };

                db.report_investigation_status.Add(report_investigation_status);

                report report = db.report.Where(item => (item.id == report_id)).FirstOrDefault();
                report.status_id = 1;
                report.last_update_dt = DateTime.Now;

                db.SaveChanges();
                return true;
            }
            catch (System.Data.DataException ex)
            {
                Console.WriteLine("Case wasn't resolved. Ex:" + ex.Data);
                return false;
            }
        }

        public bool ResolveCase(int report_id, int mediator_id, string description, int new_status, int? outcome_id, string outcome)
        {
            try
            {
                report_investigation_status report_investigation_status;
                if (outcome_id.HasValue)
                {
                    report_investigation_status = new report_investigation_status()
                    {
                        report_id = report_id,
                        investigation_status_id = new_status,
                        created_date = DateTime.Now,
                        user_id = mediator_id,
                        description = description,
                        outcome_id = outcome_id,
                        outcome_message = outcome
                    };
                }
                else
                {
                    report_investigation_status = new report_investigation_status()
                    {
                        report_id = report_id,
                        investigation_status_id = new_status,
                        created_date = DateTime.Now,
                        user_id = mediator_id,
                        description = description, 
                        outcome_message = ""
                    };
                }

                db.report_investigation_status.Add(report_investigation_status);

                report report = db.report.Where(item => (item.id == report_id)).FirstOrDefault();
                report.status_id = 1;
                report.last_update_dt = DateTime.Now;

                db.SaveChanges();
                return true;
            }
            catch (System.Data.DataException ex)
            {
                Console.WriteLine("Case wasn't resolved. Ex:" + ex.Data);
                return false;
            }
        }

        public bool ReassignTask(int task_id, int mediator_id)
        {
            try
            {
                task currentTask = new task();

                currentTask = db.task.Where(item => (item.id == task_id)).FirstOrDefault();
                currentTask.assigned_to = mediator_id;
                db.task.AddOrUpdate(currentTask);
                db.SaveChanges();


                TaskExtended tsk = new TaskExtended(task_id, mediator_id);
                glb.UpdateReportLog(mediator_id, 12, tsk.TaskReportID, tsk._task.title, mediator_id, "");

                #region Task Reassigned - Email to Asignee
                UserModel um = new UserModel(mediator_id);
                ReportModel _rm = new ReportModel(tsk.TaskReportID);

                if ((um._user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(um._user.email.Trim()))
                {
                    List<string> to = new List<string>();
                    List<string> cc = new List<string>();
                    List<string> bcc = new List<string>();

                    to.Add(um._user.email.Trim());
                    ///     bcc.Add("timur160@hotmail.com");

                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                    eb.NewTask(um._user.first_nm, um._user.last_nm, _rm._report.display_name);
                    string body = eb.Body;
                    em.Send(to, cc, App_LocalResources.GlobalRes.Email_Title_NewTask, body, true);
                }

                #endregion

                return true;
            }
            catch (System.Data.DataException ex)
            {
                Console.WriteLine("Task wasn't reassigned");
                Console.WriteLine(ex.Data);
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
    }
}