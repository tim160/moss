using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;

namespace EC.Models.ECModel
{
    public class TaskCommentExtended : BaseEntity
    {
        public task_comment _taskComment;
        public TaskExtended _parentTask;

        public int TaskCommentID;
        public int TaskCommentCallerID;
        public int TaskID;

        public string PosterPath = "~/Content/Icons/noPhoto.png";


        public TaskCommentExtended()
        {
            TaskCommentCallerID = 0;
            TaskID = 0;
            TaskCommentID = 0;
        }

        public TaskCommentExtended(int task_comment_id, int user_id)
        {
            TaskCommentCallerID = user_id;
            TaskCommentID = task_comment_id;

            if (task_comment_id != 0)
            {
                task_comment _task_comment_original = db.task_comment.Where(item => item.id == task_comment_id).FirstOrDefault();
                _taskComment = _task_comment_original;

                if (_taskComment.id != 0)
                {
                    _parentTask = new TaskExtended(_taskComment.task_id, user_id);
                }
                
                if (_task_comment_original.user_id != 0)
                {
                    UserModel temp_user = new UserModel(_task_comment_original.user_id);
                    if (temp_user!= null && !string.IsNullOrWhiteSpace(temp_user._user.photo_path))
                        PosterPath = temp_user._user.photo_path;
                }
            }
        }

        public bool IsRead()
        {
            if ((TaskCommentID != 0) && (TaskCommentCallerID != 0))
            {
                bool read = ((TaskCommentCallerID == _taskComment.user_id) || (db.task_comment_user_read.Any(item => ((item.task_comment_id == TaskCommentID) && (item.user_id == TaskCommentCallerID)))));
                return read;
            }
            return false;
        }

    }
}