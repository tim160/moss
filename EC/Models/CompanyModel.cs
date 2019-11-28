using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Controllers.ViewModel;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Models.Utils;
using System.Data.Entity.Migrations;
using EC.Constants;
using EC.Model.Interfaces;
using EC.Model.Impl;
using EC.Models.ViewModels;
using EC.Localization;


namespace EC.Models
{
  public class CompanyModel : BaseModel
  {
    //public static CompanyModel inst = new CompanyModel();

    #region Properties
    public int ID
    { get; set; }

    public company _company;


    /*   public string _company_subdomain
       {
           get
           {
               string name = "registration";

               if (_company != null)
               {
                   if (!string.IsNullOrEmpty(_company.subdomain))
                   {
                       name = _company.subdomain.Trim();
                   }
               }
               name = "http://" + name + ".employeeconfidential.com";

               return name;
           }
       }*/

    #endregion

    public CompanyModel()
    {
    }

    public CompanyModel(int id)
    {
      ID = id;
      _company = GetById(id);
    }
    public CompanyModel(string link)
    {
      ID = 0;
      if (db.company.Any(item => item.subdomain.ToLower().Trim() == link.ToLower().Trim()))
      {
        if (db.company.Where(item => item.subdomain.ToLower().Trim() == link.ToLower().Trim()).ToList().Count() == 1)
        {
          company _link_company = db.company.FirstOrDefault(item => item.subdomain.ToLower().Trim() == link.ToLower().Trim());
          ID = _link_company.id;
          _company = _link_company;
        }
      }
    }


    public List<company_location> Locations(int companyId, int? statusId = null)
    {
      if (statusId.HasValue)
        return db.company_location.Where(s => (s.company_id == companyId && s.status_id == statusId)).OrderBy(t => t.location_en).ToList();
      else
        return db.company_location.Where(s => s.company_id == companyId).OrderBy(t => t.location_en).ToList();
    }

    public List<company_outcome> Outcomes(int companyId, int? statusId = null)
    {
      List<company_outcome> list = new List<company_outcome>();
      if (statusId.HasValue)
        list = db.company_outcome.Where(s => (s.company_id == companyId && s.status_id == statusId && s.outcome_en.ToLower() != "none")).OrderBy(t => t.outcome_en).ToList();
      else
        list = db.company_outcome.Where(s => s.company_id == companyId && s.outcome_en.ToLower() != "none").OrderBy(t => t.outcome_en).ToList();

      var none_outcome = db.company_outcome.Where(s => s.company_id == companyId && s.status_id == 2 && s.outcome_en.ToLower() == "none").SingleOrDefault();
      if (none_outcome != null)
      {
        list.Insert(0, none_outcome);
      }


      return list;

    }


    public List<user> AllMediators(int companyId, bool isActiveOnly, int? roleId)
    {
      List<int> role_ids = new List<int>();
      role_ids.Add(4);
      role_ids.Add(5);
      role_ids.Add(6);
      role_ids.Add(7);

      return roleId == null
        ? isActiveOnly
                 ? db.user.Where(s => s.company_id == companyId && s.status_id == ECStatusConstants.Active_Value && role_ids.Contains(s.role_id)).ToList()
                 : db.user.Where(s => s.company_id == companyId && role_ids.Contains(s.role_id)).ToList()

         : isActiveOnly
                 ? db.user.Where(s => s.company_id == companyId && s.role_id == roleId.Value && s.status_id == ECStatusConstants.Active_Value && role_ids.Contains(s.role_id)).ToList()
                 : db.user.Where(s => s.company_id == companyId && s.role_id == roleId.Value && role_ids.Contains(s.role_id)).ToList();

    }

    public List<user> AllSupervisingMediators(int companyId, bool isActiveOnly)
    {
      return AllMediators(companyId, isActiveOnly, 5);
    }

    public List<user> AllNonSupervisingMediators(int companyId, bool isActiveOnly)
    {
      List<user> EscalationMediators = AllMediators(companyId, isActiveOnly, 4);
      List<user> RegularMediators = AllMediators(companyId, isActiveOnly, 6);
      List<user> NonSupervisingMediators = (EscalationMediators.Union(RegularMediators)).ToList();

      return NonSupervisingMediators;
    }

    public List<user> MediatorsAvailableForSignOff(int companyId)
    {
      List<user> mediatorsList = new List<user>();

      List<int> role_ids = new List<int>();
      role_ids.Add(4);
      role_ids.Add(6);

      mediatorsList = db.user.Where(s => s.company_id == companyId && ((role_ids.Contains(s.role_id) && s.status_id == 2 && s.user_permissions_approve_case_closure.HasValue && s.user_permissions_approve_case_closure.Value == 1) || s.role_id == ECLevelConstants.level_supervising_mediator)).ToList();

      return mediatorsList;




    }


