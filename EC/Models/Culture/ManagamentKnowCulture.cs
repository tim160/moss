using EC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.Culture
{
    public class ManagamentKnowCulture
    {
        private List<ManagamentViewModel> ManagamentKnow;
        private CompanyModel companyModel;
        public ManagamentKnowCulture(CompanyModel companyModel)
        {
            this.companyModel = companyModel;
            ManagamentKnow = new List<ManagamentViewModel>();
        }
        public List<ManagamentViewModel> GetManagamentKnowCulture()
        {
            var allManagamentKnow = companyModel.getManagamentKnow();
            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var man in allManagamentKnow)
                    {
                        ManagamentViewModel temp = new ManagamentViewModel();
                        temp.id = man.id;
                        temp.ManagamentKnow = man.text_en;
                        temp.ManagamentValueEn = man.text_en;
                        ManagamentKnow.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var man in allManagamentKnow)
                    {
                        ManagamentViewModel temp = new ManagamentViewModel();
                        temp.id = man.id;
                        if (man.text_es != null && man.text_es != "")
                        {
                            temp.ManagamentKnow = man.text_es;
                        }
                        else
                        {
                            temp.ManagamentKnow = man.text_en;
                        }
                        temp.ManagamentValueEn = man.text_en;
                        ManagamentKnow.Add(temp);
                    }
                    break;
            }
            return ManagamentKnow;
        }
    }
}