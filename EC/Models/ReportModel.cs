using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using EC.Controllers.ViewModel;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Models.Utils;
using EC.App_LocalResources;
using System.Data.Entity.Validation;
using EC.Constants;
using EC.Models.ViewModels;
using Newtonsoft.Json;
using EC.Common.Interfaces;
using EC.Core.Common;
using System.IO;
using EC.Utils;

namespace EC.Models
{
    public class ReportModelResult
    {
        public report report { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ReportModel : BaseModel
    {

        #region Properties
        public int ID
        { get; set; }

        public report _report
        {
            get
            {
                return ReportById(ID);
            }
        }

        public user _reporter_user
        {
            get
            {
                if ((_report != null) && (ID != 0))
                {
                    user _user = db.user.Where(item => item.id == _report.reporter_user_id).FirstOrDefault();
                    return _user;
                }
                return null;
            }
        }

        public string Get_reporter_name(int caller_id)
        {
            string name = "";
            if (_reporter_user != null)
            {
                if (caller_id == _reporter_user.id)
                    return App_LocalResources.GlobalRes.You;

                user _caller = db.user.Where(item => item.id == caller_id).FirstOrDefault();

                if (_caller != null)
                {
                    if ((_caller.role_id < 8) && (_caller.role_id > 3))
                    {
                        if (_report.incident_anonymity_id == 3)
                        {
                            if ((_reporter_user.first_nm + " " + _reporter_user.last_nm).Trim().Length > 0)
                                return _reporter_user.first_nm + " " + _reporter_user.last_nm;
                        }
                    }
                    if ((_caller.role_id < 4) && (_caller.role_id > 0))
                    {
                        if ((_report.incident_anonymity_id == 3) || (_report.incident_anonymity_id == 2))
                        {
                            if ((_reporter_user.first_nm + " " + _reporter_user.last_nm).Trim().Length > 0)
                                return _reporter_user.first_nm + " " + _reporter_user.last_nm;
                        }
                    }
                    if (_caller.role_id == 8)
                    {
                        name = _reporter_user.first_nm + " " + _reporter_user.last_nm;
                        if ((_reporter_user.first_nm + " " + _reporter_user.last_nm).Trim().Length > 0)
                            return name;
                        if (name.Trim().Length == 0)
                            name = App_LocalResources.GlobalRes.anonymous_reporter;
                    }
                }

            }
            if (name.Trim().Length == 0)
                name = App_LocalResources.GlobalRes.anonymous_reporter;

            return name.Trim();

        }
        /// <summary>
        /// green line on top of report - Review, Investigation, etc
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>

        /// <summary>
        /// Returns the location of report in string
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string LocationString()
        {
            string location = App_LocalResources.GlobalRes.unknown_location;
            if ((_report != null) && (ID != 0))
            {
                if (_report.location_id.HasValue)
                {
                    company_location _c_location = db.company_location.Where(item => item.id == _report.location_id.Value).FirstOrDefault();
                    if (_c_location != null)
                        location = _c_location.location_en;
                }
                else if ((_report != null) && (_report.other_location_name != null) && (_report.other_location_name.Trim().Length > 0))
                    location = _report.other_location_name.Trim();
            }

            return location;
        }

        /// <summary>
        /// Returns string country of report
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string CountryString()
        {
            string country_nm = App_LocalResources.GlobalRes.unknown;

            if (_report != null)
            {
                if (_report.reporter_country_id.HasValue)
                {
                    country _country = db.country.Where(item => item.id == _report.reporter_country_id.Value).FirstOrDefault();
                    if (_country != null)
                        country_nm = _country.country_nm;
                }
            }
            return country_nm;
        }

        public string IsReportedOutside()
        {

            string report_outside = App_LocalResources.GlobalRes.unknown;

            if (_report != null)
            {
                if (_report.reported_outside_id.HasValue)
                {
                    reported_outside reported_outside =
                        db.reported_outside.Where(item => item.id == _report.reported_outside_id.Value).FirstOrDefault();
                    if (reported_outside != null)
                    {
                        report_outside = reported_outside.description_en;
                        if (_report.reported_outside_id.Value != 1)
                        {
                            report_outside = report_outside;// +". Description : " + _report.reported_outside_text;
                        }
                    }
                }
            }
            return report_outside;
        }
        public string IsReportedUrgent()
        {
            string report_urgent = App_LocalResources.GlobalRes.unknown;

            if (_report.priority_id == 0)
                report_urgent = App_LocalResources.GlobalRes.No;
            else if (_report.priority_id == 1)
                report_urgent = App_LocalResources.GlobalRes.Yes;

            return report_urgent;
        }

        public string IsOngoing()
        {
            string is_ongoing = App_LocalResources.GlobalRes.unknown;

            if (_report != null)
            {
                if (_report.is_ongoing == 1)
                {
                    is_ongoing = GlobalRes.No;
                }
                else if (_report.is_ongoing == 2)
                {
                    is_ongoing = GlobalRes.Yes;// +" Description : " + _report.report_frequency_text;
                }
                else if (_report.is_ongoing == 3)
                {
                    is_ongoing = GlobalRes.NotSureUp;
                }
            }
            return is_ongoing;
        }

        public string HasInjuryDamage()
        {
            string injury_damage = App_LocalResources.GlobalRes.unknown;

            if (_report != null)
            {
                injury_damage injuryOrDamage =
                    db.injury_damage.Where(item => item.id == _report.injury_damage_id).FirstOrDefault();
                if (injuryOrDamage != null)
                {
                    injury_damage = injuryOrDamage.text_en;
                    if (_report.reported_outside_id.Value == 2)
                    {
                        injury_damage = injury_damage;// +". Description : " + _report.injury_damage;
                    }
                }
            }
            return injury_damage;
        }

        public List<company_department> Departments(bool? added_by_reporter)
        {
            List<int> list;
            if (added_by_reporter.HasValue)
                list = db.report_department
                    .Where(item => item.report_id == _report.id && item.added_by_reporter == added_by_reporter)
                    .Select(x => x.department_id)
                    .ToList();
            else
                list = db.report_department
                    .Where(item => item.report_id == _report.id)
                    .Select(x => x.department_id)
                    .ToList();

            return db.company_department.Where(x => list.Contains(x.id)).ToList();
        }

        public List<string> Departments()
        {
            var list = new List<string>();

            if (_report != null)
            {
                List<report_department> _c_departments = db.report_department.Where(item => item.report_id == _report.id && item.added_by_reporter != false).ToList();
                company_department temp_c_dep;
                foreach (report_department _temp_dep in _c_departments)
                {
                    temp_c_dep = db.company_department.Where(item => item.id == _temp_dep.department_id).FirstOrDefault();
                    if (temp_c_dep != null)
                    {
                        if (list.IndexOf(temp_c_dep.department_en.Trim()) == -1)
                        {
                            list.Add(temp_c_dep.department_en.Trim());
                        }
                    }
                }

                if (_report.other_department_name.Trim().Length > 0)
                {
                    if (list.IndexOf(_report.other_department_name.Trim()) == -1)
                    {
                        list.Add(_report.other_department_name.Trim());
                    }

                }
            }
            if (list.Count == 0)
            {
                list.Add(App_LocalResources.GlobalRes.unknown_departments);
            }

            return list;
        }

        /// <summary>
        /// Returns the departments involved in reported
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string DepartmentsString()
        {
            string departments = "";

            if (_report != null)
            {
                List<report_department> _c_departments = db.report_department.Where(item => item.report_id == _report.id && item.added_by_reporter != false).ToList();
                company_department temp_c_dep;
                foreach (report_department _temp_dep in _c_departments)
                {
                    temp_c_dep = db.company_department.Where(item => item.id == _temp_dep.department_id).FirstOrDefault();
                    if (temp_c_dep != null)
                    {
                        if (departments.Length > 0)
                            departments = departments + ", " + temp_c_dep.department_en.Trim();
                        else
                            departments = temp_c_dep.department_en.Trim();
                    }
                }

                if (_report.other_department_name.Trim().Length > 0)
                    if (departments.Length > 0)
                        departments = departments + ", " + _report.other_department_name.Trim();
                    else
                        departments = _report.other_department_name.Trim();
            }
            if (departments.Length == 0)
                departments = App_LocalResources.GlobalRes.unknown_departments;

            return departments;
        }

        public string ReporterCompanyRelationShort()
        {
            string relationship_text = App_LocalResources.GlobalRes.Other;

            if (_report != null)
            {
                if (db.report_relationship.Any(t => t.report_id == _report.id))
                {
                    report_relationship _report_relationship = db.report_relationship.Where(t => t.report_id == _report.id).First();
                    if ((_report_relationship.relationship_id.HasValue) && (_report_relationship.relationship_id.Value != 0) && (_report_relationship.relationship_id.Value != -1))
                    {
                        relationship temp_relationship = db.relationship.Where(item => item.id == _report_relationship.relationship_id.Value).FirstOrDefault();
                        relationship_text = temp_relationship.relationship_en;
                    }
                    // 
                    if ((_report_relationship.company_relationship_id.HasValue) && (_report_relationship.company_relationship_id.Value != 0) && (_report_relationship.company_relationship_id.Value != -1))
                    {
                        company_relationship temp_comp_relationship = db.company_relationship.Where(item => item.id == _report_relationship.company_relationship_id.Value).FirstOrDefault();
                        if (temp_comp_relationship != null)
                        {
                            relationship_text = temp_comp_relationship.relationship_en.Trim();
                        }
                    }
                }
            }

            return relationship_text;
        }
        /// <summary>
        /// gets the secondary type of report
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string SecondaryTypeString()
        {
            string secondary_type = "";

            if (_report != null)
            {
                if (db.report_secondary_type.Any(t => t.report_id == _report.id && t.added_by_reporter != false))
                {
                    List<report_secondary_type> _report_secondary_type_list = db.report_secondary_type.Where(t => t.report_id == _report.id && t.added_by_reporter != false).ToList();
                    foreach (report_secondary_type _report_secondary_type in _report_secondary_type_list)
                    {
                        if (_report_secondary_type.mandatory_secondary_type_id != null)
                        {
                            secondary_type_mandatory temp_sec_type = db.secondary_type_mandatory.Where(item => item.id == _report_secondary_type.mandatory_secondary_type_id).FirstOrDefault();
                            if (temp_sec_type != null)
                            {
                                if ((secondary_type.Length > 0) && (!secondary_type.Contains(temp_sec_type.secondary_type_en.Trim())))
                                    secondary_type = secondary_type + ", " + temp_sec_type.secondary_type_en.Trim();
                                else
                                    secondary_type = temp_sec_type.secondary_type_en.Trim();
                            }
                        }

                        if ((_report_secondary_type.secondary_type_id != 0) && (_report_secondary_type.secondary_type_id != -1))
                        {
                            company_secondary_type temp_comp_sec_type = db.company_secondary_type.Where(item => item.id == _report_secondary_type.secondary_type_id).FirstOrDefault();
                            if (temp_comp_sec_type != null)
                            {
                                if ((secondary_type.Length > 0) && (!secondary_type.Contains(temp_comp_sec_type.secondary_type_en.Trim())))
                                    secondary_type = secondary_type + ", " + temp_comp_sec_type.secondary_type_en.Trim();
                                else
                                    secondary_type = temp_comp_sec_type.secondary_type_en.Trim();
                            }
                        }

                        if (_report_secondary_type.secondary_type_nm.Trim() != "")
                        {
                            if ((secondary_type.Length > 0) && (!secondary_type.Contains(_report_secondary_type.secondary_type_nm.Trim())))
                                secondary_type = secondary_type + ", " + _report_secondary_type.secondary_type_nm.Trim();
                            else
                                secondary_type = _report_secondary_type.secondary_type_nm.Trim();
                        }
                        else if (_report_secondary_type.secondary_type_id == 0)
                        {
                            if (secondary_type.Length > 0)
                            {
                                secondary_type = secondary_type + ", " + GlobalRes.Other;
                            }
                            else
                            {
                                secondary_type = GlobalRes.Other;
                            }
                        }
                    }

                }
            }
            if (secondary_type.Length == 0)
                secondary_type = App_LocalResources.GlobalRes.unknown_secondary_type;

            return secondary_type.Trim();
        }

        public List<string> SecondaryTypeAll()
        {
            var list = new List<string>();

            if (_report != null)
            {
                if (db.report_secondary_type.Any(t => t.report_id == _report.id))
                {
                    List<report_secondary_type> _report_secondary_type_list = db.report_secondary_type.Where(t => t.report_id == _report.id).ToList();
                    foreach (report_secondary_type _report_secondary_type in _report_secondary_type_list)
                    {
                        if (_report_secondary_type.mandatory_secondary_type_id != null)
                        {
                            secondary_type_mandatory temp_sec_type = db.secondary_type_mandatory.Where(item => item.id == _report_secondary_type.mandatory_secondary_type_id).FirstOrDefault();
                            if (temp_sec_type != null)
                            {
                                if (list.IndexOf(temp_sec_type.secondary_type_en.Trim()) != -1)
                                {
                                    list.Add(temp_sec_type.secondary_type_en.Trim());
                                }
                            }
                        }

                        if ((_report_secondary_type.secondary_type_id != 0) && (_report_secondary_type.secondary_type_id != -1))
                        {
                            company_secondary_type temp_comp_sec_type = db.company_secondary_type.Where(item => item.id == _report_secondary_type.secondary_type_id).FirstOrDefault();
                            if (temp_comp_sec_type != null)
                            {
                                if (list.IndexOf(temp_comp_sec_type.secondary_type_en.Trim()) == -1)
                                {
                                    list.Add(temp_comp_sec_type.secondary_type_en.Trim());
                                }
                            }
                        }

                        if (_report_secondary_type.secondary_type_nm.Trim() != "")
                        {
                            if (list.IndexOf(_report_secondary_type.secondary_type_nm.Trim()) == 0)
                            {
                                list.Add(_report_secondary_type.secondary_type_nm.Trim());
                            }
                        }
                        else if (_report_secondary_type.secondary_type_id == 0)
                        {
                            list.Add(GlobalRes.Other);
                        }
                    }
                }
            }

            return list;
        }

        public List<report_secondary_type> SecondaryTypeModelAll()
        {
            var list = db.report_secondary_type
                .Where(x => x.report_id == ID)
                .ToList();

            var company_secondary_types = db.company_secondary_type
                .Where(x => x.company_id == _report.company_id)
                .OrderBy(x => x.secondary_type_en)
                .ToList();

            list.ForEach(x =>
            {
                x.secondary_type_nm = company_secondary_types.FirstOrDefault(z => z.id == x.secondary_type_id)?.secondary_type_en;
            });

            return list;
        }

        public report_investigation_methodology InvestigationMethodologies(int secondary_type_id)
        {
            return db.report_investigation_methodology
                .FirstOrDefault(x => x.report_id == ID && x.report_secondary_type_id == secondary_type_id);
        }

        public List<company_root_cases_behavioral> RootCasesBehavioral()
        {
            return db.company_root_cases_behavioral
                .Where(x => x.company_id == _report.company_id)
                .ToList();
        }

        public List<company_root_cases_external> RootCasesExternal()
        {
            return db.company_root_cases_external
                .Where(x => x.company_id == _report.company_id)
                .ToList();
        }

        public List<company_root_cases_organizational> RootCasesOrganizational()
        {
            return db.company_root_cases_organizational
                .Where(x => x.company_id == _report.company_id)
                .ToList();
        }

        public string SecondaryTypeStringAll()
        {
            string secondary_type = "";

            if (_report != null)
            {
                if (db.report_secondary_type.Any(t => t.report_id == _report.id))
                {
                    List<report_secondary_type> _report_secondary_type_list = db.report_secondary_type.Where(t => t.report_id == _report.id).ToList();
                    foreach (report_secondary_type _report_secondary_type in _report_secondary_type_list)
                    {
                        if (_report_secondary_type.mandatory_secondary_type_id != null)
                        {
                            secondary_type_mandatory temp_sec_type = db.secondary_type_mandatory.Where(item => item.id == _report_secondary_type.mandatory_secondary_type_id).FirstOrDefault();
                            if (temp_sec_type != null)
                            {
                                if ((secondary_type.Length > 0) && (!secondary_type.Contains(temp_sec_type.secondary_type_en.Trim())))
                                    secondary_type = secondary_type + ", " + temp_sec_type.secondary_type_en.Trim();
                                else if (secondary_type.Length == 0)
                                    secondary_type = temp_sec_type.secondary_type_en.Trim();
                            }
                        }

                        if ((_report_secondary_type.secondary_type_id != 0) && (_report_secondary_type.secondary_type_id != -1))
                        {
                            company_secondary_type temp_comp_sec_type = db.company_secondary_type.Where(item => item.id == _report_secondary_type.secondary_type_id).FirstOrDefault();
                            if (temp_comp_sec_type != null)
                            {
                                if ((secondary_type.Length > 0) && (!secondary_type.Contains(temp_comp_sec_type.secondary_type_en.Trim())))
                                    secondary_type = secondary_type + ", " + temp_comp_sec_type.secondary_type_en.Trim();
                                else if (secondary_type.Length == 0)
                                    secondary_type = temp_comp_sec_type.secondary_type_en.Trim();
                            }
                        }

                        if (_report_secondary_type.secondary_type_nm.Trim() != "")
                        {
                            if ((secondary_type.Length > 0) && (!secondary_type.Contains(_report_secondary_type.secondary_type_nm.Trim())))
                                secondary_type = secondary_type + ", " + _report_secondary_type.secondary_type_nm.Trim();
                            else if (secondary_type.Length == 0)
                                secondary_type = _report_secondary_type.secondary_type_nm.Trim();
                        }
                        else if (_report_secondary_type.secondary_type_id == 0)
                        {
                            if (secondary_type.Length > 0)
                            {
                                secondary_type = secondary_type + ", " + GlobalRes.Other;
                            }
                            else
                            {
                                secondary_type = GlobalRes.Other;
                            }
                        }
                    }

                }
            }
            if (secondary_type.Length == 0)
                secondary_type = App_LocalResources.GlobalRes.unknown_secondary_type;

            return secondary_type.Trim();
        }

        /// <summary>
        /// Returns "Jan 31, 2015"
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string IncidentDateString()
        {
            string date_string = "";
            DateTime dt;
            if (_report != null)
            {
                dt = _report.incident_dt;
                date_string = m_DateTimeHelper.GetShortMonth(dt.Month) + " " + dt.Day.ToString() + ", " + dt.Year.ToString();
            }

            if (date_string.Trim().Length == 0)
                date_string = App_LocalResources.GlobalRes.unknown_date;

            return date_string.Trim();
        }

        /// <summary>
        /// Returns "January 31, 2015"
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string IncidentDateStringMonthLong()
        {
            string date_string = "";
            DateTime dt;
            if (_report != null)
            {
                dt = _report.incident_dt;
                date_string = m_DateTimeHelper.GetFullMonth(dt.Month) + " " + dt.Day.ToString() + ", " + dt.Year.ToString();
            }

            if (date_string.Trim().Length == 0)
                date_string = App_LocalResources.GlobalRes.unknown_date;

            return date_string.Trim();
        }
        /// <summary>
        /// Returns "Jan 31, 2015"
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string ReportedDateString()
        {
            string date_string = "";
            DateTime dt;
            if (_report != null)
            {
                dt = _report.reported_dt;
                date_string = m_DateTimeHelper.GetShortMonth(dt.Month) + " " + dt.Day.ToString() + ", " + dt.Year.ToString();
            }

            if (date_string.Trim().Length == 0)
                date_string = App_LocalResources.GlobalRes.unknown_date;

            return date_string.Trim();
        }
        /// <summary>
        /// Returns "January 31, 2015"
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string ReportedDateStringMonthLong()
        {
            string date_string = "";
            DateTime dt;
            if (_report != null)
            {
                dt = _report.reported_dt;
                date_string = m_DateTimeHelper.GetFullMonth(dt.Month) + " " + dt.Day.ToString() + ", " + dt.Year.ToString();
            }

            if (date_string.Trim().Length == 0)
                date_string = App_LocalResources.GlobalRes.unknown_date;

            return date_string.Trim();
        }
        public string ColorCode()
        {
            string color_code = "";
            color _color;

            if (_report != null)
            {
                int color_id = _report.report_color_id;

                _color = db.color.Where(item => item.id == _report.report_color_id).FirstOrDefault();
                if (_color != null)
                    color_code = _color.color_code;
                else
                {
                    _color = db.color.Where(item => item.id == 1).FirstOrDefault();
                    color_code = _color.color_code;
                }
            }
            else
            {
                _color = db.color.Where(item => item.id == 1).FirstOrDefault();
                color_code = _color.color_code;
            }

            return color_code.Trim();
        }
        public string ColorDescr()
        {
            string color_descr = "";
            color _color;

            if (_report != null)
            {
                int color_id = _report.report_color_id;

                _color = db.color.Where(item => item.id == _report.report_color_id).FirstOrDefault();
                if (_color != null)
                    color_descr = _color.color_description;
                else
                {
                    _color = db.color.Where(item => item.id == 1).FirstOrDefault();
                    color_descr = _color.color_description;
                }
            }
            else
            {
                _color = db.color.Where(item => item.id == 1).FirstOrDefault();
                color_descr = _color.color_description;
            }

            return color_descr.Trim();
        }

        public string ColorSecondaryCode()
        {
            string color_code = "";
            color _color;

            if (_report != null)
            {
                int color_id = _report.report_color_id;

                _color = db.color.Where(item => item.id == _report.report_color_id).FirstOrDefault();
                if (_color != null)
                    color_code = _color.secondary_color_code;
                else
                {

                    _color = db.color.Where(item => item.id == 1).FirstOrDefault();
                    color_code = _color.secondary_color_code;
                }
            }
            else
            {
                _color = db.color.Where(item => item.id == 1).FirstOrDefault();
                color_code = _color.secondary_color_code;
            }

            return color_code.Trim();
        }

        public string ManagementKnowString()
        {
            string management_know = App_LocalResources.GlobalRes.unknown;

            if (_report != null)
            {
                if (_report.management_know_id.HasValue)
                {
                    management_know _management_know = db.management_know.Where(item => item.id == _report.management_know_id.Value).FirstOrDefault();
                    if (_management_know != null)
                    {
                        management_know = _management_know.text_en;
                    }
                }
            }
            return management_know;
        }


        public string CompanyName()
        {
            string company_name = "";

            if (_report != null)
            {
                if (_report.company_id != 1)
                {
                    company _company = db.company.Where(item => item.id == _report.company_id).FirstOrDefault();
                    company_name = _company.company_nm.Trim();
                }
                else
                    company_name = _report.submitted_company_nm.Trim();
            }

            return company_name;
        }


        /// <summary>
        /// Allowed delay for investigation of current report investigation status ( i.e. 3 days for step3_delay, 5 for step2_delay)
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public int GetDelayAllowed()
        {
            int delay_allowed = 5;

            int company_id = _report.company_id;
            company _company = db.company.Where(item => item.id == company_id).FirstOrDefault();

            int status_id = _investigation_status;

            switch (status_id)
            {
                case 1:
                    delay_allowed = _company.step1_delay;
                    break;
                case 2:
                    delay_allowed = _company.step2_delay;
                    break;
                case 3:
                    delay_allowed = _company.step3_delay;
                    break;
                case 4:
                    delay_allowed = _company.step4_delay;
                    break;
                case 5:
                    delay_allowed = _company.step5_delay;
                    break;
                default:
                    delay_allowed = 5;
                    break;
            }
            return delay_allowed;
        }

        /// <summary>
        /// Report total days in progress(if closed or spam - how much time did it take)
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public int GetTotalDays()
        {
            int total_days = 0;
            if (_report != null)
            {

                if ((_investigation_status == 6) || (_investigation_status == 7))
                {
                    //report is closed or marked as spam on LastPromotedDate()
                    total_days = (LastPromotedDate() - _report.reported_dt).Days;
                }
                else
                {
                    total_days = (System.DateTime.Now - _report.reported_dt).Days;
                }
            }

            return total_days;
        }


        /// <summary>
        /// number in green circle - how much days are left for next step.
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public int GetThisStepDaysLeft()
        {
            int delay_allowed = GetDelayAllowed();
            return GetThisStepDaysLeft(delay_allowed);

        }

        // Overloaded method when we know the delay
        public int GetThisStepDaysLeft(int step_delay_allowed)
        {
          if (_report.status_id == ECGlobalConstants.investigation_status_closed || _report.status_id == ECGlobalConstants.investigation_status_spam)
            return 0;
            DateTime promoted_date = LastPromotedDate();
            double days_left = 2;
            double days_ongoing = 0;
            days_ongoing = (DateTime.Today - promoted_date).TotalDays;
            days_left = step_delay_allowed - days_ongoing;
            int days = (int)days_left;

            if (days >= 0)
                return days;
            else if (days < -1)
            {
                //expired I guess
                return 0;
                //     return days;
            }
            else
                return 0;
        }
        public DateTime LastPromotedDate()
        {
            int status_id = _investigation_status;
            DateTime promoted_date = _report.reported_dt;
            if (_report.last_update_dt != null)
              promoted_date = _report.last_update_dt;


            //if (db.report_investigation_status.Any(item => ((item.report_id == ID) && (item.investigation_status_id == status_id))))
            //{
            //    promoted_date = db.report_investigation_status.Where(item => ((item.report_id == ID) && (item.investigation_status_id == status_id))).OrderByDescending(a => a.created_date).Select(t => t.created_date).FirstOrDefault();
            //}
            return promoted_date;
        }

        public DateTime LastReadDate(int user_id)
        {

            if (db.report_user_read.Where(item => ((item.user_id == user_id) && (item.report_id == ID))).Count() == 0)
            {
                return ECGlobalConstants._default_date;
            }
            //unread_report.Add(temp_report);

            List<report_user_read> _list_read = db.report_user_read.Where(item => ((item.user_id == user_id) && (item.report_id == ID))).ToList();
            if (_list_read.Count > 0)
                return _list_read[0].read_date;
            else
                return ECGlobalConstants._default_date;
        }

        public bool IsSpamScreen()
        {

            int _prev_inv_status_id = _previous_investigation_status_id();
            bool is_spam = false;
            if ((_investigation_status == ECGlobalConstants.investigation_status_spam) || (_prev_inv_status_id == ECGlobalConstants.investigation_status_spam && _investigation_status == ECGlobalConstants.investigation_status_closed))
            {
                is_spam = true;
                // at least its spam

                /*         if (promotion_status_date(Constant.investigation_status_spam) > LastReadDate(user_id))
                         {
                             is_spam = true;
                         }*/

            }
            return is_spam;
        }
        public bool IsCompletedScreen()
        {
            bool is_completed = false;
            if ((_investigation_status == ECGlobalConstants.investigation_status_completed) || (_investigation_status == ECGlobalConstants.investigation_status_resolution))
            {
                is_completed = true;
                /*    // at least its spam
                    if (promotion_status_date(Constant.investigation_status_closed) > LastReadDate(user_id))
                    {
                        is_closed = true;
                    }*/
            }
            return is_completed;
        }
        public bool IsClosedScreen()
        {
            int _prev_inv_status_id = _previous_investigation_status_id();
            bool is_closed = false;
            if (((_investigation_status == ECGlobalConstants.investigation_status_closed) && (_prev_inv_status_id != ECGlobalConstants.investigation_status_spam)))
            {
                is_closed = true;
                /*    // at least its spam
                    if (promotion_status_date(Constant.investigation_status_closed) > LastReadDate(user_id))
                    {
                        is_closed = true;
                    }*/
            }
            return is_closed;

        }
        public bool IsPendingScreen()
        {
            bool is_closed = false;
            if ((_investigation_status == ECGlobalConstants.investigation_status_pending) || (_investigation_status == ECGlobalConstants.investigation_status_review))
            {
                is_closed = true;
                /*    // at least its spam
                    if (promotion_status_date(Constant.investigation_status_closed) > LastReadDate(user_id))
                    {
                        is_closed = true;
                    }*/
            }
            return is_closed;
        }

        public DateTime promotion_toactive_status_date()
        {
            if ((_report == null) || (ID == 0))
            {
                // error in report or its not created yet
                return ECGlobalConstants._default_date;
            }
            List<int> _active_classes = new List<int>();
            _active_classes.Add(ECGlobalConstants.investigation_status_investigation);
            _active_classes.Add(ECGlobalConstants.investigation_status_completed);
            _active_classes.Add(ECGlobalConstants.investigation_status_closed);

            report_investigation_status last_status = new report_investigation_status();
            if (db.report_investigation_status.Any(item => (item.report_id == ID)))
            {
                List<report_investigation_status> _statuses = db.report_investigation_status.Where(item => (item.report_id == ID)).OrderByDescending(x => x.created_date).ToList();

                if (_statuses.Count > 0)
                {
                    int first_non_active_status_place = -1;
                    int last_active_status_place = -1;

                    int count = _statuses.Count;
                    while ((last_active_status_place < 0) && (count >= 0))
                    {
                        if (_active_classes.Contains(_statuses[count - 1].investigation_status_id))
                        {
                            last_active_status_place = count;
                        }

                        count--;
                    }
                    if (last_active_status_place < 0)
                    {
                        // no active statuses
                        return ECGlobalConstants._default_date;
                    }
                    else
                    {
                        count = last_active_status_place;
                        // we need to go and find last active status before not active
                        while ((count > 0) && (_active_classes.Contains(_statuses[count - 1].investigation_status_id)))
                        {
                            count--;
                        }
                        first_non_active_status_place = count;


                        if (first_non_active_status_place < 0)
                        {
                            // no active statuses
                            return ECGlobalConstants._default_date;
                        }
                        return _statuses[first_non_active_status_place + 1].created_date;
                    }

                }
                else
                    return ECGlobalConstants._default_date;
            }
            else
                return ECGlobalConstants._default_date;

            //          return 0;
        }


        public report_investigation_status LastPromotion()
        {
            int status_id = _investigation_status;
            if (db.report_investigation_status.Any(item => ((item.report_id == ID) && (item.investigation_status_id == status_id))))
            {
                report_investigation_status _current_report_status = db.report_investigation_status.Where(item => ((item.report_id == ID) && (item.investigation_status_id == status_id))).OrderByDescending(a => a.created_date).FirstOrDefault();
                return _current_report_status;
            }
            else
            {
                return null;
            }
        }

        public DateTime promotion_status_date(int status_id)
        {
            if ((_report == null) || (ID == 0))
            {
                // error in report or its not created yet
                return ECGlobalConstants._default_date;
            }

            report_investigation_status last_status = new report_investigation_status();
            if (db.report_investigation_status.Any(item => (item.report_id == ID && item.investigation_status_id == status_id)))
            {
                List<report_investigation_status> _statuses = db.report_investigation_status.Where(item => (item.report_id == ID && item.investigation_status_id == status_id)).OrderByDescending(x => x.created_date).ToList();

                if (_statuses.Count > 0)
                {
                    last_status = _statuses[0];
                    return last_status.created_date;
                }
                else
                    return ECGlobalConstants._default_date;
            }
            else
                return ECGlobalConstants._default_date;

            //          return 0;
        }

        public DateTime LastEventDate()
        {
            if ((_report == null) || (ID == 0))
            {
                // error in report or its not created yet
                return ECGlobalConstants._default_date;
            }
            report_log last_status_log = new report_log();
            if (db.report_log.Any(item => (item.report_id == ID)))
            {
                List<report_log> _statuses = db.report_log.Where(item => (item.report_id == ID)).OrderByDescending(x => x.id).ToList();

                if (_statuses.Count > 0)
                {
                    last_status_log = _statuses[0];
                    return last_status_log.created_dt;
                }
                else
                    return ECGlobalConstants._default_date;
            }
            else
                return ECGlobalConstants._default_date;

        }

        public bool _Is_New_Activity(int user_id)
        {
            bool _new_activity = false;

            if (LastEventDate() > LastReadDate(user_id))
            {
                _new_activity = true;
            }

            return _new_activity;
        }
        /// <summary>
        /// show Reporter Anon Level to reporter
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string _anonymousLevel_reporterVersion()
        {

            string anon_level = "";
            int anon_level_id = 0;

            if (_report != null)
            {
                anon_level_id = _report.incident_anonymity_id;
                anonymity _anonymity = db.anonymity.Where(item => (item.id == anon_level_id)).FirstOrDefault();

                if (_anonymity != null)
                {
                    if ((anon_level_id == 1) || (anon_level_id == 3))
                        anon_level = _anonymity.anonymity_en;
                    if (anon_level_id == 2)
                        anon_level = String.Format(_anonymity.anonymity_en_company, CompanyName());
                }
            }

            return anon_level;
        }

        public string _anonymousLevel_mediatorVersion()
        {
            string anon_level = "";
            int anon_level_id = 0;

            if (_report != null)
            {
                anon_level_id = _report.incident_anonymity_id;
                anonymity _anonymity = db.anonymity.Where(item => (item.id == anon_level_id)).FirstOrDefault();

                if (_anonymity != null)
                {
                    if ((anon_level_id == 1) || (anon_level_id == 3))
                        anon_level = _anonymity.anonymity_en;
                    if (anon_level_id == 2)
                        anon_level = String.Format(_anonymity.anonymity_en, CompanyName());
                }
            }

            return anon_level;
        }

        public string _anonymousLevel_mediatorVersionByCaller(int caller_id)
        {

            string anon_level = "";
            int anon_level_id = 0;

            if (_report != null)
            {
                anon_level_id = _report.incident_anonymity_id;
                anonymity _anonymity = db.anonymity.Where(item => (item.id == anon_level_id)).FirstOrDefault();

                if (_anonymity != null)
                {
                    if ((anon_level_id == 1) || (anon_level_id == 3))
                        anon_level = _anonymity.anonymity_en;
                    if (anon_level_id == 2)
                    {
                        UserModel um = new UserModel(caller_id);
                        int role_id = um._user.role_id;
                        if ((role_id > 3) && (role_id < 8))
                        {
                            return EC.App_LocalResources.GlobalRes.anonymous_reporter;
                        }
                        if ((role_id > 0) && (role_id < 4))
                        {
                            return String.Format(_anonymity.anonymity_en, CompanyName());
                        }
                        return EC.App_LocalResources.GlobalRes.anonymous_reporter;

                    }

                }
            }

            return anon_level;

        }
        public bool _has_attachments()
        {
            return (_attachments().Count > 0);
        }

        public List<string> _attachments()
        {
            List<string> _list = new List<string>();

            if (db.attachment.Any(item => (item.report_id == ID) && (item.status_id == 2) && !item.visible_mediators_only.HasValue && !item.visible_reporter.HasValue))
            {
                _list = db.attachment.Where(item1 => (item1.report_id == ID) && (item1.status_id == 2) && !item1.visible_mediators_only.HasValue && !item1.visible_reporter.HasValue).Select(item => item.path_nm).ToList();
            }

            return _list;
        }
        #endregion

        #region Base constructors
        GlobalFunctions glb = new GlobalFunctions();
        public ReportModel()
        {
            ID = 0;
        }

        public ReportModel(int report_id)
        {
            ID = report_id;
        }

        public ReportModelResult AddReport(ReportViewModel model, out string password)
        {
            var resultModel = new ReportModelResult();
            Guid _guid = Guid.NewGuid();
            var currentReport = new report();
            
            using (ECEntities adv = new ECEntities())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region creating report
                        /**
                        * addUserID to the getAttachment.user_id, add report_id to the getAttachment.report_id
                        * */
                        attachment getAttachment = FileUtils.SaveFile(model.files, "attachDocuments", "attachDocuments");
                        MailAddress mail = null;
                        string nameOfEmail = String.Empty;

                        if (model.userEmail != null)
                        {
                            mail = new MailAddress(model.userEmail);
                            nameOfEmail = mail.User;
                        }

                        //GlobalFunctions gfFunctions = new GlobalFunctions();
                        int notification = Convert.ToInt16(model.sendUpdates);
                        //if not checked = 3, if  check = 1
                        if (notification != 1)
                        {
                            notification = 3;
                        }
                       
                        password = glb.GeneretedPassword();
                        user newUser = new user()
                        {
                            company_id = model.currentCompanyId,
                            role_id = 8,
                            status_id = 2,
                            address_id = 0,
                            first_nm = model.userName == null ? "" : model.userName,
                            last_nm = model.userLastName == null ? "" : model.userLastName,
                            login_nm = "",
                            password = PasswordUtils.GetHash(password),
                            email = model.userEmail == null ? "" : model.userEmail,
                            preferred_contact_method_id = 2,
                            question_ds = "",
                            answer_ds = "",
                            user_id = 0,
                            last_update_dt = DateTime.Now,
                            preferred_email_language_id = 1,
                            photo_path = "",
                            notification_messages_actions_flag = notification,
                            notification_new_reports_flag = 1,
                            notification_marketing_flag = 1,
                            guid = _guid,
                            notification_summary_period = 1,
                            phone = model.userTelephone
                        };

                        newUser = adv.user.Add(newUser);
                        adv.SaveChanges();
                        string reporter_login = GenerateReporterLogin();

                        while (glb.isLoginInUse(reporter_login))
                        {
                            reporter_login = GenerateReporterLogin();
                        }

                        newUser.login_nm = reporter_login;
                        currentReport = model.Merge(currentReport);
                        currentReport.agent_id = model.agentId;
                        currentReport.user_id = newUser.id;
                        currentReport.reporter_user_id = newUser.id;
                        currentReport.report_by_myself = model.report_by_myself;
                        currentReport.guid = Guid.NewGuid();
                        db.user.AddOrUpdate(newUser);
                        currentReport.status_id = 1;
                        currentReport.last_update_dt = DateTime.Now;
                        currentReport.user_id = newUser.id;

                        currentReport = adv.report.Add(currentReport);
                        adv.SaveChanges();
                        //  t = db.SaveChanges();
                        currentReport.display_name = GenerateCaseNumber(currentReport.id, currentReport.company_id, currentReport.company_nm);
                        currentReport.report_color_id = GetNextColor(currentReport.company_id, currentReport.id);
                        //t = adv.SaveChanges();

                        if (getAttachment != null)
                        {
                            getAttachment.report_id = currentReport.id;
                            getAttachment.user_id = newUser.id;
                            db.attachment.Add(getAttachment);
                            //t = adv.SaveChanges();
                        }

                        if (model.mediatorsInvolved != null)
                        {
                            foreach (string mediatorId in model.mediatorsInvolved)
                            {
                                report_mediator_involved result = new report_mediator_involved()
                                {
                                    user_id = 1,
                                    mediator_id = Convert.ToInt32(mediatorId),
                                    status_id = 2,
                                    last_update_dt = DateTime.Now,
                                    report_id = currentReport.id
                                };

                                adv.report_mediator_involved.Add(result);
                                //adv.SaveChanges();
                            }
                        }

                        List<report_department> departments = model.GetReportDepartment(currentReport.id);
                        foreach (report_department department in departments)
                        {
                            department.added_by_reporter = true;
                            AddReportDepartment(department);
                            //t = adv.SaveChanges();
                        }


                        #region Status Saving = pending
                        report_investigation_status _review_status = new report_investigation_status();
                        _review_status.created_date = DateTime.Now;
                        _review_status.investigation_status_id = 1;
                        _review_status.report_id = currentReport.id;
                        _review_status.user_id = newUser.id;
                        _review_status.description = "";
                        ///adv.report_investigation_status.Add(_review_status);
                        ////   t = adv.SaveChanges();
                        //AddPendingStatus(_review_status);
                        adv.report_investigation_status.Add(_review_status);

                        //var report = adv.report.FirstOrDefault(x => x.id == _review_status.report_id);


                        //adv.SaveChanges();
                        #endregion

                        //savind secondary type 

                        /*CustomSecondaryType == false*/
                        List<report_secondary_type> secondaryTypeList = new List<report_secondary_type>();
                        foreach (var item in model.whatHappened)
                        {
                            report_secondary_type temp = new report_secondary_type();
                            temp.report_id = currentReport.id;
                            temp.secondary_type_nm = "";
                            temp.secondary_type_id = 0;
                            //check is it custom
                            if (model.CustomSecondaryType)
                            {
                                temp.mandatory_secondary_type_id = item;
                            }
                            else
                            {
                                temp.secondary_type_id = item;
                            }
                            //check is it other
                            if (item == 0 && model.caseInformationReportDetail != null)
                            {
                                temp.secondary_type_nm = model.caseInformationReportDetail;
                            }

                            temp.last_update_dt = DateTime.Now;
                            temp.user_id = 1;
                            temp.added_by_reporter = true;
                            //adv.
                            //AddSecondaryType(temp);
                            adv.report_secondary_type.Add(temp);
                        }

                        /*report_relationship*/
                        report_relationship rep = new report_relationship();
                        rep.report_id = currentReport.id;
                        rep.user_id = currentReport.user_id;
                        rep.last_update_dt = DateTime.Now;

                        if (currentReport.not_current_employee != null && currentReport.not_current_employee.Trim().Length > 0)
                        {
                            //other
                            rep.relationship_nm = currentReport.not_current_employee.Trim();
                            rep.relationship_id = null;
                            if (currentReport.company_relationship_id != 0)
                            {
                                //'Former Employee'
                                rep.company_relationship_id = currentReport.company_relationship_id;
                            }
                        }
                        else
                        {
                            int count = db.company_relationship.Where(rel => rel.status_id == 2).Count();
                            if (count > 0)
                            {
                                //custom
                                rep.company_relationship_id = currentReport.company_relationship_id;
                            }
                            else
                            {
                                rep.relationship_id = currentReport.company_relationship_id;
                            }
                        }
                        adv.report_relationship.Add(rep);

                        /*for(var i = 0; i < model.personName.Count;  i++)
                        {
                            var person = new report_non_mediator_involved
                            {
                                created_dt = DateTime.Now,
                                last_name = model.personLastName.ToList()[i],
                                Name = model.personName.ToList()[i],
                                report_id = currentReport.id,
                                Title
                            };
                        }*/
                        //foreach(var person in model.pe)

                        ////   if (model.mediatorsInvolved != null)
                        foreach (var item in db.company_case_routing_location.Where(x => x.company_id == currentReport.company_id & x.company_location_id == currentReport.location_id).ToList())
                        {
                            if ((model.mediatorsInvolved == null) || !model.mediatorsInvolved.Contains(item.user_id.ToString()))
                            {
                                var cl = db.company_location.FirstOrDefault(x => x.id == item.company_location_id);
                                if ((cl != null) && (cl.status_id == 2))
                                {
                                    db.report_mediator_assigned.Add(new report_mediator_assigned
                                    {
                                        report_id = currentReport.id,
                                        by_location_id = item.company_location_id,
                                        mediator_id = item.user_id,
                                        last_update_dt = DateTime.Now,
                                        assigned_dt = DateTime.Now,
                                        status_id = 2,
                                        user_id = currentReport.user_id,
                                    });
                                }
                            }
                        }

                        ////////    if (model.mediatorsInvolved != null)
                        {
                            foreach (var item in db.company_case_routing.Where(x => x.company_id == currentReport.company_id && model.whatHappened.Contains(x.company_secondary_type_id)).ToList())
                            {
                                if ((model.mediatorsInvolved == null) || !model.mediatorsInvolved.Contains(item.user_id.ToString()))
                                {
                                    var inc = db.company_secondary_type.FirstOrDefault(x => x.id == item.company_secondary_type_id);
                                    if ((inc != null) && (inc.status_id == 2))
                                    {
                                        db.report_mediator_assigned.Add(new report_mediator_assigned
                                        {
                                            report_id = currentReport.id,
                                            mediator_id = item.user_id,
                                            by_secondary_type_id = item.company_secondary_type_id,
                                            last_update_dt = DateTime.Now,
                                            assigned_dt = DateTime.Now,
                                            status_id = 2,
                                            user_id = currentReport.user_id,
                                        });
                                    }
                                }
                            }
                        }

                        //test = adv.SaveChanges();


                        List<report_non_mediator_involved> mediators = model.GetModeMediatorInvolveds();
                        foreach (var item in mediators)
                        {
                            item.added_by_reporter = true;
                            item.report_id = currentReport.id;
                            db.report_non_mediator_involved.Add(item);
                            //AddReportNonMediatorInvolved(item, currentReport);
                        }

                        db.SaveChanges();
                        #region Create Folder for Report

                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Upload/reports/" + _guid));

                        adv.SaveChanges();
                        transaction.Commit();
                        #endregion
                        resultModel.report = currentReport;
                        resultModel.StatusCode = 200;
                        #endregion
                        return resultModel;
                    }
                    catch (DbEntityValidationException e)
                    {
                        resultModel.StatusCode = 500;

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            string str = String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            Console.WriteLine(str);
                            resultModel.ErrorMessage = str;

                            foreach (var ve in eve.ValidationErrors)
                            {
                                string tempError = String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                                Console.WriteLine(tempError);
                                resultModel.ErrorMessage += tempError;
                            }
                        }
                        password = null;
                        transaction.Rollback();
                        return resultModel;
                    }
                    catch (Exception ex)
                    {
                        resultModel.StatusCode = 500;
                        resultModel.ErrorMessage = ex.Message;
                        password = null;
                        transaction.Rollback();
                        return resultModel;
                    }
                }
            }
        }


    public report_investigation_status AddPendingStatus(report_investigation_status item)
        {
            using (ECEntities adv = new ECEntities())
            {
                adv.report_investigation_status.Add(item);

                var report = adv.report.FirstOrDefault(x => x.id == item.report_id);
                report.status_id = item.investigation_status_id;
                report.last_update_dt = DateTime.Now;
                report.user_id = item.user_id;

                adv.SaveChanges();
            }
            return item;
        }


        public report_non_mediator_involved AddReportNonMediatorInvolved(report_non_mediator_involved item, report currentReport)
        {
            item.report_id = currentReport.id;
            using (ECEntities adv = new ECEntities())
            {
                adv.report_non_mediator_involved.Add(item);
                adv.SaveChanges();
            }
            return item;
        }

        public management_know AddManagementKnow(management_know item)
        {
            using (ECEntities adv = new ECEntities())
            {
                adv.management_know.Add(item);
                adv.SaveChanges();
            }
            return item;
        }

        public report_department AddReportDepartment(report_department item)
        {
            using (ECEntities adv = new ECEntities())
            {
                adv.report_department.Add(item);
                adv.SaveChanges();
            }
            return item;
        }

        public report_secondary_type AddSecondaryType(report_secondary_type item)
        {
            using (ECEntities adv = new ECEntities())
            {
                adv.report_secondary_type.Add(item);
                adv.SaveChanges();
            }
            return item;
        }

        public report ReportByName(string name)
        {
            return db.report.FirstOrDefault(item => item.display_name == name);
        }

        public report ReportById(int id)
        {
            ID = id;

            if (id != 0)
            {
                return db.report.Where(item => item.id == id).FirstOrDefault();
            }
            else
                return null;
            //    return null;
            ; //db.report.FirstOrDefault(item => item.id == id);
        }
        #endregion

        #region User - Report
        /// <summary>
        /// mediators who has access to report already = goes on top of "add more mediators" button
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public List<user> MediatorsWhoHasAccessToReport()
        {
            List<user> result = new List<user>();
            user _user;

            List<user> all_top_mediators = db.user.Where(item => (item.company_id == _report.company_id) && (item.role_id == 4 || item.role_id == 5) && item.status_id == 2).ToList();
            List<user> involved_mediators = InvolvedMediatorsUserList();
            List<user> assigned_mediators = AssignedMediatorsUserList();
            var owner = db.report_owner.FirstOrDefault(x => x.report_id == _report.id & x.status_id == 2);

            for (int i = 0; i < all_top_mediators.Count; i++)
            {
                _user = all_top_mediators[i];
                if ((!involved_mediators.Any(item => (item.id == _user.id))) && (!result.Contains(_user)))
                {
                    if ((owner != null) && (owner.user_id == _user.id))
                    {
                        result.Insert(0, _user);
                    }
                    else
                    {
                        result.Add(_user);
                    }
                }
            }
            for (int i = 0; i < assigned_mediators.Count; i++)
            {
                _user = assigned_mediators[i];
                if ((!involved_mediators.Any(item => (item.id == _user.id))) && (!result.Contains(_user)))
                {
                    if ((owner != null) && (owner.user_id == _user.id))
                    {
                        result.Insert(0, _user);
                    }
                    else
                    {
                        result.Add(_user);
                    }
                }
            }


            return result;
        }

        /// <summary>
        /// quick function to get 4 mediators with first as owner
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<QuickUserViewModel> MediatorsWhoHasAccessToReportQuick(IEnumerable<int> top_mediator_ids)
        {
            List<int> result_ids = new List<int>();
            int owner_id = 0;
            var owner = db.report_owner.FirstOrDefault(x => x.report_id == ID & x.status_id == 2);
            if (owner != null)
            {
                owner_id = owner.user_id;
            }
            //IEnumerable<int> top_mediator_ids = db.user.Where(item => (item.company_id == _report.company_id) && (item.role_id == 4 || item.role_id == 5)).Select(t => t.id);
            IEnumerable<int> assigned_mediator_ids = (db.report_mediator_assigned.Where(item => ((item.report_id == ID) && (item.status_id == 2)))).Select(t => t.mediator_id);
            IEnumerable<int> involved_mediator_ids = (db.report_mediator_involved.Where(item => (item.report_id == ID) && item.added_by_reporter != false)).Select(t => t.mediator_id);

            IEnumerable<int> mediator_ids = top_mediator_ids.Concat(assigned_mediator_ids).Distinct();
            mediator_ids = mediator_ids.Except(involved_mediator_ids);
            List<QuickUserViewModel> users = (mediator_ids == null || mediator_ids.Count() == 0) ? new List<QuickUserViewModel>() : db.user.Where(item => (item.status_id == 2 && mediator_ids.Contains(item.id))).Select(z => new QuickUserViewModel
            {
                id = z.id,
                first_nm = z.first_nm,
                last_nm = z.last_nm,
                photo_path = z.photo_path,
                is_owner = false,
                is_signoff = false
            }).ToList();
            // List<user> users = db.user.Where(item => (mediator_ids.Contains(item.id))).ToList();

            if (owner_id != 0 && users.Count > 0 && mediator_ids.Count() > 0 && mediator_ids.Contains(owner_id))
            {
                int tempIndex = users.FindIndex(a => a.id == owner_id);
                if (tempIndex != -1)
                {
                    QuickUserViewModel tmp = users[tempIndex];
                    users[tempIndex] = users[0];
                    tmp.is_owner = true;
                    users[0] = tmp;
                }
            }

            return users;
        }

        public List<report_non_mediator_involved> NonInvolvedMediators()
        {
            return (db.report_non_mediator_involved.Where(item => (item.report_id == ID) && item.added_by_reporter != false)).ToList();
        }

        public List<report_non_mediator_involved> NonInvolvedMediatorsAll()
        {
            return db.report_non_mediator_involved.Where(item => item.report_id == ID).ToList();
        }

        public List<report_case_closure_outcome> CaseClosureOutcome()
        {
            return db.report_case_closure_outcome.Where(item => item.report_id == ID).ToList();
        }

        public List<company_outcome> CompanyOutcomes()
        {
            return db.company_outcome.Where(item => item.company_id == _report.company_id).ToList();
        }

        public List<role_in_report> RoleInReport()
        {
            return db.role_in_report.ToList();
        }

        public List<report_mediator_involved> InvolvedMediators()
        {
            return (db.report_mediator_involved.Where(item => (item.report_id == ID) && item.added_by_reporter != false)).ToList();
        }

        public user SignoffMediator()
        {
            var som = db.report_signoff_mediator.OrderByDescending(x => x.created_on).FirstOrDefault(item => item.report_id == ID);
            return som == null ? null : db.user.FirstOrDefault(x => x.id == som.user_id);
        }

        public List<user> InvolvedMediatorsUserList()
        {
            List<user> result = new List<user>();
            List<report_mediator_involved> mediators = InvolvedMediators();
            user _user;
            int mediator_id = 0;
            for (int i = 0; i < mediators.Count; i++)
            {
                mediator_id = mediators[i].mediator_id;
                _user = db.user.FirstOrDefault(item => item.id == mediator_id);
                result.Add(_user);
            }

            return result;
        }

        public List<report_mediator_assigned> AssignedMediators()
        {
            List<report_mediator_assigned> result = new List<report_mediator_assigned>();
            List<report_mediator_assigned> assigned_list = (db.report_mediator_assigned.Where(item => ((item.report_id == ID) && (item.status_id == 2)))).ToList();
            report_mediator_assigned _assigned;

            // just in case to check mediator is not involved
            List<report_mediator_involved> involved_list = (db.report_mediator_involved.Where(item => (item.report_id == ID))).ToList();

            for (int i = 0; i < assigned_list.Count; i++)
            {
                _assigned = assigned_list[i];
                if ((!involved_list.Any(item => (item.user_id == _assigned.mediator_id))) && (!result.Contains(_assigned)))
                    result.Add(_assigned);
            }

            return result;
        }

        public List<report_non_mediator_involved> GetWitnesses()
        {
            List<report_non_mediator_involved> result = (db.report_non_mediator_involved.Where(item => (item.report_id == ID && item.added_by_reporter != false))).ToList();
            foreach (var item in result)
            {
                var role = db.role_in_report.FirstOrDefault(m => m.id == item.role_in_report_id);
                item.Role = role != null ? role.role_en : App_LocalResources.GlobalRes.Other;
            }
            return result;
        }
        public List<user> AssignedMediatorsUserList()
        {
            List<user> result = new List<user>();
            List<report_mediator_assigned> mediators = AssignedMediators();
            List<user> _users;
            int mediator_id = 0;
            for (int i = 0; i < mediators.Count; i++)
            {
                mediator_id = mediators[i].mediator_id;
                _users = db.user.Where(item => item.id == mediator_id && item.status_id == 2).ToList();
                if (_users.Count > 0)
                    result.Add(_users[0]);
            }

            return result;
        }

        public List<report_owner> ReportOwners()
        {
            return (db.report_owner.Where(item => item.report_id == ID && item.status_id == 2)).ToList();
        }
        public List<user> ReportOwnersUserList()
        {
            List<user> result = new List<user>();
            List<report_owner> mediators = ReportOwners();
            user _user;

            foreach (var mediator in mediators)
            {
                _user = db.user.FirstOrDefault(item => item.id == mediator.user_id);
                result.Add(_user);
            }

            return result;
        }

        /// <summary>
        /// mediators who doesn't have access =  not involved and are not owners ( just ready to assign by owners, and cannot)
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public List<user> AvailableToAssignMediators()
        {
            report _report = db.report.FirstOrDefault(item => item.id == ID);
            List<user> result = new List<user>();
            List<user> users = db.user.Where(item => (item.company_id == _report.company_id) && (item.role_id == 6) && (item.status_id == 2)).ToList();

            user _user;

            for (int i = 0; i < users.Count; i++)
            {
                _user = users[i];
                // if regular mediator is not involved
                if (!db.report_mediator_involved.Any(item => (item.mediator_id == _user.id) && (item.report_id == ID)))
                {
                    // if regular mediator is not assigned already
                    if (!db.report_mediator_assigned.Any(item => (item.mediator_id == _user.id) && (item.report_id == ID) && (item.status_id == 2)))
                        result.Add(_user);
                }
            }

            return result;
        }
        #endregion

        #region Report - Last Message Routine
        /// <summary>
        /// returns last message_id of the report or 0 if message is not exists
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public int ReportLastMessageID(int user_id)
        {
            int message_id = 0;

            message _message;
            UserModel um = new UserModel(user_id);
            if (HasAccessToReport(user_id))
            {
                if (_report != null)
                {
                    if (um._user.role_id != 8)
                    {
                        if (db.message.Any(item => (item.report_id == ID) && ((item.reporter_access != 3) || ((item.reporter_access == 3) && ((item.sent_to_id == user_id || item.sender_id == user_id))))))
                        {
                            _message = db.message.Where(item => (item.report_id == ID) && ((item.reporter_access != 3) || ((item.reporter_access == 3) && ((item.sent_to_id == user_id || item.sender_id == user_id))))).OrderByDescending(item => item.created_dt).First();
                            message_id = _message.id;
                        }
                    }
                    else
                    {
                        // we need to show last message for reporter quey if requestor = reporter
                        if (db.message.Any(item => (item.report_id == ID) && (item.reporter_access == 1)))
                        {
                            _message = db.message.Where(item => (item.report_id == ID) && (item.reporter_access == 1)).OrderByDescending(item => item.created_dt).First();
                            message_id = _message.id;
                        }
                    }
                }
            }

            return message_id;
        }

        public MessageExtended LastMessage(int user_id)
        {
            MessageExtended msg = new MessageExtended();
            int msg_id = ReportLastMessageID(user_id);
            if (msg_id != 0)
            {
                msg = new MessageExtended(msg_id, user_id);
            }
            return msg;
        }
        #endregion

        #region Report - Last Task Routine

        public int ReportLastTaskID(int user_id)
        {
            int task_id = 0;

            task _task;

            if (_report != null)
            {
                user _user = db.user.Where(item => item.id == user_id).FirstOrDefault();
                if (_user.role_id != 8)
                {
                    if (db.task.Any(item => (item.report_id == ID) && (item.assigned_to == user_id)))
                    {
                        _task = db.task.Where(item => (item.report_id == ID) && (item.assigned_to == user_id)).OrderByDescending(item => item.created_on).First();
                        task_id = _task.id;
                    }
                }
                else
                {
                    // no tasks for reporter
                }
            }

            return task_id;
        }

        public TaskExtended LastTask(int user_id)
        {
            TaskExtended tsk = new TaskExtended();
            int task_id = ReportLastTaskID(user_id);
            if (task_id != 0)
            {
                tsk = new TaskExtended(task_id, user_id);
            }
            return tsk;
        }

        #endregion

        #region Report - 3 LastTask Routine

        public List<TaskExtended> ThreeLastTasks(int user_id)
        {
            int task_id = 0;
            List<task> _task_list = new List<task>();
            List<TaskExtended> list_tsk = new List<TaskExtended>();
            if (_report != null)
            {
                user _user = db.user.Where(item => item.id == user_id).FirstOrDefault();
                if (db.task.Any(item => (item.report_id == ID)))
                {
                    _task_list = db.task.Where(item => (item.report_id == ID)).OrderByDescending(item => item.created_on).Take(3).ToList();
                    foreach (task _task in _task_list)
                    {
                        task_id = _task.id;
                        TaskExtended tsk = new TaskExtended(_task.id, user_id);
                        list_tsk.Add(tsk);
                    }
                }
            }

            return list_tsk;
        }

        public List<TaskExtended> ExtendedTasks(int user_id)
        {
            int task_id = 0;
            List<task> _task_list = new List<task>();
            List<TaskExtended> list_tsk = new List<TaskExtended>();
            if (_report != null)
            {
                user _user = db.user.Where(item => item.id == user_id).FirstOrDefault();
                if (db.task.Any(item => (item.report_id == ID)))
                {
                    _task_list = db.task.Where(item => (item.report_id == ID)).OrderByDescending(item => item.created_on).ToList();
                    foreach (task _task in _task_list)
                    {
                        task_id = _task.id;
                        TaskExtended tsk = new TaskExtended(_task.id, user_id);
                        list_tsk.Add(tsk);
                    }
                }
            }

            return list_tsk;
        }
        /// <summary>
        ///  number of tasks in case
        /// </summary>
        /// <param name="user_id">user_id</param>
        /// <param name="task_status">0 - all tasks, 1 - active, 2 - competed</param>
        /// <returns></returns>
        public List<task> ReportTasks(int task_status)
        {
            List<task> all_tasks = new List<task>();
            if (ID != 0)
            {
                if (task_status == 0)
                    all_tasks = db.task.Where(item => item.report_id == ID).OrderByDescending(t => t.created_by).ToList();
                if (task_status == 1)
                    all_tasks = db.task.Where(item => item.report_id == ID && item.is_completed == false).OrderByDescending(t => t.created_by).ToList();
                if (task_status == 2)
                    all_tasks = db.task.Where(item => item.report_id == ID && item.is_completed == true).OrderByDescending(t => t.created_by).ToList();
            }

            return all_tasks;

        }
        /// <summary>
        /// just a number of tasks in case
        /// </summary>
        /// <param name="user_id">user_id</param>
        /// <param name="task_status">0 - all tasks, 1 - active, 2 - competed</param>
        /// <returns></returns>
        public int ReportTasksCount(int task_status)
        {
            int tasks_count = 0;
            if (ID != 0)
            {
                if (task_status == 0)
                    tasks_count = db.task.Where(item => item.report_id == ID).Count();
                if (task_status == 1)
                    tasks_count = db.task.Where(item => item.report_id == ID && item.is_completed == false).Count();
                if (task_status == 2)
                    tasks_count = db.task.Where(item => item.report_id == ID && item.is_completed == true).Count();
            }
            return tasks_count;
        }

        /// <summary>
        /// number of tasks by previous month
        /// </summary>
        /// <param name="task_status"></param>
        /// <returns></returns>
        public List<task> ReportTaskByMonthEnd(int task_status)
        {
            DateTime _month_end_date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);

            List<task> all_tasks = new List<task>();
            if (ID != 0)
            {
                if (task_status == 0)
                    all_tasks = db.task.Where(item => ((item.report_id == ID) && (item.created_on >= _month_end_date))).ToList();
                if (task_status == 1)
                    all_tasks = db.task.Where(item => item.report_id == ID && item.is_completed == false && item.created_on >= _month_end_date).ToList();
                if (task_status == 2)
                    all_tasks = db.task.Where(item => item.report_id == ID && item.is_completed == true && item.created_on >= _month_end_date).ToList();
            }

            return all_tasks;
        }


        /// <summary>
        /// NO CHECK - just when you know user belongs to report
        /// </summary>
        /// <param name="report_id">if report_id = null or 0, its total messages for this user</param>
        /// <param name="thread_id">0 - all messages, 1 - reporter thread, 2 - mediators thread, 3 - privilege thread</param>
        /// <returns></returns>
        public int UserMessagesCountNotSecure(int user_id, int thread_id)
        {
            int messages_count = 0;

            #region Got All messages for current user
            if (thread_id == 1)
            {
                // reporter can see only messages with reporter_access == 1 
                messages_count = (db.message.Where(item => (item.report_id == ID && (item.reporter_access == 1)))).Count();
            }
            else if (thread_id == 0)
            {
                // all messages
                messages_count = (db.message.Where(item => (item.report_id == ID))).Count();
            }
            else
            {
                messages_count = (db.message.Where(item => (item.report_id == ID && (item.reporter_access == 2)))).Count();
            }
            #endregion

            return messages_count;
        }


        #endregion


        /// <summary>
        /// by user_id you can always tell will this user have access to this report
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public bool HasAccessToReport(int user_id)
        {
            user _user = db.user.FirstOrDefault(item => item.id == user_id);
            if ((_user != null) && (_user.id != 0))
            {
                // if user is from Ec - it's always true
                if ((_user.role_id == 1) && (_user.role_id == 2) && (_user.role_id == 3))
                {
                    return true;
                }

                // if user is reporter - checking if report belong to him, reporter_user_id should be equal to user_id
                if (_user.role_id == 8)
                {
                    if (db.report.Any(item => (item.id == ID && item.reporter_user_id == user_id)))
                        return true;
                }

                // if user is mediator from a company
                if ((_user.role_id == 4) || (_user.role_id == 5) || (_user.role_id == 6) || (_user.role_id == 7))
                {

                  UserModel um = new UserModel(user_id);
                  var user_report_ids = um.GetReportIds(null);
                  if (user_report_ids.Contains(ID))
                          return true;
                }

                //legal cannot access any report
                ///  if (_user.role_id == 7)
                ///      return false;

            }
            return false;
        }

        public List<report_log> ReportActions(int user_id, int report_id)
        {
            user _user = db.user.FirstOrDefault(item => item.id == user_id);

            List<report_log> report_actions = new List<report_log>();

            if (_user.role_id == 8)
            {
                // reporter can see only messages with reporter_visible == 1 
                List<int> reporter_actions_list = db.action.Where(item => (item.reporter_visible == true)).Select(item => item.id).ToList();
                report_actions = (db.report_log.Where(item => ((item.report_id == report_id) && (item.action_id.HasValue) && (reporter_actions_list.Contains(item.action_id.Value)))).OrderByDescending(dt => dt.id)).ToList();
            }
            else
                report_actions = (db.report_log.Where(item => (item.report_id == report_id)).OrderByDescending(dt => dt.id)).ToList();

            return report_actions;
        }

        #region Last Investigation Status
        public int _investigation_status
        {
            get
            {
                report_investigation_status last_status = _last_investigation_status();
                if (_last_investigation_status() != null)
                {
                    return last_status.investigation_status_id;
                }
                else
                    return 1;
            }
        }

        /// <summary>
        /// returns the investigation status of report in string.
        /// </summary>
        /// <param name="report_id"></param>
        /// <returns></returns>
        public string InvestigationStatusString()
        {
            string status = "";

            investigation_status _status = db.investigation_status.Where(item => item.id == _investigation_status).FirstOrDefault();
            if (_status != null)
            {
                status = _status.investigation_status_en;
            }

            return status.Trim();
        }

        public report_investigation_status _last_investigation_status()
        {
            report_investigation_status last_status = new report_investigation_status();
            if (db.report_investigation_status.Any(item => item.report_id == ID))
                last_status = db.report_investigation_status.Where(item => item.report_id == ID).OrderByDescending(x => x.id).FirstOrDefault();

            if ((last_status != null) && (last_status.investigation_status_id != 0))
            {
                return last_status;
            }

            return null;
        }

        /// <summary>
        /// User of last investigation status
        /// </summary>
        /// <returns></returns>
        public int Last_investigation_status_user_id()
        {
            report_investigation_status last_status = _last_investigation_status();

            if (last_status != null)
            {
                return last_status.user_id;
            }

            return 0;
        }

        public user Last_investigation_status_user()
        {
            report_investigation_status last_status = _last_investigation_status();

            if (last_status == null)
            {
                return null;
            }

            return db.user.FirstOrDefault(x => x.id == last_status.user_id);
        }

        /// <summary>
        /// User of last investigation status
        /// </summary>
        /// <returns></returns>
        public DateTime? Last_investigation_status_date()
        {
            report_investigation_status last_status = _last_investigation_status();

            if (last_status != null)
            {
                return last_status.created_date;
            }

            return null;
        }

        public string Last_investigation_case_closure_reason()
        {
            report_investigation_status last_status = _last_investigation_status();

            if ((last_status == null) || (!last_status.case_closure_reason_id.HasValue))
            {
                return null;
            }

            var reason = db.case_closure_reason.FirstOrDefault(x => x.id == last_status.case_closure_reason_id);

            return reason == null ? null : reason.case_closure_reason_en;
        }
        #endregion


        #region Previous Investigation Status

        /// <summary>
        /// return last, but 1 status. need it to check where case came from
        /// </summary>
        public int _previous_investigation_status_id()
        {
            report_investigation_status last_status = new report_investigation_status();
            report_investigation_status previous_last_status = new report_investigation_status();

            if (db.report_investigation_status.Any(item => item.report_id == ID))
            {
                List<report_investigation_status> statuses = db.report_investigation_status.Where(item => item.report_id == ID).OrderByDescending(x => x.id).ToList();
                if (statuses.Count > 1)
                {
                    previous_last_status = statuses[1];
                    return previous_last_status.investigation_status_id;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public report_investigation_status _previous_investigation_status()
        {
            List<report_investigation_status> statuses = new List<report_investigation_status>();
            if (db.report_investigation_status.Any(item => item.report_id == ID))
                statuses = db.report_investigation_status.Where(item => item.report_id == ID).OrderByDescending(x => x.id).ToList();

            if ((statuses != null) && (statuses.Count > 1))
            {
                return statuses[1];
            }

            return null;
        }
        public int _previous_investigation_status_user_id()
        {
            report_investigation_status _status = _previous_investigation_status();

            if (_status != null)
            {
                return _status.user_id;
            }

            return 0;
        }
        #endregion

        public int report_status_id_by_date(DateTime dt)
        {
            if ((_report == null) || (_report.reported_dt > dt))
            {
                // error in report or its not created yet
                return 0;
            }

            report_investigation_status last_status = new report_investigation_status();
            if (db.report_investigation_status.Any(item => item.report_id == ID))
            {
                List<report_investigation_status> _statuses = db.report_investigation_status.Where(item => item.report_id == ID && item.created_date < dt).OrderByDescending(x => x.created_date).ToList();
                if (_statuses.Count > 0)
                {
                    last_status = _statuses[0];
                    return last_status.investigation_status_id;
                }
                else
                    return 1;
            }
            else
                return 1;

            //          return 0;
        }

        public bool isCustomIncidentTypes(int companyId)
        {
            bool flag = false;
            int array = 0;
            array = db.company_secondary_type.Where(item => item.company_id == companyId && item.status_id == 2).Count();
            if (array > 0)
            {
                flag = true;
            }
            return flag;
        }
        public List<secondary_type_mandatory> getSecondaryTypeMandatory()
        {
            List<secondary_type_mandatory> types = new List<secondary_type_mandatory>();
            types = db.secondary_type_mandatory.Where(item => item.type_id == 1).ToList();
            return types;
        }
        public List<company_secondary_type> getCompanySecondaryType(int companyId)
        {
            List<company_secondary_type> types = new List<company_secondary_type>();
            types = db.company_secondary_type.Where(item => item.company_id == companyId && item.status_id == 2).ToList();
            return types;
        }
        public List<company_relationship> getCustomRelationshipCompany(int idCompany)
        {

            /*List<company_relationship> relationShipCompany = new List<company_relationship>();
            relationShipCompany = db.company_relationship.Where(item => item.company_id == idCompany && item.status_id == 2).ToList();*/
            List<company_relationship> relationShipCompany = db.company_relationship.Where(item => item.company_id == idCompany && item.status_id == 2).ToList();
            return relationShipCompany;
        }
        public string SaveLoginChanges(int userId, string password)
        {
            if (userId > 0)
            {
                try
                {
                    string result = GlobalFunctions.IsValidPass(password);
                    if (result.ToLower() == "success")
                    {

                        user user = db.user.FirstOrDefault(item => (item.id == userId));
                        if (user != null)
                        {
                            using (ECEntities adv = new ECEntities())
                            {
                                user.password = PasswordUtils.GetHash(password);
                                user.last_update_dt = DateTime.Now;
                                adv.user.AddOrUpdate(user);
                                adv.SaveChanges();

                                //  db.user.AddOrUpdate(user);
                                //  db.SaveChanges();
                            }
                            return result;
                        }
                        else
                        {
                            return GlobalRes.NoUserFound;
                        }
                    }
                    else
                        return result;

                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    return "Cannot update password " + ex.ToString();// GlobalRes.ErrorSavingLoginPass;
                }
            }
            else
            {
                return "Cannot update your password "; // GlobalRes.ErrorSavingLoginPass;
            }
        }

        public List<CaseInvestigationStatusViewModel> CaseClosuresMessages()
        {
            List<report_investigation_status> dbReport_Investigation_statuses = db.report_investigation_status.Where(item => item.report_id == ID).OrderBy(item => item.created_date).ToList();
            List<CaseInvestigationStatusViewModel> investiogationStatusesList = new List<CaseInvestigationStatusViewModel>();

            foreach (report_investigation_status ris in dbReport_Investigation_statuses)
            {
                ///  cannot cast directly -> casting through json   CaseInvestigationStatusViewModel cisvm = (CaseInvestigationStatusViewModel)ris;
                var serializedParent = JsonConvert.SerializeObject(ris);
                CaseInvestigationStatusViewModel cisvm = JsonConvert.DeserializeObject<CaseInvestigationStatusViewModel>(serializedParent);

                cisvm.previous_investigation_status = 0;
                cisvm.investigation_status_name = "";
                cisvm.query_new_investigation_status_name = "";

                investiogationStatusesList.Add(cisvm);
            }

            if (investiogationStatusesList.Count > 0)
            {
                CaseInvestigationStatusViewModel cisvm = investiogationStatusesList[0];
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Resolution)
                    cisvm.query_new_investigation_status_name = App_LocalResources.GlobalRes.CaseSentToEsacaltionMediatorForReview;

            }

            for (int i = 1; i < investiogationStatusesList.Count(); i++)
            {
                CaseInvestigationStatusViewModel cisvm = investiogationStatusesList[i];
                CaseInvestigationStatusViewModel cisvm_prev = investiogationStatusesList[i - 1];

                cisvm.previous_investigation_status = cisvm_prev.investigation_status_id;
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Resolution)
                    cisvm.query_new_investigation_status_name = App_LocalResources.GlobalRes.CaseSentToEsacaltionMediatorForReview;

                //case just closed
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Closed)
                    cisvm.query_new_investigation_status_name = App_LocalResources.GlobalRes.Approved;

                //case just sent to investigation
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Investigation && (cisvm.previous_investigation_status == 0 || cisvm.previous_investigation_status == 1 || cisvm.previous_investigation_status == 2 || cisvm.previous_investigation_status == 7))
                    cisvm.query_new_investigation_status_name = "";

                //case just sent to investigation
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Investigation && cisvm.previous_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Resolution)
                    cisvm.query_new_investigation_status_name = App_LocalResources.GlobalRes.CaseReturnedFutherInvestigation;

                //case just sent to investigation
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Investigation && cisvm.previous_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Closed)
                    cisvm.query_new_investigation_status_name = App_LocalResources.GlobalRes.CaseReOpened;

            }
            return investiogationStatusesList;
        }

        public string CaseStatusGreenBarTitle()
        {
            string _green_bar_status = "";
            int _prev_inv_status_id = _previous_investigation_status_id();

            //case just closed
            if (_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Closed)
                _green_bar_status = GlobalRes.CaseClosed;

            //current - investigation, previous - closed => Re-opened
            if (_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Investigation && _prev_inv_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Closed)
            {
                _green_bar_status = GlobalRes.CaseReOpened;
            }

            //current - investigation, previous - Resolution => Returned for futher investigation
            if (_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Investigation && _prev_inv_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Resolution)
            {
                _green_bar_status = GlobalRes.CaseReturnedFutherInvestigation;
            }

            //current - Resolution
            if (_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Resolution)
            {
                _green_bar_status = GlobalRes.CaseClosureReport;
            }


            return _green_bar_status;
        }

        public string CaseStatusGreenBarSubTitle()
        {
            string _green_bar_status = "";
            //current - Resolution
            if (_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Resolution)
            {
                _green_bar_status = GlobalRes.CaseSentToEsacaltionMediatorForReview;
            }

            return _green_bar_status;
        }

        private string GenerateCaseNumber(int report_id, int company_id, string company_name)
        {
            string letter = "EMP";
            if (company_name.Length > 0)
                letter = company_name[0].ToString().ToUpper();
            if (company_id == 1)
                letter = "UNK";
            if (company_id == 2)
                letter = "STA";
            int number = 2000 + report_id;
            string case_number = number.ToString() + "-" + letter + "-" + company_id.ToString();

            return case_number;
        }

        private string GenerateReporterLogin()
        {
            Random rd = new Random();
            string reporter_login = "";

            do
            {
                reporter_login = reporter_login + rd.Next(0, 9).ToString();
            }
            while (reporter_login.Length < 6);

            //     ?while ((reporter_login.Length + report_id.ToString().Length) < 6);

            return "RE" + reporter_login;
        }

        private int GetNextColor(int company_id, int report_id)
        {
            int color_id = 1;

            List<report> reports = new List<report>();


            if (db.report.Any(item => ((item.company_id == company_id) && (item.id != report_id))))
                reports = (db.report.Where(item => ((item.company_id == company_id) && (item.id != report_id))).OrderByDescending(item => item.id)).ToList();

            if (reports.Count > 0)
            {
                report _report = reports[0];
                color_id = _report.report_color_id;
                int _all_colors_count = db.color.Count();

                if (color_id < _all_colors_count)
                {
                    color_id++;
                }
                else
                    color_id = 1;
            }

            return color_id;
        }
        public static List<role_in_report> getRoleInReport()
        {
            using (ECEntities adv = new ECEntities())
            {
                return adv.role_in_report.ToList();
            }
        }

        public List<user> MediatorsApproveCaseClosure()
        {
            List<user> access_mediators = MediatorsWhoHasAccessToReport().Where(t => t.role_id == 5 || (t.user_permissions_approve_case_closure.HasValue && t.user_permissions_approve_case_closure.Value == 1)).ToList();
            return access_mediators;

            /*    return (
                    from m in db.report_mediator_assigned.Where(x => x.report_id == ID).Select(x => x.mediator_id).Distinct().ToList()
                    join u in db.user.Where(x => x.user_permissions_approve_case_closure != 1) on m equals u.id
                    select u).ToList();*/
        }

        public List<UserViewModel> MediatorsAcceptCase()
        {

            var res = new List<UserViewModel>();

            var list1 = MediatorsWhoHasAccessToReport();
            var list2 = InvolvedMediatorsUserList();
            var list3 = AvailableToAssignMediators();

            //By level
            res.AddRange(list1
                .Where(x => x.role_id == 4)
                .Select(x => new UserViewModel(x) { Detail = "Case Reviewer" })
                .OrderBy(x => x.FullName)
                .ToList());

            res.AddRange(list1
                .Where(x => x.role_id == 5)
                .Select(x => new UserViewModel(x) { Detail = "Platform Manager" })
                .OrderBy(x => x.FullName)
                .ToList());

            //ids not top level
            var ids = list1.Where(x => x.role_id != 4 && x.role_id != 5).Select(x => x.id).ToList();

            var list = (
                from ma in db.report_mediator_assigned.Where(x => ids.Contains(x.mediator_id) & x.report_id == ID)
                join ju in db.user on ma.mediator_id equals ju.id into j1
                join jcl in db.company_location on ma.by_location_id equals jcl.id into j2
                join jst in db.company_secondary_type on ma.by_secondary_type_id equals jst.id into j3
                from u in j1.DefaultIfEmpty()
                from cl in j2.DefaultIfEmpty()
                from st in j3.DefaultIfEmpty()
                select new
                {
                    User = u,
                    MA = ma,
                    CL = cl,
                    ST = st
                }
                )
                .Where(x => x.User != null)
                .ToList()
                .Select(x =>
                    new UserViewModel(x.User)
                    {
                        Detail = x.MA.by_location_id != null ? "On case team due to location: " + x.CL.location_en : x.MA.by_secondary_type_id != null ? "On case team due to incident type: " + x.ST.secondary_type_en : ""
                    }
                ).OrderBy(x => x.FullName).ToList();
            res.AddRange(list);

            //Other
            res.AddRange(list2
                .Select(x => new UserViewModel(x) { Detail = "" })
                .OrderBy(x => x.FullName)
                .ToList()
                );

            res.AddRange(list3
                .Select(x => new UserViewModel(x) { Detail = "" })
                .OrderBy(x => x.FullName)
                .ToList()
                );

            return res.GroupBy(x => x.User.id).Select(x => x.First()).ToList();
        }

        public string InvestigationMethodology()
        {
            return $"{db.report_inv_notes.FirstOrDefault(x => x.report_id == ID & x.type == 2)?.note}";
        }

        public string FactsEstablished()
        {
            return $"{db.report_inv_notes.FirstOrDefault(x => x.report_id == ID & x.type == 1)?.note}";
        }

        public string ExecutiveSummary()
        {
            var report_cc_crime = db.report_cc_crime
                .Where(x => x.report_id == ID)
                .FirstOrDefault();

            return $"{report_cc_crime?.executive_summary}";
        }

        /// <summary>
        /// Returns mediator, who is having sign-off priviligies.
        /// </summary>
        /// <returns></returns>
        public user GetSignOffMeditoar()
        {
            return db.report_signoff_mediator.FirstOrDefault(x => x.report_id == ID && x.status_id == 2)?.user1;
        }

        public string DaysLeftClosedSpamMessage(int delay_allowed)
        {
          if (_report.status_id == ECGlobalConstants.investigation_status_spam)
            return $"Sent on {m_DateTimeHelper.ConvertDateToShortString(_report.last_update_dt) }";
          else if (_report.status_id == ECGlobalConstants.investigation_status_closed)
            return $"Closed on {m_DateTimeHelper.ConvertDateToShortString(_report.last_update_dt) }";
          else
          {
            int days_left = GetThisStepDaysLeft(delay_allowed);
            return $"{days_left}" + (days_left == 1 ? " day left" : " days left");
          }
        }
        public string GetAgentName()
        {
            if(this._report.agent_id!=null && this._report.agent_id > 0)
            {
                return db.user.Find(this._report.agent_id).first_nm;
            } else
            {
                return "";
            }
            
        }
    }

}