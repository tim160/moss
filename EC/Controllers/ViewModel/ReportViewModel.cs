using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using EC.Controllers.Utils;
using EC.Models.Database;
using System.Globalization;
using EC.Common.Util;


namespace EC.Controllers.ViewModel
{
    public class ReportViewModel : BaseViewModel
    {
        public bool isPreviusReported { get; set; }
        public string caseNumber { get; set; }
        public string caseDescription { get; set; }
        public int reportFrom { get; set; }
        public int incident_anonymity_id { get; set; }
        public int reporterTypeDetail { get; set; }
        public string companyRelationshipOther { get; set; }
        public int locationsOfIncident { get; set; }
        public string locationOfIncidentInput { get; set; }

        //hidden company
        public string currentCompany { get; set; }
        public string currentCompanySubmitted { get; set; }
        public int currentCompanyId { get; set; }

        public ICollection<string> personName { get; set; }
        public ICollection<string> personLastName { get; set; }
        public ICollection<string> personTitle { get; set; }
        public ICollection<int> personRole { get; set; }
        public int managamentKnowId { get; set; }
        public int isUrgent { get; set; }
        public string caseInformationReport { get; set; }
        public DateTime dateIncidentHappened { get; set; }
        public int incidentFrequency { get; set; }
        public string describeHappened { get; set; }
        public ICollection<int> whatHappened { get; set; }

        public int isOnGoing { get; set; }
        public bool witnessFlag { get; set; }

        public int incidentResultReport { get; set; }
        public string anyQuestions { get; set; }
        public string adviceManagement { get; set; }

