using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.COM.Data
{
    public class user
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public Nullable<int> address_id { get; set; }
        public int role_id { get; set; }
        public int status_id { get; set; }
        public string first_nm { get; set; }
        public string last_nm { get; set; }
        public string login_nm { get; set; }
        public string password { get; set; }
        public string photo_path { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int preferred_contact_method_id { get; set; }
        public string title_ds { get; set; }
        public string employee_no { get; set; }
        public string notepad_tx { get; set; }
        public string question_ds { get; set; }
        public string answer_ds { get; set; }
        public Nullable<System.DateTime> last_login_dt { get; set; }
        public Nullable<System.DateTime> previous_login_dt { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public int user_id { get; set; }
        public int preferred_email_language_id { get; set; }
        public int notification_messages_actions_flag { get; set; }
        public int notification_new_reports_flag { get; set; }
        public int notification_marketing_flag { get; set; }
        public int notification_summary_period { get; set; }
        public Nullable<int> company_location_id { get; set; }
        public string location_nm { get; set; }
        public string sign_in_code { get; set; }
        public Nullable<System.Guid> guid { get; set; }
        public Nullable<int> company_department_id { get; set; }
        public Nullable<int> user_permissions_approve_case_closure { get; set; }
        public Nullable<int> user_permissions_change_settings { get; set; }
        public string password2 { get; set; }
    }
}