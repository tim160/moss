using EC.Models.Database;
using System.Linq;

namespace EC.Utils
{
    public static class Localization
    {
        public static string T(this object obj, string var)
        {
            return GetLocale(obj, var, SessionManager.inst.Lang);
        }

        public static string GetLocale(object data, string attr, string lang)
        {
            var target = attr + "_" + lang;
            var propertyInfos = data.GetType().GetProperties();
            var prop = propertyInfos.FirstOrDefault(property => property.Name.Equals(target));
            return prop != null ? (string)prop.GetValue(data) : "";
        }

        public static string Description(this country country)
        {
            return country.T("country_description");
        }
    }
}