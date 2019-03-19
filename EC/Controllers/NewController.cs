using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Models;
using EC.Models.Database;
using EC.Common.Interfaces;
using EC.Core.Common;
using EC.Constants;
using log4net;
using LavaBlast.Util.CreditCards;
using EC.Common.Util;
using EC.Localization;
using EC.Utils;

namespace EC.Controllers
{
    public class NewController : BaseController
    {

        // GET: New
        public ActionResult Index(string code, string email)
        {
            #region EC-CC Viewbag
            //     bool is_cc = glb.IsCC(Request.Url.AbsoluteUri.ToLower());
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            List<SelectListItem> temp_items = new List<SelectListItem>();
            SelectListItem _temp_department = new SelectListItem { Text = LocalizationGetter.GetString("Other"), Value = "0" };
            temp_items.Add(_temp_department);
            ViewBag.currentDepartmens = temp_items;

            if (!string.IsNullOrEmpty(code))
            {
                ViewBag.code = code;
                List<invitation> invitations = db.invitation.Where(t => ((t.email.ToLower().Trim() == email.ToLower().Trim())
                && (t.used_flag == 0) && (t.code.Trim().ToLower() == code.Trim().ToLower()))).ToList();
                invitation _invitation = invitations[0];
                var invitation_id = _invitation.id;
                UserModel um = new UserModel(_invitation.sent_by_user_id);

                company selectedCompany = db.company.Where(m => m.id == um._user.company_id).FirstOrDefault();
                if (selectedCompany != null)
                {
                    List<company_department> currentDepartmens = db.company_department.Where(m => m.company_id == selectedCompany.id && m.status_id == 2).ToList();
                    /*put drop daun*/
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (var item in currentDepartmens)
                    {
                        SelectListItem dep = new SelectListItem { Text = item.department_en, Value = item.id.ToString() };
                        items.Add(dep);
                    }
                    /*put other*/
                    SelectListItem temp = new SelectListItem { Text = LocalizationGetter.GetString("Other"), Value = "0" };
                    items.Add(temp);
                    ViewBag.currentDepartmens = items;
                    ViewBag.company_id = selectedCompany.id;
                }
            }


            string _email = "";
            if (!string.IsNullOrEmpty(email))
                _email = email;

            ViewBag.email = _email;
            return View();
        }
        public ActionResult Company(string id, string code, string data)
        {
            string _code = "";
            if (!string.IsNullOrEmpty(code))
                _code = code;

            ViewBag.code = _code;
            ViewBag.id = id;
            #region EC-CC Viewbag
            bool is_cc = DomainUtil.IsCC(Request.Url.AbsoluteUri.ToLower());
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            if (!String.IsNullOrEmpty(data))
            {
                return View("CompanyA");
            }
            if (id != null && id != "0")
            {
                var model = db.var_info.FirstOrDefault(x => x.emailed_code_to_customer == id) ?? new var_info();

                return View(model);
            }
            return View(new var_info());
        }

