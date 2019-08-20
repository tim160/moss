using EC.Localization;
using EC.Models.Database;
using EC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EC.Common.Base.HtmlDataHelper;

namespace EC.Models.Culture
{
    public class RoleInReportCulture
    {
        private List<RoleInReportViewModel> roleInReportViewModels;
        private ECEntities db;
        private bool is_cc;
        public RoleInReportCulture(ECEntities db, bool is_cc)
        {
            this.db = db;
            this.is_cc = is_cc;
            roleInReportViewModels = new List<RoleInReportViewModel>();
        }

        public List<RoleInReportViewModel> getRoleInReportCulture()
        {
            var AllroleInReport = db.role_in_report.ToList();
            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var role in AllroleInReport)
                    {
                        RoleInReportViewModel temp = new RoleInReportViewModel();
                        temp.id = role.id;
                        temp.RoleInReport = role.role_en;
                        roleInReportViewModels.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var role in AllroleInReport)
                    {
                        RoleInReportViewModel temp = new RoleInReportViewModel();
                        temp.id = role.id;
                        if (role.role_es != null && role.role_es != "")
                        {
                            temp.RoleInReport = role.role_es;
                        }
                        else
                        {
                            temp.RoleInReport = role.role_en;
                        }
                        roleInReportViewModels.Add(temp);
                    }
                    break;
            }
            return roleInReportViewModels;
        }
        public List<SelectListItem> getRoleInReportCultureSelect()
        {
            if (roleInReportViewModels.Count == 0)
                getRoleInReportCulture();
            List<SelectListItem> selectedRoleInReport = new List<SelectListItem>();
            selectedRoleInReport.Add(new SelectListItem { Text = LocalizationGetter.GetString("Other", this.is_cc), Value = "0", Selected = true });
            foreach (var role in roleInReportViewModels)
            {
                selectedRoleInReport.Add(new SelectListItem { Text = role.RoleInReport, Value = role.id.ToString() });
            }
            return selectedRoleInReport;
        }
    }
}