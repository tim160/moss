using EC.Constants;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.Culture
{
    public class InjuryDamageCulture
    {
        private List<InjuryDamageViewModel> injuryDamageViewModels;
        private CompanyModel companyModel;
        GetDBEntityModel getDBEntityModel = new GetDBEntityModel();

        public InjuryDamageCulture(CompanyModel companyModel)
        {
            this.companyModel = companyModel;
            injuryDamageViewModels = new List<InjuryDamageViewModel>();
        }

        public List<InjuryDamageViewModel> getInjuryDamageCulture()
        {
            var allDamages = getDBEntityModel.GetInjuryDamages().ToList();

            foreach (var damage in allDamages)
            {
                InjuryDamageViewModel temp = new InjuryDamageViewModel();
                temp.id = damage.id;
                temp.InjuryDamage = damage.text_en;
                temp.InjuryDamage = getCulturyDamage(damage);
                temp.InjuryDamageVal = damage.text_en;
                injuryDamageViewModels.Add(temp);
            }

            return injuryDamageViewModels;
        }

        private string getCulturyDamage(injury_damage damage)
        {
            switch(Localization.LocalizationGetter.Culture.TwoLetterISOLanguageName)
            {
                case ECLanguageConstants.LanguageArabic:
                    return !String.IsNullOrEmpty(damage.text_ar) ? damage.text_ar : damage.text_en;
                case ECLanguageConstants.LanguageFrench:
                    return !String.IsNullOrEmpty(damage.text_fr) ? damage.text_fr : damage.text_en;
                case ECLanguageConstants.LanguageRussian:
                    return !String.IsNullOrEmpty(damage.text_ru) ? damage.text_ru : damage.text_en;
                case ECLanguageConstants.LanguageSpanish:
                    return !String.IsNullOrEmpty(damage.text_es) ? damage.text_es : damage.text_en;
            }
            return damage.text_en;
        }
    }
}