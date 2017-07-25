using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Common.Util;

namespace EC.Models.ECModel
{
    public class MessageExtended : BaseEntity
    {
        public int MessageID;
        public string MessagePoster;
        public int MessagePosterID;
        public string MessagePosterPhoto;
        public string MessageDate;
        public string MessageBody;
        public int MessageStatus;
        public int MessageCaseID;
        public string MessageStatusString
        {
            get
            {
                switch (MessageStatus)
                {
                    case 2:
                        return "Unread";
                    default:
                        return "Read";
                }
            }

        }

        private int MessageCallerID;
        public message _message;
        /// <summary>
        /// un read task - bold in description
        /// </summary>
        /// <returns></returns>
        public bool IsRead()
        {
            if ((MessageID != 0) && (MessageCallerID != 0))
            {
                bool read = ((MessagePosterID == MessageCallerID) || (db.message_user_read.Any(item => ((item.message_id == MessageID) && (item.user_id == MessageCallerID)))));
                return read;
            }

            return false;
        }

        public MessageExtended()
        {
            MessageID = 0;
            MessagePoster = "";
            MessagePosterPhoto = "";
            MessagePosterID = 0;
            MessageDate = "";
            MessageBody = "";
            MessageStatus = 0;
            MessageCaseID = 0;
            MessageCallerID = 0;
            _message = null;
        }
        public MessageExtended(int message_id, int user_id)
        {
            if (message_id != 0)
            {


                MessageCallerID = user_id;
                MessageID = message_id;
                message _message_temp = db.message.Where(item => item.id == message_id).FirstOrDefault();
                _message = _message_temp;
                MessageCaseID = _message.report_id;
                MessagePosterID = _message.sender_id;
                UserModel um = new UserModel(MessagePosterID);

                MessagePosterPhoto = "~/Content/Icons/noPhoto.png";
                if (um._user.role_id == 8)
                    MessagePosterPhoto = "~/Content/Icons/anonimousReporterIcon.png";
                else
                {
                    if (um._user.photo_path.Trim().Length > 0)
                        MessagePosterPhoto = um._user.photo_path.Trim();
                }

                #region Message Poster
                MessagePoster = um._user.first_nm.Trim() + " " + um._user.last_nm.Trim();

                if ((um._user != null) && (um._user.role_id == 8))
                {
                    ReportModel rm = new ReportModel(MessageCaseID);
                    MessagePoster = rm.Get_reporter_name(user_id);
                }


                #endregion

                #region Body
                MessageBody = _message.body_tx;
                MessageBody = StringUtil.FirstWords(MessageBody, 10).Trim();
                if (MessageBody.Length < _message.body_tx.Trim().Length)
                    MessageBody = MessageBody + "....";
                #endregion

                DateTime? dt = _message.created_dt;
                if (dt.HasValue)
                {
                    MessageDate = dt.Value.ToShortDateString();
                }
                else
                    MessageDate = "";
            }
        }

    }
}