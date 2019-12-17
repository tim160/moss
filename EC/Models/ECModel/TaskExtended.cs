using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Core.Common;
using EC.Common.Util;
using EC.Localization;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using EC.Common.Interfaces;
using log4net;
using System.Configuration;
using System.Data.Entity.Migrations;
using EC.Constants;

namespace EC.Models.ECModel
{
    public class TaskExtended : BaseEntity
    {
        protected IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();
        public EmailNotificationModel emailNotificationModel = new EmailNotificationModel();
        public ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region Properties

        public task _task;
        public int TaskID;

        public string TaskAssigner;
        public int TaskAssignerID;
        public string TaskAssignerPhoto;

        public string TaskDate;
        public string TaskDescr;
        public string TaskDescrFull;

        public string TaskTitle;

        public int TaskStatus;
        public int TaskReportID;

        public string TaskAssignee;
        public int TaskAssigneeID;
        public string TaskAssigneePhoto;
        public bool file;
        public List<attachment> fileAttach;

        public DateTime? TaskDueDate;
        public string TaskDueDateString
        {
            get
            {
                if (TaskDueDate.HasValue)
                {
                    return m_DateTimeHelper.ConvertDateToLongMonthString(TaskDueDate.Value);
                }
                else
                    return "";
            }
        }
        public string TaskStatusString
        {
            get
            {
                switch (TaskStatus)
                {
                    case 2:
                        return "Completed";
                    case 3:
                        return "Expired";
                    case 4:
                        return "Unread";
                    case 1:
                        return "Active";
                    default:
                        return "";
                }
            }

        }

        private int TaskCallerID;
        #endregion
        public bool IsExpired
        {
            get
            {
                if (TaskDueDate.HasValue)
                {
                    DateTime dt = DateTime.Today;
                    int total_days = (TaskDueDate.Value - dt).Days;
                    if (total_days < 0)
                    {
                        return true;
                    }
                    return false;
                }
                else
                    return false;
            }
        }

        public bool IsCompleted()
        {
            if (TaskID != 0)
            {
                task _task = db.task.Where(item => item.id == TaskID).FirstOrDefault();
                if (_task.is_completed)
                    return true;
                else
                    return false;
            }

            return false;
        }

        /// <summary>
        /// un read task - bold in description
        /// </summary>
        /// <returns></returns>
        public bool IsRead()
        {
            if ((TaskID != 0) && (TaskCallerID != 0))
            {
                bool read = ((TaskCallerID == TaskAssignerID) || (db.task_user_read.Any(item => ((item.task_id == TaskID) && (item.user_id == TaskCallerID)))));
                return read;
            }

            return false;
        }

        /// <summary>
        /// if task contains new messages for user
        /// </summary>
        /// <returns></returns>
        public bool HasNewActivity()
        {
            if ((TaskID != 0) && (TaskCallerID != 0))
            {
                bool _new_activity = false;
                List<task_comment> _comments = db.task_comment.Where(item => item.task_id == TaskID).ToList();

                bool temp = false;
                foreach (task_comment _comment in _comments)
                {
                    temp = db.task_comment_user_read.Any(item => ((item.task_comment_id == _comment.id) && (item.user_id == TaskCallerID)));
                    if (temp)
                        _new_activity = true;
                }

                return _new_activity;
            }

            return false;
        }

        public string TaskColor
        {
            get
            {
                if (TaskReportID != 0)
                {
                    ReportModel rm = new ReportModel(TaskReportID);
                    return rm._reportStringModel.ColorDescr();
                }
                else
                    return "blue";
            }
        }
        public string TaskColorCode
        {
            get
            {
                if (TaskReportID != 0)
                {
                    ReportModel rm = new ReportModel(TaskReportID);
                    return rm._reportStringModel.ColorCode();
                }
                else
                    return "3099be";
            }
        }

        public int TaskLength
        {
            get
            {
                if ((_task.is_completed) && (_task.completed_on.HasValue))
                {
                    return (_task.completed_on.Value - _task.created_on).Days;
                }
                else
                {
                    return (DateTime.Today - _task.created_on).Days;
                }
            }
        }

