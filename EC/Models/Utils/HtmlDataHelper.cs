using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog.LayoutRenderers.Wrappers;
using System.Reflection;
using EC.Models.Database;

namespace EC.Controllers.Utils
{
    public static class Locliztion
    {
        #region company
            public static string Path(this company company)
            {
                return company.T("path");
            }

            public static string Notepad(this company company)
            {

                return company.T("notepad");
            }

            public static string  Alert(this company company)
            {
                return company.T("alert");
            }
        #endregion

        #region location
            public static string Location(this company_location location)
            {
                return location.T("location");
            } 
        #endregion

        #region anonymity
            public static string Anonymity(this anonymity anonymity)
            {
                return anonymity.T("anonymity");
            }
            public static string AnonymityDs(this anonymity anonymity)
            {
                return anonymity.T("anonymity_ds");
            }
            public static string AnonymityCompany(this anonymity anonymity)
            {
                return anonymity.T("anonymity_company");
            } 
        #endregion
        
        #region country
            public static string Description(this country country)
            {
                return country.T("country_description");
            }
        #endregion

        #region management
            public static string Managament(this management_know management)
            {
                return management.T("text");
            }
        #endregion

         #region relationship
            public static string Relationship(this relationship relationship)
            {
                return relationship.T("relationship");
            }
         #endregion

            #region injury_damage
            public static string InjuryDamage(this injury_damage injuryDamage)
            {
                return injuryDamage.T("text");
            }
            #endregion

            public static string T(this object obj, string var)
        {
            return GetLocale(obj, var, SessionManager.inst.Lang);
        }

        public static string GetLocale(object data, string attr, string lang)
        {
            var target = attr + "_" + lang;
            var propertyInfos = data.GetType().GetProperties();
            var prop = propertyInfos.FirstOrDefault(property => property.Name.Equals(target));
            return  prop != null ? (string)prop.GetValue(data) : "";
        }
    }
    
}