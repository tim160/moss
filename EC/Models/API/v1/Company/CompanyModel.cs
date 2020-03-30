using System;
using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.Company
{
    public class CompanyModel
    {
         [Required]
        public string partnerCompanyID { get; set; }
  /*      [Required]
        public int address_id { get; set; }
        [Required]
        public int billing_info_id { get; set; }
        [Required]
        public int status_id { get; set; }*/
        [Required]
        [StringLength(500)]
        public string companyName { get; set; }
    /*    [Required]
        public DateTime registration_dt { get; set; }
        [Required]
        public int language_id { get; set; }*/
        [Required]
        [StringLength(250)]
        public string employeeQuantity { get; set; }
    /*    [Required]
        public int implicated_title_name_id { get; set; }
        [Required]
        public int witness_show_id { get; set; }
        [Required]
        public int show_location_id { get; set; }
        [Required]
        public int show_department_id { get; set; }
        [Required]
        public int default_anonymity_id { get; set; }
        [Required]
        public DateTime last_update_dt { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public int time_zone_id { get; set; }
        [Required]
        public int step1_delay { get; set; }
        [Required]
        public int step1_postpone { get; set; }
        [Required]
        public int step2_delay { get; set; }
        [Required]
        public int step2_postpone { get; set; }
        [Required]
        public int step3_delay { get; set; }
        [Required]
        public int step3_postpone { get; set; }
        [Required]
        public int step4_delay { get; set; }
        [Required]
        public int step4_postpone { get; set; }
        [Required]
        public int step5_delay { get; set; }
        [Required]
        public int step5_postpone { get; set; }
        [Required]
        public int step6_delay { get; set; }
        [Required]
        public int step6_postpone { get; set; }
        [Required]
        public int onboard_sessions_paid { get; set; }
        [Required]
        public bool controls_client { get; set; }*/
        public string partnerClientID { get; set; }
        public string customLogoPath { get; set; }
  }
}