using EC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.Culture
{
    public class SecondaryMandatoryCulture
    {
        private List<SecondaryMandatoryViewModel> secondaryMandatoryViews;
        private ReportModel reportModel;
        private int companyId;
        public SecondaryMandatoryCulture(ReportModel reportModel, int companyId)
        {
            this.reportModel = reportModel;
            this.companyId = companyId;
            secondaryMandatoryViews = new List<SecondaryMandatoryViewModel>();
        }
        public List<SecondaryMandatoryViewModel> getSecondaryTypeMandatory()
        {
            var allTypes = reportModel.getSecondaryTypeMandatory().Where(t => t.status_id == 2).OrderBy(x => x.secondary_type_en).ToList();

            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var type in allTypes)
                    {
                        SecondaryMandatoryViewModel temp = new SecondaryMandatoryViewModel();
                        temp.id = type.id;
                        temp.SecondaryMandatory = type.secondary_type_en;
                        secondaryMandatoryViews.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var type in allTypes)
                    {
                        SecondaryMandatoryViewModel temp = new SecondaryMandatoryViewModel();
                        temp.id = type.id;
                        if (type.description_es != null && type.description_es != "")
                        {
                            temp.SecondaryMandatory = type.secondary_type_es;
                        }
                        else
                        {
                            temp.SecondaryMandatory = type.secondary_type_en;
                        }
                        secondaryMandatoryViews.Add(temp);
                    }
                    break;
            }

            return secondaryMandatoryViews;
        }
        public List<SecondaryMandatoryViewModel> GetSecondaryMandCustom()
        {
            var allTypes = reportModel.getCompanySecondaryType(companyId);

            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var type in allTypes)
                    {
                        SecondaryMandatoryViewModel temp = new SecondaryMandatoryViewModel();
                        temp.id = type.id;
                        temp.SecondaryMandatory = type.secondary_type_en;
                        secondaryMandatoryViews.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var type in allTypes)
                    {
                        SecondaryMandatoryViewModel temp = new SecondaryMandatoryViewModel();
                        temp.id = type.id;
                        if (type.secondary_type_es != null && type.secondary_type_es != "")
                        {
                            temp.SecondaryMandatory = type.secondary_type_es;
                        }
                        else
                        {
                            temp.SecondaryMandatory = type.secondary_type_en;
                        }
                        secondaryMandatoryViews.Add(temp);
                    }
                    break;


            }
            return secondaryMandatoryViews;
        }
    }
}