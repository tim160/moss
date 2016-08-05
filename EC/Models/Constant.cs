using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models
{
    public class Constant
    {
        #region Language
        public static String LanguageFrench = "fr";
        public static String LanguageFrenchLb = "Français";
        public static String LanguageEnglish = "en";
        public static String LanguageEnglishLb = "English";
        public static String LanguageSpanish = "es";
        public static String LanguageSpanishLb = "Español";
        public static String LanguageRussian = "ru";
        public static String LanguageRussianLb = "Русский";
        public static String LanguageArabic = "ar";
        public static String LanguageArabicLb = "ar"; 
        #endregion

        #region Session

        public static String session_login_nm = "login_nm";
        public static String session_client_nm = "client_nm";
        public static String session_client_id = "client_id";
        public static String session_user_id = "user_id";
        public static String session_privilege_level_id = "privilege_level_id";
        public static String session_language_cl = "language_cl";
        public static String session_company_nm = "company_nm";
        public static String session_company_id = "company_id";

        public static String session_incident_id = "incident_id";
        public static String session_incident_nm = "incident_nm";

        public static String session_password = "password";


        public static String session_incident_editor_tab = "incident_editor_tab";

        public static String session_incident_search_dataview = "incident_search_dataview";
        public static String session_incident_search_conditions = "incident_search_conditions";
        public static String session_incident_search_sorting = "incident_search_sorting";

        public static String session_client_search_dataview = "client_search_table";
        public static String session_client_search_conditions = "client_search_conditions";
        public static String session_client_search_sorting = "client_search_sorting";

        public static String session_office_add_hashtable = "office_add_hashtable";
        //public static String session_client_search_conditions = "client_search_conditions";

        public static String session_connection_string_name = "conneciton_string_name";

        public static String session_available_auditors = "available_auditors";
        public static String session_assigned_auditors = "assigned_auditors";

        #endregion

        #region level / user_role
        public const int level_superuser = 1;
        public const int level_ec_top_mediator = 2;
        public const int level_ec_mediator = 3;
        public const int level_escalation_mediator = 4;
        public const int level_supervising_mediator = 5;
        public const int level_mediator = 6;
        public const int level_administrator = 7;
        public const int level_informant = 8;
        #endregion

        public static DateTime _default_date = new DateTime(1900, 1, 1);



        public static String FormerEmployeeID = "2";


        public static String UnknownCompanyId = "1";
        public static String CountryIdUSA = "1";
        public static String CountryIdCanada = "2";
        /// <summary>
        /// the folder where email template is located.
        /// </summary>
    ////    public static String EmailFolder = ConfigurationSettings.AppSettings["ProgramUrl"]; // "https://testwebsite.com/testsite1";


        public static int status_pending = 1;
        public static int status_active = 2;
        public static int status_inactive = 3;
        public static int status_reopened = 4;
        public static int status_responded = 5;
        public static int status_closed = 6;
        public static int status_assigned = 7;
        public static int status_escalated = 8;


        public static int investigation_status_pending = 1;
        public static int investigation_status_review = 2;
        public static int investigation_status_investigation = 3;
        public static int investigation_status_resolution = 4;
        public static int investigation_status_escalation = 5;
        public static int investigation_status_completed = 6;
        public static int investigation_status_spam = 7;
        public static int investigation_status_completed_not_resolved_to_remove = 8;
        public static int investigation_status_closed = 9;

        public static int anonymity_Anonymous = 1;
        public static int anonymity_Anonymous_to_company_only = 2;
        public static int anonymity_Shared_info = 3;
        public string[] AnonymousArray = { App_LocalResources.GlobalRes.Anonymous, App_LocalResources.GlobalRes.ConfidentialToZero, App_LocalResources.GlobalRes.ContactInfoShared };
    }

    public class ActionType
    {
        public static int Reviewed_Report = 1;
        public static int Case_Submitted = 2;
        public static int Updated_Report_Status = 3;
        public static int Updated_Report_Validation_Type = 4;
        public static int Assigned_Mediator = 5;
        public static int Removed_Mediator = 6;
        public static int Posted_Message_Reporter = 7;
        public static int Posted_Message_Mediator = 8;
        public static int Escalated_Case   = 10;
        public static int Task_Created = 11;
        public static int Task_Closed = 12;
        public static int Updated_Report_Priority = 13;
        public static int Updated_Report_Summary = 14;
        public static int Report_Escalated_Timeline_Violation = 15;
        public static int Case_Approved = 17;
        public static int Spam_Case = 18;
        public static int Reported_Opened = 19;
        public static int Review_Stage = 20;
        public static int Investigation = 21;
        public static int Resolution = 22;
        public static int Escalation = 23;
        public static int Closed = 25;
       
    }

}