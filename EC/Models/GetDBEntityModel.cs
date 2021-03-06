﻿using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models
{
    public class GetDBEntityModel
    {
        ECEntities db = new ECEntities();
        ///  Companies()? - remove 
        /////public void SaveFile(HttpPostedFileBase file, string path)   -- move
        /// 
        ///     AddReportDepartment?
        /// 
  
        //Used in EC\Views\Shared\EditorTemplates\CreateTaskModal.cshtml 
        public List<case_closure_reason> GetCaseClosureReasonsWithStatus(bool isCC)
        {
            //[company_nm]
            //return (from comp in db.company where comp.status_id == 2 select comp.company_nm, comp.id).ToList();
            var r = db.case_closure_reason.Where(item => item.status_id == 2).ToList();
            var other = r.FirstOrDefault(x => x.id == 5);
            r.Remove(other);
            r.Add(other);
            var unfounded = r.FirstOrDefault(x => x.id == 6);
            if (!isCC)
            {
                r.Remove(unfounded);
            }

            return r;
        }
 
     

        //used in
        //EC\Views\Case\Activity.cshtml 
        //EC\Views\Case\GetAjaxActivity.cshtml 
        //EC\Views\Case\Task.cshtml 
        //EC\Views\ReporterDashboard\Activity.cshtml
        public action GetActionById(int id)
        {
            return db.action.FirstOrDefault(item => item.id == id);
        }

        public List<secondary_type_mandatory> GetSecondaryTypeMandatory()
        {
          List<secondary_type_mandatory> types = new List<secondary_type_mandatory>();
          types = db.secondary_type_mandatory.Where(item => item.type_id == 1).ToList();
          return types;
        }

        public List<frequency> getFrequencies()
        {
            return db.frequency.ToList();
        }


        public List<country> getCountries()
        {
            return db.country.ToList();
        }

        public country getCountryById(int idCountry)
        {
          return db.country.Where(item => item.id == idCountry).FirstOrDefault();
        }

        public List<management_know> getManagamentKnow()
        {
            return db.management_know.ToList();
        }

        public List<relationship> getRelationships()
        {
            return db.relationship.ToList();
        }

        public List<priority> getPriorities()
        {
            return db.priority.ToList();
        }

        public List<injury_damage> GetInjuryDamages()
        {
            return db.injury_damage.ToList();
        }
        public List<reported_outside> getReportedOutside()
        {
            return db.reported_outside.ToList();
        }

        public List<role_in_report> getRoleInReport()
        {
          using (ECEntities adv = new ECEntities())
          {
            return adv.role_in_report.ToList();
          }
        }


        public List<company> GeCompaniesWithStatus()
        {
            //[company_nm]
            //return (from comp in db.company where comp.status_id == 2 select comp.company_nm, comp.id).ToList();
            return db.company.Where(item => item.status_id == 2).ToList();
        }
        public List<company> GeCompaniesWithStatusAndTerm(string term)
        {
            return db.company.Where(item => item.status_id == 2 && item.company_nm.ToLower().Contains(term.ToLower())).ToList();
        }

        public List<company> GeCompaniesWithStatus(string company_name)
        {
            if (company_name != null && company_name.Trim().Length >= 3)
                return db.company.Where(item => item.status_id == 2 && item.company_nm.ToLower().Trim().Contains(company_name.ToLower().Trim())).ToList();
            else
                return new List<company>();
        }

        public int GetCompanyByCode(string companyCode)
        {
            company getInfoCompany = db.company.FirstOrDefault(item => item.company_code.ToLower() == companyCode.ToLower());
            if (getInfoCompany != null)
            {
                return getInfoCompany.id;
            }
            else
            {
                return 0;
            }
        }

  }
}