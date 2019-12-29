using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    public static class ECSessionConstants
    {

        public static int status_pending = 1;
        public static int status_active = 2;
        public static int status_inactive = 3;
        public static int status_reopened = 4;
        public static int status_responded = 5;
        public static int status_closed = 6;
        public static int status_assigned = 7;
        public static int status_escalated = 8;
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

        public const String CurrentUserMarcker = "CurentUser";
        public const String AuthUserCookies = "temp_templ_store";
        public const String SessionIsSSO = "isSSO";
    
    }
}
