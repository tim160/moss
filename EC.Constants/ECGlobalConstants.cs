﻿using System;
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

        public static DateTime _default_date = new DateTime(1900, 1, 1);

        public const string FormerEmployeeID = "2";

        public const string UnknownCompanyId = "1";
        public const string CountryIdUSA = "1";
        public const string CountryIdCanada = "2";


        public static readonly string[] ReportFlowStatusesList = {
            LocalizationGetter.GetString("NewReportUp"),
            LocalizationGetter.GetString("ReportReviewUp"),
            LocalizationGetter.GetString("UnderInvestigationUp"),
            LocalizationGetter.GetString("AwaitingSignOff"),
            LocalizationGetter.GetString("Closed") };



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

        public const string APP_SETTING_HEADER_COLOR = "HeaderColor";
        public const string APP_SETTING_HEADER_COLOR_LINK = "HeaderLinksColor";
        public const string APP_SETTING_IS_PARTNER_SSO_DOMAIN = "isPartnerSSODomain";
  }
}