        public ActionResult Success(string show)
        {
            #region EC-CC Viewbag
            bool is_cc = DomainUtil.IsCC(Request.Url.AbsoluteUri.ToLower());
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            int _show = 0;
            if (!string.IsNullOrEmpty(show))
            {
                int.TryParse(show, out _show);

            }
            //int user_id = 2;
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");


            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            ViewBag.user_nm = um._user.login_nm;
            ViewBag.password = um._user.password;
            CompanyModel cm = new CompanyModel(um._user.company_id);
            ViewBag.company_nm = cm._company.company_nm;

            if (_show == 1)
            {
                ViewBag.code = cm._company.company_code;
            }
            else
            {
                ViewBag.code = "";
            }
            ViewBag.logo = cm._company.path_en;


            string auth = "";
            decimal amount = 0;
            if (Session["Auth"] != null)
                auth = Session["Auth"].ToString();
            if (Session["Amount"] != null)
                amount = Convert.ToDecimal(Session["Amount"]);

            ViewBag.authCode = auth;
            if (amount > 0)
                ViewBag.amount = string.Format("{0:0.00}", amount);
            else
                ViewBag.amount = "0";

            return View();
        }
        public string CreateCompany(
            string emailed_code_to_customer,
            string code,
            string location,
            string company_name,
            string number,
            string first,
            string last,
            string email,
            string title,
            string departments,
            string amount,
            string cardnumber,
            string cardname,
            string csv,
            string selectedMonth,
            string selectedYear/*,
            int contractors_number = 0,
            int customers_number = 0,
            int oboarding_sessions_number = 0*/)
        {
            int company_id = 0;
            int user_id = 0;
            int location_id = 0;
            int language_id = 1;


            if (
                (string.IsNullOrEmpty(code)) ||
                (string.IsNullOrEmpty(location)) ||
                (string.IsNullOrEmpty(company_name)) ||
                (string.IsNullOrEmpty(number)) ||
                (string.IsNullOrEmpty(first)) ||
                (string.IsNullOrEmpty(last)) ||
                (string.IsNullOrEmpty(email)) ||
                (string.IsNullOrEmpty(title)))
            {
                return LocalizationGetter.GetString("EmptyData");
            }
            if (glb.isCompanyInUse(company_name))
            {
                return LocalizationGetter.GetString("CompanyInUse", is_cc);
            }
            var company_short_name = StringUtil.ShortString(company_name.Trim());
            if (glb.isCompanyShortInUse(company_short_name))
            {
                for (var i = 1; i < 1000; i++)
                {
                    if (!glb.isCompanyShortInUse(company_short_name + i))
                    {
                        company_short_name = company_short_name + i;
                        break;
                    }
                    if (i == 999)
                    {
                        return LocalizationGetter.GetString("CompanyInUse", is_cc);
                    }
                }
            }
            if (!m_EmailHelper.IsValidEmail(email))
            {
                return LocalizationGetter.GetString("EmailInvalid");
            }
 
            if (!db.company_invitation.Any(t => ((t.is_active == 1) && (t.invitation_code.Trim().ToLower() == code.Trim().ToLower()))))
                return LocalizationGetter.GetString("InvalidCode");

            decimal _amount = 0;
            if (!string.IsNullOrEmpty(amount) && amount != "0")
            {
                // amount more than 0 -> we have a registration with money involved
                decimal.TryParse(amount, out _amount);
            }

            //if (_amount > 0)
            //{
                /*if ((string.IsNullOrEmpty(cardnumber)) || (string.IsNullOrEmpty(cardname)) || (string.IsNullOrEmpty(csv)) || (string.IsNullOrEmpty(selectedMonth)) || (string.IsNullOrEmpty(selectedYear)))
                {
                    return LocalizationGetter.GetString("EmptyData");
                }*/
            //}
            #region Credit Card
            string auth_code = "";
            string payment_auth_code = "";
            /*if (_amount > 0)
            {
                /// amount, string cardnumber, string cardname, string csv
                BeanStream.beanstream.TransactionProcessRequest tpr = new BeanStream.beanstream.TransactionProcessRequest();
                BeanStream.beanstream.TransactionProcessAuthRequest tpar = new BeanStream.beanstream.TransactionProcessAuthRequest();

                // to do - move constants to AppSettingsConstants
                BeanStreamProcessing bsp = new BeanStreamProcessing(ConfigurationManager.AppSettings["bs_merchant_id"]);
                string cc_error_message = "";

                int _month = 0;
                int _year = 0;
                if (selectedMonth.StartsWith("0"))
                    selectedMonth = selectedMonth[1].ToString();

                int.TryParse(selectedMonth, out _month);
                int.TryParse(selectedYear, out _year);

                if (_month == 0 || _year == 0)
                    return LocalizationGetter.GetString("EmptyData");

                var random = new Random();
                payment_auth_code = glb.GenerateInvoiceNumber(); // "INV_" + random.Next(10001, 99999).ToString(); 



                auth_code = payment_auth_code;
            }*/

            #endregion

            #region CompanySaving
            int company_invitation_id = 0;
            int client_id = 0;

            List<company_invitation> invitations = db.company_invitation.Where(t => ((t.is_active == 1) && (t.invitation_code.Trim().ToLower() == code.Trim().ToLower()))).ToList();
            company_invitation _invitation = invitations[0];
            company_invitation_id = _invitation.id;
            client_id = _invitation.created_by_company_id;



            string company_code = glb.GenerateCompanyCode(company_name);

            var varinfo = db.var_info.FirstOrDefault(x => x.emailed_code_to_customer == emailed_code_to_customer);

            company _company = new company();
            _company.company_nm = company_name.Trim();
            _company.company_invitation_id = company_invitation_id;
            _company.client_id = client_id;
            _company.status_id = 2;
            _company.registration_dt = DateTime.Now;
            _company.company_code = company_code.Trim();
            _company.employee_quantity = number;

            if (varinfo != null)
            {
                _company.contractors_number = varinfo.employee_no;
                _company.customers_number = varinfo.customers_no;
                _company.onboard_sessions_paid = varinfo.onboarding_session_numbers;
                if (varinfo.onboarding_session_numbers > 0)
                {
                    _company.onboard_sessions_expiry_dt = DateTime.Today.AddYears(1);
                }
            }
            _company.language_id = language_id;
            _company.company_short_name = company_short_name;

            _company.address_id = 1;
            _company.billing_info_id = 0;
            _company.contact_nm = first.Trim() + " " + last.Trim();
            _company.implicated_title_name_id = 1;
            _company.witness_show_id = 0;
            _company.show_location_id = 1;
            _company.show_department_id = 1;

            _company.notepad_en = "";
            _company.notepad_fr = "";
            _company.notepad_es = "";
            _company.notepad_ru = "";
            _company.notepad_ar = "";

            _company.path_en = "";
            _company.path_fr = "";
            _company.path_es = "";
            _company.path_ru = "";
            _company.path_ar = "";

            _company.alert_en = "";
            _company.alert_fr = "";
            _company.alert_es = "";
            _company.alert_ru = "";
            _company.alert_ar = "";

            _company.last_update_dt = DateTime.Now;

            _company.user_id = 1;
            _company.time_zone_id = 8;
            _company.step1_delay = 2;
            _company.step1_postpone = 2;
            _company.step2_delay = 2;
            _company.step2_postpone = 2;
            _company.step3_delay = 14;
            _company.step3_postpone = 2;
            _company.step4_delay = 5;
            _company.step4_postpone = 2;
            _company.step5_delay = 7;
            _company.step5_postpone = 2;
            _company.step6_delay = 7;
            _company.step6_postpone = 2;

            _company.guid = Guid.NewGuid(); ;

            string url = Request.Url.AbsoluteUri.ToLower();
            string _url = "registration";
            if (url.Contains("cai."))
                _url = "cai";
            if (url.Contains("demo."))
                _url = "demo";
            if (url.Contains("stark."))
                _url = "stark";
            _company.subdomain = _url;
            ////// _company.reseller = client_id;
            if (_amount > 0)
            {
                _company.next_payment_amount = _amount;
                _company.next_payment_date = DateTime.Today.AddYears(1);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    try
                    {
                        db.company.Add(_company);
                        db.SaveChanges();
                        company_id = _company.id;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        return LocalizationGetter.GetString("CompanySavingFailed", is_cc);
                    }
                    #endregion

                    #region Location Saving
                    if (company_id != 0)
                    {
                        // LocationSavingFailed
                        company_location _location = new company_location();
                        _location.location_en = location.Trim();
                        _location.location_fr = location.Trim();
                        _location.location_es = location.Trim();
                        _location.location_ru = location.Trim();
                        _location.location_ar = location.Trim();
                        _location.status_id = 2;
                        _location.company_id = company_id;
                        _location.client_id = client_id;
                        _location.last_update_dt = DateTime.Now;
                        _location.user_id = 1;
                        try
                        {
                            db.company_location.Add(_location);
                            db.SaveChanges();
                            location_id = _location.id;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            return LocalizationGetter.GetString("LocationSavingFailed");
                        }
                    }
                    else
                    {
                        return LocalizationGetter.GetString("CompanySavingFailed", is_cc);
                    }

                    #endregion

                    #region Departments
                    /// easy part - Administrative, Accounting, Management, Sales and Support 
                    /// 
                    company_department selectedDepartment = null;
                    List<string> list_departments = new List<string>();
                    if (!is_cc)
                    {
                        list_departments.Add(LocalizationGetter.GetString("Administration"));
                        list_departments.Add(LocalizationGetter.GetString("AccountingFinance"));
                        list_departments.Add(LocalizationGetter.GetString("CustomerService"));
                        list_departments.Add(LocalizationGetter.GetString("HumanResources"));
                        list_departments.Add(LocalizationGetter.GetString("IT"));
                        list_departments.Add(LocalizationGetter.GetString("Legal"));
                        list_departments.Add(LocalizationGetter.GetString("Marketing"));
                        list_departments.Add(LocalizationGetter.GetString("Production"));
                        list_departments.Add(LocalizationGetter.GetString("Purchasing"));
                        list_departments.Add(LocalizationGetter.GetString("RD"));
                        list_departments.Add(LocalizationGetter.GetString("Sales"));
                    }

                    if (is_cc)
                    {
                        list_departments.Add(LocalizationGetter.GetString("AcademicAffairs"));
                        list_departments.Add(LocalizationGetter.GetString("AccountingFinance"));
                        list_departments.Add(LocalizationGetter.GetString("Athletics"));
                        list_departments.Add(LocalizationGetter.GetString("HumanResources"));
                        list_departments.Add(LocalizationGetter.GetString("IT"));
                        list_departments.Add(LocalizationGetter.GetString("Medical"));
                        list_departments.Add(LocalizationGetter.GetString("Research"));
                        list_departments.Add(LocalizationGetter.GetString("RiskSafetyMatters"));
                        list_departments.Add(LocalizationGetter.GetString("StudentsAffairs"));
                    }




                    if (company_id != 0)
                    {
                        List<company_department> departmentsNewCompany = new List<company_department>();
                        foreach (string _dep in list_departments)
                        {
                            if (_dep.Trim().Length > 0)
                            {
                                company_department _department = new company_department();
                                _department.department_en = _dep.Trim();
                                _department.department_ar = _dep.Trim();
                                _department.department_es = _dep.Trim();
                                _department.department_fr = _dep.Trim();
                                _department.department_ru = _dep.Trim();
                                //         _department.department_en = _dep.Trim();
                                _department.client_id = client_id;
                                _department.company_id = company_id;
                                _department.last_update_dt = DateTime.Now;
                                _department.status_id = 2;
                                departmentsNewCompany.Add(_department);
                            }
                        }
                        /*saving List Departments*/
                        db.company_department.AddRange(departmentsNewCompany);
                        //try
                        //{
                            
                        //    //db.SaveChanges();
                        //}
                        //catch (Exception ex)
                        //{
                        //    logger.Error(ex.ToString());
                        //    return LocalizationGetter.GetString("DepartmentSavingFailed");
                        //}

                        /*check other deparment*/
                        if (departments != null && departments != "")
                        {
                            departments = departments.Trim();
                            string tempDepartment = departments.ToLower();
                            company_department otherDepartment = departmentsNewCompany.Where(m => m.department_en.Trim().ToLower() == tempDepartment).SingleOrDefault();

                            if (otherDepartment != null)
                            {
                                selectedDepartment = otherDepartment;
                            }
                            else
                            {
                                //создаем other department

                                company_department other_department = new company_department();
                                other_department.department_en = departments.Trim();
                                other_department.department_ar = departments.Trim();
                                other_department.department_es = departments.Trim();
                                other_department.department_fr = departments.Trim();
                                other_department.department_ru = departments.Trim();

                                other_department.client_id = client_id;
                                other_department.company_id = company_id;
                                other_department.last_update_dt = DateTime.Now;
                                other_department.status_id = 2;

                                //try
                                //{
                                db.company_department.Add(other_department);
                                //db.SaveChanges();
                                selectedDepartment = other_department;
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        logger.Error(ex.ToString());
                                //        return LocalizationGetter.GetString("OtherDepartmentSavingFailed");
                                //    }
                            }
                        }
                        else
                        {
                            selectedDepartment = new company_department
                            {
                                id = 0
                            };
                        }
                    }
                    #endregion

                    #region Incident types
                    if (company_id != 0)
                    {
                        ///List<string> list_types = db.secondary_type_mandatory.Where(t => t.type_id == 1 && t.status_id == 2).Select(item => item.secondary_type_en).ToList();
                        // LocalizationGetter.GetString("Administrative") 
                        List<string> list_types = new List<string>();
                        if (!is_cc)
                        {
                            list_types.Add(LocalizationGetter.GetString("AccountingAuditRelated"));
                            list_types.Add(LocalizationGetter.GetString("AccountingError"));
                            list_types.Add(LocalizationGetter.GetString("AccountingMisrepresentation"));
                            list_types.Add(LocalizationGetter.GetString("AuditingMatters"));
                            list_types.Add(LocalizationGetter.GetString("BriberyKickbacks"));
                            list_types.Add(LocalizationGetter.GetString("Embezzlement"));
                            list_types.Add(LocalizationGetter.GetString("FinancialConcerns"));
                            list_types.Add(LocalizationGetter.GetString("Falsification"));
                            list_types.Add(LocalizationGetter.GetString("MisappropriationFunds"));
                            list_types.Add(LocalizationGetter.GetString("HumanResourcesIssues"));
                            list_types.Add(LocalizationGetter.GetString("Discrimination"));
                            list_types.Add(LocalizationGetter.GetString("DomesticViolence"));
                            list_types.Add(LocalizationGetter.GetString("SubstanceAbuse"));
                            list_types.Add(LocalizationGetter.GetString("CommunicateNonManagement"));
                            list_types.Add(LocalizationGetter.GetString("ComplianceRegulationViolations"));
                            list_types.Add(LocalizationGetter.GetString("CorporateScandal"));
                            list_types.Add(LocalizationGetter.GetString("Harassment"));
                            list_types.Add(LocalizationGetter.GetString("Mistreatment"));
                            list_types.Add(LocalizationGetter.GetString("Retaliation"));

                            list_types.Add(LocalizationGetter.GetString("SexualHarassment"));
                            list_types.Add(LocalizationGetter.GetString("ThreatViolence"));
                            list_types.Add(LocalizationGetter.GetString("WorkplaceViolence"));
                            list_types.Add(LocalizationGetter.GetString("PrivacyIssues"));
                            list_types.Add(LocalizationGetter.GetString("AcceptableUseViolations"));
                            list_types.Add(LocalizationGetter.GetString("HIPAACompliance"));
                            list_types.Add(LocalizationGetter.GetString("IdentityTheft"));
                            list_types.Add(LocalizationGetter.GetString("InformationSecurity"));
                            list_types.Add(LocalizationGetter.GetString("EthicsViolations"));
                            list_types.Add(LocalizationGetter.GetString("CodeEthicsViolation"));
                            list_types.Add(LocalizationGetter.GetString("EthicalViolations"));
                            list_types.Add(LocalizationGetter.GetString("Misconduct"));
                            list_types.Add(LocalizationGetter.GetString("TheftEquipment"));

                            list_types.Add(LocalizationGetter.GetString("Theft"));
                            list_types.Add(LocalizationGetter.GetString("UnfairLaborPractices"));
                            list_types.Add(LocalizationGetter.GetString("VendorConcerns"));
                            list_types.Add(LocalizationGetter.GetString("WorkplaceSafety"));
                            list_types.Add(LocalizationGetter.GetString("EnvironmentalDamage"));
                            list_types.Add(LocalizationGetter.GetString("EnvironmentalIssue"));
                            list_types.Add(LocalizationGetter.GetString("IndustrialAccidents"));

                            list_types.Add(LocalizationGetter.GetString("Sabotage"));
                            list_types.Add(LocalizationGetter.GetString("SafeDrivingConcerns"));
                            list_types.Add(LocalizationGetter.GetString("UnsafeWorkConditions"));
                            list_types.Add(LocalizationGetter.GetString("UnusualSuspiciousActivities"));
                            list_types.Add(LocalizationGetter.GetString("Vandalism"));
                            list_types.Add(LocalizationGetter.GetString("PoorCustomerService"));
                            list_types.Add(LocalizationGetter.GetString("CustomerMistreatment"));
                            list_types.Add(LocalizationGetter.GetString("EmployeeRelations"));
                            list_types.Add(LocalizationGetter.GetString("SecuritiesViolation"));
                            list_types.Add(LocalizationGetter.GetString("ShareholderConcerns"));

                        }
                        if (is_cc)
                        {
                            list_types.Add(LocalizationGetter.GetString("AcademicMisconduct"));
                            list_types.Add(LocalizationGetter.GetString("AlcoholDrugAbuse"));
                            list_types.Add(LocalizationGetter.GetString("CheatingPlagiarism"));
                            list_types.Add(LocalizationGetter.GetString("CredentialMisrepresentation"));
                            list_types.Add(LocalizationGetter.GetString("Hazing"));
                            list_types.Add(LocalizationGetter.GetString("SexualHarassment"));

                            list_types.Add(LocalizationGetter.GetString("SpikingDrinks"));
                            list_types.Add(LocalizationGetter.GetString("StudentSafety"));
                            list_types.Add(LocalizationGetter.GetString("StudentTravel"));
                            list_types.Add(LocalizationGetter.GetString("Terrorism"));
                            list_types.Add(LocalizationGetter.GetString("AccountingAuditingMatters"));
                            list_types.Add(LocalizationGetter.GetString("DonorStewardship"));
                            list_types.Add(LocalizationGetter.GetString("FalsificationContracts"));
                            list_types.Add(LocalizationGetter.GetString("Records"));
                            list_types.Add(LocalizationGetter.GetString("Fraud"));


                            list_types.Add(LocalizationGetter.GetString("ImproperDisclosure"));
                            list_types.Add(LocalizationGetter.GetString("ImproperGiving"));
                            list_types.Add(LocalizationGetter.GetString("ImproperSupplier"));
                            list_types.Add(LocalizationGetter.GetString("TheftEmbezzlement"));
                            list_types.Add(LocalizationGetter.GetString("WasteAbuse"));
                            list_types.Add(LocalizationGetter.GetString("OtherFinancialMatters"));
                            list_types.Add(LocalizationGetter.GetString("FraudulentActivities"));
                            list_types.Add(LocalizationGetter.GetString("ImproperGivingGifts"));
                            list_types.Add(LocalizationGetter.GetString("InappropriateActivities"));

                            list_types.Add(LocalizationGetter.GetString("MisuseAssets"));
                            list_types.Add(LocalizationGetter.GetString("RecruitingMisconduct"));
                            list_types.Add(LocalizationGetter.GetString("ScholarshipFinancial"));
                            list_types.Add(LocalizationGetter.GetString("SubstanceAbuse"));
                            list_types.Add(LocalizationGetter.GetString("BiasIncidents"));
                            list_types.Add(LocalizationGetter.GetString("ConflictInterest"));
                            list_types.Add(LocalizationGetter.GetString("DiscriminationHarassment"));
                            list_types.Add(LocalizationGetter.GetString("EEOCADA"));

                            list_types.Add(LocalizationGetter.GetString("EmployeeBenefitsAbuse"));
                            list_types.Add(LocalizationGetter.GetString("EmployeeMisconduct"));
                            list_types.Add(LocalizationGetter.GetString("Nepotism"));
                            list_types.Add(LocalizationGetter.GetString("OffensiveInappropriateCommunication"));
                            list_types.Add(LocalizationGetter.GetString("ThreatInappropriate"));
                            list_types.Add(LocalizationGetter.GetString("TimeAbuse"));
                            list_types.Add(LocalizationGetter.GetString("UnsafeWorkingConditions"));
                            list_types.Add(LocalizationGetter.GetString("ViolenceThreat"));
                            list_types.Add(LocalizationGetter.GetString("WorkersCompensationDisability"));
                            list_types.Add(LocalizationGetter.GetString("BenefitsAbuses"));
                            list_types.Add(LocalizationGetter.GetString("DataprivacyIntegrity"));
                            list_types.Add(LocalizationGetter.GetString("MaliciousUseTechnology"));

                            list_types.Add(LocalizationGetter.GetString("SoftwarePiracy"));
                            list_types.Add(LocalizationGetter.GetString("Infringement"));
                            list_types.Add(LocalizationGetter.GetString("MisuseResources"));
                            list_types.Add(LocalizationGetter.GetString("HealthcareFraud"));
                            list_types.Add(LocalizationGetter.GetString("HIPAA"));
                            list_types.Add(LocalizationGetter.GetString("InsuranceIssues"));
                            list_types.Add(LocalizationGetter.GetString("PatientCare"));
                            list_types.Add(LocalizationGetter.GetString("PatientAbuse"));
                            list_types.Add(LocalizationGetter.GetString("PatientRights"));
                            list_types.Add(LocalizationGetter.GetString("ResearchMisconduct"));

                            list_types.Add(LocalizationGetter.GetString("SponsoredProjects"));
                            list_types.Add(LocalizationGetter.GetString("OtherMedicalResearch"));
                            list_types.Add(LocalizationGetter.GetString("ConflictInterest"));
                            list_types.Add(LocalizationGetter.GetString("DataPrivacy"));
                            list_types.Add(LocalizationGetter.GetString("DisclosureConfidential"));
                            list_types.Add(LocalizationGetter.GetString("EnvironmentalSafetyMatters"));
                            list_types.Add(LocalizationGetter.GetString("HumanAnimalResearch"));
                            list_types.Add(LocalizationGetter.GetString("ResearchGrantMisconduct"));
                        }

                        foreach (string _types in list_types)
                        {
                            if (_types.Trim().Length > 0)
                            {
                                company_secondary_type _incident_type = new company_secondary_type();
                                _incident_type.secondary_type_en = _types.Trim();
                                _incident_type.secondary_type_ar = _types.Trim();
                                _incident_type.secondary_type_es = _types.Trim();
                                _incident_type.secondary_type_fr = _types.Trim();
                                _incident_type.secondary_type_ru = _types.Trim();
                                //       _incident_type.secondary_type_ar = _types.Trim();
                                _incident_type.client_id = client_id;
                                _incident_type.company_id = company_id;
                                _incident_type.last_update_dt = DateTime.Now;
                                _incident_type.status_id = 2;
                                _incident_type.type_id = 1;
                                _incident_type.weight = 200;
                                _incident_type.parent_secondary_type = 0;

                                //try
                                //{
                                    db.company_secondary_type.Add(_incident_type);
                                    //db.SaveChanges();
                                //}
                                //catch (Exception ex)
                                //{
                                //    ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                                //    logger.Error(ex.ToString());
                                //    return LocalizationGetter.GetString("IncidentTypeSavingFailed");
                                //}
                            }
                        }
                    }
                    #endregion

                    #region Relationship // Reporter Types
                    if (company_id != 0)
                    {
                        List<string> list_relationship = db.relationship.Select(item => item.relationship_en).ToList();
                        //   LocalizationGetter.GetString("Administrative")


                        foreach (string _relationships in list_relationship)
                        {
                            if ((_relationships.Trim().Length > 0) && (_relationships.Trim().ToLower() != "other"))
                            {
                                company_relationship _company_relationship = new company_relationship();
                                _company_relationship.relationship_en = _relationships.Trim();
                                _company_relationship.relationship_ar = _relationships.Trim();
                                _company_relationship.relationship_es = _relationships.Trim();
                                _company_relationship.relationship_fr = _relationships.Trim();
                                _company_relationship.relationship_jp = _relationships.Trim();
                                _company_relationship.relationship_ru = _relationships.Trim();
                                _company_relationship.client_id = client_id;
                                _company_relationship.company_id = company_id;
                                ///??     _company_relationship.last_update_dt = DateTime.Now;
                                _company_relationship.status_id = 2;

                                //try
                                //{
                                db.company_relationship.Add(_company_relationship);
                                    //db.SaveChanges();
                                //}
                                //catch (Exception ex)
                                //{
                                //    logger.Error(ex.ToString());
                                //    return LocalizationGetter.GetString("RelationshipsSavingFailed");
                                //}
                            }
                        }
                    }
                    #endregion

                    #region Anonymity
                    if (company_id != 0)
                    {
                        List<int> list_anonymity = db.anonymity.Select(item => item.id).ToList();

                        foreach (int _anon in list_anonymity)
                        {
                            if (_anon > 0)
                            {
                                company_anonymity _anonymity = new company_anonymity();
                                _anonymity.company_id = company_id;
                                _anonymity.last_update_dt = DateTime.Now;
                                _anonymity.status_id = 2;
                                _anonymity.anonymity_id = _anon;
                                _anonymity.user_id = 1;

                                //try
                                //{
                                db.company_anonymity.Add(_anonymity);
                                //    //db.SaveChanges();
                                //}
                                //catch (Exception ex)
                                //{
                                //    logger.Error(ex.ToString());
                                //    return LocalizationGetter.GetString("AnonymitySavingFailed");
                                //}
                            }
                        }
                    }
                    #endregion

                    #region Relationship
                    if (company_id != 0)
                    {
                        List<string> list_outcomes = new List<string>();
                        list_outcomes.Add(LocalizationGetter.GetString("OutcomeCompany1"));
                        list_outcomes.Add(LocalizationGetter.GetString("OutcomeCompany2"));
                        list_outcomes.Add(LocalizationGetter.GetString("OutcomeCompany3"));
                        list_outcomes.Add(LocalizationGetter.GetString("OutcomeCompany4"));
                        list_outcomes.Add(LocalizationGetter.GetString("OutcomeCompany5"));
                        list_outcomes.Add(LocalizationGetter.GetString("OutcomeCompany6"));
                        list_outcomes.Add(LocalizationGetter.GetString("OutcomeCompany7"));
                        list_outcomes.Add(LocalizationGetter.GetString("OutcomeCompany8"));



                        foreach (string _outcome in list_outcomes)
                        {
                            if ((_outcome.Trim().Length > 0) && (_outcome.Trim().ToLower() != "other"))
                            {
                                company_outcome _company_outcome = new company_outcome();
                                _company_outcome.outcome_en = _outcome.Trim();
                                _company_outcome.outcome_ar = _outcome.Trim();
                                _company_outcome.outcome_es = _outcome.Trim();
                                _company_outcome.outcome_fr = _outcome.Trim();
                                _company_outcome.outcome_jp = _outcome.Trim();
                                _company_outcome.outcome_ru = _outcome.Trim();
                                _company_outcome.company_id = company_id;
                                _company_outcome.last_update_dt = DateTime.Now;
                                _company_outcome.status_id = 2;
                                _company_outcome.user_id = 1;
                                //try
                                //{
                                db.company_outcome.Add(_company_outcome);
                                //    //db.SaveChanges();
                                //}
                                //catch (Exception ex)
                                //{
                                //    logger.Error(ex.ToString());
                                //    return LocalizationGetter.GetString("RelationshipsSavingFailed");
                                //}
                            }
                        }
                    }
                    #endregion

                    #region Root Causes
                    var list_root_cases = new List<string>();
                    if (company_id != 0)
                    {
                        list_root_cases.Add(LocalizationGetter.GetString("CulturalInfluences"));
                        list_root_cases.Add(LocalizationGetter.GetString("CustomerDemands"));
                        list_root_cases.Add(LocalizationGetter.GetString("CommunicationBarriers"));
                    }
                    foreach (var itemString in list_root_cases)
                    {
                        var temp_cases_external = new company_root_cases_external { company_id = company_id, name_en = itemString, name_es = itemString, name_fr = itemString, status_id = 2 };
                        db.company_root_cases_external.Add(temp_cases_external);
                        //db.SaveChanges();
                    }
                    list_root_cases = new List<string>();
                    list_root_cases.Add(LocalizationGetter.GetString("CostControlGoals"));
                    list_root_cases.Add(LocalizationGetter.GetString("FinancialorPerformanceIncentives"));
                    list_root_cases.Add(LocalizationGetter.GetString("LackofTeamwork"));
                    list_root_cases.Add(LocalizationGetter.GetString("PoorProcessDesign"));
                    list_root_cases.Add(LocalizationGetter.GetString("PressureofMeetingSalesQuotas"));
                    list_root_cases.Add(LocalizationGetter.GetString("RemoteorInadequateSupervision"));
                    list_root_cases.Add(LocalizationGetter.GetString("UnsupportiveEnvironmentorDepartment"));
                    list_root_cases.Add(LocalizationGetter.GetString("WeakControls"));
                    list_root_cases.Add(LocalizationGetter.GetString("LackofTraining"));
                    list_root_cases.Add(LocalizationGetter.GetString("LimitedResources"));
                    foreach (var itemString in list_root_cases)
                    {
                        var temp_cases_organizational = new company_root_cases_organizational { company_id = company_id, name_en = itemString, name_es = itemString, name_fr = itemString, status_id = 2 };
                        db.company_root_cases_organizational.Add(temp_cases_organizational);
                        //db.SaveChanges();
                    }
                    list_root_cases = new List<string>();
                    list_root_cases.Add(LocalizationGetter.GetString("CompanyLoyaltyRealization"));
                    list_root_cases.Add(LocalizationGetter.GetString("ExtrenalLocusofControl"));
                    list_root_cases.Add(LocalizationGetter.GetString("Insubordination"));
                    list_root_cases.Add(LocalizationGetter.GetString("LackofAwareness"));
                    list_root_cases.Add(LocalizationGetter.GetString("LackofSensitivity"));
                    list_root_cases.Add(LocalizationGetter.GetString("LegitimateActionRationalization"));
                    list_root_cases.Add(LocalizationGetter.GetString("NoHarmRationalization"));
                    list_root_cases.Add(LocalizationGetter.GetString("SelfInterest"));
                    list_root_cases.Add(LocalizationGetter.GetString("CulturalDifferencesPersonalValues"));
                    list_root_cases.Add(LocalizationGetter.GetString("LackofSkills"));

                    foreach (var itemString in list_root_cases)
                    {
                        var temp_cases_behavioral = new company_root_cases_behavioral { company_id = company_id, name_en = itemString, name_es = itemString, name_fr = itemString, status_id = 2 };
                        db.company_root_cases_behavioral.Add(temp_cases_behavioral);
                        //db.SaveChanges();
                    }
                    #endregion


                    string login = glb.GenerateLoginName(first, last);
                    string pass = glb.GeneretedPassword().Trim();

                    #region User Saving
                    if (company_id != 0)
                    {
                        user _user = new user();
                        _user.first_nm = first.Trim();
                        _user.last_nm = last.Trim();
                        _user.company_id = company_id;
                        _user.role_id = 5;
                        _user.status_id = 2;
                        _user.login_nm = login.Trim();
                        _user.password = PasswordUtils.GetHash(pass);
                        _user.photo_path = "";
                        _user.email = email.Trim();
                        _user.phone = "";
                        _user.preferred_contact_method_id = 1;
                        _user.title_ds = title.Trim();
                        _user.employee_no = "";
                        _user.question_ds = "";
                        _user.answer_ds = "";
                        _user.previous_login_dt = DateTime.Now;
                        _user.previous_login_dt = null;
                        _user.last_update_dt = DateTime.Now;
                        _user.user_id = 1;
                        _user.preferred_email_language_id = language_id;
                        _user.notification_messages_actions_flag = 1;
                        _user.notification_new_reports_flag = 1;
                        _user.notification_marketing_flag = 1;
                        _user.notification_summary_period = 1;
                        _user.company_location_id = location_id;
                        _user.location_nm = "";
                        _user.sign_in_code = null;
                        _user.guid = Guid.NewGuid();
                        _user.company_department_id = selectedDepartment.id;
                        _user.user_permissions_approve_case_closure = 1;
                        _user.user_permissions_change_settings = 1;


                        try
                        {
                            db.user.Add(_user);
                            db.SaveChanges();
                            user_id = _user.id;
                            _user.password = pass;
                            logger.Info("AddedUser" + _user?.login_nm);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            transaction.Rollback();
                            return LocalizationGetter.GetString("UserSavingFailed");
                        }
                    }
                    else
                    {
                        return LocalizationGetter.GetString("CompanySavingFailed", is_cc);
                    }

                    if (login != null && login.Length > 0)
                    {
                        UserModel userModel = new UserModel();
                        var user = userModel.Login(login, pass);
                        Session[ECGlobalConstants.CurrentUserMarcker] = user;
                        Session["userName"] = "";
                        Session["userId"] = user.id;


                        #region Email to Case Admin

                        if ((user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(user.email.Trim()))
                        {
                            EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
                            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                            eb.NewCompany(user.first_nm, user.last_nm, login.Trim(), pass.Trim(), company_name.Trim(), company_code.Trim());
                            glb.SaveEmailBeforeSend(0, user.id, user.company_id, user.email.Trim(), ConfigurationManager.AppSettings["emailFrom"], "",
                                LocalizationGetter.GetString("Email_Title_NewCompany", is_cc), eb.Body, false, 2);
                        }


                        #endregion


                    }
                    else
                        return LocalizationGetter.GetString("UserSavingFailed");
                    #endregion

                    #region Saving CC_Payment
                    /*if (_amount > 0)
                    {
                        company_payments _cp = new company_payments();
                        _cp.amount = _amount;
                        _cp.auth_code = auth_code.Trim();
                        _cp.local_invoice_number = payment_auth_code.Trim();
                        _cp.cc_csv = Convert.ToInt32(csv);

                        _cp.cc_month = Convert.ToInt32(1);
                        _cp.cc_year = Convert.ToInt32(2017);

                        _cp.cc_name = cardname.Trim();
                        _cp.cc_number = StringUtil.ConvertCCInfoToLast4DigitsInfo(cardnumber.Trim());

                        _cp.company_id = company_id;
                        _cp.payment_date = DateTime.Today;
                        _cp.id = Guid.NewGuid();
                        _cp.user_id = user_id;

                        try
                        {
                            db.company_payments.Add(_cp);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }*/
                    #endregion

                    Session["Auth"] = auth_code;
                    Session["Amount"] = _amount;
                    return LocalizationGetter.GetString("_Completed").ToLower();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return LocalizationGetter.GetString("UserSavingFailed");
                }
            }
        }

