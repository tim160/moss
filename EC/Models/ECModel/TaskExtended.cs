using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Core.Common;
using EC.Common.Util;

namespace EC.Models.ECModel
{
    public class TaskExtended : BaseEntity
    {
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
                    return m_DateTimeHelper.ConvertDateToShortString(TaskDueDate.Value);
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
                    return rm._color_descr;
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
                    return rm._color_code;
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
                    TaskAssigner = App_LocalResources.GlobalRes.You;
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
                    TaskAssignee = App_LocalResources.GlobalRes.You;
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

    }
}