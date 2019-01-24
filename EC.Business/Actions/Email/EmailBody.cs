using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EC.Common.Util;

namespace EC.Business.Actions.Email
{
    public class EmailBody
    {
        DomainUtil domainUtil = new DomainUtil();

        #region Properties
        private string m_filename;
        public string FileName
        {
            get
            {
                return m_filename;
            }
            set
            {
                m_filename = value;
            }
        }

        private string m_body;
        public string Body
        {
            get
            {
                return m_body;
            }
            set
            {
                m_body = value;
            }
        }

        private int m_language_id;
        public int Language
        {
            get
            {
                return m_language_id;
            }
            set
            {
                m_language_id = value;
            }
        }

        private int m_project;
        public int Project
        {
            get
            {
                return m_project;
            }
            set
            {
                m_project = value;
            }
        }

        private string m_url;
        public string URL
        {
            get
            {
                return m_url;
            }
            set
            {
                m_url = value;
            }
        }
        #endregion
        public EmailBody(int project, int language_id, string url)
        {
            m_project = project;
            m_language_id = language_id;
            m_url = url;

        }

        public void GetBody(int body_selector)
        {
            switch (body_selector)
            {
                case 1:
                    // request for contact on front site - its internal email - for me/margot
                    m_filename = "NewRequest";
                    break;
                case 2:
                    // company registered   - message for Admin mediator ( no cc to Partner as login info here. msg for partner would be in PartnerNewCompanyArrived)
                    m_filename = "NewCompany";
                    break;
                case 3:
                    // case submitted - message for admin mediators
                    m_filename = "NewCase";
                    break;
                case 4:
                    // case submitted with involved mediators- individual message for admin mediators w/o involved
                    m_filename = "NewCaseInvolved";
                    break;
                case 5:
                    // individual message for mediators in case
                    m_filename = "NewMessage";
                    break;
                case 6:
                    // task for mediator ( cc for the one who assigned?)
                    m_filename = "NewTask";
                    break;
                case 7:
                    // case promoted - individual message for mediators in case
                    m_filename = "NextStep";
                    break;
                case 8:
                    // case reopened - individual message for mediators in case
                    m_filename = "CaseReopened";
                    break;
                case 9:
                    // case needs to be signed-off ( by 2 top mediator levels only)
                    m_filename = "CaseCloseApprove";
                    break;
                case 10:
                    // case needs to be signed-off ( by 2 top mediator levels only)
                    m_filename = "CaseCloseApprovePlatformManager";
                    break;
                case 11:
                    // case escalated - don't have red button to escalate yet
                    m_filename = "EscalatedCase";
                    break;
                case 12:
                    // case escalated because of timeline violation
                    m_filename = "EscalatedCaseTimeline";
                    break;
                case 13:
                    // new mediator registered - message for mediator - no cc for admin, as login here
                    m_filename = "NewUser";
                    break;
                case 14:
                    // new mediator registered - message for admin mediator 
                    m_filename = "NewUserArrived";
                    break;
                case 15:
                    // case promoted - individual message for mediators in case
                    m_filename = "SeniorMediatorCaseConfirm";
                    break;

                case 20:
                    // new company invitation sent - message for new company ( cc for partner??)
                    m_filename = "PartnerNewCompany";
                    break;
                case 21:
                    // new company registered - message for partner
                    m_filename = "PartnerNewCompanyArrived";
                    break;
                case 22:
                    // new company status changed - message for company  ( cc for partner???)
                    m_filename = "PartnerNewCompanyStatusChange";
                    break;


                case 30:
                    // new case submitted - message for reporter
                    m_filename = "ReporterNewCase";
                    break;
                case 31:
                    // new case message - message for reporter
                    m_filename = "ReporterNewMessage";
                    break;
                case 32:
                    // case promoted to next step - message for reporter
                    m_filename = "ReporterNewStep";
                    break;
                case 33:
                    // // case re-opened - message for reporter
                    m_filename = "ReporterCaseReopened";
                    break;
                case 40:
                    //mediator assigned to case   (?? cc for the one who assigned?)
                    m_filename = "MediatorAssigned";
                    break;
                case 41:
                    // mediator invited to company  (??I think we don't need cc for the one who invited?)
                    m_filename = "MediatorInvited";
                    break;
                case 42:
                    //mediator status promoted to Active ONLY (??I think we don't need cc for the one who changed status?)
                    m_filename = "MediatorStatusChange";
                    break;
                case 43:
                    // mediator Role changed to any - (??I think we don't need cc for the one who changed?)
                    m_filename = "MediatorRoleChange";
                    break;

                case 50:
                    // message to mediator with the link to update password 
                    m_filename = "ForgetPassword";
                    break;
                case 51:
                    // message to mediator with the link to update password 
                    m_filename = "CampusSecurityAlert";
                    break;
                case 52:
                    // message to mediator with the link to update password 
                    m_filename = "MediatorInvitedUserCreated";
                    break;
                case 53:
                    // message to mediator with the link to update password 
                    m_filename = "ForgetPasswordNew";
                    break;
                case 60:
                    // message about case deadline past due
                    m_filename = "CaseDeadlinePastDue";
                    break;
                case 61:
                    // message about case deadline past due
                    m_filename = "CompanyCalendarEventAdded";
                    break;
                case 62:
                    // message about case deadline past due
                    m_filename = "CompanyCalendarEventRemoved";
                    break;
                case 63:
                    // message about case deadline past due
                    m_filename = "TrainerCalendarEventAdded";
                    break;
                case 64:
                    // message about case deadline past due
                    m_filename = "TrainerCalendarEventRemoved";
                    break;
                case 65:
                    //mediator assigned to case   (?? cc for the one who assigned?)
                    m_filename = "SetCaseOwner";
                    break;
                case 66:
                    //mediator assigned to case   (?? cc for the one who assigned?)
                    m_filename = "EC.COM.VAR";
                    break;
                case 67:
                    //mediator assigned to case   (?? cc for the one who assigned?)
                    m_filename = "OrderConfirmation_Email";
                    break;
                case 68:
                    //user not complete registration
                    m_filename = "UserNotCompleteRegistration_Email";
                    break;
                case 69:
                    //After 4 hours of signup in VAR
                    m_filename = "VarAfter4HoursAfterSignUp";
                    break;
                case 70:
                    //After 4 hours of signup in VAR
                    m_filename = "VarAfter24HoursAfterSignUpToUser";
                    break;
                case 71:
                    //After 3 weeks to user
                    m_filename = "VarAfter3WeekAfterSignUpToUser";
                    break;
                    
            }

            string appPath = Path.GetFullPath("~/EmailText/" + m_filename + ".html");
            string path1 = Path.Combine(Environment.CurrentDirectory, @"EmailText\", m_filename + ".html");
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\emails\", m_filename + ".html");
            /// string path1 = Path.Combine(AppDomain.CurrentDomain.ExecuteAssembly, @"EmailText\", m_filename + ".html");
            /// 
            string temp = "";
            m_body = "";

            if (File.Exists(path))
            {
                temp = path;
                string text = System.IO.File.ReadAllText(path);
                m_body = text;

                string entrance_link = DomainUtil.GetSubdomainLink(path, m_url) + "/Service/Login";

                m_body = m_body.Replace("[BaseUrl]", entrance_link);
            }

            /*      string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                  string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                  if (System.IO.File.Exists(AppDomain.CurrentDomain.RelativeSearchPath + '\\' +
                      assemblyName.Substring(0, assemblyName.IndexOf(',')) + ".html"))
                  {

                     temp = AppDomain.CurrentDomain.RelativeSearchPath + '\\' +
                          assemblyName.Substring(0, assemblyName.IndexOf(',')) + ".html";
                  }
                  else if (System.IO.File.Exists(AppDomain.CurrentDomain.RelativeSearchPath + '\\' + "Content" + '\\' + "emails" + '\\' + m_filename + ".html"))
                  {
                      temp = AppDomain.CurrentDomain.RelativeSearchPath + '\\' + "Content" + '\\' + "emails" + '\\' + m_filename + ".html";
                  }
                  else if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(assemblyPath) + '\\' +
                      assemblyName.Substring(0, assemblyName.IndexOf(',')) + ".html"))
                  {
                     temp = System.IO.Path.GetDirectoryName(assemblyPath) + '\\' +
                          assemblyName.Substring(0, assemblyName.IndexOf(',')) + ".html";
                  }
                  else if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(assemblyPath) + '\\' + "Content" + '\\' + "emails" + '\\' + m_filename + ".html"))
                  {
                      temp = System.IO.Path.GetDirectoryName(assemblyPath) + '\\' + "Content" + '\\' + "emails" + '\\'  + m_filename + ".html";
                  }*/



        }

        #region GenerateEmail Body
        public void NewRequest(string first, string last, string company, string phone, string email)
        {
            GetBody(1);
            m_body = m_body.Replace("[First]", first);
            m_body = m_body.Replace("[Last]", last);
            m_body = m_body.Replace("[Company]", company);
            m_body = m_body.Replace("[Phone]", phone);
            m_body = m_body.Replace("[Email]", email);
        }
        public void NewCompany(string first, string last, string login, string pass, string company, string company_code)
        {
            GetBody(2);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CompanyName]", company.Trim());
            m_body = m_body.Replace("[LoginName]", login.Trim());
            m_body = m_body.Replace("[Password]", pass.Trim());
            m_body = m_body.Replace("[CompanyCode]", company_code.Trim());

        }