    public List<company_department> CompanyDepartments(int companyId)
    {
      return db.company_department.Where(s => s.company_id == companyId).OrderBy(t => t.department_en).ToList();
    }
    public List<company_department> CompanyDepartmentsActive(int companyId)
    {
      return db.company_department.Where(s => s.company_id == companyId && s.status_id == 2).OrderBy(t => t.department_en).ToList();
    }

    /// <summary>
    /// Company Relationship in DB
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="statusId"></param>
    /// <returns></returns>
    public List<DictionaryEntry> CompanyReporterTypes(int companyId, int? statusId = null)
    {
      Hashtable _reporter_type = new Hashtable();


      if (db.company_relationship.Any(o => o.company_id == companyId))
      {
        List<company_relationship> _list = db.company_relationship.Where(s => s.company_id == companyId && s.status_id == 2).OrderBy(t => t.relationship_en).ToList();
        foreach (company_relationship cst in _list)
        {
          if (statusId.HasValue)
          {
            if (cst.status_id == statusId)
              _reporter_type.Add(cst.id, cst.relationship_en);
          }
          else
            _reporter_type.Add(cst.id, cst.relationship_en);
        }
      }
      else
      {
        List<relationship> _list = db.relationship.ToList();
        foreach (relationship stm in _list)
        {
          _reporter_type.Add(stm.id, stm.relationship_en);
        }
      }
      return _reporter_type.Cast<DictionaryEntry>().OrderBy(entry => entry.Value).ToList();
    }

    public List<DictionaryEntry> CompanyIncidentTypes(int companyId, int? statusId = null)
    {
      Hashtable _incident_types = new Hashtable();

      if (db.company_secondary_type.Any(o => o.company_id == companyId))
      {
        List<company_secondary_type> _list = db.company_secondary_type.Where(s => s.company_id == companyId && s.status_id == 2).OrderBy(t => t.secondary_type_en).ToList();
        foreach (company_secondary_type cst in _list)
        {
          if (statusId.HasValue)
          {
            if (cst.status_id == statusId)
              //_location.Add(cst.type_id, cst.secondary_type_en); I change it for id - need to delete item
              _incident_types.Add(cst.id, cst.secondary_type_en);
          }
          else
            //_location.Add(cst.type_id, cst.secondary_type_en);I change it for id - need to delete item
            _incident_types.Add(cst.id, cst.secondary_type_en);
        }
      }
      else
      {
        List<secondary_type_mandatory> _list = db.secondary_type_mandatory.Where(s => s.type_id == 1).OrderBy(s => s.weight).ToList();
        foreach (secondary_type_mandatory stm in _list)
        {
          if (statusId.HasValue)
          {
            if (stm.status_id == statusId)
              _incident_types.Add(stm.id, stm.secondary_type_en);
          }
          else
            _incident_types.Add(stm.id, stm.secondary_type_en);
        }
      }
      return _incident_types.Cast<DictionaryEntry>().OrderBy(entry => entry.Value).ToList();

    }


    public company GetById(int id)
    {
      ID = id;

      if (id != 0)
      {
        return db.company.First(item => item.id == id);
      }
      else
        return null;

    }

    public void SaveFile(HttpPostedFileBase file, string path)
    {
      file.SaveAs(path);
    }

    public List<AnonimityViewModel> getCA(int companyId, int country_id)
    {
      List<anonymity> list = GetAnonymities(companyId, country_id);
      List<company_anonymity> company = db.company_anonymity.Where(item => item.company_id == companyId).ToList();
      List<AnonimityViewModel> listAnonimities = new List<AnonimityViewModel>();
      if (company.Count > 0)
      {
        foreach (anonymity item in list)
        {
          foreach (company_anonymity itemCompany in company)
          {
            AnonimityViewModel model = new AnonimityViewModel();
            model.anonymity = item;
            if (item.id == itemCompany.anonymity_id && itemCompany.status_id == 2)
            {

              model.isChecked = true;
              listAnonimities.Add(model);
            }
            else if (item.id == itemCompany.anonymity_id)
            {
              model.isChecked = false;
              listAnonimities.Add(model);
            }

          }
        }
      }

      return listAnonimities;
    }
    public List<anonymity> GetAnonymities(int companyId, int country_id)
    {
      List<anonymity> anonymities;
      if (db.company_anonymity.Any(o => o.company_id == companyId))
      {
        // Return anonymity from company_anonymity table
        var companyAnonymitiesId = (from ca in db.company_anonymity
                                    where (ca.company_id == companyId)
                                    select ca.anonymity_id).ToList();
        anonymities = db.anonymity
            //.AsNoTracking()
            .Where(s => companyAnonymitiesId.Contains(s.id))
            .ToList();
      }
      else
      {
        // Return all anonymity from anonymity table
        anonymities = db.anonymity.ToList();
        if ((country_id == 171) || (country_id == 198))
        {
          anonymities = anonymities.Where(s => s.id != 1).ToList();
        }
      }
      return anonymities;
    }