        public int TaskLengthPreviousMonth
        {
            get
            {
                DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                if (_task.created_on < _month_end_date)
                {
                    if ((_task.is_completed) && (_task.completed_on.HasValue) && (_task.completed_on < _month_end_date))
                    {
                        return (_task.completed_on.Value - _task.created_on).Days;
                    }
                    else
                    {
                        return (_month_end_date - _task.created_on).Days;
                    }
                }
                else
                    return 0;
            }
        }
        public TaskExtended()
        {
            TaskID = 0;
            TaskAssigner = "";
            TaskAssignerPhoto = "";
            TaskAssignerID = 0;
            TaskDate = "";
            TaskDescr = "";
            TaskDescrFull = "";
            TaskStatus = 0;
            TaskReportID = 0;
            TaskAssignee = "";
            TaskAssigneeID = 0;
            TaskAssigneePhoto = "";
            TaskDueDate = null;
            TaskTitle = "";
            TaskCallerID = 0;
        }
        public TaskExtended(int task_id, int user_id)
        {
            if (task_id != 0)
            {

                TaskCallerID = user_id;

                TaskAssignerPhoto = "~/Content/Icons/noPhoto.png";
                TaskAssigneePhoto = "~/Content/Icons/noPhoto.png";

                TaskID = task_id;
                task _task_original = db.task.Where(item => item.id == task_id).FirstOrDefault();

                _task = _task_original;

                TaskReportID = _task_original.report_id;

                #region TaskAssignedBy
                if (_task_original.created_by == user_id)
                    TaskAssigner = LocalizationGetter.GetString("You");
                user _user = db.user.Where(item => item.id == _task_original.created_by).FirstOrDefault();
                if ((_user != null) && (_task_original.created_by != user_id))
                    TaskAssigner = _user.first_nm.Trim() + " " + _user.last_nm.Trim();
                if (_user != null)
                {
                    if (_user.photo_path.Trim().Length > 0)
                        TaskAssignerPhoto = _user.photo_path;
                    TaskAssignerID = _user.id;
                }
                #endregion

                #region TaskAssignedTo
                if (_task_original.assigned_to == user_id)
                    TaskAssignee = LocalizationGetter.GetString("You");
                _user = db.user.Where(item => item.id == _task_original.assigned_to).FirstOrDefault();
                if ((_user != null) && (_task_original.assigned_to != user_id))
                    TaskAssignee = _user.first_nm.Trim() + " " + _user.last_nm.Trim();
                if (_user != null)
                {
                    if (_user.photo_path.Trim().Length > 0)
                        TaskAssigneePhoto = _user.photo_path;
                    TaskAssigneeID = _user.id;
                }
                #endregion

                TaskDate = _task_original.created_on.ToShortDateString();

                #region TaskTitle
                TaskTitle = _task_original.title;
                TaskTitle = StringUtil.FirstWords(TaskTitle, 3).Trim();
                if (TaskTitle.Length < _task_original.title.Trim().Length)
                    TaskTitle = (TaskTitle + "....").Trim();
                #endregion

                #region TaskDescription
                TaskDescr = _task_original.description;
                TaskDescr = StringUtil.FirstWords(TaskDescr, 3).Trim();
                if (TaskDescr.Length < _task_original.description.Trim().Length)
                    TaskDescr = (TaskDescr + "....").Trim();
                #endregion

                #region Task Due Date/Due date string
                DateTime? due_date = _task_original.due_date;
                if (due_date.HasValue)
                {
                    TaskDueDate = due_date;
                }
                #endregion


                TaskStatus = 1;
                if (_task_original.is_completed)
                    TaskStatus = 2;
                else
                {
                    if ((due_date.HasValue && due_date.Value >= DateTime.Today) || (!due_date.HasValue))
                    {
                        // either active or unread, return active for now.
                        if (_task_original.id == 5)
                            TaskStatus = 4;
                        else
                            TaskStatus = 1;
                    }
                    else
                    {
                        // task is expired
                        TaskStatus = 3;
                    }
                }
            }
        }

        public List<task_comment> Comments()
        {
            List<task_comment> task_comments = new List<task_comment>();
            task_comments = db.task_comment.Where(item => (item.task_id == TaskID)).OrderByDescending(item => item.created_date).ToList();

            return task_comments;
        }


