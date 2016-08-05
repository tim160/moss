using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ECModel
{
    public class Report : BaseEntity
    {
        public int submitted_language_id
        {
            get;
            set;
        }

        #region Properties
        public int id
        {
            get;
            set;
        }
        public string display_name
        {
            get;
            set;
        }
        public string report_name_generic
        {
            get;
            set;
        }
        public int company_id
        {
            get;
            set;
        }
        public string company_nm
        {
            get;
            set;
        }
        public string submitted_company_nm
        {
            get;
            set;
        }
        
        public int assign_company_flag
        {
            get;
            set;
        }
        public int client_id
        {
            get;
            set;
        }
        public int status_id
        {
            get;
            set;
        }
        public int reporter_user_id
        {
            get;
            set;
        }
        public int company_relationship_id
        {
            get;
            set;
        }
        public string not_current_employee
        {
            get;
            set;
        }
        
        public int priority_id
        {
            get;
            set;
        }
        public DateTime reported_dt
        {
            get;
            set;
        }
        public DateTime incident_dt
        {
            get;
            set;
        }
        public int is_ongoing
        {
            get;
            set;
        }
        public int type_id
        {
            get;
            set;
        }
        public int location_id
        {
            get;
            set;
        }
       
        public string other_location_name
        {
            get;
            set;
        }
        public string other_department_name
        {
            get;
            set;
        }
        public string other_secondary_type_name
        {
            get;
            set;
        }
        public string description
        {
            get;
            set;
        }
        public string how_often_occurred
        {
            get;
            set;
        }
        public string inititiated_involved
        {
            get;
            set;
        }
        
        public string injury_damage
        {
            get;
            set;
        }
        public int incident_anonymity_id
        {
            get;
            set;
        }
        public string reportedbefore_towho
        {
            get;
            set;
        }
        public string reportedbefore_actions
        {
            get;
            set;
        }
        public string witness_names
        {
            get;
            set;
        }
        public string witness_will_cooperate
        {
            get;
            set;
        }
        
        public string refferred_outside_organization
        {
            get;
            set;
        }
        public string additional_comments
        {
            get;
            set;
        }
        public int source_type_id
        {
            get;
            set;
        }
        public int validation_type_id
        {
            get;
            set;
        }
        public int action_type_id
        {
            get;
            set;
        }
        public string outcome_action_ds
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
        public string ip
        {
            get;
            set;
        }
        #endregion
         #region Constructor
        public Report()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report_id"></param>
        /// <param name="language_id"></param>
        public Report(int report_id, int? language_id)
        {
            id = 0;
            Database.report _report = db.report.First(a => a.id == report_id);
            if (_report.id != null && _report.id != 0)
            {
                id = _report.id;
                display_name = _report.display_name;
                report_name_generic = _report.report_name_generic;
                company_id = _report.company_id;
                submitted_company_nm = _report.submitted_company_nm;
                company_nm = _report.company_nm;

                assign_company_flag = _report.assign_company_flag;
                client_id = _report.client_id;
                status_id = _report.status_id;
                reporter_user_id = _report.reporter_user_id;
                company_relationship_id = _report.company_relationship_id;
                not_current_employee = _report.not_current_employee;

                priority_id = _report.priority_id;
                reported_dt = _report.reported_dt;
                incident_dt = _report.incident_dt;
                is_ongoing = _report.is_ongoing;
                type_id = _report.type_id;
     /////           location_id = _report.location_id;

                other_location_name = _report.other_location_name;
                other_department_name = _report.other_department_name;
                other_secondary_type_name = _report.other_secondary_type_name;
                description = _report.description;
                how_often_occurred = _report.how_often_occurred;
                inititiated_involved = _report.inititiated_involved;

                injury_damage = _report.injury_damage;
                incident_anonymity_id = _report.incident_anonymity_id;
                reportedbefore_towho = _report.reportedbefore_towho;
                reportedbefore_actions = _report.reportedbefore_actions;
                witness_names = _report.witness_names;
                witness_will_cooperate = _report.witness_will_cooperate;

                refferred_outside_organization = _report.refferred_outside_organization;
                additional_comments = _report.additional_comments;
      /////          source_type_id = _report.source_type_id;
      /////          validation_type_id = _report.validation_type_id;
     ////           action_type_id = _report.action_type_id;
      /////          outcome_action_ds = _report.outcome_action_ds;

                last_update_dt = _report.last_update_dt;
                user_id = _report.user_id;
                ip = _report.ip;
            }
        }
        #endregion
    }
}