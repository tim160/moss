using EC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.Culture
{
    public class ReportedOutsideCulture
    {
        private List<ReportedOutsideViewModel> reportedOutsideViewModels;
        private CompanyModel companyModel;
        public ReportedOutsideCulture(CompanyModel companyModel)
        {
            this.companyModel = companyModel;
            reportedOutsideViewModels = new List<ReportedOutsideViewModel>();
        }
        public List<ReportedOutsideViewModel> getReportedOutside()
        {
            var allReports = companyModel.getReportedOutside();

            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var report in allReports)
                    {
                        ReportedOutsideViewModel temp = new ReportedOutsideViewModel();
                        temp.id = report.id;
                        temp.ReportedOutside = report.description_en;
                        reportedOutsideViewModels.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var report in allReports)
                    {
                        ReportedOutsideViewModel temp = new ReportedOutsideViewModel();
                        temp.id = report.id;
                        if (report.description_es != null && report.description_es != "")
                        {
                            temp.ReportedOutside = report.description_es;
                        } else
                        {
                            temp.ReportedOutside = report.description_en;
                        }
                        reportedOutsideViewModels.Add(temp);
                    }
                    break;
            }

            return reportedOutsideViewModels;
        }
    }
}