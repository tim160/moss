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

using EC.Common.Interfaces;
using EC.Core.Common;
using System.Web.Script.Serialization;
using log4net;
using EC.Common.Util;
using EC.Common.Base;
using System.Net;
using EC.Localization;
using EC.Constants;
using EC.Utils;

/// <summary>
/// Global Functions for EC Project
/// </summary>
public class GlobalFunctions
{
    ECEntities db = new ECEntities();
    IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();


    ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public GlobalFunctions()
    {

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

        List<String> ReportsSecondaryTypesIDs = new List<String>();

        List<String> ReportsRelationTypesIDs = new List<String>();

        List<String> ReportsDepartmentIDs = new List<String>();

        List<String> ReportsLocationIDs = new List<String>();



        if (ReportsSecondaryTypesIDStrings.Trim().Length > 0)
            ReportsSecondaryTypesIDs = ReportsSecondaryTypesIDStrings.Split(',').ToList();

        if (ReportsRelationTypesIDStrings.Trim().Length > 0)
            ReportsRelationTypesIDs = ReportsRelationTypesIDStrings.Split(',').ToList();

        if (ReportsDepartmentIDStringss.Trim().Length > 0)
            ReportsDepartmentIDs = ReportsDepartmentIDStringss.Split(',').ToList();

        if (ReportsLocationIDStrings.Trim().Length > 0)
            ReportsLocationIDs = ReportsLocationIDStrings.Split(',').ToList();

        if (ReportsDepartmentIDs.Count > 0)
        {


            var idReports = _all_reports_old.Select(report => report.id).ToList();
            var DepAndReports = db.report_department.Join(db.company_department,
                                    post => post.department_id,
                                    meta => meta.id,
                                    (post, meta) => new { Post = post, Meta = meta })
                                    .Where(postAndMeta => ReportsDepartmentIDs.Any(repDep => repDep.Equals(postAndMeta.Meta.department_en)) && idReports.Contains(postAndMeta.Post.report_id));
            _all_reports_old = _all_reports_old.Where(oldReport => DepAndReports.Select(res => res.Post.report_id).Contains(oldReport.id)).ToList();
        }
        if (ReportsRelationTypesIDs.Count > 0)
        {
            var idReports = _all_reports_old.Select(report => report.id).ToList();
            HashSet<report> listReports = new HashSet<report>();

            if (ReportsRelationTypesIDs.Where(x => x == "Other").ToList().Count > 0)
            {
                var listOther = db.report_relationship.Where(rel_rel => rel_rel.company_relationship_id == null && idReports.Contains(rel_rel.report_id)).Select(rep => rep.report_id).ToList();

                var reportsContainstOther = _all_reports_old.Where(old_rep => listOther.Contains(old_rep.id));

                foreach (var item in reportsContainstOther)
                {
                    listReports.Add(item);
                }
            }
            //здесь неправильно скорей всего
            var relationship = _all_reports_old.Join(db.company_relationship,
                                            post => post.company_relationship_id,
                                            meta => meta.id,
                                            (post, meta) => new { Post = post, Meta = meta })
                                            .Where(postAndMeta => ReportsRelationTypesIDs.Any(listReportsRel => listReportsRel.Equals(postAndMeta.Meta.relationship_en)));
            var reportsWithFilter = relationship.Select(rel => rel.Post).ToList();
            foreach (var item in reportsWithFilter)
            {
                listReports.Add(item);
            }
            _all_reports_old = listReports.ToList();
        }
        if (ReportsSecondaryTypesIDs.Count > 0)
        {
            var idReports = _all_reports_old.Select(report => report.id).ToList();
            HashSet<report> listReports = new HashSet<report>();
            if (ReportsSecondaryTypesIDs.Where(x => x.Equals("Other")).ToList().Count > 0)
            {
                var listOther = db.report_secondary_type.Where(sec_type => sec_type.secondary_type_id == 0 && idReports.Contains(sec_type.report_id)).Select(sec_type => sec_type.report_id).ToList();

                var reportsContainstOther = _all_reports_old.Where(old_rep => listOther.Contains(old_rep.id));

                foreach (var item in reportsContainstOther)
                {
                    listReports.Add(item);
                }
            }

            var secondaryTypes = db.report_secondary_type.Join(db.company_secondary_type,
                post => post.secondary_type_id,
                meta => meta.id,
                (post, meta) => new { Post = post, Meta = meta })
                .Where(postAndMeta => ReportsSecondaryTypesIDs.Any(listSecTypes => listSecTypes.Equals(postAndMeta.Meta.secondary_type_en))
                && idReports.Contains(postAndMeta.Post.report_id));
            var reportsWithFilter = secondaryTypes.Select(types => types.Post.report_id).ToList();

            var selectedReports = _all_reports_old.Where(rep => reportsWithFilter.Contains(rep.id));

            foreach (var item in selectedReports)
            {
                listReports.Add(item);
            }
            _all_reports_old = listReports.ToList();
        }

        if (ReportsLocationIDs.Count > 0)
        {
            var temp = _all_reports_old.Join(db.company_location,
                                            post => post.location_id,
                                            meta => meta.id,
                                            (post, meta) => new { Post = post, Meta = meta })
                                            .Where(postAndMeta => ReportsLocationIDs.Any(listLocation => listLocation.Equals(postAndMeta.Meta.location_en))
                                            );
            _all_reports_old = new List<report>();
            foreach (var item in temp)
            {
                _all_reports_old.Add(item.Post);
            };
        }
        #endregion

        return _all_reports_old;
    }

