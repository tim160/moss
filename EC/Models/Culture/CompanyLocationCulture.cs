using EC.Constants;
using EC.Models.Database;
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

            foreach(var location in allLocations)
            {
                CompanyLocationViewModel temp = new CompanyLocationViewModel();
                temp.id = location.id;
                temp.locationName = location.location_en;
                temp.locationName = getCulturyLocation(location);
                locations.Add(temp);
            }


            return locations;
        }
        private string getCulturyLocation(company_location location)
        {
            switch (Localization.LocalizationGetter.Culture.TwoLetterISOLanguageName)
            {
                case ECLanguageConstants.LanguageArabic:
                    return location.location_ar != null ? location.location_ar : location.location_en;
                case ECLanguageConstants.LanguageFrench:
                    return location.location_fr != null ? location.location_fr : location.location_en;
                case ECLanguageConstants.LanguageRussian:
                    return location.location_ru != null ? location.location_ru : location.location_en;
                case ECLanguageConstants.LanguageSpanish:
                    return location.location_es != null ? location.location_es : location.location_en;
            }
            return location.location_en;
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