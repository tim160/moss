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
namespace EC.Models
{
  public class LogModel : BaseModel
  {
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
  }
}