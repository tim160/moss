using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.App_LocalResources;
using EC.Models.App;
using EC.Core.Common;
using EC.Common.Interfaces;
using EC.Constants;
using log4net;

namespace EC.Models.ViewModel
{
    public class CasePreviewViewModel
    {
        protected IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();

        public int report_id { get; set; }
        public string case_secondary_types { get; set; }
        public string case_secondary_types_all { get; set; }
        public string current_status { get; set; }
        public int investigation_status_number { get; set; }
        public int previous_investigation_status_number { get; set; }
        public string case_number { get; set; }

        public string case_color_code { get; set; }

        public int days_left { get; set; }

        public string reported_dt { get; set; }
        public string case_dt { get; set; }

        public string reported_by_whom { get; set; }
        public string departments { get; set; }
        public string location { get; set; }


        public string tasks_number { get; set; }
        public string messages_number { get; set; }
        public string team_number { get; set; }


        public int new_messages { get; set; }
        public int new_tasks { get; set; }
        public bool team_invovled { get; set; }

        public string team_involved_names { get; set; }

        #region Reaction1
        public bool show_previous_status { get; set; }
        public string previous_header_message { get; set; }
        public string previous_sender_name { get; set; }
        public int previous_sender_id { get; set; }
        public string previous_sender_date { get; set; }
        public string previous_status_message { get; set; }
        #endregion

        #region Reaction2
        public bool show_last_status { get; set; }
        public string last_header_message { get; set; }
        public string last_sender_name { get; set; }
        public int last_sender_id { get; set; }
        public string last_sender_date { get; set; }
        public string last_status_message { get; set; }
        #endregion

        public double total_days { get; set; }
        public double case_dt_s { get; set; }
        public bool? cc_is_life_threating { get; set; }
        public string severity_s { get; set; }
        public int? severity_id { get; set; }

        public IEnumerable<dynamic> mediators { get; set; }
        public IEnumerable<report_owner> owners { get; set; }

        public CasePreviewViewModel()
        {
        }
        public CasePreviewViewModel(int report_id, int caller_id)
        {
            ReportModel rm = new ReportModel(report_id);
            BindCaseModelToCasePreviewViewModel(rm, caller_id);
        }

        public CasePreviewViewModel(ReportModel rm, int caller_id)
        {
            BindCaseModelToCasePreviewViewModel(rm, caller_id);
        }

        public CasePreviewViewModel BindCaseModelToCasePreviewViewModel(ReportModel rm, int caller_id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CasePreviewViewModel vm_case = new CasePreviewViewModel();

            this.report_id = rm._report.id;
            this.case_dt = rm.IncidentDateString();
            this.reported_dt = rm.ReportedDateString();
            this.case_number = rm._report.display_name;
            this.location = rm.LocationString();
            this.case_secondary_types = rm.SecondaryTypeString();
            this.case_secondary_types_all = rm.SecondaryTypeStringAll();            
            this.case_color_code = rm.ColorCode();
            this.days_left = rm.GetThisStepDaysLeft();
            this.current_status = rm.InvestigationStatusString();


            this.tasks_number = rm.ReportTasks(0).Count().ToString();
            this.messages_number = rm.UserMessagesCountNotSecure(caller_id, 0).ToString();

            //?
            return vm_case;

        }
    }
}