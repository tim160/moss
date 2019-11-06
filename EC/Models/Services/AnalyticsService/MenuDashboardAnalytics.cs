using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models
{
    public class MenuDashboardAnalytics
    {
        private ECEntities DB;
        private GlobalFunctions global;

        public MenuDashboardAnalytics(ECEntities DB)
        {
            this.DB = DB;
        }




        //    return null;
        //}

        public List<CompanyLocation> DepartmentsList(List<report> _all_reports)
        {
            List<int> report_ids_list = _all_reports.Select(t => t.id).ToList();

            var DepAndReports = DB.report_department.Join(DB.company_department,
                                                post => post.department_id,
                                                meta => meta.id,
                                                (post, meta) => new { Post = post, Meta = meta })
                                                .Where(postAndMeta => report_ids_list.Contains(postAndMeta.Post.report_id));
            List<CompanyLocation> companyDepatments = new List<CompanyLocation>();

            foreach (var department in DepAndReports)
            {
                //checkisAlreadyAdded
                int countAlreadyadded = 0;
                if (companyDepatments.Count > 0)
                {
                    foreach (var rrlocation in companyDepatments)
                    {
                        if (rrlocation.NameLocation.Equals(department.Meta.department_en, StringComparison.OrdinalIgnoreCase))
                        {
                            countAlreadyadded++;
                        }
                    }
                }

                if (countAlreadyadded == 0)
                {
                    int countSameLocations = DepAndReports.Where(sameLoc => sameLoc.Meta.department_en.Equals(department.Meta.department_en)).Count();
                    CompanyLocation newLocation = new CompanyLocation();
                    newLocation.id = department.Meta.id;
                    newLocation.NameLocation = department.Meta.department_en;
                    newLocation.countLocations = countSameLocations;
                    companyDepatments.Add(newLocation);
                }
            }
            return companyDepatments;
        }
        //private List<company_location> LocationsList()
        //{

        //}
        //private List<company_secondary_type> SecondaryTypesList()
        //{

        //}
        //private List<company_relationship> RelationTypesList()
        //{

        //}

    }
    public class CompanyLocation
    {
        public int id { get; set; }
        public int countLocations { get; set; }
        public string NameLocation { get; set; }
    }
}