    #region Analytics Helpers - new version



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

    public DataTable dtDoughnutTable()
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
            return_array.Add(new Tuple<string, string>(LocalizationGetter.GetString("Other"), string.Join(",", report_ids_by_rel_id.ToArray())));
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
                return_array.Add(new Tuple<string, string>(LocalizationGetter.GetString("Other"), string.Join(",", report_ids_by_rel_id.ToArray())));
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
            return_array.Add(new Tuple<string, string>(LocalizationGetter.GetString("Other"), string.Join(",", report_ids_by_dept_id.ToArray())));
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
            return_array.Add(new Tuple<string, string>(LocalizationGetter.GetString("Other"), string.Join(",", temp_location_ids.ToArray())));
        }

        return return_array;
    }
    #endregion
   

    public static string IsValidPass(string password)
    {
        if (password != "" && password.Length >= PasswordConstants.PASSWORD_MIN_LENGTH)
        {
            return "Success";
        }
        return String.Format("The password should be at least {0} characters long", PasswordConstants.PASSWORD_MIN_LENGTH.ToString());
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
               LocalizationGetter.GetString("CampusSecurityAlert", true), eb.Body, false, 51);
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

    public string resendInvitation(string email, bool is_cc, HttpRequestBase Request, user AdminUser)
    {
        email = email.ToLower().Trim();
        var invite = db.invitation.Where(i => i.email == email).FirstOrDefault();

        if (invite != null)
        {
            var userSender = db.user.Find(invite.sent_by_user_id);
            var company = db.company.FirstOrDefault(x => x.id == userSender.company_id);

            IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();

            if ((email.Length > 0) && m_EmailHelper.IsValidEmail(email))
            {
                EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
                EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

                eb.MediatorInvited(AdminUser.first_nm, AdminUser.last_nm,
                    userSender.first_nm, userSender.last_nm, company.company_nm, invite.code,
                    DomainUtil.GetSubdomainLink(Request.Url.AbsoluteUri.ToLower(),
                    Request.Url.AbsoluteUri.ToLower()) + "/new/?code=" + invite.code + "&email=" + email);

                SaveEmailBeforeSend(userSender.id, 0, 0, email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
                   LocalizationGetter.GetString("Email_Title_MediatorInvited", is_cc), eb.Body, false, 41);

                return LocalizationGetter.GetString("InvitationWasSent", is_cc);
            }
        }
        else
        {
            var newAdminUser = db.user.Where(u =>
                u.email.Equals(email) &&
                u.company_id == AdminUser.company_id &&
                u.status_id == ECStatusConstants.Pending_Value).FirstOrDefault();

            if (newAdminUser != null)
            {
                var company = db.company.Where(c => c.id == newAdminUser.company_id).FirstOrDefault();

                sendAddNewAdmin(AdminUser, company, newAdminUser, is_cc);

                return LocalizationGetter.GetString("InvitationWasSent", is_cc);
            }
        }
        return LocalizationGetter.GetString("ErrorResendingEmail", is_cc);
    }
    public void sendAddNewAdmin(user adminUser, company company, user newUser, bool is_cc)
    {
        var generateModel = new GenerateRecordsModel();

        var password = generateModel.GeneretedPassword();

        EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
        EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
        eb.NewMediator(
            $"{adminUser.first_nm} {adminUser.last_nm}",
            $"{company.company_nm}",
            HttpContext.Current.Request.Url.AbsoluteUri.ToLower(),
            HttpContext.Current.Request.Url.AbsoluteUri.ToLower(),
            $"{newUser.login_nm}",
            $"{password}");
        string body = eb.Body;
        SaveEmailBeforeSend(adminUser.id, newUser.id, adminUser.company_id,
            newUser.email, System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", "You have been added as a Case Administrator", body, false, 0);

        var newUserDb = db.user.Find(newUser.id);
        newUserDb.password = PasswordUtils.GetHash(password);
        db.SaveChanges();
    }
}
