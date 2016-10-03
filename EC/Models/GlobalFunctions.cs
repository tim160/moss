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
using System.Web.Script.Serialization;
/// <summary>
/// Global Functions for EC Project
/// </summary>
public class GlobalFunctions
{
    ECEntities db = new ECEntities();
    private  const int PasswordLength = 6;
    private const int PasswordExtraSubmolsCount = 0;


    public GlobalFunctions()
    {

    }
    // Content/img/cai_logo.png
    public bool IsSubdomain(string url)
    {
        if (url.ToLower().Contains("localhost") || url.ToLower().Contains("stark.") || url.ToLower().Contains("democompany.") || url.ToLower().Contains("report.") || url.ToLower().Contains("cai."))
        {
            return true;
        }
        return false;
    }

    public bool IsCC(string url)
    {
        // uncomment for campus-confidential testing
      //  return true;
        if ( (url.ToLower().Contains("campus"))  || (url.ToLower().Contains("cc.employeeconfidential")) )
        {
            return true;
        }

        return false;
    }


    public string LogoBaseUrl(string url)
    {
        if (url.ToLower().Contains("campus"))
        {
            return "/Content/img/secondLogo.jpg";
        }
        else if (url.ToLower().Contains("stark."))
        {
            return "/Content/img/secondLogo.jpg";
        }
        else if (url.ToLower().Contains("report."))
        {
            return "/Content/img/secondLogo.jpg";

        }
        else if (url.ToLower().Contains("cai.employeeconfidential.com"))
        {
            return "/Content/img/cai_logo.png";
        }
        return "/Content/img/secondLogo.jpg";

    }


    /// <summary>
    /// duplicated - the function in EC/Business/Email Body
    /// </summary>
    /// <param name="url"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public string EmailBaseUrl(string url, int flag)
    {
        if (url.ToLower().Contains("campus"))
        {
            return "campus-confidential.com/Index/Page";
        }
        else if(url.ToLower().Contains("stark."))
        {
            return "stark.employeeconfidential.com/Index/Page";
        }
        else if (url.ToLower().Contains("report."))
        {
            return "report.employeeconfidential.com/Index/Page";
        }
        else if (url.ToLower().Contains("cai.employeeconfidential.com"))
        {
            return "cai.employeeconfidential.com/Index/Start";
        }
        return "report.employeeconfidential.com/Index/Page";
    }

    #region Months

    public Dictionary<int, string> FullMonth()
    {
        Dictionary<int, string> month = new Dictionary<int, string>();

        month.Add(1, "January");
        month.Add(2, "February");
        month.Add(3, "March");
        month.Add(4, "April");
        month.Add(5, "May");
        month.Add(6, "June");
        month.Add(7, "July");
        month.Add(8, "August");
        month.Add(9, "September");
        month.Add(10, "October");
        month.Add(11, "November");
        month.Add(12, "December");

        return month;
    }
    public Dictionary<int, string> ShortMonth()
    {
        Dictionary<int, string> month = new Dictionary<int, string>();

        month.Add(1, "Jan");
        month.Add(2, "Feb");
        month.Add(3, "Mar");
        month.Add(4, "Apr");
        month.Add(5, "May");
        month.Add(6, "Jun");
        month.Add(7, "Jul");
        month.Add(8, "Aug");
        month.Add(9, "Sep");
        month.Add(10, "Oct");
        month.Add(11, "Nov");
        month.Add(12, "Dec");

        return month;
    }

    /// <summary>
    /// returns February
    /// </summary>
    /// <param name="month"></param>
    /// <returns></returns>
    public string GetFullMonth(int month)
    {
        string month_s = "";
        Dictionary<int, string> Monthes = FullMonth();
        if ((month > 0) && (month < 13))
            Monthes.TryGetValue(month, out month_s);

        return month_s;

    } 
    public string GetShortMonth(int month)
    {
        string month_s = "";
        Dictionary<int, string> Monthes = ShortMonth();
        if ((month > 0) && (month < 13))
            Monthes.TryGetValue(month, out month_s);

        return month_s;

    } 
    #endregion

    #region ReportGeneration

    public string GeneretedPassword()
    {
        string password = System.Web.Security.Membership.GeneratePassword(PasswordLength, PasswordExtraSubmolsCount);
        password = ReplaceForUI(password);
        return password;
    }

    public string GenerateCaseNumber(int report_id, int company_id, string company_name)
    {
        string letter = "EMP";
        if (company_name.Length > 0)
            letter = company_name[0].ToString().ToUpper();
        if (company_id == 1)
            letter = "UNK";
        if (company_id == 2)
            letter = "STA";
        int number = 22000 + report_id;
        string case_number = number.ToString() + "-" + letter + "-" + company_id.ToString();
        
        return case_number;
    }

    public string GenerateReporterLogin(int report_id)
    {
        Random rd = new Random();
        string reporter_login = "";

        do
        {
            reporter_login = reporter_login + rd.Next(0, 9).ToString();
        }
        while ((reporter_login.Length) < 6 && isLoginInUse("EC" + reporter_login));

   //     ?while ((reporter_login.Length + report_id.ToString().Length) < 6);

        return "EC" + reporter_login;
    }
    
    #endregion

    public string GetCountryNameByID(string country_id, int? language_id)
    {
        string country_nm = "";

        var item = db.country.Find(country_id);
        if (item != null)
        {
            //Record exists.Let's read the Name property value
            country_nm = item.country_nm;
           
        }

        return country_nm;
    }

    public List<outcome> GetOutcomesWithStatus()
    {
        //[company_nm]
        //return (from comp in db.company where comp.status_id == 2 select comp.company_nm, comp.id).ToList();
        return db.outcome.Where(item => item.is_active == 1).ToList();
    }

    public string GetOutcomeNameById(int id)
    {
        if (id != 0)
        {
           // EC.Models.Database.outcome _outcome = db.outcomes.FirstOrDefault(item => item.id == id);
            var item = db.outcome.Find(id);
            return item.outcome_en;
        }
        else
            return "";

    }

    public void Load_Provinces(string strCountryID, DropDownList ddl_province)
    {

    //    Hashtable ht = new Hashtable();
    //    ht.Add("@country_id", strCountryID);
     //   sce.BindDropDownList(System.Web.HttpContext.Current.Session[Constants.session_connection_string_name].ToString(), ddl_province, "spc_GetProvinceList", "province_nm", "id", ht);
    }


    public action GetActionById(int id)
    {
        return db.action.FirstOrDefault(item => item.id == id);
    }

    #region mediators from company, who are involved or have access to report

    /// <summary>
    /// list of all reports user can access.
    /// </summary>
    /// <param name="user_id">just pass id of user to look for.</param>
    /// <param name="company_id">pass null or 0 if you need all reports. Pass company_id if user_role=1,2,3 is looking for specific company</param>
    /// <returns></returns>
    public List<report> ReportsSearch(int user_id, int? company_id)
    { 
        List<report> reports = new List<report>();

        user _user = db.user.FirstOrDefault(item => item.id == user_id);

        if ((_user != null) && (_user.id != 0))
        { 
            // if user is from Ec - we can show all reports
            if ((_user.role_id == 1) && (_user.role_id == 2) && (_user.role_id == 3))
            {
                if ((company_id.HasValue) && (company_id.Value!=0))
                    reports = (db.report.Where(item =>(item.company_id == company_id.Value))).ToList();
                else
                    reports = (db.report).ToList();

            }
            // if user is top mediator - we can show all reports from company where user is not involved
            if ((_user.role_id == 4) || (_user.role_id == 5))
            {
                List<int> involved_report_ids = (db.report_mediator_involved.Where(item => (item.mediator_id == _user.id)).Select(item => item.report_id)).ToList();
                reports = (db.report.Where(item => ((item.company_id == _user.company_id) && (!involved_report_ids.Contains(item.id))))).ToList();
            }
            // if user is regular mediator / legal - we show only reports where he is assigned and hide all others
            if ((_user.role_id == 6) || (_user.role_id == 7))
            {
                List<int> assigned_report_ids = (db.report_mediator_assigned.Where(item => (item.mediator_id == _user.id)).Select(item => item.report_id)).ToList();
                reports = (db.report.Where(item => assigned_report_ids.Contains(item.id))).ToList();
            }

            // if user is reporter - we can only show his report
            if ((_user.role_id == 8))
            {
                reports = (db.report.Where(item => item.reporter_user_id == _user.id)).ToList();
            }
        }

        return reports;
    }

