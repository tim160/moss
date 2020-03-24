using System;
using System.Collections.Generic;
using System.Linq;
using EC.Models.Database;
using EC.Constants;
using EC.Models.ViewModels;
using Newtonsoft.Json;
using EC.Localization;

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

        #region Base constructors
        public ReportModel()
        {
            ID = 0;
        }

        public ReportModel(int report_id)
        {
            ID = report_id;
            _report = ReportById(ID);
            _reporter_user = GetReporterUser(ID);
            _reportStringModel = GetReportStringModel(_report, _reporter_user);
        }
        private report ReportById(int id)
        {
            ID = id;

            if (id != 0)
            {
                return db.report.Where(item => item.id == id).FirstOrDefault();
            }
            else
                return null;

        }
        private ReportStringModel GetReportStringModel(int id)
        {
            if (id != 0)
            {
                ReportStringModel rsm = new ReportStringModel(id);
                return rsm;
            }
            else
                return null;
        }
        private ReportStringModel GetReportStringModel(report _report_from_parent, user _report_user)
        {
            if (_report_from_parent != null && _report_user != null)
            {
                ReportStringModel rsm = new ReportStringModel(_report_from_parent, _report_user);
                return rsm;
            }
            else
                return null;
        }
        private user GetReporterUser(int id)
        {
            if ((_report != null) && (ID != 0))
            {
                user _user = db.user.Where(item => item.id == _report.reporter_user_id).FirstOrDefault();
                return _user;
            }
            return null;
        }
        #endregion

        #region Properties
        public int ID
        { get; set; }

        public report _report;
        public user _reporter_user;
        public ReportStringModel _reportStringModel;
        #endregion

        public string Get_reporter_name(int caller_id)
        {
            string name = "";
            if (_reporter_user != null)
            {
                if (caller_id == _reporter_user.id)
                    return LocalizationGetter.GetString("You");

                user _caller = db.user.Where(item => item.id == caller_id).FirstOrDefault();

                if (_caller != null)
                {
                    if ((_caller.role_id < 8) && (_caller.role_id > 3))
                    {
                        if (_report.incident_anonymity_id == 3)
                        {
                            return Get_reporter_name_reporterView();
                        }
                    }
                    if ((_caller.role_id < 4) && (_caller.role_id > 0))
                    {
                        if ((_report.incident_anonymity_id == 3) || (_report.incident_anonymity_id == 2))
                        {
                            return Get_reporter_name_reporterView();
                        }
                    }
                    if (_caller.role_id == 8)
                    {
                        name = _reporter_user.first_nm + " " + _reporter_user.last_nm;
                        if ((_reporter_user.first_nm + " " + _reporter_user.last_nm).Trim().Length > 0)
                            return name;
                        if (name.Trim().Length == 0)
                            name = LocalizationGetter.GetString("anonymous_reporter");
                    }
                }

            }
            if (name.Trim().Length == 0)
                name = LocalizationGetter.GetString("anonymous_reporter");

            return name.Trim();

        }

        /// <summary>
        /// Use it in Reporter Pages ONLY!!!! - Report/New, ReporterDashboard ( pdfs for them)   - GET THE REPORTER NAME WITHOUT ANONYMITY CHECK
        /// </summary>
        /// <returns></returns>
        public string Get_reporter_name_reporterView()
        {
            string name = "";

            if ((_reporter_user != null) && ((_reporter_user.first_nm + " " + _reporter_user.last_nm).Trim().Length > 0))
                name = _reporter_user.first_nm + " " + _reporter_user.last_nm;

            return name.Trim();
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
                list.Add(LocalizationGetter.GetString("unknown_departments"));
            }

            return list;
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
                            list.Add(LocalizationGetter.GetString("Other"));
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
            if (_report.status_id == (int)CaseStatusConstants.CaseStatusValues.Closed || _report.status_id == (int)CaseStatusConstants.CaseStatusValues.Spam)
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
            if ((_investigation_status == (int)CaseStatusConstants.CaseStatusValues.Spam) || (_prev_inv_status_id == (int)CaseStatusConstants.CaseStatusValues.Spam && _investigation_status == (int)CaseStatusConstants.CaseStatusValues.Closed))
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
            if ((_investigation_status == (int)CaseStatusConstants.CaseStatusValues.Completed) || (_investigation_status == (int)CaseStatusConstants.CaseStatusValues.Resolution))
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
            if (((_investigation_status == (int)CaseStatusConstants.CaseStatusValues.Closed) && (_prev_inv_status_id != (int)CaseStatusConstants.CaseStatusValues.Spam)))
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
            if ((_investigation_status == (int)CaseStatusConstants.CaseStatusValues.Pending) || (_investigation_status == (int)CaseStatusConstants.CaseStatusValues.Review))
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
                            return LocalizationGetter.GetString("anonymous_reporter");
                        }
                        if ((role_id > 0) && (role_id < 4))
                        {
                            return String.Format(_anonymity.anonymity_en, CompanyName());
                        }
                        return LocalizationGetter.GetString("anonymous_reporter");

                    }

                }
            }

            return anon_level;

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
                item.Role = role != null ? role.role_en : LocalizationGetter.GetString("Other");
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


        #region Report - Tasks  

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


        public report_investigation_status _last_investigation_status()
        {
            report_investigation_status last_status = new report_investigation_status();
            if (db.report_investigation_status.Where(item => item.report_id == ID).Any())
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
                    cisvm.query_new_investigation_status_name = LocalizationGetter.GetString("CaseSentToEsacaltionMediatorForReview");

            }

            for (int i = 1; i < investiogationStatusesList.Count(); i++)
            {
                CaseInvestigationStatusViewModel cisvm = investiogationStatusesList[i];
                CaseInvestigationStatusViewModel cisvm_prev = investiogationStatusesList[i - 1];

                cisvm.previous_investigation_status = cisvm_prev.investigation_status_id;
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Resolution)
                    cisvm.query_new_investigation_status_name = LocalizationGetter.GetString("CaseSentToEsacaltionMediatorForReview");

                //case just closed
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Closed)
                    cisvm.query_new_investigation_status_name = LocalizationGetter.GetString("Approved");

                //case just sent to investigation
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Investigation && (cisvm.previous_investigation_status == 0 || cisvm.previous_investigation_status == 1 || cisvm.previous_investigation_status == 2 || cisvm.previous_investigation_status == 7))
                    cisvm.query_new_investigation_status_name = "";

                //case just sent to investigation
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Investigation && cisvm.previous_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Resolution)
                    cisvm.query_new_investigation_status_name = LocalizationGetter.GetString("CaseReturnedFutherInvestigation");

                //case just sent to investigation
                if (cisvm.investigation_status_id == (Int32)CaseStatusConstants.CaseStatusValues.Investigation && cisvm.previous_investigation_status == (Int32)CaseStatusConstants.CaseStatusValues.Closed)
                    cisvm.query_new_investigation_status_name = LocalizationGetter.GetString("CaseReOpened");

            }
            return investiogationStatusesList;
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

        public WrapperUserViewModel MediatorsAcceptCase()
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
                           .ToList();
            var listSelected = list
                .Select(x =>
                    new UserViewModel(x.User)
                    {
                        Detail = x.MA.by_location_id != null ? "On case team due to location: " + x.CL.location_en : x.MA.by_secondary_type_id != null ? "On case team due to incident type: " + x.ST.secondary_type_en : ""
                    }
                ).OrderBy(x => x.FullName).ToList();

            res.AddRange(listSelected);

            var ListByCaseType = list.Where(x => x.MA.by_secondary_type_id != null).Select(x =>
                    new UserViewModel(x.User)
                    {
                        Detail = "On case team due to incident type: " + x.ST.secondary_type_en
                    }
                ).OrderBy(x => x.FullName).ToList();

            var ListByLocation = list.Where(x => x.MA.by_location_id != null).Select(x =>
                    new UserViewModel(x.User)
                    {
                        Detail = x.MA.by_location_id != null ? "On case team due to location: " + x.CL.location_en : x.MA.by_secondary_type_id != null ? "On case team due to incident type: " + x.ST.secondary_type_en : ""
                    }
                ).OrderBy(x => x.FullName).ToList();

            ListByCaseType.AddRange(ListByLocation);
            int selectedMediatorId = 0;
            if (ListByCaseType.Count > 0)
            {
                selectedMediatorId = ListByCaseType[0].User.id;
            }


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
            WrapperUserViewModel model = new WrapperUserViewModel();
            model.listMediators = res.GroupBy(x => x.User.id).Select(x => x.First()).ToList();
            model.Selectedmediator = selectedMediatorId;
            return model;
        }


        /// <summary>
        /// Returns mediator, who is having sign-off priviligies.
        /// </summary>
        /// <returns></returns>
        public user GetSignOffMeditoar()
        {
            return db.report_signoff_mediator.FirstOrDefault(x => x.report_id == ID && x.status_id == 2)?.user1;
        }

        public List<report> ReportsListForCompany(int[] company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
        {
            //UserModel um = new UserModel(user_id);
            //List<report> _all_reports_old = um.ReportsSearch(company_id, 0);
            var _all_reports_old = db.report.Where(r => company_id.Contains(r.company_id)).ToList();

            if (dtReportCreationStartDate.HasValue)
            {
                _all_reports_old = _all_reports_old.Where(t => t.reported_dt.Date >= dtReportCreationStartDate.Value.Date).ToList();
            }

            if (dtReportCreationEndDate.HasValue)
            {
                _all_reports_old = _all_reports_old.Where(t => t.reported_dt.Date <= dtReportCreationEndDate.Value.Date).ToList();
            }
            List<report> _all_reports = new List<report>();

            #region Get the list of ReportIDs allowed

            List<String> ReportsSecondaryTypesIDs = new List<String>();

            List<String> ReportsRelationTypesIDs = new List<String>();

            List<String> ReportsDepartmentIDs = new List<String>();

            List<String> ReportsLocationIDs = new List<String>();



            if (ReportsSecondaryTypesIDStrings.Trim().Length > 0)
                ReportsSecondaryTypesIDs = ReportsSecondaryTypesIDStrings.Split(',').ToList();

            if (ReportsRelationTypesIDStrings.Trim().Length > 0)
                ReportsRelationTypesIDs = ReportsRelationTypesIDStrings.Split(',').ToList();

            if (ReportsDepartmentIDStringss.Trim().Length > 0)
                ReportsDepartmentIDs = ReportsDepartmentIDStringss.Split(',').ToList();

            if (ReportsLocationIDStrings.Trim().Length > 0)
                ReportsLocationIDs = ReportsLocationIDStrings.Split(',').ToList();

            if (ReportsDepartmentIDs.Count > 0)
            {


                var idReports = _all_reports_old.Select(report => report.id).ToList();
                var DepAndReports = db.report_department.Join(db.company_department,
                                        post => post.department_id,
                                        meta => meta.id,
                                        (post, meta) => new { Post = post, Meta = meta })
                                        .Where(postAndMeta => ReportsDepartmentIDs.Any(repDep => repDep.Equals(postAndMeta.Meta.department_en)) && idReports.Contains(postAndMeta.Post.report_id));
                _all_reports_old = _all_reports_old.Where(oldReport => DepAndReports.Select(res => res.Post.report_id).Contains(oldReport.id)).ToList();
            }
            if (ReportsRelationTypesIDs.Count > 0)
            {
                var idReports = _all_reports_old.Select(report => report.id).ToList();
                HashSet<report> listReports = new HashSet<report>();

                if (ReportsRelationTypesIDs.Where(x => x == "Other").ToList().Count > 0)
                {
                    var listOther = db.report_relationship.Where(rel_rel => rel_rel.company_relationship_id == null && idReports.Contains(rel_rel.report_id)).Select(rep => rep.report_id).ToList();

                    var reportsContainstOther = _all_reports_old.Where(old_rep => listOther.Contains(old_rep.id));

                    foreach (var item in reportsContainstOther)
                    {
                        listReports.Add(item);
                    }
                }
                //здесь неправильно скорей всего
                var relationship = _all_reports_old.Join(db.company_relationship,
                                                post => post.company_relationship_id,
                                                meta => meta.id,
                                                (post, meta) => new { Post = post, Meta = meta })
                                                .Where(postAndMeta => ReportsRelationTypesIDs.Any(listReportsRel => listReportsRel.Equals(postAndMeta.Meta.relationship_en)));
                var reportsWithFilter = relationship.Select(rel => rel.Post).ToList();
                foreach (var item in reportsWithFilter)
                {
                    listReports.Add(item);
                }
                _all_reports_old = listReports.ToList();
            }
            if (ReportsSecondaryTypesIDs.Count > 0)
            {
                var idReports = _all_reports_old.Select(report => report.id).ToList();
                HashSet<report> listReports = new HashSet<report>();
                if (ReportsSecondaryTypesIDs.Where(x => x.Equals("Other")).ToList().Count > 0)
                {
                    var listOther = db.report_secondary_type.Where(sec_type => sec_type.secondary_type_id == 0 && idReports.Contains(sec_type.report_id)).Select(sec_type => sec_type.report_id).ToList();

                    var reportsContainstOther = _all_reports_old.Where(old_rep => listOther.Contains(old_rep.id));

                    foreach (var item in reportsContainstOther)
                    {
                        listReports.Add(item);
                    }
                }

                var secondaryTypes = db.report_secondary_type.Join(db.company_secondary_type,
                    post => post.secondary_type_id,
                    meta => meta.id,
                    (post, meta) => new { Post = post, Meta = meta })
                    .Where(postAndMeta => ReportsSecondaryTypesIDs.Any(listSecTypes => listSecTypes.Equals(postAndMeta.Meta.secondary_type_en))
                    && idReports.Contains(postAndMeta.Post.report_id));
                var reportsWithFilter = secondaryTypes.Select(types => types.Post.report_id).ToList();

                var selectedReports = _all_reports_old.Where(rep => reportsWithFilter.Contains(rep.id));

                foreach (var item in selectedReports)
                {
                    listReports.Add(item);
                }
                _all_reports_old = listReports.ToList();
            }

            if (ReportsLocationIDs.Count > 0)
            {
                var temp = _all_reports_old.Join(db.company_location,
                                                post => post.location_id,
                                                meta => meta.id,
                                                (post, meta) => new { Post = post, Meta = meta })
                                                .Where(postAndMeta => ReportsLocationIDs.Any(listLocation => listLocation.Equals(postAndMeta.Meta.location_en))
                                                );
                _all_reports_old = new List<report>();
                foreach (var item in temp)
                {
                    _all_reports_old.Add(item.Post);
                };
            }
            #endregion

            return _all_reports_old;
        }
    }

}