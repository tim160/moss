using EC.Common.Interfaces;
using EC.Common.Util;
using EC.Core.Common;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EC.Models.Services.AnalyticsService
{
    public class AverageStage
    {
        private IDateTimeHelper m_DateTimeHelper;
        ECEntities db = new ECEntities();
        public AverageStage()
        {
            m_DateTimeHelper = new DateTimeHelper();

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

        public string AnalyticsTimelineAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
        {
            DataTable dt = AnalyticsTimelineAdvanced(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
            return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
        }
        #region Analytics Helpers - new version



        /// <summary>
        /// generates average number days per stage.
        /// </summary>
        /// <param name="company_id"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>


        public DataTable AnalyticsTimelineAdvanced(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
        {
            DataTable dt = AnalyticsTimeLineTable.dtAnalyticsTimeLineTable();
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
            DataRow dr = AnalyticsTimeLineTable.dtAnalyticsTimeLineTable().NewRow();

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

    }
}