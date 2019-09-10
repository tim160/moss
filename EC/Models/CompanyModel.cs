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

namespace EC.Models
{
    public class CompanyModel : BaseModel
    {
        //public static CompanyModel inst = new CompanyModel();

        #region Properties
        public int ID
        { get; set; }

        public company _company
        {
            get
            {
                return GetById(ID);
            }
        }

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
                }
            }
        }

        /*
        public List<ECModel.Location> CompanyLocations(int company_id, int? language_id)
        {
            List<ECModel.Location> company_locations = new List<ECModel.Location>();
            List<Database.company_location> _locations_list = new List<Database.company_location>();
            Location _new_location;

            _locations_list = db.company_location.Where(s => s.company_id == company_id).ToList();
            
            for (int i = 0; i < _locations_list.Count; i++)
            {
                _new_location = new Location(_locations_list[i].id, language_id);
                company_locations.Add(_new_location);
            }

            return company_locations;

        }
         */

        public List<company_location> Locations(int companyId, int? statusId = null)
        {
            if(statusId.HasValue)
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

        /*
        public List<ECModel.User> Company_All_Mediators(int company_id, bool active_only_flag, int? role_id)
        {
            List<User> users = new List<User>();
            List<Database.user> _users_list = new List<Database.user>();
            User _new_user;

            if (role_id == null)
            {
                // returns all or only active mediators from current company
                if (active_only_flag == false)
                    _users_list = db.user.Where(s => s.company_id == company_id).ToList();
                else
                    _users_list = db.user.Where(s => s.company_id == company_id && s.status_id == Constant.status_active).ToList();
            }
            else
            {
                if (active_only_flag == false)
                    _users_list = db.user.Where(s => s.company_id == company_id && s.role_id == role_id.Value).ToList();
                else
                    _users_list = db.user.Where(s => s.company_id == company_id && s.role_id == role_id.Value).ToList();
            }

            for (int i = 0; i < _users_list.Count; i++)
            {
                _new_user = new User(_users_list[i].id);
                users.Add(_new_user);
            }

            return users;
        }
 */

        public List<user> AllMediators(int companyId, bool isActiveOnly, int? roleId)
        {
            List<int> role_ids = new List<int>();
            role_ids.Add(4);
            role_ids.Add(5);
            role_ids.Add(6);
            role_ids.Add(7);

         return  roleId == null 
           ? isActiveOnly
                    ? db.user.Where(s => s.company_id == companyId && s.status_id == ECGlobalConstants.status_active && role_ids.Contains(s.role_id)).ToList()
                    : db.user.Where(s => s.company_id == companyId && role_ids.Contains(s.role_id)).ToList()
            
            : isActiveOnly
                    ? db.user.Where(s => s.company_id == companyId && s.role_id == roleId.Value && s.status_id == ECGlobalConstants.status_active && role_ids.Contains(s.role_id)).ToList()
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
        /*
          public List<ECModel.Anonymity> CompanyAnonimities(int company_id, int? language_id)
          {
            List<ECModel.Anonymity> company_anonymities = new List<ECModel.Anonymity>();
            List<Database.anonymity> _anonymities_list = new List<Database.anonymity>();
            Anonymity _new_anonymity;

            if (db.company_anonymity.Any(o => o.company_id == company_id))
            {
                // Return anonymity from company_anonymity table
                List<int> company_anonymities_id = (from _ca in db.company_anonymity
                                                    where (_ca.company_id == company_id)
                                                    select _ca.anonymity_id).ToList();

                _anonymities_list = db.anonymity.Where(s => company_anonymities_id.Contains(s.id)).ToList();
            }
            else
            {
                // Return all anonymity from anonymity table
                _anonymities_list = db.anonymity.ToList();
            }
            
            for (int i = 0; i < _anonymities_list.Count; i++)
            {
                _new_anonymity = new Anonymity(_anonymities_list[i].id, language_id);
                company_anonymities.Add(_new_anonymity);
            }

             return company_anonymities;

        } 
         
         */

        /*      
        public List<ECModel.Department> CompanyDepartments(int company_id, int? language_id)
        {
            List<ECModel.Department> company_departments = new List<ECModel.Department>();
            List<Database.company_department> _departments_list = new List<Database.company_department>();
            Department _new_department;

            _departments_list = db.company_department.Where(s => s.company_id == company_id).ToList();

            for (int i = 0; i < _departments_list.Count; i++)
            {
                _new_department = new Department(_departments_list[i].id, language_id);
                company_departments.Add(_new_department);
            }

            return company_departments;

        }
         */
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
        public List<company> Companies()
        {
            return db.company.ToList();
        }

        public void SaveFile(HttpPostedFileBase file, string path)
        {
            file.SaveAs(path);
        }

        public List<frequency> getFrequencies()
        {
            return db.frequency.ToList();
        }
        public List<AnonimityViewModel> getCA(int companyId, int country_id)
        {
            List<anonymity> list= GetAnonymities(companyId, country_id);
            List<company_anonymity> company = db.company_anonymity.Where(item => item.company_id == companyId).ToList();
            List<AnonimityViewModel> listAnonimities = new List<AnonimityViewModel>();
            if (company.Count > 0)
            {
                foreach (anonymity item in list)
                {
                    foreach(company_anonymity itemCompany in company)
                    {
                        AnonimityViewModel model = new AnonimityViewModel();
                        model.anonymity = item;
                        if (item.id == itemCompany.anonymity_id && itemCompany.status_id == 2)
                        {

                            model.isChecked = true;
                            listAnonimities.Add(model);
                        }
                        else if(item.id == itemCompany.anonymity_id)
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
                   anonymities = anonymities.Where(s=> s.id != 1).ToList();
                }
            }
            return anonymities;
        }
        public List<country> getCountries()
        {
            return db.country.ToList();
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

        public user GetUser(int id)
        {
            return db.user.FirstOrDefault(item => item.id == id);
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
            if (company_name !=null && company_name.Trim().Length >= 3)
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


        public bool hideLocationCompany(int companyCode, int userId, string nameLocation)
        {
            bool status = false; 
            try
            {
                if (companyCode != 0)
                {
                    company_location location = db.company_location.FirstOrDefault(item => (item.company_id == companyCode && item.location_en == nameLocation));

                    if (location != null)
                    {
                        location.status_id = 1;
                    }

                    db.company_location.AddOrUpdate(location);
                    db.SaveChanges();
                    status = true;
                }
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
            return status;
        }

        public bool addDepartmentCompany(int companyCode, int userId, string nameDepartment)
        {
            try
            {
                if (companyCode != 0)
                {
                    company_department department = new company_department
                    {
                        client_id = userId,
                        company_id = companyCode,
                        status_id = 2,
                        department_en = nameDepartment,
                        last_update_dt = DateTime.Now,
                        user_id = 1
                    };
                    db.company_department.Add(department);
                    db.SaveChanges();
                }
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
            return true;
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
        public company_relationship getOtherRelationship()
        {
            company_relationship other = null;
            try
            {
                relationship otherDefault = db.relationship.Where(item => item.id == 20 || item.relationship_en.ToLower() == "other").FirstOrDefault();
                if(otherDefault != null)
                {
                    other = new company_relationship();
                    other.relationship_en = otherDefault.relationship_en;
                    other.relationship_ar = otherDefault.relationship_ar;
                    other.relationship_es = other.relationship_es;
                    other.relationship_fr = otherDefault.relationship_fr;
                    other.relationship_ru = otherDefault.relationship_ru;
                }
                
            }
            catch (System.Data.DataException ex)
            {
                logger.Error(ex.ToString());
            }
            return other;
        }
        public string getLogoCompany(int id = 0)
        {
            if (id == 0)
            {
                return null;
            }
            return db.company.Where(m => m.id == id).Select(m => m.path_en).FirstOrDefault();
        }

        public List<PosterItem> GetAllPosters()
        {
            List<PosterItem> list = new List<PosterItem>();
            PosterItem _test = new PosterItem();
            #region Poster 1
            _test.posterName = "Communication is a Two-Way Street";
            _test.fileName = "communication_two_way";
            _test.Id = 1;
            _test.imageName = "img1";

            IPosterCategory ipc = new PosterCategory();
            List<IPosterCategory> ipc_list = new List<IPosterCategory>();
            ipc.Id = 1;
            ipc.posterCategoryName = "E-Commerce";
            ipc_list.Add(ipc);

            ipc = new PosterCategory();
            ipc.Id = 2;
            ipc.posterCategoryName = "MANUFACTURING";
            ipc_list.Add(ipc);
            _test.posterCategoryNames = ipc_list;


            IPosterMessage ipm = new PosterMessage();
            ipm.Id = 1;
            ipm.posterMessageName = "COMPANY CULTURE";
            _test.posterMessage = ipm;
            list.Add(_test);
            #endregion

            #region Poster 2
            _test = new PosterItem();
            _test.posterName = "How solid is your integrity?";
            _test.fileName = "how_solid_integrity?";
            _test.Id = 2;
            _test.imageName = "img2";

            ipc_list = new List<IPosterCategory>();
            ipc = new PosterCategory();
            ipc.Id = 3;
            ipc.posterCategoryName = "AVIATION";
            ipc_list.Add(ipc);
            ipc = new PosterCategory();
            ipc.Id = 2;
            ipc.posterCategoryName = "MANUFACTURING";
            ipc_list.Add(ipc);
            _test.posterCategoryNames = ipc_list;

            ipm = new PosterMessage();
            ipm.Id = 2;
            ipm.posterMessageName = "CUSTOMER SUPPORT";
            _test.posterMessage = ipm;
            list.Add(_test);
            #endregion

            #region Poster 3
            _test = new PosterItem();
            _test.posterName = "When the right thing to do isn’t clear… Focus on integrity";
            _test.fileName = "focus_on_integrity";
            _test.Id = 3;
            _test.imageName = "img3";

            ipc_list = new List<IPosterCategory>();
            ipc = new PosterCategory();
            ipc.Id = 4;
            ipc.posterCategoryName = "HOSPITAL AND FOOD";
            ipc_list.Add(ipc);
            ipc = new PosterCategory();
            ipc.Id = 5;
            ipc.posterCategoryName = "HEALTHCARE";
            ipc_list.Add(ipc);
            _test.posterCategoryNames = ipc_list;

            ipm = new PosterMessage();
            ipm.Id = 2;
            ipm.posterMessageName = "CUSTOMER SUPPORT";
            _test.posterMessage = ipm;
            list.Add(_test);
            #endregion

            #region Poster 4
            _test = new PosterItem();
            _test.posterName = "When the right thing to do isn’t clear… Focus on integrity";
            _test.fileName = "focus_on_integrity2";
            _test.Id = 4;
            _test.imageName = "img4";

            ipc_list = new List<IPosterCategory>();
            ipc = new PosterCategory();
            ipc.Id = 5;
            ipc.posterCategoryName = "HEALTHCARE";
            ipc_list.Add(ipc);
            _test.posterCategoryNames = ipc_list;

            ipm = new PosterMessage();
            ipm.Id = 1;
            ipm.posterMessageName = "COMPANY CULTURE";
            _test.posterMessage = ipm;
            list.Add(_test);
            #endregion

            return list;
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

  }
}