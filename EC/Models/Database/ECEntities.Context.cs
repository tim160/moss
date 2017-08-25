﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EC.Models.Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ECEntities : DbContext
    {
        public ECEntities()
            : base("name=ECEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<action> action { get; set; }
        public virtual DbSet<address> address { get; set; }
        public virtual DbSet<anonymity> anonymity { get; set; }
        public virtual DbSet<attachment> attachment { get; set; }
        public virtual DbSet<billing_option> billing_option { get; set; }
        public virtual DbSet<client> client { get; set; }
        public virtual DbSet<color> color { get; set; }
        public virtual DbSet<company> company { get; set; }
        public virtual DbSet<company_anonymity> company_anonymity { get; set; }
        public virtual DbSet<company_custom_question> company_custom_question { get; set; }
        public virtual DbSet<company_department> company_department { get; set; }
        public virtual DbSet<company_invitation> company_invitation { get; set; }
        public virtual DbSet<company_location> company_location { get; set; }
        public virtual DbSet<company_relationship> company_relationship { get; set; }
        public virtual DbSet<company_secondary_type> company_secondary_type { get; set; }
        public virtual DbSet<company_type> company_type { get; set; }
        public virtual DbSet<company_unread_report_reminder> company_unread_report_reminder { get; set; }
        public virtual DbSet<country> country { get; set; }
        public virtual DbSet<custom_question> custom_question { get; set; }
        public virtual DbSet<custom_question_answer> custom_question_answer { get; set; }
        public virtual DbSet<demoq_request> demoq_request { get; set; }
        public virtual DbSet<frequency> frequency { get; set; }
        public virtual DbSet<industry> industry { get; set; }
        public virtual DbSet<injury_damage> injury_damage { get; set; }
        public virtual DbSet<investigation_status> investigation_status { get; set; }
        public virtual DbSet<invitation> invitation { get; set; }
        public virtual DbSet<language> language { get; set; }
        public virtual DbSet<login_history> login_history { get; set; }
        public virtual DbSet<management_know> management_know { get; set; }
        public virtual DbSet<message> message { get; set; }
        public virtual DbSet<message_user_read> message_user_read { get; set; }
        public virtual DbSet<notification_processed> notification_processed { get; set; }
        public virtual DbSet<notification_summary_period> notification_summary_period { get; set; }
        public virtual DbSet<ongoing> ongoing { get; set; }
        public virtual DbSet<outcome> outcome { get; set; }
        public virtual DbSet<partner> partner { get; set; }
        public virtual DbSet<preferred_contact_method> preferred_contact_method { get; set; }
        public virtual DbSet<priority> priority { get; set; }
        public virtual DbSet<province> province { get; set; }
        public virtual DbSet<relationship> relationship { get; set; }
        public virtual DbSet<report> report { get; set; }
        public virtual DbSet<report_department> report_department { get; set; }
        public virtual DbSet<report_investigation_status> report_investigation_status { get; set; }
        public virtual DbSet<report_log> report_log { get; set; }
        public virtual DbSet<report_mediator_assigned> report_mediator_assigned { get; set; }
        public virtual DbSet<report_mediator_involved> report_mediator_involved { get; set; }
        public virtual DbSet<report_non_mediator_involved> report_non_mediator_involved { get; set; }
        public virtual DbSet<report_owner> report_owner { get; set; }
        public virtual DbSet<report_relationship> report_relationship { get; set; }
        public virtual DbSet<report_resolution> report_resolution { get; set; }
        public virtual DbSet<report_secondary_type> report_secondary_type { get; set; }
        public virtual DbSet<report_type_mandatory_question> report_type_mandatory_question { get; set; }
        public virtual DbSet<report_user_read> report_user_read { get; set; }
        public virtual DbSet<reported_outside> reported_outside { get; set; }
        public virtual DbSet<resolution> resolution { get; set; }
        public virtual DbSet<role_in_report> role_in_report { get; set; }
        public virtual DbSet<secondary_type_mandatory> secondary_type_mandatory { get; set; }
        public virtual DbSet<source> source { get; set; }
        public virtual DbSet<status> status { get; set; }
        public virtual DbSet<task> task { get; set; }
        public virtual DbSet<task_comment> task_comment { get; set; }
        public virtual DbSet<task_comment_user_read> task_comment_user_read { get; set; }
        public virtual DbSet<task_user_read> task_user_read { get; set; }
        public virtual DbSet<time_zone> time_zone { get; set; }
        public virtual DbSet<type> type { get; set; }
        public virtual DbSet<unread_report_reminder> unread_report_reminder { get; set; }
        public virtual DbSet<unread_report_reminder_sent> unread_report_reminder_sent { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<user_change_password> user_change_password { get; set; }
        public virtual DbSet<user_role> user_role { get; set; }
        public virtual DbSet<validation_type> validation_type { get; set; }
        public virtual DbSet<case_closure_reason> case_closure_reason { get; set; }
        public virtual DbSet<company_outcome> company_outcome { get; set; }
        public virtual DbSet<company_payments> company_payments { get; set; }
        public virtual DbSet<industry_posters> industry_posters { get; set; }
        public virtual DbSet<message_posters> message_posters { get; set; }
        public virtual DbSet<poster> poster { get; set; }
        public virtual DbSet<poster_industry_posters> poster_industry_posters { get; set; }
        public virtual DbSet<scope> scope { get; set; }
        public virtual DbSet<severity> severity { get; set; }
        public virtual DbSet<cc_crime_statistics_category> cc_crime_statistics_category { get; set; }
        public virtual DbSet<cc_crime_statistics_location> cc_crime_statistics_location { get; set; }
        public virtual DbSet<company_third_level_type> company_third_level_type { get; set; }
        public virtual DbSet<report_third_level_type> report_third_level_type { get; set; }
        public virtual DbSet<location_cc_extended> location_cc_extended { get; set; }
        public virtual DbSet<company_root_cases_behavioral> company_root_cases_behavioral { get; set; }
        public virtual DbSet<company_root_cases_external> company_root_cases_external { get; set; }
        public virtual DbSet<company_root_cases_organizational> company_root_cases_organizational { get; set; }
        public virtual DbSet<company_case_admin_department> company_case_admin_department { get; set; }
        public virtual DbSet<company_case_routing> company_case_routing { get; set; }
        public virtual DbSet<company_case_routing_attachments> company_case_routing_attachments { get; set; }
        public virtual DbSet<company_case_routing_location> company_case_routing_location { get; set; }
    }
}
