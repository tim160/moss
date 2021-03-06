﻿using EC.Models;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Localization;

namespace EC.Controllers.ViewModel
{
    public class ReportSubmit
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }


        public string reportingFrom { get; set; }//
        public string confidentialLevel { get; set; } //]
        public int confidentialLevelInt { get; set; }
        public string reportedAs { get; set; } //
        public string incidentLocation { get; set; } //
        public string affectedDepartments { get; set; }//


        public string report_by_myself { get;set; } 
        public string managamentKnow { get; set; } //
        public string outOrganization { get; set; }//
        public string isCaseUrgent { get; set; } //

        public List<string> personName { get; set; }
        public List<string> personLastName { get; set; }
        public List<string> personTitle { get; set; }
        public List<string> personRole { get; set; }


        public string IncidentType { get; set; } //
        public string IncidentDate { get; set; } //
        public string incidentOngoing { get; set; } //
        public string incidentResult { get; set; } //
        public string incidentDescription { get; set; } //


        public List<string> namesFile { get; set; }
        public string attachFiles { get; set; }
        public ReportModelResult result { get; set; }

        GetDBEntityModel getDBEntityModel = new GetDBEntityModel();
        public void merge(ReportViewModel rvm, CompanyModel companyModel, ReportModel reportModel, ReportViewModel model)
        {
            this.confidentialLevelInt = model.incident_anonymity_id;

            if (model.incident_anonymity_id != 1)
            {
                name = model.userName;
                surname = model.userLastName;
                email = model.userEmail;
            }

            if(companyModel != null && rvm != null)
            {
                country selectedCountry = getDBEntityModel.getCountryById(model.reportFrom);
                if(selectedCountry != null)
                    reportingFrom = selectedCountry.country_nm;
            }
            List<anonymity> list_anon = companyModel.GetAnonymities(companyModel._company.id, 0);
            if (list_anon != null)
            {
                var temp = from n in list_anon where n.id == model.incident_anonymity_id select n;
                anonymity anon = temp.FirstOrDefault();
                confidentialLevel = anon.anonymity_company_en;
            }

            CompanyModel cm = new CompanyModel(companyModel._company.id);
            List<company_relationship> relationship = cm.GetCustomRelationshipCompany();
            if(relationship!=null)
            {
                var temp = from n in relationship where n.id == model.reporterTypeDetail select n;
                company_relationship rel = temp.FirstOrDefault();
                if(rel!=null)
                {
                    reportedAs = rel.relationship_en;
                }
            }

            var locations = companyModel.Locations(companyModel._company.id).ToList();
            if(locations != null)
            {
                var temp = from n in locations where n.id == model.locationsOfIncident select n.location_en;
                if(temp !=null)
                {
                    incidentLocation = String.Join(", ", temp);
                } 
            }

            List<company_department> depActive = companyModel.CompanyDepartmentsActive(companyModel._company.id).ToList();
            List<string> departments = model.departments;
            List<int> departmentsInt = new List<int>();
            foreach(string item in departments)
            {
                try
                {
                    int temp = 0;
                    Int32.TryParse(item, out temp);
                    if (temp > 0)
                    {
                        departmentsInt.Add(temp);
                    } else if(temp == 0)
                    {
                        departmentsInt.Add(0);
                    }
                }
                catch (Exception) { }
            }
            if(depActive!=null && depActive.Count > 0 && departments !=null && departments.Count > 0)
            {
                var temp = from n in depActive
                           join entry in departmentsInt
                           on n.id equals entry
                           select n.department_en;
                foreach(string item in temp)
                {
                    affectedDepartments = String.Join(", ", item);
                }
                if(affectedDepartments == null)
                {
                    affectedDepartments = LocalizationGetter.GetString("notListed");
                }
            }

            List<management_know> managament = getDBEntityModel.getManagamentKnow();
            if(managament != null && model.managamentKnowId > 0)
            {
                var temp = from n in managament
                           where n.id == model.managamentKnowId
                           select n.text_en;
                managamentKnow = temp.FirstOrDefault();
            }
            List<reported_outside> reported_outside = getDBEntityModel.getReportedOutside();
            if(model.reported_outside_id > 0 && reported_outside!=null)
            {
                var temp = from n in reported_outside where n.id == model.reported_outside_id select n;
                outOrganization = temp.FirstOrDefault().description_en;
            }

            if(model.isUrgent == 0)
            {
                isCaseUrgent = LocalizationGetter.GetString("No");
            } else
            {
                isCaseUrgent = LocalizationGetter.GetString("Yes");
            }
            CompanyModel cmModel = new CompanyModel(model.currentCompanyId);
            if (cmModel.IsCustomIncidentTypes())
            {
                /*custom types*/
                
                List<company_secondary_type> company_secondary_type = cmModel.GettCompanySecondaryType();
                foreach(int item in model.whatHappened)
                {
                    if(item != 0)
                    {
                        var something = company_secondary_type.Where(m => m.id == item).FirstOrDefault();
                        IncidentType += something.secondary_type_en + ", ";
                    } else
                    {
                        IncidentType += LocalizationGetter.GetString("Other") + ", ";
                    }
                }
            }
            else
            {
                /*default*/
                List<secondary_type_mandatory> secondary_type_mandatory = getDBEntityModel.GetSecondaryTypeMandatory();
                foreach (int item in model.whatHappened)
                {

                    if (item != 0)
                    {
                        var something = secondary_type_mandatory.Where(m => m.id == item).FirstOrDefault();
                        IncidentType += something.secondary_type_en + ", ";
                    }
                    else
                    {
                        IncidentType += LocalizationGetter.GetString("Other") + ", ";
                    }
                }
            }
            IncidentType = IncidentType.Remove(IncidentType.Length - 2);

            IncidentDate = model.dateIncidentHappened.ToShortDateString();
            switch (model.isOnGoing)
            {
                case 1:
                    incidentOngoing = LocalizationGetter.GetString("No");
                    break;
                case 2:
                    incidentOngoing = LocalizationGetter.GetString("Yes");
                    break;
                case 3:
                    incidentOngoing = LocalizationGetter.GetString("NotSureUp");
                    break;
            }



            List<injury_damage> injuryDamage = getDBEntityModel.GetInjuryDamages().ToList();
            if(injuryDamage!=null && injuryDamage.Count > 0 && model.incidentResultReport > 0)
            {
                var temp = from n in injuryDamage where n.id == model.incidentResultReport select n;
                incidentResult = temp.FirstOrDefault().text_en;
            }


            incidentDescription = model.describeHappened;

            if(model.files.Count > 0)
            {
                HttpFileCollectionBase files = model.files;
                var fileItem = files["attachDocuments"];
                if(fileItem!=null)
                {
                    attachFiles = fileItem.FileName;
                }
            }
            this.report_by_myself = model.report_by_myself == true ? LocalizationGetter.GetString("Myself") : LocalizationGetter.GetString("SomeoneElse");

            this.personName = new List<string>();
            this.personLastName = new List<string>();
            this.personTitle = new List<string>();
            this.personRole = new List<string>();
            var personRoles = new List<int>();
            if (model.personName != null)
            {
                this.personName = model.personName.ToList();
                this.personLastName = model.personLastName.ToList();
                this.personTitle = model.personTitle.ToList();
                personRoles = model.personRole.ToList();
            }


            List<role_in_report> roleInReport = getDBEntityModel.getRoleInReport();
            for(int i=0; i< personRoles.Count; i++)
            {
                role_in_report nameRole = roleInReport.Where(m => m.id == personRoles[i]).FirstOrDefault();
                if (nameRole != null)
                {
                    personRole.Add(nameRole.role_en);
                }
            }
        }
    }
}