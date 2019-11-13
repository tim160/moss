using EC.Common.Util;
using EC.Localization;
using EC.Models.Database;
using EC.Models.ECModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EC.Models
{
    public class MenuDashboardAnalytics
    {
        private ECEntities DB;
        private GlobalFunctions global;

        public MenuDashboardAnalytics(ECEntities DB, GlobalFunctions globalFunctions)
        {
            this.global = globalFunctions;
            this.DB = DB;
        }
        public string ReportAdvancedJson(ReportTypes types, int userId)
        {
            List<report> _all_reports = global.ReportsListForCompany(types.companyIdArray,
                userId,
                types.ReportsSecondaryTypesIDStrings, 
                types.ReportsRelationTypesIDStrings,
                types.ReportsDepartmentIDStringss,
                types.ReportsLocationIDStrings,
                types.dateStart, types.dateEnd);

            string _all_json = "{\"LocationTable\":" + CompanyLocationReportAdvancedJson(_all_reports)
                    + ", \"DepartmentTable\":" + CompanyDepartmentReportAdvancedJson(_all_reports)
                    + ", \"RelationTable\":" + RelationshipToCompanyByDateAdvancedJson(_all_reports)
                    + ", \"SecondaryTypeTable\":" + SecondaryTypesByDateAdvancedJson(_all_reports)
                    + "}";

            return _all_json;
        }

        public string ReportAdvancedJson(int[] company_id, int user_id, string ReportsSecondaryTypesIDStrings, string ReportsRelationTypesIDStrings, string ReportsDepartmentIDStringss, string ReportsLocationIDStrings, DateTime? dtReportCreationStartDate, DateTime? dtReportCreationEndDate)
        {

            List<report> _all_reports = global.ReportsListForCompany(company_id, user_id, ReportsSecondaryTypesIDStrings, ReportsRelationTypesIDStrings, ReportsDepartmentIDStringss, ReportsLocationIDStrings, dtReportCreationStartDate, dtReportCreationEndDate);

            string _all_json = "{\"LocationTable\":" + CompanyLocationReportAdvancedJson(_all_reports)
                    + ", \"DepartmentTable\":" + CompanyDepartmentReportAdvancedJson(_all_reports)
                    + ", \"RelationTable\":" + RelationshipToCompanyByDateAdvancedJson(_all_reports)
                    + ", \"SecondaryTypeTable\":" + SecondaryTypesByDateAdvancedJson(_all_reports)
            + "}";

            return _all_json;
        }
        private string CompanyLocationReportAdvancedJson(List<report> _all_reports)
        {
            DataTable dt = CompanyLocationReportAdvanced(_all_reports);
            return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
        }

        private string CompanyDepartmentReportAdvancedJson(List<report> _all_reports)
        {
            DataTable dt = CompanyDepartmentReportAdvanced(_all_reports);
            return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
        }

        private string RelationshipToCompanyByDateAdvancedJson(List<report> _all_reports)
        {
            DataTable dt = RelationshipToCompanyByDateAdvanced(_all_reports);
            return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
        }

        private string SecondaryTypesByDateAdvancedJson(List<report> _all_reports)
        {
            DataTable dt = SecondaryTypesByDateAdvanced(_all_reports);
            return JsonUtil.DataTableToJSONWithJavaScriptSerializer(dt);
        }

        private DataTable CompanyDepartmentReportAdvanced(List<report> _all_reports)
        {
            DataTable dt = global.dtDoughnutTable();
            DataRow dr;


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
            foreach (var department in companyDepatments)
            {
                dr = dt.NewRow();
                dr["name"] = department.NameLocation;
                dr["val"] = department.countLocations;
                dt.Rows.Add(dr);
            }
            return dt;
        }


        private DataTable CompanyLocationReportAdvanced(List<report> _all_reports)
        {
            ReportModel rm = new ReportModel();
            DataTable dt = global.dtDoughnutTable();
            DataRow dr;

            List<CompanyLocation> ResultLocations = _all_reports.Select(rep => rep.location_id != null ? rep.location_id : 0).Select(rep => new CompanyLocation
            {
                id = rep.Value
            }).ToList();
            int[] idLocations = ResultLocations.Select(idLoc => idLoc.id).ToArray();
            var locations = DB.company_location.Where(location => idLocations.Contains(location.id)).ToArray();

            List<CompanyLocation> ResultResultLocations = new List<CompanyLocation>();
            int countOther = idLocations.Where(idLoc => idLoc == 0).Count();
            if (countOther > 0)
            {
                ResultResultLocations.Add(new CompanyLocation
                {
                    id = 0,
                    countLocations = countOther,
                    NameLocation = LocalizationGetter.GetString("Other", false)
                });

            }
            foreach (var location in locations)
            {
                //checkisAlreadyAdded
                int countAlreadyadded = 0;
                if (ResultResultLocations.Count > 0)
                {
                    foreach (var rrlocation in ResultResultLocations)
                    {
                        if (rrlocation.NameLocation.Equals(location.location_en, StringComparison.OrdinalIgnoreCase))
                        {
                            countAlreadyadded++;
                        }

                    }
                }
                if (countAlreadyadded > 0)
                {
                    int countSameLocations = idLocations.Where(idLoc => idLoc == location.id).Count();
                    CompanyLocation updateLocation = ResultResultLocations.Where(rl => rl.NameLocation == location.location_en).FirstOrDefault();
                    if (updateLocation != null) { updateLocation.countLocations += countSameLocations; }
                }
                else
                {
                    //create New Location
                    CompanyLocation newLocation = new CompanyLocation();
                    newLocation.id = location.id;
                    newLocation.NameLocation = location.location_en;
                    newLocation.countLocations = ResultLocations.Where(rl => rl.id == location.id).Count();
                    ResultResultLocations.Add(newLocation);
                }
            }
            foreach (var resultLocation in ResultResultLocations)
            {
                dr = dt.NewRow();
                dr["name"] = resultLocation.NameLocation;
                dr["val"] = resultLocation.countLocations;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private DataTable RelationshipToCompanyByDateAdvanced(List<report> _all_reports)
        {
            // merge with previous
            List<Int32> _report_ids = _all_reports.Select(t => t.id).ToList();

            DataTable dt = global.dtDoughnutTable();
            DataRow dr;

            var ShipCompany = (
                DB.report_relationship.GroupJoin(
                    DB.company_relationship,
                    left => left.company_relationship_id,
                    right => right.id,
                    (left, right) => new
                    {
                        TableA = right,
                        TableB = left
                    }).SelectMany(p => p.TableA.DefaultIfEmpty(), (x, y) => new
                    {
                        TableA = y,
                        TableB = x.TableB
                    }).Where(u => _report_ids.Contains(u.TableB.report_id)));


            List<CompanyLocation> shipComapanies = new List<CompanyLocation>();

            foreach (var ship in ShipCompany)
            {
                if (ship.TableA != null)
                {
                    //checkisAlreadyAdded
                    int countAlreadyadded = 0;
                    if (shipComapanies.Count > 0)
                    {
                        foreach (var rrlocation in shipComapanies)
                        {
                            if (rrlocation.NameLocation.Equals(ship.TableA.relationship_en, StringComparison.OrdinalIgnoreCase))
                            {
                                countAlreadyadded++;
                            }
                        }
                    }

                    if (countAlreadyadded == 0)
                    {
                        int countSameShip = ShipCompany.Where(sameLoc => sameLoc.TableA.relationship_en.Equals(ship.TableA.relationship_en)).Count();
                        CompanyLocation newLocation = new CompanyLocation();
                        newLocation.id = ship.TableA.id;
                        newLocation.NameLocation = ship.TableA.relationship_en;
                        newLocation.countLocations = countSameShip;
                        shipComapanies.Add(newLocation);
                    }
                }
            }
            //put other
            var countOther = new CompanyLocation();
            countOther.countLocations = ShipCompany.Where(t => t.TableA == null).Count();
            countOther.NameLocation = LocalizationGetter.GetString("Other", false);
            if (countOther.countLocations > 0)
            {
                shipComapanies.Add(countOther);
            }

            foreach (var ship in shipComapanies)
            {
                dr = dt.NewRow();
                dr["name"] = ship.NameLocation;
                dr["val"] = ship.countLocations;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private DataTable SecondaryTypesByDateAdvanced(List<report> _all_reports)
        {
            // merge with previous
            List<Int32> _report_ids = _all_reports.Select(t => t.id).ToList();

            DataTable dt = global.dtDoughnutTable();
            DataRow dr;
            var secondaryType =
            (
            DB.report_secondary_type.GroupJoin
            (
                DB.company_secondary_type, left => left.secondary_type_id, right => right.id,
                (left, right) => new { TableA = right, TableB = left })
                .SelectMany(p => p.TableA.DefaultIfEmpty(), (x, y) =>
                    new { TableA = y, TableB = x.TableB })
                    .Where(u => _report_ids.Contains(u.TableB.report_id))
            );

            List<CompanyLocation> secondaryTypeResult = new List<CompanyLocation>();

            foreach (var secondType in secondaryType)
            {
                if (secondType.TableA != null)
                {
                    //checkisAlreadyAdded
                    int countAlreadyadded = 0;
                    if (secondaryTypeResult.Count > 0)
                    {
                        foreach (var rrlocation in secondaryTypeResult)
                        {
                            if (rrlocation.NameLocation.Equals(secondType.TableA.secondary_type_en, StringComparison.OrdinalIgnoreCase))
                            {
                                countAlreadyadded++;
                            }
                        }
                    }

                    if (countAlreadyadded == 0)
                    {
                        int countSameShip = secondaryType.Where(sameLoc => sameLoc.TableA.secondary_type_en.Equals(secondType.TableA.secondary_type_en)).Count();
                        CompanyLocation newLocation = new CompanyLocation();
                        newLocation.id = secondType.TableA.id;
                        newLocation.NameLocation = secondType.TableA.secondary_type_en;
                        newLocation.countLocations = countSameShip;
                        secondaryTypeResult.Add(newLocation);
                    }
                }
            }
            //put other
            var countOther = new CompanyLocation();
            countOther.countLocations = secondaryType.Where(t => t.TableA == null).Count();
            countOther.NameLocation = LocalizationGetter.GetString("Other", false);
            if (countOther.countLocations > 0)
            {
                secondaryTypeResult.Add(countOther);
            }

            foreach (var ship in secondaryTypeResult)
            {
                dr = dt.NewRow();
                dr["name"] = ship.NameLocation;
                dr["val"] = ship.countLocations;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
    public class CompanyLocation
    {
        public int id { get; set; }
        public int countLocations { get; set; }
        public string NameLocation { get; set; }
    }
}