using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.App_LocalResources;
using EC.Models.App;


namespace EC.Models.ViewModel
{
    public class CasePreviewViewModel
    {

        public int report_id { get; set; }
        public string case_secondary_types { get; set; }
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

        private report_investigation_status ris;
        private UserModel temp_um = new UserModel();

        public CasePreviewViewModel()
        {
        }
        public CasePreviewViewModel(int report_id, int caller_id)
        {
            ReportModel rm = new ReportModel(report_id);
            BindCaseModelToCasePreviewViewModel(rm, caller_id);
        }

        public CasePreviewViewModel BindCaseModelToCasePreviewViewModel(ReportModel rm, int caller_id)
        {
            UserModel um = new UserModel(caller_id);

            GlobalFunctions glb = new GlobalFunctions();
            CasePreviewViewModel vm_case = new CasePreviewViewModel();
            this.previous_investigation_status_number = 0;

            this.report_id = rm._report.id;
            this.case_dt = rm._incident_date_string_month_long;
            this.reported_dt = rm._reported_date_string;
            this.case_number = rm._report.display_name;
            this.investigation_status_number = rm._investigation_status;
            this.location = rm._location_string;
            this.departments = rm._departments_string;
            this.current_status = rm._investigation_status_string;
            this.case_secondary_types = rm._secondary_type_string;
            this.case_color_code = rm._color_code;

            this.reported_by_whom = rm._reporter_company_relation_short;
            this.days_left = rm._step_days_left;

            this.tasks_number = rm.ReportTasks(0).Count().ToString();
            this.messages_number = um.UserMessages(rm._report.id, 0).Count().ToString();
            this.team_number = rm._mediators_whoHasAccess_toReport.Count().ToString();

            //?
            this.team_invovled = (rm._involved_mediators_user_list.Count() > 0);

            string names = "";
            foreach(user _inv_mediator in rm._involved_mediators_user_list)
            {

                names = names + _inv_mediator.first_nm + "_" + _inv_mediator.last_nm + ",";
            }

            if (names.Length > 0)
            {
                names = names.Remove(names.Length - 1);
            }
            this.team_involved_names = names;

            this.new_messages = um.Unread_Messages_Quantity(rm._report.id, 0);
            this.new_tasks = um.UnreadActivityUserTaskQuantity(rm._report.id, false);


            if ((rm._investigation_status == Constant.investigation_status_completed) || (rm._investigation_status == Constant.investigation_status_resolution))
            {
                // completed  // cannot be resolved
                this.show_previous_status = false;
                this.show_last_status = true;
                ris = rm._last_investigation_status();


                this.last_sender_id = ris.user_id;
                temp_um = new UserModel(ris.user_id);
                this.last_sender_name = temp_um._user.first_nm + " " + temp_um._user.last_nm;
                this.last_sender_date = glb.ConvertDateToLongMonthString(ris.created_date);
                this.last_status_message = ris.description;

                if ((rm._investigation_status == Constant.investigation_status_completed))
                {
                    this.previous_header_message = GlobalRes.CaseCantBeResolved;
                }
                else
                {
                    this.previous_header_message = GlobalRes.CaseResolved;
                }
            }


            if(((rm._investigation_status == Constant.investigation_status_closed) && (rm._previous_investigation_status_id == Constant.investigation_status_completed)) ||((rm._investigation_status == Constant.investigation_status_closed) && (rm._previous_investigation_status_id == Constant.investigation_status_resolution)))
            {
                //resolved-closed  +  completed-closed

                this.show_previous_status = true;
                this.show_last_status = true;

                #region Previous
                ris = rm._previous_investigation_status();
                this.previous_sender_id = ris.user_id;
                temp_um = new UserModel(ris.user_id);
                this.previous_sender_name = temp_um._user.first_nm + " " + temp_um._user.last_nm;
                this.previous_sender_date = glb.ConvertDateToLongMonthString(ris.created_date);
                this.previous_status_message = ris.description;
                this.previous_investigation_status_number = ris.investigation_status_id;

                if ((rm._investigation_status == Constant.investigation_status_completed))
                {
                    this.previous_header_message = GlobalRes.CaseCantBeResolved;
                }
                else
                {
                    this.previous_header_message = GlobalRes.CaseResolved;
                }
                #endregion

                #region Last

                ris = rm._last_investigation_status();
                this.last_sender_id = ris.user_id;
                temp_um = new UserModel(ris.user_id);
                this.last_sender_name = temp_um._user.first_nm + " " + temp_um._user.last_nm;
                this.last_sender_date = glb.ConvertDateToLongMonthString(ris.created_date);
                this.last_status_message = ris.description;

                if ((rm._investigation_status == Constant.investigation_status_completed))
                {
                    this.last_header_message = GlobalRes.CaseCantBeResolved;
                }
                else
                {
                    this.last_header_message = GlobalRes.CaseResolved;
                }

                #endregion

            }

         
      /*      else if ((rm._investigation_status == Constant.investigation_status_spam) || ((rm._investigation_status == Constant.investigation_status_closed) && (rm._previous_investigation_status_id == Constant.investigation_status_spam)))
            {
                // spam, closed+spam
            }*/

         

            return vm_case;

        }
    }
}