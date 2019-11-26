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

   
    public static string IsValidPass(string password)
    {
        if (password != "" && password.Length >= PasswordConstants.PASSWORD_MIN_LENGTH)
        {
            return "Success";
        }
        return String.Format("The password should be at least {0} characters long", PasswordConstants.PASSWORD_MIN_LENGTH.ToString());
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
