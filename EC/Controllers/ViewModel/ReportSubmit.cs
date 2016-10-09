﻿using EC.Models;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Controllers.ViewModel
{
    public class ReportSubmit
    {
        public string reportingFrom { get; set; }//
        public string confidentialLevel { get; set; } //
        public string reportedAs { get; set; } //
        public string incidentLocation { get; set; } //
        public string affectedDepartments { get; set; }//


        public string managamentKnow { get; set; }
        public bool outOrganization { get; set; }
        public bool isCaseUrgent { get; set; }


        public string IncidentType { get; set; }
        public string IncidentDate { get; set; }
        public bool incidentOngoing { get; set; }
        public bool incidentResult { get; set; }
        public string incidentDescription { get; set; }


        public void merge(ReportViewModel rvm, CompanyModel companyModel, ReportModel reportModel, ReportViewModel model)
        {
            if(companyModel != null && rvm != null)
            {
                reportingFrom = companyModel._company.company_nm;
            }
            List<anonymity> list_anon = companyModel.GetAnonymities(companyModel._company.id, 0);
            if (list_anon != null)
            {
                var temp = from n in list_anon where n.id == rvm.incident_anonymity_id select n;
                anonymity anon = temp.FirstOrDefault();
                confidentialLevel = anon.anonymity_company_en;
            }

            List<company_relationship> relationship = reportModel.getCustomRelationshipCompany(companyModel._company.id);
            if(relationship!=null)
            {
                var temp = from n in relationship where n.id == model.reporterTypeDetail select n;
                company_relationship rel = temp.FirstOrDefault();
                if(rel!=null)
                {
                    reportedAs = rel.relationship_en;
                }
            }

            var locations = companyModel.LocationsOfIncident(companyModel._company.id).ToList();
            if(locations != null)
            {
                var temp = from n in locations where n.id == model.locationsOfIncident select n.location_en;
                if(temp !=null)
                {
                    //foreach(string item in temp) {
                    //    incidentLocation += ',' + item;
                    //}
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
            }

            List<management_know> managament = companyModel.getManagamentKnow();
            if(managament != null && model.managamentKnowId > 0)
            {
                var temp = from n in managament
                           where n.id == model.managamentKnowId
                           select n.text_en;
                managamentKnow = temp.FirstOrDefault();
                //management_know item = temp.FirstOrDefault();
            }
            //model.managamentKnowId

            //var departments = companyModel.CompanyDepartmentsActive(companyModel._company.id).ToList();
            //if(departments != null)
            //{
            //    var temp = from n in departments where n.id == model.
            //}
            int a = 10;
            //ViewBag.locationsOfIncident = HtmlDataHelper.MakeSelect(companyModel.LocationsOfIncident(id).ToList(), item => new HtmlDataHelper.SelectItem(item.id.ToString(), item.T("location")));
        }
    }
}