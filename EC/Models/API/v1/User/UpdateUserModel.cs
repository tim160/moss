using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EC.Models.API.v1.User
{
    public class UpdateUserModel
    {
        [Required]
        public int company_id { get; set; }
        [Required]
        public int role_id { get; set; }
        [Required]
        public int status_id { get; set; }
        [Required]
        [StringLength(250)]
        public string first_nm { get; set; }
        [Required]
        [StringLength(250)]
        public string last_nm { get; set; }
        //[Required]
        //[StringLength(250)]
        //public string login_nm { get; set; }
        //[Required]
        //public string password { get; set; }
        [Required]
        public string photo_path { get; set; }
        //[Required]
        //public int preferred_contact_method_id { get; set; }
        //[Required]
        //[StringLength(500)]
        //public string question_ds { get; set; }
        //[Required]
        //[StringLength(500)]
        //public string answer_ds { get; set; }
        //public DateTime last_update_dt { get; set; }
        //[Required]
        //public int user_id { get; set; }
        //[Required]
        //public int preferred_email_language_id { get; set; }
        //[Required]
        //public int notification_messages_actions_flag { get; set; }
        //[Required]
        //public int notification_new_reports_flag { get; set; }
        //[Required]
        //public int notification_marketing_flag { get; set; }
        //[Required]
        //public int notification_summary_period { get; set; }
        [Required]
        [StringLength(250)]
        public string email { get; set; }
        public string PartnerInternalID { get; set; }
    }
}