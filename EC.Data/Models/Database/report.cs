//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EC.Data.Models.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class report
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public report()
        {
            this.report_non_mediator_involved = new HashSet<report_non_mediator_involved>();
        }
    
        public int id { get; set; }
        public string company_nm { get; set; }
        public string submitted_company_nm { get; set; }
        public int company_id { get; set; }
        public int assign_company_flag { get; set; }
        public string display_name { get; set; }
        public string report_name_generic { get; set; }
        public int report_color_id { get; set; }
        public int client_id { get; set; }
        public int status_id { get; set; }
        public int reporter_user_id { get; set; }
        public Nullable<int> reporter_country_id { get; set; }
        public int company_relationship_id { get; set; }
        public string not_current_employee { get; set; }
        public int priority_id { get; set; }
        public Nullable<int> management_know_id { get; set; }
        public string management_know_text { get; set; }
        public System.DateTime reported_dt { get; set; }
        public int is_ongoing { get; set; }
        public Nullable<int> report_frequency_id { get; set; }
        public string report_frequency_text { get; set; }
        public int type_id { get; set; }
        public Nullable<int> location_id { get; set; }
        public string other_location_name { get; set; }
        public string other_department_name { get; set; }
        public string other_secondary_type_name { get; set; }
        public string description { get; set; }
        public int injury_damage_id { get; set; }
        public string injury_damage { get; set; }
        public int incident_anonymity_id { get; set; }
        public Nullable<bool> witness_flag { get; set; }
        public string witness_names { get; set; }
        public string witness_will_cooperate { get; set; }
        public Nullable<int> reported_outside_id { get; set; }
        public string reported_outside_text { get; set; }
        public Nullable<int> source_type_id { get; set; }
        public Nullable<int> validation_type_id { get; set; }
        public Nullable<int> action_type_id { get; set; }
        public string outcome_action_ds { get; set; }
        public int user_id { get; set; }
        public string ip { get; set; }
        public bool previously_reported { get; set; }
        public string previously_reported_report_number { get; set; }
        public string previously_reported_report_description { get; set; }
        public bool previously_reported_accepted { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public System.DateTime incident_dt { get; set; }
        public bool report_by_myself { get; set; }
        public Nullable<int> scope_id { get; set; }
        public Nullable<int> scope_user_id { get; set; }
        public Nullable<int> severity_id { get; set; }
        public Nullable<int> severity_user_id { get; set; }
        public Nullable<bool> cc_is_clear_act_crime { get; set; }
        public Nullable<int> cc_crime_statistics_category_id { get; set; }
        public Nullable<int> cc_crime_statistics_location_id { get; set; }
    
        public virtual management_know management_know { get; set; }
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<report_non_mediator_involved> report_non_mediator_involved { get; set; }
    }
}
