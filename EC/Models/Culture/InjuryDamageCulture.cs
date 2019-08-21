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
        public InjuryDamageCulture(CompanyModel companyModel)
        {
            this.companyModel = companyModel;
            injuryDamageViewModels = new List<InjuryDamageViewModel>();
        }

        public List<InjuryDamageViewModel> getInjuryDamageCulture()
        {
            var allDamages = companyModel.GetInjuryDamages().ToList();

            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var damage in allDamages)
                    {
                        InjuryDamageViewModel temp = new InjuryDamageViewModel();
                        temp.id = damage.id;
                        temp.InjuryDamage = damage.text_en;
                        injuryDamageViewModels.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var damage in allDamages)
                    {
                        InjuryDamageViewModel temp = new InjuryDamageViewModel();
                        temp.id = damage.id;
                        if (damage.text_es != null && damage.text_es != "")
                        {
                            temp.InjuryDamage = damage.text_es;
                        }
                        else
                        {
                            temp.InjuryDamage = damage.text_en;
                        }
                        injuryDamageViewModels.Add(temp);
                    }
                    break;
            }
            return injuryDamageViewModels;
        }
    }
}