    /// <summary>
    /// report log ( report history for user) - 
    /// </summary>
    /// <param name="user_id"></param>
    /// <param name="report_id"></param>
    /// <returns></returns>
    public List<report_log> ReportActivity(int user_id, int report_id)
    {
        // list of log string for report.
        List<report_log> log = db.report_log.Where(item => item.report_id == report_id).ToList();

        // nado izvlech descrition iz table action
        // i 2 userza  _log.user_id - chel, kotorii sdelal deistvie
        // i _log.second_user_id (int? - obichno net ego ) - 2-oi uzer, kotorogo mojet ne bit' - naprimer kogda odin mediator assing/remove drugogo
        
        foreach (report_log _log in log)
        { 
        
        }

        return log;
    }
    #endregion


    /*

   

 
    /// <summary>
    /// Shows a pop-up Dialog box with a message
    /// </summary>
    /// <param name="message">Text of the Message</param>
    public void ShowMessage(string message, Page page)
    {
        message = message.Replace("'", "");
        if (message != "")
        {
            string Msg = "<script language='javascript'> window.alert('" + message + "'); </script>";
            ScriptManager.RegisterStartupScript(page, page.GetType(), "js_msg", Msg, false);
        }
    }

    /// <summary>
    /// returns current Language or just default language
    /// </summary>
    /// <returns></returns>
    public string CurrentLanguage()
    {
            string lang = ConfigurationManager.AppSettings["Default Language"];
            if (System.Web.HttpContext.Current.Session[Constants.session_language_cl] != null)
            {
                lang = System.Web.HttpContext.Current.Session[Constants.session_language_cl].ToString();
            }
            else
                System.Web.HttpContext.Current.Session[Constants.session_language_cl] = lang;
            return lang;
    }

    public void CreateConnectionString()
    {

        if (System.Web.HttpContext.Current.Session[Constants.session_connection_string_name] == null)
        {
            string strUrl = HttpContext.Current.Request.Url.ToString();
            if (strUrl.ToLower().Contains("vp.") == true)
                System.Web.HttpContext.Current.Session[Constants.session_connection_string_name] = "VBDEMO";
            else
                System.Web.HttpContext.Current.Session[Constants.session_connection_string_name] = "VB";
        }
    }

    public string CreateConnectionString(object connection)
    {
        string connection_string = "";

        if (connection == null)
        {
            string strUrl = HttpContext.Current.Request.Url.ToString();
            if (strUrl.ToLower().Contains("demo.") == true)
            {
                System.Web.HttpContext.Current.Session[Constants.session_connection_string_name] = "VBDEMO";
                connection_string = "VBDEMO";
            }
            else
            {
                System.Web.HttpContext.Current.Session[Constants.session_connection_string_name] = "VB";
                connection_string = "VB";
            }
        }
        return connection_string;
    }

    public string GetCurrentPageName()
    {
        string strPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
        System.IO.FileInfo oInfo = new System.IO.FileInfo(strPath);
        string strPageName = oInfo.Name;
        return strPageName.ToLower();
    }


    public bool DownloadFile(string strReportFullName, bool something)
    {
        bool bResult = false;

        string strExtensionName = Path.GetExtension(strReportFullName);
        string strError = "";

        try
        {
            switch (strExtensionName)
            {
                case ".txt":
                    {
                        HttpContext.Current.Response.ContentType = "application/txt";
                        bResult = true;
                        break;
                    }
                case ".doc":
                    {
                        HttpContext.Current.Response.ContentType = "application/msword";
                        bResult = true;
                        break;
                    }
                case ".docx":
                    {
                        HttpContext.Current.Response.ContentType = "application/msword";
                        bResult = true;
                        break;
                    }
                case ".xls":
                    {
                        HttpContext.Current.Response.ContentType = "application/xls";
                        bResult = true;
                        break;
                    }
                case ".pdf":
                    {
                        HttpContext.Current.Response.ContentType = "application/pdf";
                        bResult = true;
                        break;
                    }
                case ".rtf":
                    {
                        HttpContext.Current.Response.ContentType = "application/msword";
                        bResult = true;
                        break;
                    }
                case ".msg":
                    {
                        HttpContext.Current.Response.ContentType = "application/msoutlook";
                        bResult = true;
                        break;
                    }
                case ".jpg":
                    {
                        HttpContext.Current.Response.ContentType = "application/jpg";
                        bResult = true;
                        break;
                    }
                case ".jpeg":
                    {
                        HttpContext.Current.Response.ContentType = "application/jpg";
                        bResult = true;
                        break;
                    }
                case ".png":
                    {
                        HttpContext.Current.Response.ContentType = "application/png";
                        bResult = true;
                        break;
                    }
                default:
                    {
                        bResult = false;
                        strError = "Format doesn't supporting.";
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            strError = ex.Message;
            bResult = false;
        }

        if (bResult)
        {
            if (something)
            {
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename = " + System.IO.Path.GetFileName(strReportFullName));
                HttpContext.Current.Response.WriteFile(strReportFullName);
                HttpContext.Current.Response.End();
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename = " + System.IO.Path.GetFileName(strReportFullName));
                HttpContext.Current.Response.WriteFile(strReportFullName);
                HttpContext.Current.Response.End();
            }
        }

        return bResult;
    }
    public bool IsInternalUser(string level_id)
    {
        if ((level_id == Constants.level_Reviewer.ToString()) || (level_id == Constants.level_Primary_Reviewer.ToString()) || (level_id == Constants.level_Reporter.ToString())|| (level_id == Constants.level_Administrator.ToString()))
        {
            return false;
        }
        else
        {
            return true;
        }
    }




    public string GetProvinceByID(string province_id, string country_id, string connection)
    {
        string province_nm = "";
        Hashtable ht = new Hashtable();
        ht.Add("@country_id", country_id.Trim());

        DataTable dt = sce.ExecuteDataTable("spc_GetProvinceList", ht, connection);
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["province_id"].ToString() == province_id)
                province_nm = dr["province_nm"].ToString();
        }

        return province_nm;
    }

    public string GetClientStatusNameByID(string status_id, string language, string connection)
    {
        string status_nm = "";
        Hashtable ht = new Hashtable();
        ht.Add("@language_cl", language);

        DataTable dt = sce.ExecuteDataTable("spc_GetClientStatuses", ht, connection);
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["status_id"].ToString() == status_id)
                status_nm = dr["status_nm"].ToString();
        }

        return status_nm;
    }

    public string GetPrefferedContactMethodsByID(string preffered_method_id, string language, string connection)
    {
        string prefferred_method_nm = "";
        Hashtable ht = new Hashtable();
        ht.Add("@language_cl", language);

        DataTable dt = sce.ExecuteDataTable("spc_GetClientStatuses", ht, connection);
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["prefferred_method_id"].ToString() == preffered_method_id)
                prefferred_method_nm = dr["prefferred_method_nm"].ToString();
        }

        return prefferred_method_nm;
    }

    public string GetIncidentStatusNameByID(string status_id, string language, string connection)
    {
        string status_nm = "";
        Hashtable ht = new Hashtable();
        ht.Add("@language_cl", language);

        DataTable dt = sce.ExecuteDataTable("spi_GetIncidentStatuses", ht, connection);
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["status_id"].ToString() == status_id)
                status_nm = dr["status_nm"].ToString();
        }

        return status_nm;
    }

    public string GetIncidentPriorityNameByID(string incident_priority_id, string language, string connection)
    {
        string incident_priority_nm = "";
        Hashtable ht = new Hashtable();
        ht.Add("@language_cl", language);

        DataTable dt = sce.ExecuteDataTable("spi_GetIncidentPriorities", ht, connection);
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["incident_priority_id"].ToString() == incident_priority_id)
                incident_priority_nm = dr["incident_priority_nm"].ToString();
        }

        return incident_priority_nm;
    }

    public string GetIncidentSourceTypeNameByID(string source_type_id, string language, string connection)
    {
        string source_type_nm = "";
        Hashtable ht = new Hashtable();
        ht.Add("@language_cl", language);

        DataTable dt = sce.ExecuteDataTable("spi_GetIncidentPriorities", ht, connection);
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["source_type_id"].ToString() == source_type_id)
                source_type_nm = dr["source_type_nm"].ToString();
        }

        return source_type_nm;
    }

    public string GetShortLanguageNameByID(string language_id, string connection, out string language_nm)
    {
   //     string language_nm = "";
        string short_language_nm = "";
        language_nm = "";

        Hashtable ht = new Hashtable();
        ht.Add("@language_id", language_id);

        DataTable dt = sce.ExecuteDataTable("spa_GetLanguageByID", ht, connection);
        if (dt.Rows.Count == 1)
        {
            language_nm = dt.Rows[0]["language_ds"].ToString();
            short_language_nm= dt.Rows[0]["language_cl"].ToString();
        }

        return short_language_nm;
    }*/

