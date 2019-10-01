using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Data;
using EC.Models.Database;
using EC.Models.ECModel;
using System.Collections.Generic;
using System.Linq;
using EC.Models;
using System.Text.RegularExpressions;
using EC.App_LocalResources;
using EC.Common.Interfaces;
using EC.Core.Common;
using System.Web.Script.Serialization;
using log4net;
using EC.Common.Util;
using EC.Common.Base;
using System.Net;

/// <summary>
/// Global Functions for EC Project
/// </summary>
public class GlobalFunctions
{
    ECEntities db = new ECEntities();
    IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();

    private const int PasswordLength = 6;
    private const int PasswordExtraSubmolsCount = 0;
    ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public GlobalFunctions()
    {

    }

    #region ReportGeneration

    public string GeneretedPassword()
    {

        string newPassword = System.Web.Security.Membership.GeneratePassword(PasswordLength, PasswordExtraSubmolsCount);

        Random rnd = new Random();

        newPassword = Regex.Replace(newPassword, @"[^a-zA-Z0-9]", m => rnd.Next(0, 10).ToString());
        newPassword = StringUtil.ReplaceForUI(newPassword);



        ///string password = System.Web.Security.Membership.GeneratePassword(PasswordLength, PasswordExtraSubmolsCount);
       //// password = StringUtil.ReplaceForUI(password);
        return newPassword;
    }

    #endregion

    //Used in EC\Views\Shared\EditorTemplates\CreateTaskModal.cshtml 
    public List<case_closure_reason> GetCaseClosureReasonsWithStatus(bool isCC)
    {
        //[company_nm]
        //return (from comp in db.company where comp.status_id == 2 select comp.company_nm, comp.id).ToList();
        var r = db.case_closure_reason.Where(item => item.status_id == 2).ToList();
        var other = r.FirstOrDefault(x => x.id == 5);
        r.Remove(other);
        r.Add(other);
        var unfounded = r.FirstOrDefault(x => x.id == 6);
        if (!isCC)
        {
            r.Remove(unfounded);
        }

        return r;
    }

    //Used in 
    //EC\Views\Case\Messages.cshtml 
    //EC\Views\Case\PartialView\GreenBarCaseResolutionRequest.cshtml 
    //EC\Views\Case\Team.cshtml
    public string GetOutcomeNameById(int id)
    {
        if (id != 0)
        {
            // EC.Models.Database.outcome _outcome = db.outcomes.FirstOrDefault(item => item.id == id);
            var item = db.company_outcome.Find(id);
            return item.outcome_en;
        }
        else
            return "";

    }

    //Used in 
    //EC\Views\Case\Messages.cshtml 
    //EC\Views\Case\PartialView\GreenBarCaseResolutionRequest.cshtml 
    //EC\Views\Case\Team.cshtml
    public string GetCaseClosureReasonById(int id)
    {
        if (id != 0)
        {
            // EC.Models.Database.outcome _outcome = db.outcomes.FirstOrDefault(item => item.id == id);
            var item = db.case_closure_reason.Find(id);
            return item.case_closure_reason_en;
        }
        else
            return "";

    }

