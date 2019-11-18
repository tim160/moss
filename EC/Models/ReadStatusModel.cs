using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Common.Interfaces;
using log4net;

namespace EC.Models
{
  public class ReadStatusModel
  {

    ECEntities db = new ECEntities();
///    IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();


    ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
  }
}