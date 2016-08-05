using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace EC.Models.ECModel
{
    public class Company : BaseEntity
    {
        #region Properties
        public int id
        {
            get;
            set;
        }
        public int client_id
        {
            get;
            set;
        }
        public int address_id
        {
            get;
            set;
        }
        public int billing_info_id
        {
            get;
            set;
        }
        public int status_id
        {
            get;
            set;
        }
        public string company_nm
        {
            get;
            set;
        }
        public DateTime registration_dt
        {
            get;
            set;
        }
        public string company_code
        {
            get;
            set;
        }
        public string contact_nm
        {
            get;
            set;
        }
        public int language_id
        {
            get;
            set;
        }
        public string employee_quantity
        {
            get;
            set;
        }
        public int implicated_title_name_id
        {
            get;
            set;
        }
        public int witness_show_id
        {
            get;
            set;
        }
        public int show_location_id
        {
            get;
            set;
        }
        public int show_department_id
        {
            get;
            set;
        }
        public string notepad_en
        {
            get;
            set;
        }
        public string notepad_fr
        {
            get;
            set;
        }
        public string notepad_es
        {
            get;
            set;
        }
        public string notepad_ru
        {
            get;
            set;
        }
        public string notepad_ar
        {
            get;
            set;
        }
        public string path_en
        {
            get;
            set;
        }
        public string path_fr
        {
            get;
            set;
        }
        public string path_es
        {
            get;
            set;
        }
        public string path_ru
        {
            get;
            set;
        }
        public string path_ar
        {
            get;
            set;
        }
        public string alert_en
        {
            get;
            set;
        }
        public string alert_fr
        {
            get;
            set;
        }
        public string alert_es
        {
            get;
            set;
        }
        public string alert_ru
        {
            get;
            set;
        }
        public string alert_ar
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
        public int time_zone_id
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Company()
        {
        }

        public Company(int company_id)
        {
            id = 0;
            Database.company _company = db.company.First(a => a.id == company_id);
            if (_company.id != null && _company.id != 0)
            {
                id = _company.id;
                address_id = _company.address_id;
                client_id = _company.client_id;
                billing_info_id = _company.billing_info_id;
                status_id = _company.status_id;
                company_nm = _company.company_nm;
                registration_dt = _company.registration_dt;
                company_code = _company.company_code;
                contact_nm = _company.contact_nm;
                language_id = _company.language_id;
                employee_quantity = _company.employee_quantity;
                implicated_title_name_id = _company.implicated_title_name_id;
                witness_show_id = _company.witness_show_id;
                show_department_id = _company.show_department_id;
                show_location_id = _company.show_location_id;
                show_department_id = _company.show_department_id;
                last_update_dt = _company.last_update_dt;
                user_id = _company.user_id;
                time_zone_id = _company.time_zone_id;

                // language specific properties
                notepad_en = _company.notepad_en;
                path_en = _company.path_en;
                alert_en = _company.alert_en;
            }
        }
        #endregion

        /// <summary>
        /// List of mediators for company
        /// </summary>
        /// <param name="company_id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="company_id"></param>
        /// <param name="language_id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="company_id"></param>
        /// <param name="language_id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="company_id"></param>
        /// <param name="language_id"></param>
        /// <returns></returns>
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
   
    }
}