        public string CreateUser(string code, string first, string last, string email, string title, int currentDepartmens, int currentLocations)
        {
            int company_id = 0;
            int user_id = 0;
            int location_id = 0;
            int language_id = 1;

            if ((string.IsNullOrEmpty(code)) || (string.IsNullOrEmpty(first)) || (string.IsNullOrEmpty(last)) || (string.IsNullOrEmpty(email)) || (string.IsNullOrEmpty(title)))
            {
                return LocalizationGetter.GetString("EmptyData");
            }

            if (!m_EmailHelper.IsValidEmail(email))
            {
                return LocalizationGetter.GetString("EmailInvalid");
            }

            if (db.invitation.Any(t => ((t.email.ToLower().Trim() == email.ToLower().Trim()) && (t.used_flag == 1) && (t.code.Trim().ToLower() == code.Trim().ToLower()))))
                return LocalizationGetter.GetString("AlreadyRegistered");

            int invitation_id = 0;

            if (!db.invitation.Any(t => ((t.email.ToLower().Trim() == email.ToLower().Trim()) && (t.used_flag == 0) && (t.code.Trim().ToLower() == code.Trim().ToLower()))))
                return LocalizationGetter.GetString("InvitationCodeMismatch");
            else
            {
                List<invitation> invitations = db.invitation.Where(t => ((t.email.ToLower().Trim() == email.ToLower().Trim()) && (t.used_flag == 0) && (t.code.Trim().ToLower() == code.Trim().ToLower()))).ToList();
                invitation _invitation = invitations[0];
                invitation_id = _invitation.id;
                try
                {
                    UserModel um = new UserModel(_invitation.sent_by_user_id);
                    CompanyModel cm = new CompanyModel(um._user.company_id);
                    company_id = cm._company.id;

                    List<string> _company_user_emails = new List<string>();
                    _company_user_emails = (db.user.Where(t => ((t.company_id == company_id) && (t.role_id != ECLevelConstants.level_informant))).Select(t => t.email.Trim().ToLower())).ToList();

                    List<int> _company_user_ids = new List<int>();
                    _company_user_ids = (db.user.Where(t => ((t.company_id == company_id) && (t.role_id != ECLevelConstants.level_informant))).Select(t => t.id)).ToList();

                    if (db.user.Any(u => ((u.email.ToLower().Trim() == email.ToLower().Trim() && u.role_id != ECLevelConstants.level_informant && _company_user_ids.Contains(u.id)))))
                        return LocalizationGetter.GetString("AlreadyRegistered");
                }
                catch
                {
                    invitation_id = 0;
                    company_id = 0;
                }
            }


            if (invitation_id == 0)
            {
                return LocalizationGetter.GetString("InvitationCompanyMismatch", is_cc);
            }
            string login = glb.GenerateLoginName(first, last);
            string pass = glb.GeneretedPassword();

            #region User Saving
            if (company_id != 0)
            {
                #region Lowest Company Location ID

                List<company_location> company_locations = db.company_location.Where(t => ((t.status_id == 2))).OrderBy(t => t.id).ToList();
                if (company_locations.Count > 0)
                    location_id = company_locations[0].id;

                #endregion

                user _user = new user();
                _user.first_nm = first.Trim();
                _user.last_nm = last.Trim();
                _user.company_id = company_id;
                _user.role_id = 6;
                _user.status_id = 2;
                _user.login_nm = login.Trim();
                _user.password = pass.Trim();
                _user.password = PasswordUtils.GetHash(_user.password);
                _user.photo_path = "";
                _user.email = email.Trim();
                _user.phone = "";
                _user.preferred_contact_method_id = 1;
                _user.title_ds = title.Trim();
                _user.employee_no = "";
                _user.company_department_id = currentDepartmens;
                _user.question_ds = "";
                _user.answer_ds = "";
                _user.previous_login_dt = DateTime.Now;
                _user.previous_login_dt = null;
                _user.last_update_dt = DateTime.Now;
                _user.user_id = 1;
                _user.preferred_email_language_id = language_id;
                _user.notification_messages_actions_flag = 1;
                _user.notification_new_reports_flag = 1;
                _user.notification_marketing_flag = 1;
                _user.notification_summary_period = 1;
                _user.company_location_id = currentLocations;
                _user.location_nm = "";
                _user.sign_in_code = null;
                _user.guid = Guid.NewGuid();

                try
                {
                    db.user.Add(_user);
                    db.SaveChanges();
                    user_id = _user.id;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    return LocalizationGetter.GetString("UserSavingFailed");
                }

                #region Create Folder for User
                #endregion
            }
            else
            {
                return LocalizationGetter.GetString("InvitationCompanyMismatch", is_cc);
            }

            if (login != null && login.Length > 0)
            {
                UserModel userModel = new UserModel();
                var user = userModel.Login(login, pass);
                Session[ECGlobalConstants.CurrentUserMarcker] = user;
                Session["userName"] = "";
                Session["userId"] = user.id;

                #region Email to Case Admin

                if ((user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(user.email.Trim()))
                {
                    CompanyModel cm = new CompanyModel(user.company_id);

                    EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
                    EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());
                    eb.NewUser(user.first_nm, user.last_nm, login.Trim(), pass.Trim());

                    string email_title = LocalizationGetter.GetString("Email_Title_NewUser", is_cc);
                    email_title = email_title.Replace("[CompanyName]", cm._company.company_nm);
                    glb.SaveEmailBeforeSend(0, _user.id, _user.company_id, user.email.Trim(), ConfigurationManager.AppSettings["emailFrom"], "",
                        email_title, eb.Body, false, 13);
                    #region New Mediator Arrived - message to all Supervising mediators
                    foreach (user _user in cm.AllSupervisingMediators(cm._company.id, true))
                    {
                        if ((_user.email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(_user.email.Trim()))
                        {
                            eb.NewUserArrived(_user.first_nm, _user.last_nm, user.first_nm, user.last_nm);
                            glb.SaveEmailBeforeSend(0, _user.id, _user.company_id, _user.email.Trim(), ConfigurationManager.AppSettings["emailFrom"], "",
                                email_title, eb.Body, false, 14);
                        }
                    }
                    #endregion
                }
                #endregion

                return LocalizationGetter.GetString("_Completed").ToLower();
            }
            else
                return LocalizationGetter.GetString("UserSavingFailed");
            #endregion
        }

        /// <summary>
        /// checks if company already registered
        /// </summary>
        /// <param name="company_nm"></param>
        /// <returns></returns>
        public JsonResult IsCompanyAvailable(string company_nm)
        {
            JsonResult result_company = new JsonResult();

            if (company_nm.Length > 0)
            {
                bool result = glb.isCompanyInUse(company_nm);
                //TODO: Do the validation

                if (!result)
                    result_company.Data = 0;
                else
                    result_company.Data = 1;
            }
            else
            {
                result_company.Data = 1;
            }
            return result_company;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public JsonResult ReturnAmount(string code, string emplquant)
        {
            int empl_quant = 0;
            if (emplquant != null && emplquant != "")
            {
                if (!Int32.TryParse(emplquant, out empl_quant))
                    empl_quant = 0;
            }

            JsonResult result_company = new JsonResult();
            decimal amount = 0;



            int reseller_type = 1;
            int reseller_level = 1;

            if (!db.company_invitation.Any(item => ((item.invitation_code.ToLower().Trim() == code.Trim().ToLower()))))
            {
                // no inv code in the system
            }
            else
            {
                company_invitation ci = db.company_invitation.Where(item => ((item.invitation_code.ToLower().Trim() == code.Trim().ToLower()))).FirstOrDefault();
                if (ci.reseller_type_id.HasValue)
                    reseller_type = ci.reseller_type_id.Value;
                if (ci.reseller_level.HasValue)
                    reseller_level = ci.reseller_level.Value;
            }

            if (reseller_type == 1)
            {
                decimal pepy = 0;
                if (empl_quant <= 500)
                    pepy = 20;
                if (empl_quant > 500 && empl_quant < 1000)
                    pepy = 8;
                if (empl_quant > 1000 && empl_quant < 5000)
                    pepy = 4;
                if (empl_quant > 5000 && empl_quant < 10000)
                    pepy = 3.50m;
                if (empl_quant > 10000 && empl_quant < 25000)
                    pepy = 2.50m;
                if (empl_quant > 25000 && empl_quant < 50000)
                    pepy = 2;
                if (empl_quant > 50000 && empl_quant < 100000)
                    pepy = 1.50m;
                if (empl_quant > 100000 && empl_quant < 200000)
                    pepy = 1;
                if (empl_quant > 200000)
                    pepy = 1;

                if (empl_quant == 0)
                    amount = 0;
                else
                    amount = pepy * empl_quant;
            }
            else if (reseller_type == 2)
            {
                if (reseller_level == 1)
                    amount = 1015;
                if (reseller_level == 2)
                    amount = 435;
                if (reseller_level == 3)
                    amount = 290;
                if (reseller_level == 4)
                    amount = 145;
                if (reseller_level == 5)
                    amount = 87;
                if (reseller_level == 6)
                    amount = 58;
                if (reseller_level == 7)
                    amount = 29;
                if (reseller_level == 8)
                    amount = 15;
            }
            //     else
            //        amount = 1000;

            if ((code.Length == 5) && (code.Trim().ToLower().Contains("ec43") || code.Trim().ToLower().Contains("cc29")))
            {
                amount = 0;
            }
            if (code.Trim().ToLower() == "flat")
            {
                amount = 1300;
            }

            result_company.Data = StringUtil.ConvertDecimalToStringAmount(amount);
            return result_company;

        }

        public ActionResult Quote(string code)
        {
            string _code = "";
            if (!string.IsNullOrEmpty(code))
                _code = code;

            ViewBag.code = _code;
            #region EC-CC Viewbag
            bool is_cc = DomainUtil.IsCC(Request.Url.AbsoluteUri.ToLower());
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion
            return View();
        }

        public ActionResult QuoteSuccess(string amount)
        {
            #region EC-CC Viewbag
            bool is_cc = DomainUtil.IsCC(Request.Url.AbsoluteUri.ToLower());
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            decimal _amount = 0;
            if ((amount != null) && (amount != ""))
            {
                try
                {
                    decimal.TryParse(amount, out _amount);
                }
                catch
                {
                    return RedirectToAction("Quote", "New");
                }
            }
            ViewBag.amount = StringUtil.ConvertDecimalToStringAmount(_amount);

            return View();
        }
    }
}