        public void OrderConfirmation_Email(object emailed_code_to_customer, object p1, object last_nm1, object p2, object p3, object p4, object last_nm2, object company_nm, object nameOnCard, object last_nm3, string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }

        public void NewCase(string first, string last, string case_number)
        {
            GetBody(3);
            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void NewCaseInvolved(string first, string last, string case_number)
        {
            GetBody(4);
            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void NewMessage(string first, string last, string case_number)
        {
            GetBody(5);
            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void NewTask(string first, string last, string case_number)
        {
            GetBody(6);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void NextStep(string first, string last, string case_number)
        {
            GetBody(7);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void CaseReopened(string first, string last, string case_number, string reopen_first, string reopen_last)
        {
            GetBody(8);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
            m_body = m_body.Replace("[ReopenMediatorName]", (reopen_first + " " + reopen_last).Trim());


        }
        public void CaseCloseApprove(string case_number)
        {
            GetBody(9);

            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }

        public void CaseCloseApprovePlatformManager(string case_number)
        {
            GetBody(10);

            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }

        public void EscalatedCase(string first, string last, string escalated_first, string escalated_last, string case_number)
        {
            GetBody(11);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
            m_body = m_body.Replace("[EscalatedMediatorName]", (escalated_first + " " + escalated_last).Trim());
        }
        public void EscalatedCaseTimeline(string first, string last, string case_number)
        {
            GetBody(12);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void NewUser(string first, string last, string login, string pass)
        {
                GetBody(13);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[LoginName]", login.Trim());
            m_body = m_body.Replace("[Password]", pass.Trim());
        }
        public void NewUserArrived(string first, string last, string new_first, string new_last)
        {
            GetBody(14);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[NewMediatorName]", (new_first + " " + new_last).Trim());
        }

        /// <summary>
        /// new company - message to mediator(cc to partner)
        /// </summary>
        /// <param name="first">partner f name</param>
        /// <param name="last">partner l name</param>
        /// <param name="company">partner's company</param>
        /// <param name="code"></param>
        /// <param name="link"></param>
        public void PartnerNewCompany(string first, string last, string company, string code, string link)
        {
            GetBody(20);

            m_body = m_body.Replace("[PartnerName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[PartnerCompany]", company.Trim());
            m_body = m_body.Replace("[Link]", link.Trim());
            m_body = m_body.Replace("[Code]", code.Trim());
        }
        public void PartnerNewCompanyArrived(string first, string last, string registered_first, string registered_last, string company)
        {
            GetBody(21);

            m_body = m_body.Replace("[PartnerName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[CompanyName]", company.Trim());
            m_body = m_body.Replace("[RegistratorName]", (registered_first + " " + registered_last).Trim());
        }

        public void ReporterNewCase(string login, string pass, string case_number)
        {
            GetBody(30);
            m_body = m_body.Replace("[LoginName]", login.Trim());
            m_body = m_body.Replace("[Password]", pass.Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        //deleted?
        public void ReporterNewMessage(string case_number)
        {
            GetBody(31);
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());

        }

        //deleted
        public void ReporterNewStep(string case_number)
        {
            GetBody(31);
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());

        }

        //deleted
        public void ReporterCaseReopened(string case_number)
        {
            GetBody(33);
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void MediatorAssigned(string first, string last, string admin_first, string admin_last, string case_number)
        {
            GetBody(40);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[AdminName]", (admin_first + " " + admin_last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void SetCaseOwner(string first, string last, string admin_first, string admin_last, string case_number)
        {
            GetBody(65);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[AdminName]", (admin_first + " " + admin_last).Trim());
            m_body = m_body.Replace("[CaseNumber]", case_number.Trim());
        }
        public void MediatorInvited(string first, string last, string admin_first, string admin_last, string company_name, string code, string link)
        {
            GetBody(41);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[AdminName]", (admin_first + " " + admin_last).Trim());
            m_body = m_body.Replace("[CompanyName]", company_name.Trim());
            m_body = m_body.Replace("[Link]", link.Trim());
            m_body = m_body.Replace("[Code]", code.Trim());

        }
        public void MediatorRoleChange(string first, string last, string admin_first, string admin_last, string new_role)
        {
            GetBody(42);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[AdminName]", (admin_first + " " + admin_last).Trim());
            m_body = m_body.Replace("[NewRole]", new_role.Trim());
        }
        public void MediatorStatusChange(string first, string last, string admin_first, string admin_last, string new_status)
        {
            GetBody(43);

            m_body = m_body.Replace("[MediatorName]", (first + " " + last).Trim());
            m_body = m_body.Replace("[AdminName]", (admin_first + " " + admin_last).Trim());
            m_body = m_body.Replace("[NewStatus]", new_status.Trim());

        }

        public void ForgetPassword(string url, string email, string code)
        {
            GetBody(50);

            m_body = m_body.Replace("[RestorePass]", (DomainUtil.GetSubdomainLink(url, m_url) + "/login/restore" + "?email=" + email + "&token=" + code).Trim());
            m_body = m_body.Replace("[RestorePass2]", (DomainUtil.GetSubdomainLink(url, m_url).Replace(".", "<span>.</span>") + "/<span>login</span>/restore" + "?email=" + email.Replace("@", "<span>@</span>").Replace(".", "<span>.</span>") + "&token=" + code).Trim());
            m_body = m_body.Replace("[RestoreCode]", code);

        }

        public void CampusSecurityAlert(string caseUrl, string caseId, string platformManagerName, string platformManagerCell)
        {
            GetBody(51);

            m_body = m_body.Replace("[CaseUrl]", (DomainUtil.GetSubdomainLink(caseUrl, m_url) + "/newCase/Index/" + caseUrl).Trim());
            m_body = m_body.Replace("[CaseId]", caseId);
            m_body = m_body.Replace("[PlatformManagerName]", platformManagerName);
            m_body = m_body.Replace("[PlatformManagerCell]", platformManagerCell);
        }

        public void NewMediator(string adminName, string companyName, string baseUrl, string link, string username, string password)
        {
            GetBody(52);

            m_body = m_body.Replace("[AdminName]", adminName);
            m_body = m_body.Replace("[CompanyName]", companyName);
            m_body = m_body.Replace("[BaseUrl]", DomainUtil.GetSubdomainLink(link, m_url) + "/login/index?loginField=" + username);
            m_body = m_body.Replace("[Link]", DomainUtil.GetSubdomainLink(link, m_url) + "/settings/index");
            m_body = m_body.Replace("[Username]", username);
            m_body = m_body.Replace("[Password]", password);
            m_body = m_body.Replace("[Username]", username);
        }

        public void ForgetPasswordNew(string url, string email, string code)
        {
            GetBody(53);

            m_body = m_body.Replace("[RestorePass]", (DomainUtil.GetSubdomainLink(url, m_url) + "/service/restore" + "?email=" + email + "&token=" + code).Trim());
            m_body = m_body.Replace("[RestorePass2]", (DomainUtil.GetSubdomainLink(url, m_url).Replace(".", "<span>.</span>") + "/<span>service</span>/restore" + "?email=" + email.Replace("@", "<span>@</span>").Replace(".", "<span>.</span>") + "&token=" + code).Trim());
            m_body = m_body.Replace("[RestoreCode]", code);
        }

        public void Scheduler1(string caseNumber)
        {
            GetBody(60);

            m_body = m_body.Replace("[CaseNumber]", caseNumber); 
        }

        public void CalendarEvent(bool company, bool added,string url, string date)
        {
            if ((company) && (added)) GetBody(61);
            if ((company) && (!added)) GetBody(62);
            if ((!company) && (added)) GetBody(63);
            if ((!company) && (!added)) GetBody(64);

            m_body = m_body.Replace("[Date]", date);
        }

        public void ECCOMVAR(string firstName, string lastName, string companyName, string url)
        {
            GetBody(66);

            m_body = m_body.Replace("[FirstName]", firstName);
            m_body = m_body.Replace("[LastName]", lastName);
            m_body = m_body.Replace("[CompanyName]", companyName);
            m_body = m_body.Replace("[Url]", url);
        }
        
        public void OrderConfirmation_Email(
            string orderNumber, 
            string name, 
            string surname, 
            string annualFee, 
            string onboardingFee, 
            string registrationDate, 
            string last, 
            string companyName,
            string CCName,
            string CCSurname,
            string url,
            string link,
            string linkReg)
        {
            GetBody(67);

            m_body = m_body.Replace("@@OrderNumber", orderNumber);
            m_body = m_body.Replace("@@Surname", surname);
            m_body = m_body.Replace("@@AnnualFee", annualFee);
            m_body = m_body.Replace("@@OnboardingFee", onboardingFee);
            m_body = m_body.Replace("@@RegistrationDate", registrationDate); //October 31st, 2019
            m_body = m_body.Replace("@@CompanyName", companyName);
            m_body = m_body.Replace("@@CCName", CCName);
            m_body = m_body.Replace("@@CCSurname", CCSurname);
            m_body = m_body.Replace("@@Link", link); //Video
            m_body = m_body.Replace("@@2Link", linkReg); //Registration complete

            m_body = m_body.Replace("@@Name", name);
            m_body = m_body.Replace("@@LAST", last);
            m_body = m_body.Replace("@@sName", "block");
            if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(last))
            {
                m_body = m_body.Replace("@@sName", "none");
            }

            m_body = m_body.Replace("@@CompanyName", companyName);
            m_body = m_body.Replace("@@sCompanyName", "block");
            if (String.IsNullOrEmpty(companyName))
            {
                m_body = m_body.Replace("@@sCompanyName", "none");
            }

            m_body = m_body.Replace("@@CCName", CCName);
            m_body = m_body.Replace("@@CCSurname", CCSurname);
            m_body = m_body.Replace("@@sCCName", "block");
            if (String.IsNullOrEmpty(CCName) && String.IsNullOrEmpty(CCSurname))
            {
                m_body = m_body.Replace("@@sCCName", "none");
            }
        }

        public void UserNotCompleteRegistration_Email(Func<string, string> prepareBody)
        {
            GetBody(68);

            m_body = prepareBody(m_body);
        }

        public void VarAfter4HoursAfterSignUp(Func<string, string> prepareBody)
        {
            GetBody(69);

            m_body = prepareBody(m_body);
        }
        
        public void VarAfter24HoursAfterSignUpToUser(Func<string, string> prepareBody)
        {
            GetBody(70);

            m_body = prepareBody(m_body);
        }

        public void VarAfter3WeekAfterSignUpToUser(Func<string, string> prepareBody)
        {
            GetBody(71);

            m_body = prepareBody(m_body);
        }
        #endregion
    }

}