    public bool addLogoCompany(int companyCode, string pathLogo)
    {
      try
      {
        company logoCompany = db.company.FirstOrDefault(item => (item.id == companyCode));
        logoCompany.path_en = pathLogo;
        logoCompany.path_fr = pathLogo;
        logoCompany.path_es = pathLogo;
        logoCompany.path_ar = pathLogo;
        db.company.AddOrUpdate(logoCompany);
        db.SaveChanges();
      }
      catch (System.Data.DataException ex)
      {
        logger.Error(ex.ToString());
        return false;
      }
      return true;
    }

    public string getLogoCompany(int id = 0)
    {
      if (id == 0)
      {
        return null;
      }
      return db.company.Where(m => m.id == id).Select(m => m.path_en).FirstOrDefault();
    }


    public company_disclamer_page DisclamerPage()
    {
      return db.company_disclamer_page.FirstOrDefault(x => x.company_id == ID) ?? new company_disclamer_page { };
    }

    public List<company_disclamer_uploads> DisclamerUploads()
    {
      return db.company_disclamer_uploads.Where(x => x.company_id == ID).ToList();
    }

    public List<AllPaymentTypesViewModel> AllPayments()
    {
      List<AllPaymentTypesViewModel> all_payments = new List<AllPaymentTypesViewModel>();

      var annual_company_payments = db.company_payments.Where(x => x.company_id == ID).ToList();
      var onboarding_company_payments = db.company_payment_training.Where(x => x.company_id == ID).ToList();
      if (annual_company_payments.Count > 0)
      {
        all_payments = annual_company_payments.Select(a => new AllPaymentTypesViewModel() { auth_code = a.auth_code, payment_date = a.payment_date, amount = a.amount, local_invoice_number = a.local_invoice_number, description = "License Payment", receipt_url = a.stripe_receipt_url }).ToList();
      }
      if (onboarding_company_payments.Count > 0)
      {
        var temp = onboarding_company_payments.Select(a => new AllPaymentTypesViewModel() { auth_code = a.auth_code, payment_date = a.payment_date, amount = a.amount, local_invoice_number = a.local_invoice_number, description = "Onboarding Payment", receipt_url = a.stripe_receipt_url }).ToList();
        all_payments.AddRange(temp);
      }
      return all_payments.OrderByDescending(t => t.payment_date).ToList();
    }


    //// GetAllUserReportIdsLists  -- use here 

    /// <summary>
    ///  List of dependent companies
    /// </summary>
    /// <returns></returns>
    public List<company> AdditionalCompanies()
    {
      List<company> initial = db.company.Where(x => x.id == ID).ToList();
      var additional_companies = db.company.Where(x => x.client_id == ID).ToList();
      foreach (company cm in additional_companies)
      {
        initial.Add(cm);
      }
      return initial;
    }


    /// <summary>
    /// if company_type is AllCompanies - default ,Also can add AllMembers, AllColleges, etc
    /// </summary>
    /// <returns></returns>
    public string ChildCompaniesType()
    {
      string company_dropdown = LocalizationGetter.GetString("AllCompanies");
      if (_company != null && _company.parent_type_id != null)
      {
        if (_company.parent_type_id == 2)
          company_dropdown = LocalizationGetter.GetString("AllCHURCHES");
        if (_company.parent_type_id == 3)
          company_dropdown = LocalizationGetter.GetString("AllColleges");
      }

      return company_dropdown;
    }


    //CompanyIncidentTypes  - check if we could merge with
    public List<company_secondary_type> GettCompanySecondaryType()
    {
      List<company_secondary_type> types = new List<company_secondary_type>();
      types = db.company_secondary_type.Where(item => item.company_id == ID && item.status_id == 2).ToList();
      return types;
    }

    public List<company_relationship> GetCustomRelationshipCompany()
    {

      /*List<company_relationship> relationShipCompany = new List<company_relationship>();
      relationShipCompany = db.company_relationship.Where(item => item.company_id == idCompany && item.status_id == 2).ToList();*/
      List<company_relationship> relationShipCompany = db.company_relationship.Where(item => item.company_id == ID && item.status_id == 2).ToList();
      return relationShipCompany;
    }

    public bool IsCustomIncidentTypes()
    {
      bool flag = false;
      int array = 0;
      array = db.company_secondary_type.Where(item => item.company_id == ID && item.status_id == 2).Count();
      if (array > 0)
      {
        flag = true;
      }
      return flag;
    }
  }
}