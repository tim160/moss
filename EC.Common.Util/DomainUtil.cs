using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EC.Common.Util
{
    public class DomainUtil
    {
        // Content/img/cai_logo.png
        public static bool IsSubdomain(string url)
        {
            if (url.ToLower().Contains("localhost") || url.ToLower().Contains("local2host") || url.ToLower().Contains("stark.") || url.ToLower().Contains("democompany.") || url.ToLower().Contains("report.") || url.ToLower().Contains("cai."))
            {
                return true;
            }
            return false;
        }

        public static bool IsStark(string url)
        {
            
            url = url.ToLower();
            if ((url.ToLower().Contains("stark.")) )
            {
                return true;
            }

            return false;
        }

        public static bool IsCC(string url)
        {
            //return true;
            // uncomment for campus-confidential testing
            //      return true;
            if ((url.ToLower().Contains("campus")) || (url.ToLower().Contains("cc.employeeconfidential")))
            {
                return true;
            }

            return false;
        }

        public static bool IsCC(HttpRequestBase request)
        {
            return IsCC(request.Url.AbsoluteUri.ToLower());
        }

        public static string LogoBaseUrl(string url)
        {
            if (url.ToLower().Contains("campus"))
            {
                return "/Content/img/secondLogo.jpg";
            }
            else if (url.ToLower().Contains("stark."))
            {
                return "/Content/img/secondLogo.jpg";
            }
            else if (url.ToLower().Contains("report."))
            {
                return "/Content/Icons/logo.png";
            }
            else if (url.ToLower().Contains("cai.employeeconfidential"))
            {
                return "";
                /////  return "/Content/Icons/logo.png";

                ////// return "/Content/img/cai_logo.png";
            }
            return "/Content/Icons/logo.png";

        }

        public static string GetSubdomainLink(string url)
        {
            string entrance_link = "report.employeeconfidential.com";

            if (url.ToLower().Contains("campus"))
            {
                entrance_link = "campus-confidential.com";
            }
            else if (url.ToLower().Contains("stark."))
            {
                entrance_link = "stark.employeeconfidential.com";
            }
            else if (url.ToLower().Contains("report.employeeconfidential"))
            {
                entrance_link = "report.employeeconfidential.com";
            }
            else if (url.ToLower().Contains("cai.employeeconfidential.com"))
            {
                entrance_link = "cai.employeeconfidential.com";
            }
            if (url.ToLower().Contains("report.campus"))
            {
                entrance_link = "report.campus-confidential.com";
            }
            return entrance_link;
        }

        public static string GetUser_IP()
        {
            string VisitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }
            return VisitorsIPAddr;
        }
    }
}
