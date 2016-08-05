using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;

namespace EC.Controllers.ViewModel
{
    public class CaseSubmittedViewModel : BaseViewModel
    {
        public string ReportingFrom { get; set; }
        public string anonymityStatus { get; set; }
        public string reporterTypeDetail { get; set; }
        public string locationsOfIncident { get; set; }
        public string departmentInvolved { get; set; }

        //Parties involved
        public string managementKnow { get; set; }
        public string reported_outside_id { get; set; }
        //public string reported_outside_idOther { get; set; }

        public string isUrgent { get; set; }

        //Case Information
        public string caseInformationReport { get; set; }
        public string dateIncidentHappened { get; set; }
        public string isOnGoing { get; set; }
        public string describeHappened { get; set; }
        //public string incidentResultReport { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public CaseSubmittedViewModel merge(ReportViewModel model, report currentReport)
        {
            CaseSubmittedViewModel temp = new CaseSubmittedViewModel();
            if(model!=null)
            {
                try
                {
                    ReportingFrom = db.country.Where(item => item.id == model.reportFrom).FirstOrDefault().country_nm;
                    anonymityStatus = db.anonymity.Where(item => item.id == model.incident_anonymity_id).FirstOrDefault().anonymity_en;
                    reporterTypeDetail = db.company_relationship.Where(item => item.id == model.reporterTypeDetail).FirstOrDefault().relationship_en;
                    locationsOfIncident = db.company_location.Where(item => item.id == model.locationsOfIncident).FirstOrDefault().location_en;
                    int tempDepartmnt = db.report_department.Where(item => item.report_id == currentReport.id).FirstOrDefault().department_id;
                    departmentInvolved = db.company_department.Where(item => item.id == tempDepartmnt).FirstOrDefault().department_en;

                    managementKnow = db.management_know.Where(item => item.id == model.managamentKnowId).FirstOrDefault().text_en;
                    if(model.reported_outside_id == 6)
                    {
                        reported_outside_id = "Other";
                    } else
                    {
                        reported_outside_id = db.reported_outside.Where(item => item.id == model.reported_outside_id).FirstOrDefault().description_en;
                    }
                    
                    isUrgent = db.priority.Where(item => item.id == model.isUrgent).FirstOrDefault().priority_en;
                    caseInformationReport = model.caseInformationReport;
                    dateIncidentHappened = model.dateIncidentHappened.ToShortDateString();
                    if (model.isOnGoing == 1)
                    {
                        isOnGoing = EC.App_LocalResources.GlobalRes.No;
                    }
                    else if (model.isOnGoing == 2)
                    {
                        isOnGoing = EC.App_LocalResources.GlobalRes.Yes;// +" Description : " + _report.report_frequency_text;
                    }
                    else if (model.isOnGoing == 3)
                    {
                        isOnGoing = EC.App_LocalResources.GlobalRes.NotSureUp;
                    }
                    describeHappened = model.describeHappened;
                    this.Login = model.userName;
                    this.Password = model.password;
                    if(model.userEmail == String.Empty)
                    {
                        this.Email = "confidentiality level: Anonymous";
                    } else
                    {
                        this.Email = model.userEmail;
                    }
                } catch (Exception) { }
                //incidentResultReport = 
            }
            return temp;
        }
    }
}