    public string FirstWords(string input, int numberWords)
    {
        try
        {
            // Number of words we still want to display.
            int words = numberWords;
            // Loop through entire summary.
            for (int i = 0; i < input.Length; i++)
            {
                // Increment words on a space.
                if (input[i] == ' ')
                {
                    words--;
                }
                // If we have no more words to display, return the substring.
                if (words == 0)
                {
                    return input.Substring(0, i);
                }
            }
        }
        catch (Exception)
        {
            // Log the error.
        }
        return input;
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
            }
        }
        else
        {
            task_user_read _tur = db.task_user_read.Where(item => ((item.user_id == user_id) && (item.task_id == task_id))).FirstOrDefault();
            _tur.read_date = DateTime.Now;
            db.SaveChanges();
        }
    }

    public void UpdateMessageRead(int user_id, int message_id)
    {
        message_user_read _user_read = new message_user_read();
        _user_read.message_id = message_id;
        _user_read.user_id = user_id;
        _user_read.read_date = DateTime.Now;

        if (!db.message_user_read.Any(item => ((item.user_id == user_id) && (item.message_id == message_id))))
        {
            db.message_user_read.Add(_user_read);
            try
            {
                db.SaveChanges();
                // need to save new row to _user_read about 
            }
            catch (Exception ex)
            {
            }
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

        foreach(int _id in _message_ids)
        {
            if(db.message_user_read.Any(t=>t.message_id == _id && t.user_id == user_id))
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
                }

            }
        }

      //  if (!db.message_user_read.Any(item => ((item.user_id == user_id) && (item.message_id == message_id))))
        {
        }
    
    
    }


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
            }
        }
    } 
    #endregion

    public int GetNextColor(int company_id, int report_id)
    {
        int color_id = 1;

        List<report> reports = new List<report>();


        if (db.report.Any(item => ((item.company_id == company_id) && (item.id != report_id))))
            reports = (db.report.Where(item => ((item.company_id == company_id) && (item.id != report_id))).OrderByDescending(item => item.id)).ToList();
        
        if (reports.Count > 0)
        {
            report _report = reports[0];
            color_id = _report.report_color_id;
            int _all_colors_count = db.color.Count();

            if (color_id < _all_colors_count)
            {
                color_id++;
            }
            else
                color_id = 1;
        }

        return color_id;
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

            if ((rm._last_promoted_date >= _real_start) && (rm._last_promoted_date <= _real_end))
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
    public DataTable SecondaryTypesByDate(int company_id, int user_id)
    {
        DateTime _real_start, _real_end;

        _real_start = new DateTime(2015, 1, 1);
        _real_end = DateTime.Today.AddDays(1);
        DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);

        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<report> _all_reports = um.ReportsSearch(company_id, 0);

        List<secondary_type_mandatory> all_types = new List<secondary_type_mandatory>();
        all_types = (db.secondary_type_mandatory).ToList();

        List<report> _selected_reports = new List<report>();
        List<report> _previous_reports = new List<report>();

        foreach (report _report in _all_reports)
        {
            if ((_report.reported_dt >= _real_start) && (_report.reported_dt <= _real_end))
            {
                _selected_reports.Add(_report);
            }
        }

        // getting the data for previous month
        _real_end = _month_end_date;
        foreach (report _report in _all_reports)
        {
            if ((_report.reported_dt >= _real_start) && (_report.reported_dt <= _real_end))
            {
                _previous_reports.Add(_report);
            }
        }
        // merge with previous
        List<Int32> _report_ids = _all_reports.Select(t => t.id).ToList();
        List<Int32> _previous_report_ids = _previous_reports.Select(t => t.id).ToList();

        DataTable dt = dtAnalyticsTable();
     
        List<company_secondary_type> _c_secondary_types = db.company_secondary_type.Where(item => item.company_id == company_id).ToList();

        foreach (company_secondary_type _temp_secondary_types in _c_secondary_types)
        {
            //int prev_count = 0;
            int count = db.report_secondary_type.Where(item => ((item.secondary_type_id == _temp_secondary_types.id) && (_report_ids.Contains(item.report_id)))).Count();
            int prev_count = db.report_secondary_type.Where(item => ((item.secondary_type_id == _temp_secondary_types.id) && (_previous_report_ids.Contains(item.report_id)))).Count();

            if (count > 0)
            {
                DataRow dr;
              //  prev_count = _previous_reports.Where(item => (item.type_id == _type.id)).Count();
                dr = dt.NewRow();
                dr["name"] = _temp_secondary_types.secondary_type_en;
                dr["value"] = count;
                dr["prev"] = prev_count;
                dt.Rows.Add(dr);
            }
        }



        List<report_secondary_type> _all_types = db.report_secondary_type.Where(item => (_report_ids.Contains(item.report_id)) && ((item.secondary_type_id == 0 || item.secondary_type_id == -1))).ToList();
        
        foreach (report_secondary_type _type in _all_types)
        {
            string _temp_secondary_type_nm = _type.secondary_type_nm;
            bool is_in_dt = false;
            int row_num = 0;
            int i = 0;

            int _previous = 0;
            if (_previous_report_ids.Contains(_type.report_id))
                _previous = 1;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["name"].ToString().ToLower().Trim() == _temp_secondary_type_nm.ToLower().Trim())
                {
                    is_in_dt = true;
                    row_num = i;
                }
                i++;
            }

            if (!is_in_dt)
            {
                //  prev_count = _previous_reports.Where(item => (item.type_id == _type.id)).Count();
                DataRow dr = dt.NewRow();
                dr["name"] = _temp_secondary_type_nm;
                dr["value"] = 1;
                dr["prev"] = _previous;
                dt.Rows.Add(dr);

            }
            else
            {
                DataRow dr = dt.Rows[row_num];
                dr["value"] = Convert.ToInt32(dr["value"]) + 1;
                dr["prev"] = Convert.ToInt32(dr["prev"]) + _previous;
            }
        }

        return dt;
    }

    //done
    public DataTable RelationshipToCompanyByDate(int company_id, int user_id)
    {
        DateTime _real_start, _real_end;

        _real_start = new DateTime(2015, 1, 1);
        _real_end = DateTime.Today.AddDays(1);
        DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);

        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<report> _all_reports = um.ReportsSearch(company_id, 0);

        List<relationship> all_relationships = new List<relationship>();
        all_relationships = (db.relationship).ToList();

        List<report> _selected_reports = new List<report>();
        List<report> _previous_reports = new List<report>();

        foreach (report _report in _all_reports)
        {
            if ((_report.reported_dt >= _real_start) && (_report.reported_dt <= _real_end))
            {
                _selected_reports.Add(_report);
            }
        }

        // getting the data for previous month
        _real_end = _month_end_date;
        foreach (report _report in _all_reports)
        {
            if ((_report.reported_dt >= _real_start) && (_report.reported_dt <= _real_end))
            {
                _previous_reports.Add(_report);
            }
        }

        // merge with previous
        List<Int32> _report_ids = _all_reports.Select(t => t.id).ToList();
        List<Int32> _previous_report_ids = _previous_reports.Select(t => t.id).ToList();

        DataTable dt = dtAnalyticsTable();


        List<company_relationship> _c_relationships = db.company_relationship.Where(item => item.company_id == company_id).ToList();

        foreach (company_relationship _temp_relationships in _c_relationships)
        {
            //int prev_count = 0;
            int count = db.report_relationship.Where(item => ((item.company_relationship_id == _temp_relationships.id) && (_report_ids.Contains(item.report_id)))).Count();
            int prev_count = db.report_relationship.Where(item => ((item.company_relationship_id == _temp_relationships.id) && (_previous_report_ids.Contains(item.report_id)))).Count();

            if (count > 0)
            {
                DataRow dr;
                //  prev_count = _previous_reports.Where(item => (item.type_id == _type.id)).Count();
                dr = dt.NewRow();
                dr["name"] = _temp_relationships.relationship_en;
                dr["value"] = count;
                dr["prev"] = prev_count;
                dt.Rows.Add(dr);
            }
        }


        List<report_relationship> _all_types = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id)) && ((item.company_relationship_id == null || item.company_relationship_id.Value == 0 || item.company_relationship_id.Value == -1))).ToList();

        foreach (report_relationship _type in _all_types)
        {
            string _temp_relationship_nm = _type.relationship_nm;
            bool is_in_dt = false;
            int row_num = 0;
            int i = 0;

            int _previous = 0;
            if (_previous_report_ids.Contains(_type.report_id))
                _previous = 1;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["name"].ToString().ToLower().Trim() == _temp_relationship_nm.ToLower().Trim())
                {
                    is_in_dt = true;
                    row_num = i;
                }
                i++;
            }

            if (!is_in_dt)
            {
                //  prev_count = _previous_reports.Where(item => (item.type_id == _type.id)).Count();
                DataRow dr = dt.NewRow();
                dr["name"] = _temp_relationship_nm;
                dr["value"] = 1;
                dr["prev"] = _previous;
                dt.Rows.Add(dr);

            }
            else
            {
                DataRow dr = dt.Rows[row_num];
                dr["value"] = Convert.ToInt32(dr["value"]) + 1;
                dr["prev"] = Convert.ToInt32(dr["prev"]) + _previous;
            }
        }


        return dt;
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
        Dictionary<int, string> Monthes = FullMonth();
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

    //done
    public DataTable CompanyLocationReport(int company_id, int user_id)
    {
        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();
        List<report> _all_reports = um.ReportsSearch(company_id, 0);
        DataTable dt = dtDoughnutTable();

        DataRow dr;
        bool is_in_table = false;
        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);
            is_in_table = false;

            foreach (DataRow _dr in dt.Rows)
            {
                if (_dr["name"].ToString().ToLower().Trim() == rm._location_string.ToLower().Trim())
                {
                    _dr["val"] = Convert.ToInt32(_dr["val"]) + 1;
                    is_in_table = true;
                }
            }
            if (!is_in_table)
            {
                dr = dt.NewRow();
                dr["name"] = rm._location_string.Trim();
                dr["val"] = 1;
                dt.Rows.Add(dr);
            }
        }
        return dt;
    }

    //done
    public DataTable CompanyDepartmentReport(int company_id, int user_id)
    {
        UserModel um = new UserModel(user_id);
        List<report> _all_reports = um.ReportsSearch(company_id, 0);
        DataTable dt = dtDoughnutTable();
        List<string> _dep_names = new List<string>();

        DataRow dr;
        bool is_in_table = false;
        company_department temp_c_dep;
        bool one_department_added = false;


        foreach (report _report in _all_reports)
        {
            one_department_added = false;
            List<report_department> _c_departments = db.report_department.Where(item => item.report_id == _report.id).ToList();

            foreach (report_department _temp_dep in _c_departments)
            {
                temp_c_dep = db.company_department.Where(item => item.id == _temp_dep.department_id).FirstOrDefault();
                if (temp_c_dep != null)
                {
                    one_department_added = true;
                    _dep_names.Add(temp_c_dep.department_en.Trim());
                }
            }

            if (_report.other_department_name.Trim().Length > 0)
            {
                _dep_names.Add(_report.other_department_name.Trim());
                one_department_added = true;
            }

            if (!one_department_added)
            { 
                _dep_names.Add(GlobalRes.unknown_departments);
            }
        }
        foreach (string _department in _dep_names)
        {
            is_in_table = false;
            foreach (DataRow _dr in dt.Rows)
            {
                if (_dr["name"].ToString().ToLower().Trim() == _department.ToLower().Trim())
                {
                    _dr["val"] = Convert.ToInt32(_dr["val"]) + 1;
                    is_in_table = true;
                }
            }
            if (!is_in_table)
            {
                dr = dt.NewRow();
                dr["name"] = _department.Trim();
                dr["val"] = 1;
                dt.Rows.Add(dr);
            }
        }
        return dt;
    }


    #endregion


    #region Analytics Helpers - new version
  
    public DataTable CompanyDepartmentReportAdvanced(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //List<string> result = names.Split(',').ToList();
        UserModel um = new UserModel(user_id);
        List<report> _all_reports_old = um.ReportsSearch(company_id, 0);
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
            if ((dtReportCreationStartDate.HasValue) && (dtReportCreationStartDate.Value.Date >= _report.reported_dt.Date))
                _flag5 = false;
            if ((dtReportCreationEndDate.HasValue) && (dtReportCreationEndDate.Value.Date <= _report.reported_dt.Date))
                _flag6 = false;
            if (_flag1 & _flag2 && _flag3 & _flag4 && _flag5 & _flag6)
                _all_reports.Add(_report);
        }
        #endregion

        DataTable dt = dtDoughnutTable();
        List<string> _dep_names = new List<string>();

        DataRow dr;
        bool is_in_table = false;
        company_department temp_c_dep;
        bool one_department_added = false;


        foreach (report _report in _all_reports)
        {
            one_department_added = false;
            List<report_department> _c_departments = db.report_department.Where(item => item.report_id == _report.id).ToList();

            foreach (report_department _temp_dep in _c_departments)
            {
                temp_c_dep = db.company_department.Where(item => item.id == _temp_dep.department_id).FirstOrDefault();
                if (temp_c_dep != null)
                {
                    one_department_added = true;
                    _dep_names.Add(temp_c_dep.department_en.Trim());
                }
            }

            if (_report.other_department_name.Trim().Length > 0)
            {
                _dep_names.Add(_report.other_department_name.Trim());
                one_department_added = true;
            }

            if (!one_department_added)
            {
                _dep_names.Add(GlobalRes.unknown_departments);
            }
        }
        foreach (string _department in _dep_names)
        {
            is_in_table = false;
            foreach (DataRow _dr in dt.Rows)
            {
                if (_dr["name"].ToString().ToLower().Trim() == _department.ToLower().Trim())
                {
                    _dr["val"] = Convert.ToInt32(_dr["val"]) + 1;
                    is_in_table = true;
                }
            }
            if (!is_in_table)
            {
                dr = dt.NewRow();
                dr["name"] = _department.Trim();
                dr["val"] = 1;
                dt.Rows.Add(dr);
            }
        }
        return dt;
    }


    public DataTable CompanyLocationReportAdvanced(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //List<string> result = names.Split(',').ToList();
        UserModel um = new UserModel(user_id);
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
            if ((dtReportCreationStartDate.HasValue) && (dtReportCreationStartDate.Value.Date >= _report.reported_dt.Date))
                _flag5 = false;
            if ((dtReportCreationEndDate.HasValue) && (dtReportCreationEndDate.Value.Date <= _report.reported_dt.Date))
                _flag6 = false;
            if (_flag1 & _flag2 && _flag3 & _flag4 && _flag5 & _flag6)
                _all_reports.Add(_report);
        }
        #endregion
    
        ReportModel rm = new ReportModel();

        DataTable dt = dtDoughnutTable();

        DataRow dr;
        bool is_in_table = false;
        foreach (report _report in _all_reports)
        {
            rm = new ReportModel(_report.id);
            is_in_table = false;

            foreach (DataRow _dr in dt.Rows)
            {
                if (_dr["name"].ToString().ToLower().Trim() == rm._location_string.ToLower().Trim())
                {
                    _dr["val"] = Convert.ToInt32(_dr["val"]) + 1;
                    is_in_table = true;
                }
            }
            if (!is_in_table)
            {
                dr = dt.NewRow();
                dr["name"] = rm._location_string.Trim();
                dr["val"] = 1;
                dt.Rows.Add(dr);
            }
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

    public DataTable RelationshipToCompanyByDateAdvanced(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //List<string> result = names.Split(',').ToList();
        UserModel um = new UserModel(user_id);
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
            if ((dtReportCreationStartDate.HasValue) && (dtReportCreationStartDate.Value.Date >= _report.reported_dt.Date))
                _flag5 = false;
            if ((dtReportCreationEndDate.HasValue) && (dtReportCreationEndDate.Value.Date <= _report.reported_dt.Date))
                _flag6 = false;
            if (_flag1 & _flag2 && _flag3 & _flag4 && _flag5 & _flag6)
                _all_reports.Add(_report);
        }
        #endregion


        DateTime _real_start, _real_end;

        _real_start = new DateTime(2015, 1, 1);
        _real_end = DateTime.Today.AddDays(1);
        DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);

        ReportModel rm = new ReportModel();

        List<relationship> all_relationships = new List<relationship>();
        all_relationships = (db.relationship).ToList();

        List<report> _selected_reports = new List<report>();
        List<report> _previous_reports = new List<report>();

        foreach (report _report in _all_reports)
        {
            if ((_report.reported_dt >= _real_start) && (_report.reported_dt <= _real_end))
            {
                _selected_reports.Add(_report);
            }
        }

        // getting the data for previous month
        _real_end = _month_end_date;
        foreach (report _report in _all_reports)
        {
            if ((_report.reported_dt >= _real_start) && (_report.reported_dt <= _real_end))
            {
                _previous_reports.Add(_report);
            }
        }

        // merge with previous
        List<Int32> _report_ids = _all_reports.Select(t => t.id).ToList();
        List<Int32> _previous_report_ids = _previous_reports.Select(t => t.id).ToList();

        DataTable dt = dtAnalyticsTable();


        List<company_relationship> _c_relationships = db.company_relationship.Where(item => item.company_id == company_id).ToList();

        foreach (company_relationship _temp_relationships in _c_relationships)
        {
            //int prev_count = 0;
            int count = db.report_relationship.Where(item => ((item.company_relationship_id == _temp_relationships.id) && (_report_ids.Contains(item.report_id)))).Count();
            int prev_count = db.report_relationship.Where(item => ((item.company_relationship_id == _temp_relationships.id) && (_previous_report_ids.Contains(item.report_id)))).Count();

            if (count > 0)
            {
                DataRow dr;
                //  prev_count = _previous_reports.Where(item => (item.type_id == _type.id)).Count();
                dr = dt.NewRow();
                dr["name"] = _temp_relationships.relationship_en;
                dr["value"] = count;
                dr["prev"] = prev_count;
                dt.Rows.Add(dr);
            }
        }


        List<report_relationship> _all_types = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id)) && ((item.company_relationship_id == null || item.company_relationship_id.Value == 0 || item.company_relationship_id.Value == -1))).ToList();

        foreach (report_relationship _type in _all_types)
        {
            string _temp_relationship_nm = _type.relationship_nm;
            bool is_in_dt = false;
            int row_num = 0;
            int i = 0;

            int _previous = 0;
            if (_previous_report_ids.Contains(_type.report_id))
                _previous = 1;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["name"].ToString().ToLower().Trim() == _temp_relationship_nm.ToLower().Trim())
                {
                    is_in_dt = true;
                    row_num = i;
                }
                i++;
            }

            if (!is_in_dt)
            {
                //  prev_count = _previous_reports.Where(item => (item.type_id == _type.id)).Count();
                DataRow dr = dt.NewRow();
                dr["name"] = _temp_relationship_nm;
                dr["value"] = 1;
                dr["prev"] = _previous;
                dt.Rows.Add(dr);

            }
            else
            {
                DataRow dr = dt.Rows[row_num];
                dr["value"] = Convert.ToInt32(dr["value"]) + 1;
                dr["prev"] = Convert.ToInt32(dr["prev"]) + _previous;
            }
        }


        return dt;
    }

    public DataTable SecondaryTypesByDateAdvanced(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //List<string> result = names.Split(',').ToList();
        UserModel um = new UserModel(user_id);
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
            if ((dtReportCreationStartDate.HasValue) && (dtReportCreationStartDate.Value.Date >= _report.reported_dt.Date))
                _flag5 = false;
            if ((dtReportCreationEndDate.HasValue) && (dtReportCreationEndDate.Value.Date <= _report.reported_dt.Date))
                _flag6 = false;
            if (_flag1 & _flag2 && _flag3 & _flag4 && _flag5 & _flag6)
                _all_reports.Add(_report);
        }
        #endregion
        DateTime _real_start, _real_end;

        _real_start = new DateTime(2015, 1, 1);
        _real_end = DateTime.Today.AddDays(1);
        DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);

        ReportModel rm = new ReportModel();
        List<secondary_type_mandatory> all_types = new List<secondary_type_mandatory>();
        all_types = (db.secondary_type_mandatory).ToList();

        List<report> _selected_reports = new List<report>();
        List<report> _previous_reports = new List<report>();

        foreach (report _report in _all_reports)
        {
            if ((_report.reported_dt >= _real_start) && (_report.reported_dt <= _real_end))
            {
                _selected_reports.Add(_report);
            }
        }

        // getting the data for previous month
        _real_end = _month_end_date;
        foreach (report _report in _all_reports)
        {
            if ((_report.reported_dt >= _real_start) && (_report.reported_dt <= _real_end))
            {
                _previous_reports.Add(_report);
            }
        }
        // merge with previous
        List<Int32> _report_ids = _all_reports.Select(t => t.id).ToList();
        List<Int32> _previous_report_ids = _previous_reports.Select(t => t.id).ToList();

        DataTable dt = dtAnalyticsTable();

        List<company_secondary_type> _c_secondary_types = db.company_secondary_type.Where(item => item.company_id == company_id).ToList();

        foreach (company_secondary_type _temp_secondary_types in _c_secondary_types)
        {
            //int prev_count = 0;
            int count = db.report_secondary_type.Where(item => ((item.secondary_type_id == _temp_secondary_types.id) && (_report_ids.Contains(item.report_id)))).Count();
            int prev_count = db.report_secondary_type.Where(item => ((item.secondary_type_id == _temp_secondary_types.id) && (_previous_report_ids.Contains(item.report_id)))).Count();

            if (count > 0)
            {
                DataRow dr;
                //  prev_count = _previous_reports.Where(item => (item.type_id == _type.id)).Count();
                dr = dt.NewRow();
                dr["name"] = _temp_secondary_types.secondary_type_en;
                dr["value"] = count;
                dr["prev"] = prev_count;
                dt.Rows.Add(dr);
            }
        }

        List<report_secondary_type> _all_types = db.report_secondary_type.Where(item => (_report_ids.Contains(item.report_id)) && ((item.secondary_type_id == 0 || item.secondary_type_id == -1))).ToList();

        foreach (report_secondary_type _type in _all_types)
        {
            string _temp_secondary_type_nm = _type.secondary_type_nm;
            bool is_in_dt = false;
            int row_num = 0;
            int i = 0;

            int _previous = 0;
            if (_previous_report_ids.Contains(_type.report_id))
                _previous = 1;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["name"].ToString().ToLower().Trim() == _temp_secondary_type_nm.ToLower().Trim())
                {
                    is_in_dt = true;
                    row_num = i;
                }
                i++;
            }

            if (!is_in_dt)
            {
                //  prev_count = _previous_reports.Where(item => (item.type_id == _type.id)).Count();
                DataRow dr = dt.NewRow();
                dr["name"] = _temp_secondary_type_nm;
                dr["value"] = 1;
                dr["prev"] = _previous;
                dt.Rows.Add(dr);

            }
            else
            {
                DataRow dr = dt.Rows[row_num];
                dr["value"] = Convert.ToInt32(dr["value"]) + 1;
                dr["prev"] = Convert.ToInt32(dr["prev"]) + _previous;
            }
        }

        return dt;
    }

    public int[] AnalyticsByDateAdvanced(DateTime? _start, DateTime? _end, int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        //List<string> result = names.Split(',').ToList();
        UserModel um = new UserModel(user_id);
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

            if ((rm._last_promoted_date >= _real_start) && (rm._last_promoted_date <= _real_end))
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


            dr = AnalyticsTimeLineRowAdvanced(_start, company_id, user_id,  ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
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
        Dictionary<int, string> Monthes = FullMonth();
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

    public string ReportAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        int[] _today_spanshot = AnalyticsByDateAdvanced(null, null, company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

        DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
        int[] _month_end_spanshot = AnalyticsByDateAdvanced(null, _month_end_date, company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate) ;



        string _all_json = "{\"LocationTable\":" + CompanyLocationReportAdvancedJson(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            + ", \"DepartmentTable\":" + CompanyDepartmentReportAdvancedJson(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            + ", \"RelationTable\":" + RelationshipToCompanyByDateAdvancedJson(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            + ", \"SecondaryTypeTable\":" + SecondaryTypesByDateAdvancedJson(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            + ", \"AverageStageDaysTable\":" + AverageStageDaysAdvancedJson(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            + ", \"TodaySnapshotTable\":" + ListToJsonWithJavaScriptSerializer(new List<int>(_today_spanshot))
            + ", \"MonthEndSnapshotTable\":" + ListToJsonWithJavaScriptSerializer(new List<int>(_month_end_spanshot))
            + ", \"AnalyticsTimeline\":" + AnalyticsTimelineAdvancedJson(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate)
            + "}";

       return _all_json;
    }



    public string CompanyLocationReportAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = CompanyLocationReportAdvanced(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return DataTableToJSONWithJavaScriptSerializer(dt);
    }

    public string CompanyDepartmentReportAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = CompanyDepartmentReportAdvanced(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return DataTableToJSONWithJavaScriptSerializer(dt);
    }

    public string RelationshipToCompanyByDateAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = RelationshipToCompanyByDateAdvanced(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return DataTableToJSONWithJavaScriptSerializer(dt);
    }

    
    public string SecondaryTypesByDateAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = SecondaryTypesByDateAdvanced(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return DataTableToJSONWithJavaScriptSerializer(dt);
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
                _json += "{ \"name\": \"Pre-Review\",\"val\":" + _array[i].ToString()+ "},";
            }
            else if (i == 1)
            {
                _json += "{ \"name\": \"Review\",\"val\":" + _array[i].ToString() + "},";
            }
            else if (i == 2)
            {
                _json += "{ \"name\": \"Investigation\",\"val\":" + _array[i].ToString() + "},";
            }
            else if (i == 3)
            {
                _json += "{ \"name\": \"Resolution\",\"val\":" + _array[i].ToString() + "},";
            }
            else if (i == 4)
            {
                _json += "{ \"name\": \"Escalation\",\"val\":" + _array[i].ToString() + "}";
            }
        }
        _json += "]";
        return _json;
        
        //return DataTableToJSONWithJavaScriptSerializer(dt);
    }

    public string AnalyticsTimelineAdvancedJson(int company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
    {
        DataTable dt = AnalyticsTimelineAdvanced(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);
        return DataTableToJSONWithJavaScriptSerializer(dt);
    }
    
    
    public string ListToJsonWithJavaScriptSerializer(List<int> _list)
    {
      //  for(int i )
        
      //  int[]


        JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

        var json = jsSerializer.Serialize(_list);
        return json;

      /*  List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
        Dictionary<string, object> childRow;
        for (int i=0; i< _list.Count(); i++)
        {
            childRow = new Dictionary<string, object>();
            foreach (DataColumn col in table.Columns)
            {
                childRow.Add(col.ColumnName, row[col]);
            }
            parentRow.Add(childRow);
        }
        return jsSerializer.Serialize(parentRow);*/
    }

    public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
    {
        JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
        Dictionary<string, object> childRow;
        foreach (DataRow row in table.Rows)
        {
            childRow = new Dictionary<string, object>();
            foreach (DataColumn col in table.Columns)
            {
                childRow.Add(col.ColumnName, row[col]);
            }
            parentRow.Add(childRow);
        }
        return jsSerializer.Serialize(parentRow);
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

    #region Analytics Helpers List

    /// <summary>
    /// 
    /// </summary>
    /// <param name="company_id"></param>
    /// <param name="user_id"></param>
    /// <returns></returns>
    public List<Tuple<string, string>> SecondaryTypesListDistinct(int company_id, int user_id)
    {
        /// string - name, int - id, int -1 - primary, 2- secondary, 3 - 'Other', bool - to merge
        List<Tuple<string,int,int, bool>> _list_types = new List<Tuple<string,int,int, bool>>();

        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<Int32> _report_ids = um.ReportsSearchIds(company_id, 0);

        List<int> mandatory_types_ids = new List<int>();
        List<int> secondary_types_ids = new List<int>();
        List<string> other_types_names = new List<string>();

        List<report_secondary_type> _all_secondary_types_for_company = db.report_secondary_type.Where(item => (_report_ids.Contains(item.report_id))).ToList();
        foreach (report_secondary_type _secondary_type in _all_secondary_types_for_company)
        {
         ////   _list_types.Add(new Tuple<string, int, int, bool>(_secondary_type.secondary_type_nm, _secondary_type.secondary_type_id, 1, true));
            if ((_secondary_type.secondary_type_nm != null) && (_secondary_type.secondary_type_nm.Trim() != ""))
            {
                other_types_names.Add(_secondary_type.secondary_type_nm.Trim());
                _list_types.Add(new Tuple<string, int, int, bool>(_secondary_type.secondary_type_nm, _secondary_type.report_id, 3, true));
            }
            else
            {
                if ((_secondary_type.mandatory_secondary_type_id != null) && (_secondary_type.mandatory_secondary_type_id != 0))
                {
                    mandatory_types_ids.Add(_secondary_type.mandatory_secondary_type_id.Value);
                }
                else if ((_secondary_type.secondary_type_id != -1) && (_secondary_type.secondary_type_id != 0))
                {
                    secondary_types_ids.Add(_secondary_type.secondary_type_id);
                }
            }
        }

        other_types_names = other_types_names.Distinct().ToList();
        mandatory_types_ids = mandatory_types_ids.Distinct().ToList();
        secondary_types_ids = secondary_types_ids.Distinct().ToList();


        /////mandatory
        List<secondary_type_mandatory> _all_mandatory_types = db.secondary_type_mandatory.Where(item => (mandatory_types_ids.Contains(item.id))).ToList();
        foreach (secondary_type_mandatory _temp_secondary_type_mandatory in _all_mandatory_types)
        {
       ///     _list_types.Add(new Tuple<string, int, int, bool>(_temp_secondary_type_mandatory.secondary_type_en, _temp_secondary_type_mandatory.id, 1, true));
            _list_types.Add(new Tuple<string, int, int, bool>(_temp_secondary_type_mandatory.secondary_type_en, _temp_secondary_type_mandatory.id, 1, true));

        }
        /////company_secondary_type
        List<company_secondary_type> _all_secondary_types = db.company_secondary_type.Where(item => (secondary_types_ids.Contains(item.id))).ToList();
        foreach (company_secondary_type _temp_company_secondary_type in _all_secondary_types)
        {
        ///    _list_types.Add(new Tuple<string, int, int, bool>(_temp_company_secondary_type.secondary_type_en, _temp_company_secondary_type.id, 2, true));
            _list_types.Add(new Tuple<string, int, int, bool>(_temp_company_secondary_type.secondary_type_en, _temp_company_secondary_type.id, 2, true));

        }


        List<Tuple<string, List<int>>> _secondary_types = new List<Tuple<string, List<int>>>();

        for (int i = 0; i < _list_types.Count; i++)
        {
            Tuple<string, int, int, bool> _current_item = _list_types[i];
            bool is_in_list = false;

            List<int> temp_list = new List<int>();
            int temp_index = -1;
            for (int j = 0; j < _secondary_types.Count; j++ )
            {
                Tuple<string, List<int>> _temp_item = _secondary_types[j];
                if (_temp_item.Item1.Trim().ToLower() == _current_item.Item1.Trim().ToLower())
                {
                    temp_index = j;
                    temp_list = _temp_item.Item2;
                    is_in_list = true;
                }
            }
            if (!is_in_list)
            {
                if (_current_item.Item3 == 3)
                {
                    //other
                    if (!temp_list.Contains(_current_item.Item2))
                        temp_list.Add(_current_item.Item2);
                    _secondary_types.Add(new Tuple<string, List<int>>(_current_item.Item1, temp_list));
                }

               
            }
            else
            {
                Tuple<string, List<int>> _temp_item = _secondary_types[temp_index];
                if (_current_item.Item3 == 3)
                {
                    temp_list.Add(_current_item.Item2);
                }
                _temp_item = new Tuple<string, List<int>>(_temp_item.Item1, temp_list);
                _secondary_types[temp_index] = _temp_item;
            }




            if (_current_item.Item3 == 2)
            {
                //secondary
                List<report_secondary_type> secondary_types_in_reports = db.report_secondary_type.Where(item => (_report_ids.Contains(item.report_id) && item.secondary_type_id == _current_item.Item2 && item.secondary_type_nm.Trim() == "")).ToList();
                for (int k = 0; k < secondary_types_in_reports.Count; k++)
                {
                    report_secondary_type _temp_secondary_types_in_reports = secondary_types_in_reports[k];
                    if (!temp_list.Contains(_temp_secondary_types_in_reports.report_id))
                        temp_list.Add(_temp_secondary_types_in_reports.report_id);
                }
                if (is_in_list)
                {
                    _secondary_types[temp_index] = new Tuple<string, List<int>>(_secondary_types[temp_index].Item1, temp_list);
                }
                else
                {
                    _secondary_types.Add(new Tuple<string, List<int>>(_current_item.Item1, temp_list));
                }
            }
            else if (_current_item.Item3 == 1)
            {
                //primary
                List<report_secondary_type> mandatory_types_in_reports = db.report_secondary_type.Where(item => (_report_ids.Contains(item.report_id) && item.mandatory_secondary_type_id == _current_item.Item2 && item.secondary_type_nm.Trim() == "" && item.secondary_type_id == -1 && item.secondary_type_id == 0)).ToList();
                for (int k = 0; k < mandatory_types_in_reports.Count; k++)
                {
                    report_secondary_type _temp_secondary_types_in_reports = mandatory_types_in_reports[k];
                    if (!temp_list.Contains(_temp_secondary_types_in_reports.report_id))
                        temp_list.Add(_temp_secondary_types_in_reports.report_id);
                }

                if (is_in_list)
                {
                    _secondary_types[temp_index] = new Tuple<string, List<int>>(_secondary_types[temp_index].Item1, temp_list);
                }
                else
                {
                    _secondary_types.Add(new Tuple<string, List<int>>(_current_item.Item1, temp_list));
                }
   
            }
            
        }


        List<Tuple<string, string>> return_array = new List<Tuple<string, string>>();
        for (int i = 0; i < _secondary_types.Count; i++)
        {
            Tuple<string, List<int>> _current_item = _secondary_types[i];
            return_array.Add(new Tuple<string, string>(_current_item.Item1, string.Join(",", _current_item.Item2.ToArray())));
        }
        return return_array;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="company_id"></param>
    /// <param name="user_id"></param>
    /// <returns></returns>
    public List<Tuple<string, string>> RelationTypesListDistinct(int company_id, int user_id)
    {
        /// string - name, int - id, int -1 - primary, 2- secondary, 3 - 'Other', bool - to merge
        List<Tuple<string, int, int, bool>> _list_types = new List<Tuple<string, int, int, bool>>();

        UserModel um = new UserModel(user_id);
        ReportModel rm = new ReportModel();

        List<Int32> _report_ids = um.ReportsSearchIds(company_id, 0);

        List<int> mandatory_relation_ids = new List<int>();
        List<int> company_relation_ids = new List<int>();
        List<string> other_relation_names = new List<string>();

        List<report_relationship> _all_relationships_for_company = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id))).ToList();
        foreach (report_relationship _relationship in _all_relationships_for_company)
        {
            ////   _list_types.Add(new Tuple<string, int, int, bool>(_secondary_type.secondary_type_nm, _secondary_type.secondary_type_id, 1, true));
            if ((_relationship.relationship_nm != null) && (_relationship.relationship_nm.Trim() != "") && ((_relationship.relationship_id == null) || (_relationship.relationship_id == -1) || (_relationship.relationship_id == 0)) && ((_relationship.company_relationship_id == null) || (_relationship.company_relationship_id == -1) || (_relationship.company_relationship_id == 0)))
            {
                other_relation_names.Add(_relationship.relationship_nm.Trim());
                _list_types.Add(new Tuple<string, int, int, bool>(_relationship.relationship_nm, _relationship.report_id, 3, true));
            }
            else
            {
                if ((_relationship.relationship_id != null) && (_relationship.relationship_id != 0))
                {
                    mandatory_relation_ids.Add(_relationship.relationship_id.Value);
                }
                else if ((_relationship.company_relationship_id.HasValue) && (_relationship.company_relationship_id != -1) && (_relationship.company_relationship_id != 0))
                {
                    company_relation_ids.Add(_relationship.company_relationship_id.Value);
                }
            }
        }

        other_relation_names = other_relation_names.Distinct().ToList();
        mandatory_relation_ids = mandatory_relation_ids.Distinct().ToList();
        company_relation_ids = company_relation_ids.Distinct().ToList();

        /////mandatory
        List<relationship> _all_mandatory_relationships = db.relationship.Where(item => (mandatory_relation_ids.Contains(item.id))).ToList();
        foreach (relationship _temp_relationship_mandatory in _all_mandatory_relationships)
        {
            ///     _list_types.Add(new Tuple<string, int, int, bool>(_temp_secondary_type_mandatory.secondary_type_en, _temp_secondary_type_mandatory.id, 1, true));
            _list_types.Add(new Tuple<string, int, int, bool>(_temp_relationship_mandatory.relationship_en, _temp_relationship_mandatory.id, 1, true));

        }
        /////company_secondary_type
        List<company_relationship> _all_secondary_relationships = db.company_relationship.Where(item => (company_relation_ids.Contains(item.id))).ToList();
        foreach (company_relationship _temp_company_relationship in _all_secondary_relationships)
        {
            ///    _list_types.Add(new Tuple<string, int, int, bool>(_temp_company_secondary_type.secondary_type_en, _temp_company_secondary_type.id, 2, true));
            _list_types.Add(new Tuple<string, int, int, bool>(_temp_company_relationship.relationship_en, _temp_company_relationship.id, 2, true));

        }


        List<Tuple<string, List<int>>> _relationships = new List<Tuple<string, List<int>>>();

        for (int i = 0; i < _list_types.Count; i++)
        {
            Tuple<string, int, int, bool> _current_item = _list_types[i];
            bool is_in_list = false;

            List<int> temp_list = new List<int>();
            int temp_index = -1;
            for (int j = 0; j < _relationships.Count; j++)
            {
                Tuple<string, List<int>> _temp_item = _relationships[j];
                if (_temp_item.Item1.Trim().ToLower() == _current_item.Item1.Trim().ToLower())
                {
                    temp_index = j;
                    temp_list = _temp_item.Item2;
                    is_in_list = true;
                }
            }
            if (!is_in_list)
            {
                if (_current_item.Item3 == 3)
                {
                    //other
                    if (!temp_list.Contains(_current_item.Item2))
                        temp_list.Add(_current_item.Item2);
                    _relationships.Add(new Tuple<string, List<int>>(_current_item.Item1, temp_list));
                }


            }
            else
            {
                Tuple<string, List<int>> _temp_item = _relationships[temp_index];
                if (_current_item.Item3 == 3)
                {
                    temp_list.Add(_current_item.Item2);
                }
                _temp_item = new Tuple<string, List<int>>(_temp_item.Item1, temp_list);
                _relationships[temp_index] = _temp_item;
            }




            if (_current_item.Item3 == 2)
            {
                //secondary
                List<report_relationship> secondary_relationships_in_reports = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id) && item.company_relationship_id == _current_item.Item2)).ToList();
                for (int k = 0; k < secondary_relationships_in_reports.Count; k++)
                {
                    report_relationship _temp_secondary_types_in_reports = secondary_relationships_in_reports[k];
                    if (!temp_list.Contains(_temp_secondary_types_in_reports.report_id))
                        temp_list.Add(_temp_secondary_types_in_reports.report_id);
                }
                if (is_in_list)
                {
                    _relationships[temp_index] = new Tuple<string, List<int>>(_relationships[temp_index].Item1, temp_list);
                }
                else
                {
                    _relationships.Add(new Tuple<string, List<int>>(_current_item.Item1, temp_list));
                }
            }
            else if (_current_item.Item3 == 1)
            {
                //primary
                List<report_relationship> mandatory_relationships_in_reports = db.report_relationship.Where(item => (_report_ids.Contains(item.report_id) && item.relationship_id == _current_item.Item2  && item.company_relationship_id == -1 && item.company_relationship_id == 0)).ToList();
                for (int k = 0; k < mandatory_relationships_in_reports.Count; k++)
                {
                    report_relationship _temp_secondary_types_in_reports = mandatory_relationships_in_reports[k];
                    if (!temp_list.Contains(_temp_secondary_types_in_reports.report_id))
                        temp_list.Add(_temp_secondary_types_in_reports.report_id);
                }

                if (is_in_list)
                {
                    _relationships[temp_index] = new Tuple<string, List<int>>(_relationships[temp_index].Item1, temp_list);
                }
                else
                {
                    _relationships.Add(new Tuple<string, List<int>>(_current_item.Item1, temp_list));
                }
            }
        }


        List<Tuple<string, string>> return_array = new List<Tuple<string, string>>();
        for (int i = 0; i < _relationships.Count; i++)
        {
            Tuple<string, List<int>> _current_item = _relationships[i];
            return_array.Add(new Tuple<string, string>(_current_item.Item1, string.Join(",", _current_item.Item2.ToArray())));
        }
        return return_array;
    }

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
            List<int> report_ids_by_dept_id = db.report_department.Where(item => (_report_ids.Contains(item.report_id) && item.department_id == _temp_company_department.id)).Select(item=>item.report_id).ToList();
            return_array.Add(new Tuple<string, string>(_temp_company_department.department_en, string.Join(",", report_ids_by_dept_id.ToArray())));
        }

        return return_array;
    }

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
        /////company_departments


        return return_array;
    }
    #endregion
    public string ConvertDataTabletoString(DataTable dt)
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row;
        foreach (DataRow dr in dt.Rows)
        {
            row = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                row.Add(col.ColumnName, dr[col]);
            }
            rows.Add(row);
        }
        return serializer.Serialize(rows);
    }

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
    public string GenerateLoginName(string first, string last)
    {
        Regex rgx = new Regex("[^a-zA-Z]");


        first = rgx.Replace(first, "");
        last = rgx.Replace(last, "");

        first = first.Replace(" ", "");
        last = last.Replace(" ", "");

        string _first_short = "";
        string _last_short = "";

        if(first.Length > 0)
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

        _login_text_part = (_login_text_part + RandomLetter(6 - _login_text_part.Length)).ToLower();
        _login_text_part = ReplaceForUI(_login_text_part);

        string _login_int_part = "";

        do
        {
            var random = new Random();
            _login_int_part = random.Next(10, 99).ToString();
            _login_int_part = ReplaceForUI(_login_int_part);

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

        _first_short = (_first_short + RandomLetter(3 - _first_short.Length)).ToUpper().Trim();

        _first_short = ReplaceForUI(_first_short);


        do
        {
            var random = new Random();
            _last_short = random.Next(1001, 9999).ToString();
            _last_short = ReplaceForUI(_last_short);
        }
        while (isCodeInUse(_first_short + _last_short));

        return _first_short + _last_short;
    }

    public string ReplaceForUI(string text)
    {
        string better_text = text.Trim();

        better_text = better_text.Replace('O', 'K');
        better_text = better_text.Replace('o', 'K');


        better_text = better_text.Replace('1', '8');
        better_text = better_text.Replace('0', '5');

        better_text = better_text.Replace('I', 'A');
        better_text = better_text.Replace('i', 'a');

        better_text = better_text.Replace('J', 'W');
        better_text = better_text.Replace('j', 'w');

        return better_text;
    }
    public bool isCodeInUse(string code)
    {
        if (db.company.Any(t => t.company_code.Trim().ToLower() == code.Trim().ToLower()))
            return true;
        else
            return false;
    }
    public static string RandomLetter(int length)
    {
        const string chars = "ABCDEFGHKLMNPQRSTUVWXYZ";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string IsValidPass(string password)
    {
        if(password!= "" && password.Length >= PasswordLength)
        {
            return "Success";
        }
        return "The password should be at least 6 characters long";
    }
    public static string GetUser_IP()
    {
        string VisitorsIPAddr = string.Empty;
        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
        {
            VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        }
        else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
        {
            VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
        }
        return VisitorsIPAddr;
    }

    /// <summary>
    /// Feb dd, yyyy
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public string ConvertDateToShortString(DateTime dt)
    {
        string sDate = "";
        string Month = GetShortMonth(dt.Month);
        string Day = dt.Day.ToString();
        if (Month.Length == 1)
            Month = "0" + Month;
        if (Day.Length == 1)
            Day = "0" + Day;
        //sDate = Month + " " + Day + ", " + dt.Year.ToString();
        sDate = Month + " " + Day + ", " + dt.Year.ToString();
        return sDate;
    }

    /// <summary>
    /// February 2, 2015
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public string ConvertDateToLongMonthString(DateTime dt)
    {
        string sDate = "";
        string Month = "";// GetFullMonth(dt.Month);
        string Day = dt.Day.ToString();
        if (Month.Length == 1)
            Month = "0" + Month;
        if (Day.Length == 1)
            Day = "0" + Day;
        //sDate = Month + " " + Day + ", " + dt.Year.ToString();
        sDate = Month + " " + Day + ", " + dt.Year.ToString();
        return sDate;
    }

       

}
