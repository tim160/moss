using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Localization;
using System.Runtime.Serialization;

namespace EC.Constants
{
    public static class ECGlobalConstants
    {

        public const string CurrentUserMarcker = "CurentUser";
        public const string AuthUserCookies = "temp_templ_store";


        public static DateTime _default_date = new DateTime(1900, 1, 1);

        public const string FormerEmployeeID = "2";

        public const string UnknownCompanyId = "1";
        public const string CountryIdUSA = "1";
        public const string CountryIdCanada = "2";

        #region Status Constants
        public const int status_active = 2;
        public const int status_inactive = 3;
        public const int status_reopened = 4;
        public const int status_responded = 5;
        public const int status_closed = 6;
        public const int status_assigned = 7;
        public const int status_escalated = 8; 
        #endregion


        #region Investigation Status Constants
        public const int investigation_status_pending = 1;
        public const int investigation_status_review = 2;
        public const int investigation_status_investigation = 3;
        public const int investigation_status_resolution = 4;
        public const int investigation_status_escalation = 5;
        public const int investigation_status_completed = 6;
        public const int investigation_status_spam = 7;
        public const int investigation_status_completed_not_resolved_to_remove = 8;
        public const int investigation_status_closed = 9; 
        #endregion

        public const int anonymity_Anonymous = 1;
        public const int anonymity_Anonymous_to_company_only = 2;
        public const int anonymity_Shared_info = 3;
  //      public string[] AnonymousArray = { Localization.Resources.GetString("Anonymous"), Localization.Resources.GetString("ConfidentialToZero"), Localization.Resources.GetString("ContactInfoShared") };

     /*   public enum AnonymousArray
        {
            [EnumMember]
            Anonymous = Localization.Resources.GetString("Anonymous"),

            [EnumMember]
            ConfidentialToZero = Localization.Resources.GetString("ConfidentialToZero"),

            [EnumMember]
            ContactInfoShared = Localization.Resources.GetString("ContactInfoShared")
        }*/

    }
}
