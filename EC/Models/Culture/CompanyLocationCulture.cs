using EC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static EC.Common.Base.HtmlDataHelper;

namespace EC.Models.Culture
{
    public class CompanyLocationCulture
    {
        private List<CompanyLocationViewModel> locations;
        private int currentCompanyId;
        private CompanyModel companyModel;
        public CompanyLocationCulture(CompanyModel companyModel, int currentCompanyId)
        {
            this.companyModel = companyModel;
            this.currentCompanyId = currentCompanyId;
            locations = new List<CompanyLocationViewModel>();
        }

        public List<CompanyLocationViewModel> getLocationsCompanyCulture()
        {
            var allLocations = companyModel.Locations(currentCompanyId);
            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var location in allLocations)
                    {
                        CompanyLocationViewModel temp = new CompanyLocationViewModel();
                        temp.id = location.id;
                        temp.locationName = location.location_en;
                        locations.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var location in allLocations)
                    {
                        CompanyLocationViewModel temp = new CompanyLocationViewModel();
                        temp.id = location.id;
                        if (location.location_es != null && location.location_es != "")
                        {
                            temp.locationName = location.location_es;
                        } else
                        {
                            temp.locationName = location.location_en;
                        }
                        locations.Add(temp);
                    }
                    break;
            }
            return locations;
        }

        public SelectViewModel getLocationsCompanyCultureSelect()
        {
            if (locations.Count == 0)
                getLocationsCompanyCulture();
            List<SelectItem> selectLocations = new List<SelectItem>();
            SelectViewModel locationsSelect = new SelectViewModel(selectLocations);
            foreach(var location in locations)
            {
                locationsSelect.Items.Add(new SelectItem { Name = location.locationName, Value = location.id.ToString() });
            }
            return locationsSelect;
        }
    }
}