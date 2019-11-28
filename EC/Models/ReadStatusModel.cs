using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Common.Interfaces;
using EC.Models.ViewModels;
using EC.Models.ECModel;
using log4net;

namespace EC.Models
{
  public class ReadStatusModel
  {

    ECEntities db = new ECEntities();
    ///  _CaseStatus --- UnreadActivityUserTaskQuantity    IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();


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


    public int Unread_Messages_Quantity(int user_id, int? report_id, int thread_id)
    {

      UserModel um = new UserModel(user_id);
      int quantity = 0;
      List<message> all_messages = um.UserMessages(report_id, thread_id);
      MessageExtended _extended;
      foreach (message _message in all_messages)
      {
        _extended = new MessageExtended(_message.id, user_id);

        // is unread
        if (!_extended.IsRead())
          quantity++;
      }
      return quantity;
    }

    public UsersUnreadEntitiesNumberViewModel GetUserUnreadEntitiesNumbers(int user_id)
    {
      UserModel um = new UserModel(user_id);
      List<int> all_reports_id = um.GetReportIds(null);

      UsersUnreadEntitiesNumberViewModel vm = new UsersUnreadEntitiesNumberViewModel();
      var all_unread_messages = (db.message.Where(item => (all_reports_id.Contains(item.report_id) && item.sender_id != user_id && !db.message_user_read.Where(mur => (mur.message_id == item.id) && (mur.user_id == user_id)).Any()))).Select(t => t.id).Count();
      vm.unread_messages = all_unread_messages;

      var all_unread_reports = (all_reports_id.Where(item => (!db.report_user_read.Where(t => ((t.user_id == user_id) && (t.report_id == item))).Any()))).Count();
      vm.unread_reports = all_unread_reports;


      var all_tasks_ids = db.task.Where(item => item.assigned_to == user_id && all_reports_id.Contains(item.report_id)).Select(item => item.id);
      var all_unread_tasks = (all_tasks_ids.Where(item => (!db.task_user_read.Where(t => ((t.user_id == user_id) && (t.task_id == item))).Any()))).Count();
      vm.unread_tasks = all_unread_tasks;

      return vm;

    }

    public UsersUnreadReportsNumberViewModel GetUserUnreadCasesNumbers(UsersReportIDsViewModel vmReportIds, int user_id)
    {
      UsersUnreadReportsNumberViewModel vm = new UsersUnreadReportsNumberViewModel();

      vm.unread_active_reports = 0;
      vm.unread_spam_reports = 0;

      vm.unread_pending_reports = 0;
      vm.unread_completed_reports = 0;
      vm.unread_closed_reports = 0;

      if (vmReportIds != null && vmReportIds.all_active_report_ids != null)
      {
        var active = vmReportIds.all_active_report_ids.Where(item => !(
         db.report.Where(t => (t.id == item &&
         db.report_user_read.Where(rl => rl.report_id == t.id && rl.user_id == user_id && rl.read_date > t.last_update_dt).Any()
         )).Any())).Count();

        vm.unread_active_reports = active;
      }

      if (vmReportIds != null && vmReportIds.all_spam_report_ids != null)
      {
        var spam = vmReportIds.all_spam_report_ids.Where(item => !(
            db.report.Where(t => (t.id == item &&
            db.report_user_read.Where(rl => rl.report_id == t.id && rl.user_id == user_id && rl.read_date > t.last_update_dt).Any()
            )).Any())).Count();
        vm.unread_spam_reports = spam;
      }

      if (vmReportIds != null && vmReportIds.all_pending_report_ids != null)
      {
        var newreport = vmReportIds.all_pending_report_ids.Where(item => !(
            db.report.Where(t => (t.id == item &&
            db.report_user_read.Where(rl => rl.report_id == t.id && rl.user_id == user_id && rl.read_date > t.last_update_dt).Any()
            )).Any())).Count();
        vm.unread_pending_reports = newreport;
      }

      if (vmReportIds != null && vmReportIds.all_completed_report_ids != null)
      {
        var completed = vmReportIds.all_completed_report_ids.Where(item => !(
            db.report.Where(t => (t.id == item &&
            db.report_user_read.Where(rl => rl.report_id == t.id && rl.user_id == user_id && rl.read_date > t.last_update_dt).Any()
            )).Any())).Count();
        vm.unread_completed_reports = completed;
      }
      if (vmReportIds != null && vmReportIds.all_closed_report_ids != null)
      {

        var closed = vmReportIds.all_closed_report_ids.Where(item => !(
            db.report.Where(t => (t.id == item &&
            db.report_user_read.Where(rl => rl.report_id == t.id && rl.user_id == user_id && rl.read_date > t.last_update_dt).Any()
            )).Any()
            )).Count();
        vm.unread_closed_reports = closed;
      }
      return vm;

    }

    public UsersUnreadReportViewModel GetUsersUnreadEntitiesInReport(int user_id, int report_id)
    {

      UsersUnreadReportViewModel vm = new UsersUnreadReportViewModel();
      UserModel um = new UserModel(user_id);
      ReportModel rm = new ReportModel(report_id);


      //public int UnreadTasksQuantity(int? report_id, bool is_user_only, int status_id)
      List<int> all_report_ids = new List<int>();
      all_report_ids = um.GetReportIds(report_id);

      int tasks_quantity = 0;

      List<int> all_task_ids = new List<int>();
      List<int> read_task_ids = new List<int>();
      List<int> unread_task_ids = new List<int>();

      // 1?? - only non-completed tasks
      List<task> tasks = um.UserTasks(1, report_id, true);
      List<TaskExtended> list_tsk = new List<TaskExtended>();
      foreach (task _task in tasks)
      {
        TaskExtended tsk = new TaskExtended(_task.id, user_id);

        if (!tsk.IsRead())
          tasks_quantity++;
      }

      vm.unread_tasks = tasks_quantity;


      int quantity = 0;
      MessageExtended _extended;

      List<message> all_messages = um.UserMessages(report_id, 1);

      foreach (message _message in all_messages)
      {
        _extended = new MessageExtended(_message.id, user_id);

        // is unread
        if (!_extended.IsRead())
          quantity++;
      }
      vm.mediatorsUnreaded = quantity;

      all_messages = um.UserMessages(report_id, 2);

      foreach (message _message in all_messages)
      {
        quantity = 0;
        _extended = new MessageExtended(_message.id, user_id);

        // is unread
        if (!_extended.IsRead())
          quantity++;
      }
      vm.reportersUnreaded = quantity;

      return vm;

    }
  }
}