        public string attachDocuments { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string userName { get; set; }
        public string userLastName { get; set; }
        public string userEmail { get; set; }
        public bool sendUpdates { get; set; }
        public string witnessNames { get; set; }
        public string witnessWillCooperate { get; set; }
        public string injury_damage { get; set; }
        public string management_know_text { get; set; }
        public string report_frequency_text { get; set; }
        public int reported_outside_id { get; set; }
        public string reported_outside_text { get; set; }
        public bool CustomSecondaryType { get; set; }
        public string caseInformationReportDetail { get; set; }

        // supervising mediators, who are not involved
        public string caseOwners { get; set; }
        // involved mediators - any mediators who were marked from 2 lists.
        public string caseMediatorsInvolved { get; set; }
        public bool report_by_myself { get; set; }

        #region 2 page


        [ListEntity("personName", typeof(string))]
        public List<string> personsNames { get; set; }

        [ListEntity("personLastName", typeof(string))]
        public List<string> personsLastNames { get; set; }

        [ListEntity("personTitle", typeof(string))]
        public List<string> personsTitles { get; set; }
        [ListEntity("personRole", typeof(string))]
        public List<string> personsRoles { get; set; }

        [ListEntity("departmentInvolved", typeof(string))]
        public List<string> departments { get; set; }


        [ListEntity("caseMediatorsInvolved", typeof(string))]
        public List<string> mediatorsInvolved { get; set; }

        [ListEntity("caseOwners", typeof(string))]
        public List<string> mediatorsOwners { get; set; }
        #endregion


        public report Merge(report data)
        {
            data.company_nm = currentCompany;
            data.submitted_company_nm = currentCompanySubmitted;
            data.company_id = currentCompanyId;
            //data.reported_dt = DateTime.Today;
            //data.incident_dt
            //var a = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd hh:mm:ss", null);

            data.assign_company_flag = 0;
            data.display_name = "";
            data.report_name_generic = "";
            data.client_id = 1;
            data.status_id = 2;
            data.priority_id = isUrgent;
            data.injury_damage = injury_damage;
            data.management_know_text = management_know_text;
            // data.refferred_outside_organization = "";
            data.outcome_action_ds = null;
            data.reporter_user_id = 2;
            data.reported_dt = DateTime.Now;
            data.type_id = 2;
            data.other_department_name = "";
            //            data.other_secondary_type_name = whatHappened;
            data.last_update_dt = DateTime.Now;
            data.ip = "";
            data.previously_reported_accepted = false;
            data.report_frequency_text = report_frequency_text;
            data.reported_outside_id = reported_outside_id;
            data.reported_outside_text = reported_outside_text;

            data.previously_reported = isPreviusReported;
            if (isPreviusReported)
            {
                data.previously_reported_report_number = caseNumber;
                data.previously_reported_report_description = caseDescription;
            }
            else
            {
                data.incident_anonymity_id = incident_anonymity_id;
                //incident_anonymity_id = data.incident_anonymity_id; 
                data.reporter_country_id = reportFrom;
                if (locationsOfIncident > 0)
                {
                    data.location_id = locationsOfIncident;
                }
                else
                {
                    data.other_location_name = locationOfIncidentInput;
                }
                //data.incident_anonymity_id = incident_anonymity_id;непонятны зачем они ...в моем случе обнуляло переменную



                data.company_relationship_id = reporterTypeDetail;
                if (reporterTypeDetail == 0 || companyRelationshipOther != null)
                {
                    data.not_current_employee = companyRelationshipOther;
                }

                //next page

                /**
                 * Does management know about this incident?
                 * */
                data.management_know_id = managamentKnowId;
                //next page
                //You are reporting
                data.incident_dt = dateIncidentHappened;

                data.is_ongoing = isOnGoing;

                if (isOnGoing > 0)
                {
                    data.report_frequency_id = incidentFrequency;
                }
                data.description = describeHappened;
                data.witness_flag = witnessFlag;
                if (witnessFlag)
                {
                    data.witness_names = witnessNames;
                    data.witness_will_cooperate = witnessWillCooperate;
                }
                //incidentResultReport;
                data.injury_damage_id = incidentResultReport;

            }
            return data;
        }

        public List<report_department> GetReportDepartment(int reportId)
        {
            List<report_department> result = new List<report_department>();
            if (reportId > 0)
            {
                for (int i = 0; i < departments.Count; i++)
                {
                    int temp = 0;
                    try
                    {
                        temp = Convert.ToInt32(departments[i]);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Input string is not a sequence of digits.");
                    }
                    if (temp > 0)
                    {
                        result.Add(new report_department()
                        {
                            department_id = temp,
                            report_id = reportId
                        });
                    }
                }
            }
            return result;

        }
        public List<report_non_mediator_involved> GetModeMediatorInvolveds()
        {
            List<report_non_mediator_involved> result = new List<report_non_mediator_involved>();

            if (personsRoles != null)
            {

                for (int i = 0; i < personsRoles.Count; i++)
                {
                    result.Add(new report_non_mediator_involved()
                    {
                        Role = personsRoles[i],
                        Name = personsNames[i],
                        last_name = personsLastNames[i],
                        Title = personsTitles[i]
                    });
                }
            }
            return result;
        }


        // mediators of the report
        public List<report_mediator_involved> GetReportInvolvedMediators(int reportId)
        {
            List<report_mediator_involved> result = new List<report_mediator_involved>();
            if (reportId > 0)
            {
                for (int i = 0; i < mediatorsInvolved.Count; i++)
                {
                    int temp = 0;
                    try
                    {
                        temp = Convert.ToInt32(mediatorsInvolved[i]);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Input string is not a sequence of digits.");
                    }
                    if (temp > 0)
                    {
                        result.Add(new report_mediator_involved()
                        {
                            mediator_id = temp,
                            report_id = reportId
                        });
                    }
                }
            }
            return result;

        }
        public List<report_owner> GetReportOwners(int reportId)
        {
            List<report_owner> result = new List<report_owner>();
            if (reportId > 0)
            {
                for (int i = 0; i < mediatorsOwners.Count; i++)
                {
                    int temp = 0;
                    try
                    {
                        temp = Convert.ToInt32(mediatorsOwners[i]);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Input string is not a sequence of digits.");
                    }
                    if (temp > 0)
                    {
                        result.Add(new report_owner()
                        {
                            user_id = temp,
                            report_id = reportId
                        });
                    }
                }
            }
            return result;

        }

        public List<report_mediator_assigned> GetReportAssignedMediators(int reportId)
        {
            List<report_mediator_assigned> result = new List<report_mediator_assigned>();
            if (reportId > 0)
            {
                for (int i = 0; i < mediatorsInvolved.Count; i++)
                {
                    int temp = 0;
                    try
                    {
                        temp = Convert.ToInt32(mediatorsInvolved[i]);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Input string is not a sequence of digits.");
                    }
                    if (temp > 0)
                    {
                        result.Add(new report_mediator_assigned()
                        {
                            mediator_id = temp,
                            report_id = reportId
                        });
                    }
                }
            }
            return result;

        }


    }
}