        public void HasTaskFile()
        {
            List<attachment> list = new List<attachment>();
            if (TaskID > 0)
            {
                list = db.attachment.Where(item => (item.report_task_id == TaskID)).ToList();
            }
            if (list.Count > 0)
            {
                file = true;
            }
            fileAttach = list;
        }
        public bool CreateNewTask(NameValueCollection form, HttpFileCollectionBase files, bool is_cc, HttpSessionStateBase httpSession)
        {
            user reporter_user = (user)httpSession[ECGlobalConstants.CurrentUserMarcker];

            if (reporter_user == null)
            {
                return false;
            }
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

            ReportModel rm = new ReportModel(report_id);
            if(!rm.HasAccessToReport(reporter_user.id))
            {
                return false;
            }
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

                logModel.UpdateReportLog(mediator_id, 10, report_id, taskName, null, "");
                logModel.UpdateReportLog(mediator_id, 12, report_id, taskName, null, "");

                #region New Task - Email to Asignee
                UserModel um = new UserModel(assignTo);
                ReportModel _rm = new ReportModel(report_id);

                if ((um._user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(um._user.email.Trim()))
                {
                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                    eb.NewTask(um._user.first_nm, um._user.last_nm, _rm._report.display_name);
                    emailNotificationModel.SaveEmailBeforeSend(mediator_id, um._user.id, um._user.company_id, um._user.email.Trim(), ConfigurationManager.AppSettings["emailFrom"], "",
                      LocalizationGetter.GetString("Email_Title_NewTask", is_cc), eb.Body, false, 6);
                }

                #endregion

                return true;
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                Console.WriteLine("Task/Attachments was not created");
                Console.WriteLine(ex.Data);
                return false;
            }
        }

        public bool CloseTask(int task_id, int mediator_id, HttpSessionStateBase httpSession)
        {
            user reporter_user = (user)httpSession[ECGlobalConstants.CurrentUserMarcker];

            if (reporter_user == null)
            {
                return false;
            }
            try
            {
                task completedTask = db.task.Where(item => (item.id == task_id)).FirstOrDefault();

                ReportModel rm = new ReportModel(completedTask.report_id);
                if (!rm.HasAccessToReport(reporter_user.id))
                {
                    return false;
                }

                completedTask.is_completed = true;
                completedTask.completed_by = mediator_id;
                completedTask.completed_on = DateTime.Now;

                db.task.AddOrUpdate(completedTask);
                db.SaveChanges();

                TaskExtended tsk = new TaskExtended(task_id, mediator_id);
                logModel.UpdateReportLog(mediator_id, 11, tsk.TaskReportID, tsk._task.title, null, "");


                return true;
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                Console.WriteLine("Task wasn't close");
                Console.WriteLine(ex.Data);
                return false;
            }
        }

        public bool ReassignTask(int task_id, int mediator_id, bool is_cc, HttpSessionStateBase httpSession)
        {
            try
            {
                user reporter_user = (user)httpSession[ECGlobalConstants.CurrentUserMarcker];

                if (reporter_user == null)
                {
                    return false;
                }


                task currentTask = new task();

                currentTask = db.task.Where(item => (item.id == task_id)).FirstOrDefault();

                ReportModel rm = new ReportModel(currentTask.report_id);
                if (!rm.HasAccessToReport(reporter_user.id))
                {
                    return false;
                }
                currentTask.assigned_to = mediator_id;
                db.task.AddOrUpdate(currentTask);
                db.SaveChanges();


                TaskExtended tsk = new TaskExtended(task_id, mediator_id);
                logModel.UpdateReportLog(mediator_id, 12, tsk.TaskReportID, tsk._task.title, mediator_id, "");

                #region Task Reassigned - Email to Asignee
                UserModel um = new UserModel(mediator_id);
                ReportModel _rm = new ReportModel(tsk.TaskReportID);

                if ((um._user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(um._user.email.Trim()))
                {
                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement();
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                    eb.NewTask(um._user.first_nm, um._user.last_nm, _rm._report.display_name);
                    emailNotificationModel.SaveEmailBeforeSend(reporter_user.id, um._user.id, um._user.company_id, um._user.email.Trim(), ConfigurationManager.AppSettings["emailFrom"], "",
                        LocalizationGetter.GetString("Email_Title_NewTask", is_cc), eb.Body, false, 6);
                }

                #endregion

                return true;
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                Console.WriteLine("Task wasn't reassigned");
                Console.WriteLine(ex.Data);
                return false;
            }
        }

    }
}