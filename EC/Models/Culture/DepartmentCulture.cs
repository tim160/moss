using EC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static EC.Common.Base.HtmlDataHelper;

namespace EC.Models.Culture
{
    public class DepartmentCulture
    {
        private List<DepartmentsViewModel> departments;
        private CompanyModel companyModel;
        private int companyId;
        public DepartmentCulture(CompanyModel companyModel, int companyId)
        {
            this.companyModel = companyModel;
            this.companyId = companyId;
            departments = new List<DepartmentsViewModel>();
        }
        public List<DepartmentsViewModel> GetDepartmentsCulture()
        {
            var allDepartments = companyModel.CompanyDepartmentsActive(companyId).ToList();
            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var department in allDepartments)
                    {
                        DepartmentsViewModel temp = new DepartmentsViewModel();
                        temp.Id = department.id;
                        temp.departmentName = department.department_en;
                        departments.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var department in allDepartments)
                    {
                        DepartmentsViewModel temp = new DepartmentsViewModel();
                        temp.Id = department.id;
                        if (department.department_es != null && department.department_es != "")
                        {
                            temp.departmentName = department.department_es;
                        }
                        else
                        {
                            temp.departmentName = department.department_en;
                        }
                        departments.Add(temp);
                    }
                    break;

            }
            return departments;
        }

        public SelectViewModel GetDepartmentsCultureSelect()
        {
            if (departments.Count == 0)
                GetDepartmentsCulture();
            List<SelectItem> selectDepartmens = new List<SelectItem>();
            SelectViewModel departmentsSelect = new SelectViewModel(selectDepartmens);
            foreach (var department in departments)
            {
                departmentsSelect.Items.Add(new SelectItem { Name = department.departmentName, Value = department.Id.ToString() });
            }
            return departmentsSelect;
        }
    }
}