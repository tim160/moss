namespace EC.COM.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("company")]
    public partial class company
    {
        public int id { get; set; }

        public int client_id { get; set; }

        public int address_id { get; set; }

        public int billing_info_id { get; set; }

        public int status_id { get; set; }

        [Required]
        [StringLength(500)]
        public string company_nm { get; set; }

        [StringLength(10)]
        public string company_code { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime registration_dt { get; set; }

        [StringLength(100)]
        public string contact_nm { get; set; }

        public int language_id { get; set; }

        [Required]
        [StringLength(250)]
        public string employee_quantity { get; set; }

        public int implicated_title_name_id { get; set; }

        public int witness_show_id { get; set; }

        public int show_location_id { get; set; }

        public int show_department_id { get; set; }

        public int default_anonymity_id { get; set; }

        public string notepad_en { get; set; }

        public string notepad_fr { get; set; }

        public string notepad_es { get; set; }

        public string notepad_ru { get; set; }

        public string notepad_ar { get; set; }

        [StringLength(255)]
        public string path_en { get; set; }

        [StringLength(255)]
        public string path_fr { get; set; }

        [StringLength(255)]
        public string path_es { get; set; }

        [StringLength(255)]
        public string path_ru { get; set; }

        [StringLength(255)]
        public string path_ar { get; set; }

        [StringLength(1000)]
        public string alert_en { get; set; }

        [StringLength(1000)]
        public string alert_fr { get; set; }

        [StringLength(1000)]
        public string alert_es { get; set; }

        [StringLength(1000)]
        public string alert_ru { get; set; }

        [StringLength(1000)]
        public string alert_ar { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime last_update_dt { get; set; }

        public int user_id { get; set; }

        public int time_zone_id { get; set; }

        public int step1_delay { get; set; }

        public int step1_postpone { get; set; }

        public int step2_delay { get; set; }

        public int step2_postpone { get; set; }

        public int step3_delay { get; set; }

        public int step3_postpone { get; set; }

        public int step4_delay { get; set; }

        public int step4_postpone { get; set; }

        public int step5_delay { get; set; }

        public int step5_postpone { get; set; }

        public int step6_delay { get; set; }

        public int step6_postpone { get; set; }

        public string subdomain { get; set; }

        public int? reseller_id { get; set; }

        public int? company_invitation_id { get; set; }

        public DateTime? invitation_confirmation_dt { get; set; }

        public int? invitation_confirmation_user_id { get; set; }

        public int? reseller { get; set; }

        public Guid? guid { get; set; }

        public DateTime? next_payment_date { get; set; }

        public decimal? next_payment_amount { get; set; }

        [StringLength(100)]
        public string cc_campus_alert_manager_first_name { get; set; }

        [StringLength(100)]
        public string cc_campus_alert_manager_last_name { get; set; }

        [StringLength(100)]
        public string cc_campus_alert_manager_email { get; set; }

        [StringLength(50)]
        public string cc_campus_alert_manager_phone { get; set; }

        [StringLength(100)]
        public string cc_daily_crime_log_manager_first_name { get; set; }

        [StringLength(100)]
        public string cc_daily_crime_log_manager_last_name { get; set; }

        [StringLength(100)]
        public string cc_daily_crime_log_manager_email { get; set; }

        [StringLength(50)]
        public string cc_daily_crime_log_manager_phone { get; set; }

        [StringLength(500)]
        public string company_short_name { get; set; }

        public int? contractors_number { get; set; }

        public int? customers_number { get; set; }

        public int onboard_sessions_paid { get; set; }

        public DateTime? onboard_sessions_expiry_dt { get; set; }
    }
}
