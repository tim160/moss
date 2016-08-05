using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ECModel
{
    public class User : BaseEntity
    {
        #region Properties
        public int id
        {
            get;
            set;
        }
        public int company_id
        {
            get;
            set;
        }
        public int address_id
        {
            get;
            set;
        }
        public int role_id
        {
            get;
            set;
        }
        public int status_id
        {
            get;
            set;
        }
        public string first_nm
        {
            get;
            set;
        }
        public string last_nm
        {
            get;
            set;
        }
        public string login_nm
        {
            get;
            set;
        }
        public string password
        {
            get;
            set;
        }
        public string email
        {
            get;
            set;
        }
        public string phone
        {
            get;
            set;
        }
        public int preferred_contact_method_id
        {
            get;
            set;
        }

        public string title_ds
        {
            get;
            set;
        }

        public string employee_no
        {
            get;
            set;
        }
        public string notepad_tx
        {
            get;
            set;
        }
        public string question_ds
        {
            get;
            set;
        }
        public string answer_ds
        {
            get;
            set;
        }

        public DateTime last_login_dt
        {
            get;
            set;
        }
        public DateTime last_update_dt
        {
            get;
            set;
        }

        public int user_id
        {
            get;
            set;
        }
        public int preferred_email_language_id
        {
            get;
            set;
        }
        public int notification_messages_actions_flag
        {
            get;
            set;
        }
        public int notification_new_reports_flag
        {
            get;
            set;
        }
        public int notification_marketing_flag
        {
            get;
            set;
        }
        public int notification_summary_period
        {
            get;
            set;
        }

        public string title
        {
            get;
            set;
        }
        #endregion

        public User(int user_id)
        {
            id = 0;
            
            Database.user _user = db.user.First(a => a.id == user_id);
            if (_user.id != null && _user.id != 0)
            {
                id = _user.id;
                company_id = _user.company_id;
                status_id = _user.status_id;
                role_id = _user.role_id;
                first_nm = _user.first_nm;
                last_nm = _user.last_nm;
                login_nm = _user.login_nm;
                email = _user.email;
                phone = _user.phone;
                preferred_email_language_id = _user.preferred_email_language_id;
                notification_messages_actions_flag = _user.notification_messages_actions_flag;
                notification_new_reports_flag = _user.notification_new_reports_flag;
                notification_marketing_flag = _user.notification_marketing_flag;
                notification_summary_period = _user.notification_summary_period;
                title = _user.title_ds;

                last_login_dt = DateTime.Today;
                if (_user.last_login_dt.HasValue)
                    last_login_dt = _user.last_login_dt.Value;
            }
        }
    }
}