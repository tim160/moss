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

    #endregion






    #region JsonReports




    #endregion

    #region ReportsDataTables
    




    private DataTable dtTaskLengthTable()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("report_id", typeof(int));
        dt.Columns.Add("value", typeof(int));
        dt.Columns.Add("length", typeof(int));

        return dt;
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
