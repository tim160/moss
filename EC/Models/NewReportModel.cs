using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EC.Models.Database;
using EC.Controllers.ViewModel;
using EC.Controllers.Utils;

namespace EC.Models.ECModel
{
    public class NewReportModel : BaseModel
    {
        public report getReport(int reporter_user_Id)
        {
            /*  SELECT * FROM [EC].[dbo].[report] WHERE id=xxxx*/

            var r = db.report.Where(s => (s.reporter_user_id == reporter_user_Id));

            if (r.First() == null)
            {
                return new report();
            }
            else
            {
                return r.First();
            }
        }

        public report_secondary_type getReportSecondaryType(int report_id)
        {
            /*  SELECT * FROM [EC].[dbo].[report] WHERE id=xxxx*/

            var reportSecondaryType = db.report_secondary_type.Where(s => (s.report_id == report_id));

            if (reportSecondaryType.First() == null)
            {
                return new report_secondary_type();
            }
            else
            {
                return reportSecondaryType.First();
            }
        }

        public string getManagmentKnowInfo(int id)
        {
            /*  SELECT * FROM [EC].[dbo].[report] WHERE id=xxxx*/

            var reportSecondaryType = db.management_know.Where(s => (s.id == id));

            if (reportSecondaryType.First() == null)
            {
                return "";
            }
            else
            {
                return reportSecondaryType.First().text_en;
            }
        }


        public List<message> getMessagesList (int report_id)
        {
            /*  SELECT * FROM [EC].[dbo].[report] WHERE id=xxxx*/

            //var messagesList = db.message.Where(s => (s.report_id == report_id)).ToList();
            // for reporter it should be: List<message> messagesList = db.message.Where(s => (s.report_id == report_id & s.reporter_access == 1)).ToList(); !!!!!!
            List<message> messagesList = db.message.Where(s => (s.report_id == report_id & s.reporter_access == 1)).ToList();

            if (messagesList == null)
            {
                return new List<message>();
            }
            else
            {
                return messagesList;
            }
        }

        public string getReporterCountry(int? reporter_country_id)
        {
            List<country> c = new List<country>();

            try
            {
                c = db.country.Where(s => (s.id == reporter_country_id)).ToList();
            }
            catch (Exception)
            {
                ;
            }

            if (c == null || c.Count == 0)
                return "NULL";
            else
                return c.First().country_nm;
        }
    }
}