    //used in
    //EC\Views\Case\Activity.cshtml 
    //EC\Views\Case\GetAjaxActivity.cshtml 
    //EC\Views\Case\Task.cshtml 
    //EC\Views\ReporterDashboard\Activity.cshtml
    public action GetActionById(int id)
    {
        return db.action.FirstOrDefault(item => item.id == id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user_id"></param>
    /// <param name="action_id"></param>
    /// <param name="report_id"></param>
    /// <param name="string_to_add"></param>
    /// <param name="second_user_id"></param>
    /// <param name="action_ds"></param>
    public void UpdateReportLog(int user_id, int action_id, int report_id, string string_to_add, int? second_user_id, string action_ds)
    {
        report_log _log = new report_log();
        _log.report_id = report_id;
        _log.action_id = action_id;
        _log.user_id = user_id;
        _log.string_to_add = string_to_add;
        _log.created_dt = DateTime.Now;
        _log.action_ds = action_ds;
        _log.second_user_id = second_user_id;

        db.report_log.Add(_log);
        try
        {
            db.SaveChanges();
            // need to save new row to report_log about 
        }
        catch (Exception ex)
        {
            logger.Error(ex.ToString());
        }

    }

    #region Update Read Status for Report, Message, Task, Task Comment

    public void UpdateReportRead(int user_id, int report_id)
    {
        report_user_read _user_read = new report_user_read();
        _user_read.report_id = report_id;
        _user_read.user_id = user_id;
        _user_read.read_date = DateTime.Now;

        if (!db.report_user_read.Any(item => ((item.user_id == user_id) && (item.report_id == report_id))))
        {
            db.report_user_read.Add(_user_read);
            try
            {
                db.SaveChanges();
                // need to save new row to _user_read about 
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
        else
        {
            report_user_read _rur = db.report_user_read.Where(item => ((item.user_id == user_id) && (item.report_id == report_id))).FirstOrDefault();
            _rur.read_date = DateTime.Now;
            db.SaveChanges();
        }
    }

    public void UpdateTaskRead(int user_id, int task_id)
    {
        task_user_read _user_read = new task_user_read();
        _user_read.task_id = task_id;
        _user_read.user_id = user_id;
        _user_read.read_date = DateTime.Now;

        if (!db.task_user_read.Any(item => ((item.user_id == user_id) && (item.task_id == task_id))))
        {
            db.task_user_read.Add(_user_read);
            try
            {
                db.SaveChanges();
                // need to save new row to _user_read about 
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
        else
        {
            task_user_read _tur = db.task_user_read.Where(item => ((item.user_id == user_id) && (item.task_id == task_id))).FirstOrDefault();
            _tur.read_date = DateTime.Now;
            db.SaveChanges();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="report_id"></param>
    /// <param name="user_id"></param>
    /// <param name="thread">1 -reporter 2 -mediator thread, 3 - legal</param>
    public void UpdateReadMessages(int report_id, int user_id, int thread_id)
    {
        message_user_read _mue = new message_user_read();

        List<int> _message_ids = db.message.Where(t => (t.report_id == report_id && t.reporter_access == thread_id)).Select(t => t.id).ToList();

        foreach (int _id in _message_ids)
        {
            if (db.message_user_read.Any(t => t.message_id == _id && t.user_id == user_id))
            {
                // message already read in db
                // we could update read date
            }
            else
            {
                _mue = new message_user_read();

                _mue.message_id = _id;
                _mue.user_id = user_id;
                _mue.read_date = DateTime.Now;

                db.message_user_read.Add(_mue);
                try
                {
                    db.SaveChanges();
                    // need to save new row to _user_read about 
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }

            }
        }

        //  if (!db.message_user_read.Any(item => ((item.user_id == user_id) && (item.message_id == message_id))))
        {
        }
    }

    //Used in
    //EC\Views\Case\Task.cshtml 
    public void UpdateTaskCommentRead(int user_id, int task_comment_id)
    {
        task_comment_user_read _user_read = new task_comment_user_read();
        _user_read.task_comment_id = task_comment_id;
        _user_read.user_id = user_id;
        _user_read.read_date = DateTime.Now;

        if (!db.task_comment_user_read.Any(item => ((item.user_id == user_id) && (item.task_comment_id == task_comment_id))))
        {
            db.task_comment_user_read.Add(_user_read);
            try
            {
                db.SaveChanges();
                // need to save new row to _user_read about 
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
    }
    #endregion

    #region Analytics Helpers

    //done
    public int[] AnalyticsByDate(DateTime? _start, DateTime? _end, int company_id, int user_id)
    {
        DateTime _real_start, _real_end;

        if (_start.HasValue)
            _real_start = _start.Value;
        else
            _real_start = new DateTime(2015, 1, 1);

        if (_end.HasValue)
            _real_end = _end.Value;
        else
            _real_end = DateTime.Today.AddDays(1);

        //  public List<report> ReportsSearch(int? company_id, int flag)
        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<report> _all_reports = um.ReportsSearch(company_id, 0);
        List<report> _selected_reports = new List<report>();

        int[] _array = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int _status = 0;
        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);
            _status = 0;

            if ((rm.LastPromotedDate() >= _real_start) && (rm.LastPromotedDate() <= _real_end))
            {
                _selected_reports.Add(_report);
                _status = rm._investigation_status;
                if (_status > 0)
                {
                    _array[_status - 1] = _array[_status - 1] + 1;
                }
            }
        }

        return _array;
    }

    public DataTable AnalyticsTimeline(int company_id, int user_id)
    {
        DataTable dt = dtAnalyticsTimeLineTable();
        DataRow dr = dt.NewRow();

        #region Initial Row
        dr["month"] = "";
        dr["pending"] = 0;
        dr["review"] = 0;
        dr["investigation"] = 0;
        dr["resolution"] = 0;
        dr["escalation"] = 0;
        dr["closed"] = 0;
        dr["spam"] = 0;
        dt.Rows.Add(dr);

        #endregion

        int _month = DateTime.Today.Month;
        int temp_month = 0;
        int year = DateTime.Today.Year - 1;
        if (_month == 12)
            year = DateTime.Today.Year;

        DateTime _start = DateTime.Today;

        for (int i = 1; i < 13; i++)
        {
            dr = dt.NewRow();
            temp_month = _month + i;
            if (temp_month > 12)
            {
                temp_month = temp_month - 12;
                year = DateTime.Today.Year;
            }
            _start = new DateTime(year, temp_month, 1);


            dr = AnalyticsTimeLineRow(_start, company_id, user_id);
            dt.Rows.Add(dr.ItemArray);
        }

        dr = AnalyticsTimeLineRow(DateTime.Today, company_id, user_id);
        dr[0] = " ";
        dt.Rows.Add(dr.ItemArray);

        return dt;
    }
    private DataRow AnalyticsTimeLineRow(DateTime _start, int company_id, int user_id)
    {
        DataRow dr = dtAnalyticsTimeLineTable().NewRow();

        #region month_name
        int month = _start.Month;
        string month_s = "";
        Dictionary<int, string> Monthes = m_DateTimeHelper.FullMonth();
        if ((month > 0) && (month < 13))
            Monthes.TryGetValue(month, out month_s);
        #endregion

        dr["month"] = month_s;
        dr["pending"] = 0;
        dr["review"] = 0;
        dr["investigation"] = 0;
        dr["resolution"] = 0;
        dr["escalation"] = 0;
        dr["completed"] = 0;
        dr["spam"] = 0;
        dr["closed"] = 0;
        dr["notused"] = 0;
        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<report> _all_reports = um.ReportsSearch(company_id, 0);

        int _status = 0;
        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);
            _status = rm.report_status_id_by_date(_start);
            if (_status != 0)
            {
                dr[_status] = (Int32)dr[_status] + 1;
            }
        }

        return dr;
    }
    //done
    public DataTable TasksPerDay(int company_id, int user_id)
    {
        DataTable dt = dtTaskLengthTable();
        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<report> _all_reports = um.ReportsSearch(company_id, 0).OrderBy(t => t.reported_dt).ToList();

        TaskExtended _tsk = new TaskExtended();
        int number_length = 0;

        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);
            List<TaskExtended> _list_tasks = rm.ExtendedTasks(user_id);
            foreach (TaskExtended _ext_task in _list_tasks)
            {
                //if(dt)
                bool contains = dt.AsEnumerable().Any(row => (_ext_task.TaskReportID == row.Field<Int32>("report_id")) && (_ext_task.TaskLength == row.Field<Int32>("length")));

                number_length = 0;
                if (contains)
                {
                    List<DataRow> dr_list = dt.AsEnumerable().Where(row => (_ext_task.TaskReportID == row.Field<Int32>("report_id")) && (_ext_task.TaskLength == row.Field<Int32>("length"))).ToList();
                    if (dr_list.Count > 0)
                    {
                        number_length = Convert.ToInt32(dr_list[0]["value"]);
                        dr_list[0]["value"] = number_length + 1;
                    }
                }
                else
                {
                    DataRow _dr = dt.NewRow();
                    _dr["report_id"] = _ext_task.TaskReportID;
                    _dr["value"] = 1;
                    _dr["length"] = _ext_task.TaskLength;
                    dt.Rows.Add(_dr);
                }
            }
        }

        List<Int32> dr_task_report_list = dt.AsEnumerable().Select(g => g.Field<Int32>("report_id")).Distinct().ToList();


        DataTable dt_length = new DataTable();

        //   dt_length.Columns.Add("color", typeof(string));
        // dt_length.Columns.Add("value", typeof(int));
        dt_length.Columns.Add("day", typeof(int));


        for (int j = 0; j < dr_task_report_list.Count(); j++)
        {
            dt_length.Columns.Add("case" + dr_task_report_list[j].ToString(), typeof(int));
        }

        for (int i = 0; i < 50; i++)
        {
            //{day: "1", case208: 29.956, case209: 90.354, case209: 14.472, task4: 28.597, task5: 91.827,task6: 20},

            DataRow _dr = dt_length.NewRow();
            _dr["day"] = i;

            for (int j = 0; j < dr_task_report_list.Count(); j++)
            {
                // _dr["case" + dr_task_report_list[j].ToString()] = j + 5;

                bool contains = dt.AsEnumerable().Any(row => (dr_task_report_list[j].ToString() == row.Field<Int32>("report_id").ToString()) && (i == row.Field<Int32>("length")));
                if (contains)
                {
                    _dr["case" + dr_task_report_list[j].ToString()] = (dt.AsEnumerable().Where(row => (dr_task_report_list[j].ToString() == row.Field<Int32>("report_id").ToString()) && (i == row.Field<Int32>("length"))).Select(g => g.Field<Int32>("value"))).ToList()[0];
                }
                else
                    _dr["case" + dr_task_report_list[j].ToString()] = 0;



            }
            dt_length.Rows.Add(_dr);

        }
        //  { valueField: "task1", name: "case 1", stack: "female" },
        //  { valueField: "case208", case_id: "208", stack: "female" },

        return dt_length;
    }

    //done
    /// <summary>
    /// generates average number days per stage.
    /// </summary>
    /// <param name="company_id"></param>
    /// <param name="user_id"></param>
    /// <returns></returns>
    public int[] AverageStageDays(int company_id, int user_id)
    {

        //  public List<report> ReportsSearch(int? company_id, int flag)
        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<report> _all_reports = um.ReportsSearch(company_id, 0);

        int[] _array_days = new int[] { 0, 0, 0, 0, 0 };
        int[] _array_stages = new int[] { 0, 0, 0, 0, 0 };
        int[] _array_ratio = new int[] { 0, 0, 0, 0, 0 };

        DateTime dt_start = DateTime.Today;
        DateTime dt_end = DateTime.Today;
        report_investigation_status _stage = new report_investigation_status();
        report_investigation_status temp_stage = new report_investigation_status();

        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);

            for (int i = 1; i < 6; i++)
            {
                if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && (item.investigation_status_id == i))))
                {
                    _stage = db.report_investigation_status.Where(item => ((item.report_id == _report.id) && (item.investigation_status_id == i))).FirstOrDefault();
                    dt_start = _stage.created_date;

                    if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && ((item.investigation_status_id == i + 1) || (item.investigation_status_id == 6) || (item.investigation_status_id == 7)))))
                    {
                        if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && ((item.investigation_status_id == i + 1)))))
                        {
                            temp_stage = db.report_investigation_status.Where(item => ((item.report_id == _report.id) && (item.investigation_status_id == i + 1))).FirstOrDefault();
                            dt_end = temp_stage.created_date;
                        }
                        else if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && ((item.investigation_status_id == 6)))))
                        {
                            temp_stage = db.report_investigation_status.Where(item => ((item.report_id == _report.id) && (item.investigation_status_id == 6))).FirstOrDefault();
                            dt_end = temp_stage.created_date;
                        }
                        else if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && ((item.investigation_status_id == 7)))))
                        {
                            temp_stage = db.report_investigation_status.Where(item => ((item.report_id == _report.id) && (item.investigation_status_id == 7))).FirstOrDefault();
                            dt_end = temp_stage.created_date;
                        }
                    }
                    else
                        dt_end = DateTime.Today;

                    _array_stages[i - 1]++;
                    _array_days[i - 1] = _array_days[i - 1] + (dt_end - dt_start).Days;
                }
            }
        }
        decimal _temp = 0;
        for (int j = 0; j < 5; j++)
        {
            if (_array_stages[j] != 0)
            {
                _temp = _array_days[j] / _array_stages[j];
                _array_ratio[j] = Convert.ToInt32(Math.Round(_temp));
            }
        }
        return _array_ratio;
    }

    #endregion

    public List<report> ReportsListForCompany(int[] company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //UserModel um = new UserModel(user_id);
        //List<report> _all_reports_old = um.ReportsSearch(company_id, 0);
        var _all_reports_old = db.report.Where(r => company_id.Contains(r.company_id)).ToList();

        if (dtReportCreationStartDate.HasValue)
        {
            _all_reports_old = _all_reports_old.Where(t => t.reported_dt.Date >= dtReportCreationStartDate.Value.Date).ToList();
        }

        if (dtReportCreationEndDate.HasValue)
        {
            _all_reports_old = _all_reports_old.Where(t => t.reported_dt.Date <= dtReportCreationEndDate.Value.Date).ToList();
        }
        List<report> _all_reports = new List<report>();

        #region Get the list of ReportIDs allowed

        List<int> ReportsSecondaryTypesIDs = new List<int>();

        List<int> ReportsRelationTypesIDs = new List<int>();

        List<int> ReportsDepartmentIDs = new List<int>();

        List<int> ReportsLocationIDs = new List<int>();



        if (ReportsSecondaryTypesIDStrings.Trim().Length > 0)
            ReportsSecondaryTypesIDs = ReportsSecondaryTypesIDStrings.Split(',').Select(Int32.Parse).ToList();

        if (ReportsRelationTypesIDStrings.Trim().Length > 0)
            ReportsRelationTypesIDs = ReportsRelationTypesIDStrings.Split(',').Select(Int32.Parse).ToList();

        if (ReportsDepartmentIDStringss.Trim().Length > 0)
            ReportsDepartmentIDs = ReportsDepartmentIDStringss.Split(',').Select(Int32.Parse).ToList();

        if (ReportsLocationIDStrings.Trim().Length > 0)
            ReportsLocationIDs = ReportsLocationIDStrings.Split(',').Select(Int32.Parse).ToList();

        if (ReportsDepartmentIDs.Count > 0)
        {
            var report_from_departments = db.report_department.Where(t => ReportsDepartmentIDs.Contains(t.department_id)).Select(t => t.report_id);
            //_all_reports_old = _all_reports_old.Where(report_from_departments.Contains(t.id)).ToList();
            _all_reports_old = _all_reports_old.Where(t => report_from_departments.Any(report => report == t.id)).ToList();
        }
        if (ReportsRelationTypesIDs.Count > 0)
        {
            var report_from_relation_type = db.report_relationship.Where(t => ReportsRelationTypesIDs.Contains(t.company_relationship_id.Value)).Select(t => t.report_id);
            //_all_reports_old = _all_reports_old.Where(report_from_relation_type.Contains(t.id)).ToList();
            _all_reports_old = _all_reports_old.Where(t => report_from_relation_type.Any(report => report == t.id)).ToList();
        }
        if (ReportsSecondaryTypesIDs.Count > 0)
        {
            var report_from_secondary_type = db.report_secondary_type.Where(t => ReportsSecondaryTypesIDs.Contains(t.secondary_type_id)).Select(t => t.report_id);
            _all_reports_old = _all_reports_old.Where(t => report_from_secondary_type.Contains(t.id)).ToList();
        }

        if (ReportsLocationIDs.Count > 0)
        {
            //_all_reports_old = _all_reports_old.Where(t => ReportsLocationIDs.Contains(t.location_id)).ToList();
            _all_reports_old = _all_reports_old.Where(t => ReportsLocationIDs.Any( report => report == t.location_id)).ToList();
        }
        #endregion

        return _all_reports_old;
    }

    #region Analytics Helpers - new version

    public DataTable CompanyDepartmentReportAdvanced(List<report> _all_reports, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = dtDoughnutTable();
        DataRow dr;
        List<int> report_ids_list = _all_reports.Select(t => t.id).ToList();

        var DepAndReports = db.report_department.Join(db.company_department,
                                            post => post.department_id,
                                            meta => meta.id,
                                            (post, meta) => new { Post = post, Meta = meta })
                                            .Where(postAndMeta => report_ids_list.Contains(postAndMeta.Post.report_id));
        List<CompanyLocation> companyDepatments = new List<CompanyLocation>();

        foreach(var department in DepAndReports)
        {
            //checkisAlreadyAdded
            int countAlreadyadded = 0;
            if (companyDepatments.Count > 0)
            {
                foreach (var rrlocation in companyDepatments)
                {
                    if (rrlocation.NameLocation.Equals(department.Meta.department_en, StringComparison.OrdinalIgnoreCase))
                    {
                        countAlreadyadded++;
                    }
                }
            }

            if (countAlreadyadded == 0)
            {
                int countSameLocations = DepAndReports.Where(sameLoc => sameLoc.Meta.department_en.Equals(department.Meta.department_en)).Count();
                CompanyLocation newLocation = new CompanyLocation();
                newLocation.id = department.Meta.id;
                newLocation.NameLocation = department.Meta.department_en;
                newLocation.countLocations = countSameLocations;
                companyDepatments.Add(newLocation);
            }
        }
        foreach (var department in companyDepatments)
        {
            dr = dt.NewRow();
            dr["name"] = department.NameLocation;
            dr["val"] = department.countLocations;
            dt.Rows.Add(dr);
        }
        return dt;
    }


    public DataTable CompanyLocationReportAdvanced(List<report> _all_reports, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        ReportModel rm = new ReportModel();
        DataTable dt = dtDoughnutTable();
        DataRow dr;

        List<CompanyLocation> ResultLocations = _all_reports.Select(rep => rep.location_id).Select(rep => new CompanyLocation { id = rep.Value }).ToList();
        int[] idLocations = ResultLocations.Select(idLoc => idLoc.id).ToArray();
        var locations = db.company_location.Where(location => idLocations.Contains(location.id)).ToArray();

        List<CompanyLocation> ResultResultLocations = new List<CompanyLocation>();
        foreach(var location in locations)
        {
            //checkisAlreadyAdded
            int countAlreadyadded = 0;
            if(ResultResultLocations.Count > 0)
            {
                foreach(var rrlocation in ResultResultLocations)
                {
                    if (rrlocation.NameLocation.Equals(location.location_en, StringComparison.OrdinalIgnoreCase))
                    {
                        countAlreadyadded++;
                    }
                    
                }
            }
            if (countAlreadyadded > 0)
            {
                int countSameLocations = idLocations.Where(idLoc => idLoc == location.id).Count();
                CompanyLocation updateLocation = ResultResultLocations.Where(rl => rl.NameLocation == location.location_en).FirstOrDefault();
                if (updateLocation!=null) { updateLocation.countLocations += countSameLocations; }
            }
            else
            {
                //create New Location
                CompanyLocation newLocation = new CompanyLocation();
                newLocation.id = location.id;
                newLocation.NameLocation = location.location_en;
                newLocation.countLocations = ResultLocations.Where(rl => rl.id == location.id).Count();
                ResultResultLocations.Add(newLocation);
            }
        }
        foreach (var resultLocation in ResultResultLocations)
        {
            dr = dt.NewRow();
            dr["name"] = resultLocation.NameLocation;
            dr["val"] = resultLocation.countLocations;
            dt.Rows.Add(dr);
        }
        return dt;
    }

    /// <summary>
    /// generates average number days per stage.
    /// </summary>
    /// <param name="company_id"></param>
    /// <param name="user_id"></param>
    /// <returns></returns>
    public int[] AverageStageDaysAdvanced(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //List<string> result = names.Split(',').ToList();
        UserModel um = new UserModel(user_id);
        List<report> _all_reports = ReportsListForCompany(new int[company_id], user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

        //  public List<report> ReportsSearch(int? company_id, int flag)
        ReportModel rm = new ReportModel();

        int[] _array_days = new int[] { 0, 0, 0, 0, 0 };
        int[] _array_stages = new int[] { 0, 0, 0, 0, 0 };
        int[] _array_ratio = new int[] { 0, 0, 0, 0, 0 };

        DateTime dt_start = DateTime.Today;
        DateTime dt_end = DateTime.Today;
        report_investigation_status _stage = new report_investigation_status();
        report_investigation_status temp_stage = new report_investigation_status();

        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);

            for (int i = 1; i < 6; i++)
            {
                if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && (item.investigation_status_id == i))))
                {
                    _stage = db.report_investigation_status.Where(item => ((item.report_id == _report.id) && (item.investigation_status_id == i))).FirstOrDefault();
                    dt_start = _stage.created_date;

                    if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && ((item.investigation_status_id == i + 1) || (item.investigation_status_id == 6) || (item.investigation_status_id == 7)))))
                    {
                        if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && ((item.investigation_status_id == i + 1)))))
                        {
                            temp_stage = db.report_investigation_status.Where(item => ((item.report_id == _report.id) && (item.investigation_status_id == i + 1))).FirstOrDefault();
                            dt_end = temp_stage.created_date;
                        }
                        else if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && ((item.investigation_status_id == 6)))))
                        {
                            temp_stage = db.report_investigation_status.Where(item => ((item.report_id == _report.id) && (item.investigation_status_id == 6))).FirstOrDefault();
                            dt_end = temp_stage.created_date;
                        }
                        else if (db.report_investigation_status.Any(item => ((item.report_id == _report.id) && ((item.investigation_status_id == 7)))))
                        {
                            temp_stage = db.report_investigation_status.Where(item => ((item.report_id == _report.id) && (item.investigation_status_id == 7))).FirstOrDefault();
                            dt_end = temp_stage.created_date;
                        }
                    }
                    else
                        dt_end = DateTime.Today;

                    _array_stages[i - 1]++;
                    _array_days[i - 1] = _array_days[i - 1] + (dt_end - dt_start).Days;
                }
            }
        }
        decimal _temp = 0;
        for (int j = 0; j < 5; j++)
        {
            if (_array_stages[j] != 0)
            {
                _temp = _array_days[j] / _array_stages[j];
                _array_ratio[j] = Convert.ToInt32(Math.Round(_temp));
            }
        }
        return _array_ratio;
    }

    public DataTable TasksPerDayAdvanced(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //List<string> result = names.Split(',').ToList();
        UserModel um = new UserModel(user_id);
        List<report> _all_reports = ReportsListForCompany(new int[company_id], user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

        DataTable dt = dtTaskLengthTable();

        ReportModel rm = new ReportModel();

        _all_reports = _all_reports.OrderBy(t => t.reported_dt).ToList();

        TaskExtended _tsk = new TaskExtended();
        int number_length = 0;

        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);
            List<TaskExtended> _list_tasks = rm.ExtendedTasks(user_id);
            foreach (TaskExtended _ext_task in _list_tasks)
            {
                //if(dt)
                bool contains = dt.AsEnumerable().Any(row => (_ext_task.TaskReportID == row.Field<Int32>("report_id")) && (_ext_task.TaskLength == row.Field<Int32>("length")));

                number_length = 0;
                if (contains)
                {
                    List<DataRow> dr_list = dt.AsEnumerable().Where(row => (_ext_task.TaskReportID == row.Field<Int32>("report_id")) && (_ext_task.TaskLength == row.Field<Int32>("length"))).ToList();
                    if (dr_list.Count > 0)
                    {
                        number_length = Convert.ToInt32(dr_list[0]["value"]);
                        dr_list[0]["value"] = number_length + 1;
                    }
                }
                else
                {
                    DataRow _dr = dt.NewRow();
                    _dr["report_id"] = _ext_task.TaskReportID;
                    _dr["value"] = 1;
                    _dr["length"] = _ext_task.TaskLength;
                    dt.Rows.Add(_dr);
                }
            }
        }

        List<Int32> dr_task_report_list = dt.AsEnumerable().Select(g => g.Field<Int32>("report_id")).Distinct().ToList();


        DataTable dt_length = new DataTable();

        //   dt_length.Columns.Add("color", typeof(string));
        // dt_length.Columns.Add("value", typeof(int));
        dt_length.Columns.Add("day", typeof(int));


        for (int j = 0; j < dr_task_report_list.Count(); j++)
        {
            dt_length.Columns.Add("case" + dr_task_report_list[j].ToString(), typeof(int));
        }

        for (int i = 0; i < 50; i++)
        {
            //{day: "1", case208: 29.956, case209: 90.354, case209: 14.472, task4: 28.597, task5: 91.827,task6: 20},

            DataRow _dr = dt_length.NewRow();
            _dr["day"] = i;

            for (int j = 0; j < dr_task_report_list.Count(); j++)
            {
                // _dr["case" + dr_task_report_list[j].ToString()] = j + 5;

                bool contains = dt.AsEnumerable().Any(row => (dr_task_report_list[j].ToString() == row.Field<Int32>("report_id").ToString()) && (i == row.Field<Int32>("length")));
                if (contains)
                {
                    _dr["case" + dr_task_report_list[j].ToString()] = (dt.AsEnumerable().Where(row => (dr_task_report_list[j].ToString() == row.Field<Int32>("report_id").ToString()) && (i == row.Field<Int32>("length"))).Select(g => g.Field<Int32>("value"))).ToList()[0];
                }
                else
                    _dr["case" + dr_task_report_list[j].ToString()] = 0;



            }
            dt_length.Rows.Add(_dr);

        }
        //  { valueField: "task1", name: "case 1", stack: "female" },
        //  { valueField: "case208", case_id: "208", stack: "female" },

        return dt_length;
    }


    public DataTable RelationshipToCompanyByDateAdvanced(List<report> _all_reports, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        ReportModel rm = new ReportModel();

        // merge with previous
        List<Int32> _report_ids = _all_reports.Select(t => t.id).ToList();

        DataTable dt = dtDoughnutTable();

        List<report_relationship> _all_relations = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id))).ToList();
        _all_relations.Where(c => c.company_relationship_id == null || !c.company_relationship_id.HasValue).ToList().ForEach(c => c.company_relationship_id = 0);

        var groups = _all_relations.GroupBy(s => s.company_relationship_id).Select(s => new { key = s.Key, val = s.Count() }).OrderByDescending(t => t.val);

        string temp_rel = "";
        company_relationship temp_c_rel;
        DataRow dr;

        foreach (var item in groups)
        {
            if (item.key != 0)
            {
                temp_rel = "";
                temp_c_rel = db.company_relationship.Where(t => t.id == item.key).FirstOrDefault();
                if (temp_c_rel != null)
                {
                    temp_rel = temp_c_rel.relationship_en;
                }
            }
            else
                temp_rel = GlobalRes.Other;

            if (temp_rel.Length > 0)
            {
                dr = dt.NewRow();
                dr["name"] = temp_rel;
                dr["val"] = item.val;
                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    public DataTable SecondaryTypesByDateAdvanced(List<report> _all_reports, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        ReportModel rm = new ReportModel();
        // merge with previous
        List<Int32> _report_ids = _all_reports.Select(t => t.id).ToList();

        DataTable dt = dtDoughnutTable();

        List<report_secondary_type> _all_types = db.report_secondary_type.Where(item => (_report_ids.Contains(item.report_id))).ToList();

        var groups = _all_types.GroupBy(s => s.secondary_type_id).Select(s => new { key = s.Key, val = s.Count() }).OrderByDescending(t => t.val);

        string temp_sec_type = "";
        company_secondary_type temp_c_sec_type;
        DataRow dr;
        foreach (var item in groups)
        {
            if (item.key != 0)
            {
                temp_sec_type = "";
                temp_c_sec_type = db.company_secondary_type.Where(t => t.id == item.key).FirstOrDefault();
                if (temp_c_sec_type != null)
                {
                    temp_sec_type = temp_c_sec_type.secondary_type_en;
                }
            }
            else
                temp_sec_type = GlobalRes.Other;

            if (temp_sec_type.Length > 0)
            {
                dr = dt.NewRow();
                dr["name"] = temp_sec_type;
                dr["val"] = item.val;
                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    public int[] AnalyticsByDateAdvanced(DateTime? _start, DateTime? _end, int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //List<string> result = names.Split(',').ToList();
        UserModel um = new UserModel(user_id);
        List<report> _all_reports = ReportsListForCompany(new int[company_id], user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

        DateTime _real_start, _real_end;

        if (_start.HasValue)
            _real_start = _start.Value;
        else
            _real_start = new DateTime(2015, 1, 1);

        if (_end.HasValue)
            _real_end = _end.Value;
        else
            _real_end = DateTime.Today.AddDays(1);

        //  public List<report> ReportsSearch(int? company_id, int flag)
        ReportModel rm = new ReportModel();

        List<report> _selected_reports = new List<report>();

        int[] _array = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int _status = 0;
        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);
            _status = 0;

            if ((rm.LastPromotedDate() >= _real_start) && (rm.LastPromotedDate() <= _real_end))
            {
                _selected_reports.Add(_report);
                _status = rm._investigation_status;
                if (_status > 0)
                {
                    _array[_status - 1] = _array[_status - 1] + 1;
                }
            }
        }

        return _array;
    }


    public DataTable AnalyticsTimelineAdvanced(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = dtAnalyticsTimeLineTable();
        DataRow dr = dt.NewRow();

        #region Initial Row
        dr["month"] = "";
        dr["pending"] = 0;
        dr["review"] = 0;
        dr["investigation"] = 0;
        dr["resolution"] = 0;
        dr["escalation"] = 0;
        dr["closed"] = 0;
        dr["spam"] = 0;
        dt.Rows.Add(dr);

        #endregion

        int _month = DateTime.Today.Month;
        int temp_month = 0;
        int year = DateTime.Today.Year - 1;
        if (_month == 12)
            year = DateTime.Today.Year;

        DateTime _start = DateTime.Today;

        for (int i = 1; i < 13; i++)
        {
            dr = dt.NewRow();
            temp_month = _month + i;
            if (temp_month > 12)
            {
                temp_month = temp_month - 12;
                year = DateTime.Today.Year;
            }
            _start = new DateTime(year, temp_month, 1);


            dr = AnalyticsTimeLineRowAdvanced(_start, company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
            dt.Rows.Add(dr.ItemArray);
        }

        dr = AnalyticsTimeLineRowAdvanced(DateTime.Today, company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        dr[0] = " ";
        dt.Rows.Add(dr.ItemArray);

        return dt;
    }
    private DataRow AnalyticsTimeLineRowAdvanced(DateTime _start, int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataRow dr = dtAnalyticsTimeLineTable().NewRow();

        #region month_name
        int month = _start.Month;
        string month_s = "";
        Dictionary<int, string> Monthes = m_DateTimeHelper.FullMonth();
        if ((month > 0) && (month < 13))
            Monthes.TryGetValue(month, out month_s);
        #endregion

        dr["month"] = month_s;
        dr["pending"] = 0;
        dr["review"] = 0;
        dr["investigation"] = 0;
        dr["resolution"] = 0;
        dr["escalation"] = 0;
        dr["completed"] = 0;
        dr["spam"] = 0;
        dr["closed"] = 0;
        dr["notused"] = 0;
        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<report> _all_reports_old = um.ReportsSearch(company_id, 0);
        List<report> _all_reports = new List<report>();
        #region Get the list of ReportIDs allowed
        List<int> ReportsSecondaryTypesIDs = new List<int>();
        List<int> ReportsRelationTypesIDs = new List<int>();
        List<int> ReportsDepartmentIDs = new List<int>();
        List<int> ReportsLocationIDs = new List<int>();

        if (ReportsSecondaryTypesIDStrings.Trim().Length > 0)
            ReportsSecondaryTypesIDs = ReportsSecondaryTypesIDStrings.Split(',').Select(Int32.Parse).ToList();
        if (ReportsRelationTypesIDStrings.Trim().Length > 0)
            ReportsRelationTypesIDs = ReportsRelationTypesIDStrings.Split(',').Select(Int32.Parse).ToList();
        if (ReportsDepartmentIDStringss.Trim().Length > 0)
            ReportsDepartmentIDs = ReportsDepartmentIDStringss.Split(',').Select(Int32.Parse).ToList();
        if (ReportsLocationIDStrings.Trim().Length > 0)
            ReportsLocationIDs = ReportsLocationIDStrings.Split(',').Select(Int32.Parse).ToList();

        bool _flag1 = true;
        bool _flag2 = true;
        bool _flag3 = true;
        bool _flag4 = true;
        bool _flag5 = true;
        bool _flag6 = true;

        foreach (report _report in _all_reports_old)
        {
            _flag1 = true;
            _flag2 = true;
            _flag3 = true;
            _flag4 = true;
            _flag5 = true;
            _flag6 = true;
            if ((ReportsSecondaryTypesIDs.Count > 0) && (!ReportsSecondaryTypesIDs.Contains(_report.id)))
                _flag1 = false;
            if ((ReportsRelationTypesIDs.Count > 0) && (!ReportsRelationTypesIDs.Contains(_report.id)))
                _flag2 = false;
            if ((ReportsDepartmentIDs.Count > 0) && (!ReportsDepartmentIDs.Contains(_report.id)))
                _flag3 = false;
            if ((ReportsLocationIDs.Count > 0) && (!ReportsLocationIDs.Contains(_report.id)))
                _flag4 = false;
            if ((dtReportCreationStartDate.HasValue) && (dtReportCreationStartDate >= _report.reported_dt))
                _flag5 = false;
            if ((dtReportCreationEndDate.HasValue) && (dtReportCreationEndDate <= _report.reported_dt))
                _flag6 = false;
            if (_flag1 & _flag2 && _flag3 & _flag4 && _flag5 & _flag6)
                _all_reports.Add(_report);
        }
        #endregion

        int _status = 0;
        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);
            _status = rm.report_status_id_by_date(_start);
            if (_status != 0)
            {
                dr[_status] = (Int32)dr[_status] + 1;
            }
        }

        return dr;
    }
    #endregion


    #region JsonReports

    public string ReportAdvancedJson(int []company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        ///   int[] _today_spanshot = AnalyticsByDateAdvanced(null, null, company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

        //   DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
        //   int[] _month_end_spanshot = AnalyticsByDateAdvanced(null, _month_end_date, company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

        List<report> _all_reports = ReportsListForCompany(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

        string _all_json = "{\"LocationTable\":" + CompanyLocationReportAdvancedJson(_all_reports, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
                + ", \"DepartmentTable\":" + CompanyDepartmentReportAdvancedJson(_all_reports, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
                + ", \"RelationTable\":" + RelationshipToCompanyByDateAdvancedJson(_all_reports, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
                + ", \"SecondaryTypeTable\":" + SecondaryTypesByDateAdvancedJson(_all_reports, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            ///    + ", \"AverageStageDaysTable\":" + AverageStageDaysAdvancedJson(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            ///    + ", \"TodaySnapshotTable\":" + JsonUtil.ListToJsonWithJavaScriptSerializer(new List<int>(_today_spanshot))
            ///   + ", \"MonthEndSnapshotTable\":" + JsonUtil.ListToJsonWithJavaScriptSerializer(new List<int>(_month_end_spanshot))
            ///   + ", \"AnalyticsTimeline\":" + AnalyticsTimelineAdvancedJson(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            + "}";

        return _all_json;
    }



    public string CompanyLocationReportAdvancedJson(List<report> _all_reports, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = CompanyLocationReportAdvanced(_all_reports, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
    }

    public string CompanyDepartmentReportAdvancedJson(List<report> _all_reports, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = CompanyDepartmentReportAdvanced(_all_reports, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
    }

    public string RelationshipToCompanyByDateAdvancedJson(List<report> _all_reports, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = RelationshipToCompanyByDateAdvanced(_all_reports, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
    }


    public string SecondaryTypesByDateAdvancedJson(List<report> _all_reports, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = SecondaryTypesByDateAdvanced(_all_reports, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
    }

    public string AverageStageDaysAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {

        int[] _array = AverageStageDaysAdvanced(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

        string _json = "[";


        /*{
		"name": "Stark Tower, NY",
		"val": 27
	}, */


        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                _json += "{ \"name\": \"New Report\",\"val\":" + _array[i].ToString() + "},";
            }
            else if (i == 1)
            {
                _json += "{ \"name\": \"New Case\",\"val\":" + _array[i].ToString() + "},";
            }
            else if (i == 2)
            {
                _json += "{ \"name\": \"Under Investigation\",\"val\":" + _array[i].ToString() + "},";
            }
            else if (i == 3)
            {
                _json += "{ \"name\": \"Awaiting Sign-Off\",\"val\":" + _array[i].ToString() + "},";
            }
            else if (i == 4)
            {
                _json += "{ \"name\": \"Closed\",\"val\":" + _array[i].ToString() + "}";
            }
        }
        _json += "]";
        return _json;

        //return DataTableToJSONWithJavaScriptSerializer(dt);
    }

    public string AnalyticsTimelineAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = AnalyticsTimelineAdvanced(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
    }

    #endregion

    #region ReportsDataTables
    private DataTable dtAnalyticsTable()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("name", typeof(string));
        dt.Columns.Add("value", typeof(int));
        dt.Columns.Add("prev", typeof(int));

        return dt;
    }

    private DataTable dtDoughnutTable()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("name", typeof(string));
        dt.Columns.Add("val", typeof(int));
        return dt;
    }

    private DataTable dtAnalyticsTimeLineTable()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("month", typeof(string));
        dt.Columns.Add("pending", typeof(int));
        dt.Columns.Add("review", typeof(int));
        dt.Columns.Add("investigation", typeof(int));
        dt.Columns.Add("resolution", typeof(int));
        dt.Columns.Add("escalation", typeof(int));
        dt.Columns.Add("completed", typeof(int));
        dt.Columns.Add("closed", typeof(int));
        dt.Columns.Add("spam", typeof(int));
        dt.Columns.Add("notused", typeof(int));

        return dt;
    }

    private DataTable dtTaskLengthTable()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("report_id", typeof(int));
        dt.Columns.Add("value", typeof(int));
        dt.Columns.Add("length", typeof(int));

        return dt;
    }

    #endregion

    #region Analytics Helpers List = Menu items

    /// Secondary Types - Menu
    public List<Tuple<string, string>> SecondaryTypesListDistinct(int company_id, int user_id)
    {
        List<Tuple<string, string>> return_array = new List<Tuple<string, string>>();
        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();
        List<Int32> _report_ids = um.ReportsSearchIds(company_id, 0);

        List<int> sec_type_ids = db.report_secondary_type.Where(item => (_report_ids.Contains(item.report_id))).Select(item => item.secondary_type_id).Distinct().ToList();

        /////company_relation

        List<company_secondary_type> _all_sec_types = db.company_secondary_type.Where(item => (sec_type_ids.Contains(item.id))).ToList();
        foreach (company_secondary_type _temp_company_sec_type in _all_sec_types)
        {
            List<int> report_ids_by_sec_type_id = db.report_secondary_type.Where(item => (_report_ids.Contains(item.report_id) && item.secondary_type_id == _temp_company_sec_type.id)).Select(item => item.report_id).ToList();
            return_array.Add(new Tuple<string, string>(_temp_company_sec_type.secondary_type_en, string.Join(",", report_ids_by_sec_type_id.ToArray())));
        }

        if (sec_type_ids.Contains(0))
        {
            List<int> report_ids_by_rel_id = db.report_secondary_type.Where(item => (_report_ids.Contains(item.id) && item.secondary_type_id == 0)).Select(item => item.report_id).ToList();
            return_array.Add(new Tuple<string, string>(GlobalRes.Other, string.Join(",", report_ids_by_rel_id.ToArray())));
        }
        return return_array;
    }

    /// Relation Types - Menu
    public List<Tuple<string, string>> RelationTypesListDistinct(int company_id, int user_id)
    {
        //check if null???
        List<Tuple<string, string>> return_array = new List<Tuple<string, string>>();

        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();
        List<Int32> _report_ids = um.ReportsSearchIds(company_id, 0);

        List<int?> relation_ids = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id))).Select(item => item.company_relationship_id).Distinct().ToList();

        /////company_relation
        List<company_relationship> _all_relations = db.company_relationship.Where(item => (relation_ids.Contains(item.id))).ToList();
        foreach (company_relationship _temp_company_rel in _all_relations)
        {
            List<int> report_ids_by_rel_id = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id) && item.company_relationship_id == _temp_company_rel.id)).Select(item => item.report_id).ToList();
            return_array.Add(new Tuple<string, string>(_temp_company_rel.relationship_en, string.Join(",", report_ids_by_rel_id.ToArray())));
        }

        // if (relation_ids.Contains(0) )
        {
            List<int> report_ids_by_rel_id = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id) && (item.company_relationship_id == 0 || !item.company_relationship_id.HasValue))).Select(item => item.report_id).ToList();
            if (report_ids_by_rel_id.Count > 0)
                return_array.Add(new Tuple<string, string>(GlobalRes.Other, string.Join(",", report_ids_by_rel_id.ToArray())));
        }

        return return_array;
    }

    /// Departments - Menu
    public List<Tuple<string, string>> DepartmentsListDistinct(int company_id, int user_id)
    {
        List<Tuple<string, string>> return_array = new List<Tuple<string, string>>();

        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<Int32> _report_ids = um.ReportsSearchIds(company_id, 0);

        List<int> departments_ids = db.report_department.Where(item => (_report_ids.Contains(item.report_id))).Select(item => item.department_id).Distinct().ToList();

        /////company_departments
        List<company_department> _all_departments = db.company_department.Where(item => (departments_ids.Contains(item.id))).ToList();
        foreach (company_department _temp_company_department in _all_departments)
        {
            List<int> report_ids_by_dept_id = db.report_department.Where(item => (_report_ids.Contains(item.report_id) && item.department_id == _temp_company_department.id)).Select(item => item.report_id).ToList();
            return_array.Add(new Tuple<string, string>(_temp_company_department.department_en, string.Join(",", report_ids_by_dept_id.ToArray())));
        }

        if (departments_ids.Contains(0))
        {
            List<int> report_ids_by_dept_id = db.report_department.Where(item => (_report_ids.Contains(item.report_id) && item.department_id == 0)).Select(item => item.report_id).ToList();
            return_array.Add(new Tuple<string, string>(GlobalRes.Other, string.Join(",", report_ids_by_dept_id.ToArray())));
        }
        return return_array;
    }

    /// Locations - Menu
    public List<Tuple<string, string>> LocationsListDistinct(int company_id, int user_id)
    {
        List<Tuple<string, string>> return_array = new List<Tuple<string, string>>();
        string _temp_loc = "";

        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();
        List<Int32> _user_report_ids = um.ReportsSearchIds(company_id, 0);

        List<int> location_ids = db.report.Where(item => (_user_report_ids.Contains(item.id) && item.location_id.HasValue)).Select(item => item.location_id.Value).Distinct().ToList();
        foreach (int _loc_id in location_ids)
        {
            _temp_loc = "";
            company_location _c_location = db.company_location.Where(item => item.id == _loc_id).FirstOrDefault();
            if (_c_location != null)
                _temp_loc = _c_location.location_en;


            List<int> _location_report_ids = db.report.Where(item => (_user_report_ids.Contains(item.id) && item.location_id.HasValue && item.location_id.Value == _loc_id)).Select(item => item.id).Distinct().ToList();
            return_array.Add(new Tuple<string, string>(_temp_loc, string.Join(",", _location_report_ids.ToArray())));

        }
        /////other location
        if (db.report.Any(item => (_user_report_ids.Contains(item.id) && (item.location_id.Value == 0 || !item.location_id.HasValue))))
        {
            List<int> temp_location_ids = db.report.Where(item => (_user_report_ids.Contains(item.id) && (item.location_id.Value == 0 || !item.location_id.HasValue))).Select(item => item.id).ToList();
            return_array.Add(new Tuple<string, string>(GlobalRes.Other, string.Join(",", temp_location_ids.ToArray())));
        }

        return return_array;
    }
    #endregion

    public bool isLoginInUse(string login)
    {
        if (db.user.Any(t => t.login_nm.Trim().ToLower() == login.Trim().ToLower()))
            return true;
        else
            return false;
    }

    public bool isCompanyInUse(string company_nm)
    {
        if (db.company.Any(t => t.company_nm.Trim().ToLower() == company_nm.Trim().ToLower()))
            return true;
        else
            return false;
    }

    public bool isCompanyShortInUse(string company_nm)
    {
        if (db.company.Any(t => t.company_short_name != null && t.company_short_name.Trim().ToLower() == company_nm.Trim().ToLower()))
            return true;
        else
            return false;
    }

    public string GenerateLoginName(string first, string last)
    {
        Regex rgx = new Regex("[^a-zA-Z]");


        first = rgx.Replace(first, "");
        last = rgx.Replace(last, "");

        first = first.Replace(" ", "");
        last = last.Replace(" ", "");

        string _first_short = "";
        string _last_short = "";

        if (first.Length > 0)
        {
            _first_short = first.ToCharArray()[0].ToString();
        }
        if (last.Length > 3)
        {
            _last_short = last.Substring(0, 4);
        }
        else
        {
            _last_short = last;
        }
        string _login_text_part = _first_short + _last_short;

        _login_text_part = (_login_text_part + StringUtil.RandomLetter(6 - _login_text_part.Length)).ToLower();
        _login_text_part = StringUtil.ReplaceForUI(_login_text_part);

        string _login_int_part = "";

        do
        {
            var random = new Random();
            _login_int_part = random.Next(10, 99).ToString();
            _login_int_part = StringUtil.ReplaceForUI(_login_int_part);

        }
        while (isLoginInUse(_login_text_part + _login_int_part));


        return _login_text_part + _login_int_part;
    }

    public string GenerateCompanyCode(string company_nm)
    {
        Regex rgx = new Regex("[^a-zA-Z]");
        company_nm = rgx.Replace(company_nm, "");
        company_nm = company_nm.Replace(" ", "");
        string _first_short = "";
        string _last_short = "";

        if (company_nm.Length > 2)
        {
            _first_short = company_nm.Substring(0, 3);
        }
        else
        {
            _first_short = company_nm;
        }

        _first_short = (_first_short + StringUtil.RandomLetter(3 - _first_short.Length)).ToUpper().Trim();

        _first_short = StringUtil.ReplaceForUI(_first_short);


        do
        {
            var random = new Random();
            _last_short = random.Next(1001, 9999).ToString();
            _last_short = StringUtil.ReplaceForUI(_last_short);
        }
        while (isCodeInUse(_first_short + _last_short));

        return _first_short + _last_short;
    }

    public string GenerateInvoiceNumber()
    {

        string _number = "INV_";
        string _invoice_ext = "";

        do
        {
            var random = new Random();
            _invoice_ext = random.Next(10001, 99999).ToString();
        }
        while (isInvoiceInUse(_number + _invoice_ext));

        return _number + _invoice_ext;
    }

    public bool isInvoiceInUse(string invoice)
    {
        if (db.company_payments.Any(t => t.auth_code.Trim().ToLower() == invoice.Trim().ToLower()))
            return true;
        else
            return false;
    }


    public bool isCodeInUse(string code)
    {
        if (db.company.Any(t => t.company_code.Trim().ToLower() == code.Trim().ToLower()))
            return true;
        else
            return false;
    }


    public static string IsValidPass(string password)
    {
        if (password != "" && password.Length >= PasswordLength)
        {
            return "Success";
        }
        return String.Format("The password should be at least {0} characters long", PasswordLength.ToString());
    }

    public void CampusSecurityAlertEmail(int user_from, report report, Uri uri, ECEntities db, string email)
    {
        ////   return;
        IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();
        EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(true);
        EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, uri.AbsoluteUri.ToLower());

        var user_to = db.user.FirstOrDefault(x => x.role_id == 5 && x.company_id == report.company_id);
        if ((user_to != null) && (email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(email.Trim()))
        {
            string phone = $"{user_to.phone}";
            if (string.IsNullOrEmpty(phone))
                phone = $"{user_to.email}";

            eb.CampusSecurityAlert(
                report.id.ToString(),
                report.display_name,
                $"{user_to.first_nm} {user_to.last_nm}",
                phone
                );
            SaveEmailBeforeSend(user_from, user_to.id, user_to.company_id, email.Trim(), ConfigurationManager.AppSettings["emailFrom"], "",
                EC.App_LocalResources.GlobalRes.CampusSecurityAlert, eb.Body, false, 51);
        }
    }


    public void CampusSecurityAlertEmail_bkp(report report, Uri uri, ECEntities db, string email, string first_nm, string last_nm)
    {
        ////   return;
        IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();
        EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(true);
        EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, uri.AbsoluteUri.ToLower());

        var user_pm = db.user.FirstOrDefault(x => x.role_id == 5 && x.company_id == report.company_id);
        if ((user_pm != null) && (email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(email.Trim()))
        {

            string phone = $"{user_pm.phone}";
            if (string.IsNullOrEmpty(phone))
                phone = $"{user_pm.email}";

            string first_nm_temp = first_nm;
            string last_nm_temp = last_nm;
            if (string.IsNullOrEmpty(first_nm_temp) && string.IsNullOrEmpty(last_nm_temp))
            {
                first_nm_temp = user_pm.first_nm;
                last_nm_temp = user_pm.last_nm;
            }
            eb.CampusSecurityAlert(
                report.id.ToString(),
                report.display_name,
                $"{first_nm_temp} {last_nm_temp}",
                phone
                );
            this.SaveEmailBeforeSend(0, user_pm.id, user_pm.company_id, email, ConfigurationManager.AppSettings["emailFrom"], "",
               GlobalRes.CampusSecurityAlert, eb.Body, false, 0);

        }
    }
    public string Photo_Path_String(string photo_path, int param, int photo_user_role)
    {
        string base_url = ConfigurationManager.AppSettings["SiteRoot"];
        string _photo_path = "";
        bool file_exist = false;

        if (photo_path != "")
        {
            HttpWebResponse response = null;
            try
            {
                WebRequest request = WebRequest.Create(photo_path);
                request.Timeout = 1200;
                response = (HttpWebResponse)request.GetResponse();
                file_exist = true;
            }
            catch (Exception ex)
            {
                file_exist = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }


        if (photo_path != "" && file_exist)
        {
            _photo_path = photo_path;
        }
        else
        {
            if (param == 1)
            {
                _photo_path = base_url + "/Content/Icons/noPhoto.png";
            }
            else if (param == 2)
            {
                _photo_path = base_url + "/Content/Icons/settingsPersonalNOPhoto.png";
            }
            else if (param == 3)
            {
                _photo_path = base_url + "/Content/Icons/settingsPersonalNOPhoto.png";
            }
            if (photo_user_role == EC.Constants.ECLevelConstants.level_informant)
            {
                _photo_path = base_url + "/Content/Icons/anonimousReporterIcon.png";
            }
        }
        return _photo_path;
    }

    public bool SaveEmailBeforeSend(int user_id_from, int user_id_to, int company_id, string to, string from, string cc, string subject, string msg, bool send, int email_type)
    {
        var email = new email
        {
            To = to,
            From = from,
            cc = cc,
            Title = subject,
            Body = msg,
            EmailType = email_type,
            is_sent = false,
            user_id_from = user_id_from,
            user_id_to = user_id_to,
            company_id = company_id,
            created_dt = DateTime.Now,
            isSSL = false
        };
        db.email.Add(email);
        db.SaveChanges();

        return true;
    }
}
class CompanyLocation
{
    public int id { get; set; }
    public int countLocations { get; set; }
    public string NameLocation { get; set; }
}