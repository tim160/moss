using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
    public class AnonimityCulture : BaseModel
    {
        private CompanyModel companyModel;
        private company company;
        private List<EC.Models.ECModel.Anonymity> anonimityView;

        public AnonimityCulture(company company, CompanyModel companyModel)
        {
            this.company = company;
            this.companyModel = companyModel;
            anonimityView = new List<ECModel.Anonymity>();
        }

        public List<EC.Models.ECModel.Anonymity> getAnonimityCultrure()
        {
            List<anonymity> allAnonymty = companyModel.GetAnonymities(company.id, 0);
            
            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var anon in allAnonymty)
                    {
                        EC.Models.ECModel.Anonymity temp = new ECModel.Anonymity();
                        temp.id = anon.id;
                        temp.anonymity = string.Format(anon.anonymity_en, company.company_nm);
                        temp.anonymity_for_company = string.Format(anon.anonymity_en_company, company.company_nm);
                        temp.anonymity_description = string.Format(anon.anonymity_ds_en, company.company_nm);
                        anonimityView.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var anon in allAnonymty)
                    {
                        EC.Models.ECModel.Anonymity temp = new ECModel.Anonymity();
                        temp.id = anon.id;
                        if (anon.anonymity_es != null && anon.anonymity_es != "")
                        {
                            temp.anonymity = string.Format(anon.anonymity_es, company.company_nm);
                        }
                        else
                        {
                            temp.anonymity = string.Format(anon.anonymity_en, company.company_nm);
                        }

                        if (anon.anonymity_es_company != null && anon.anonymity_es_company != "")
                        {
                            temp.anonymity_for_company = string.Format(anon.anonymity_es_company, company.company_nm);
                        }
                        else
                        {
                            temp.anonymity_for_company = string.Format(anon.anonymity_en_company, company.company_nm);
                        }

                        if (anon.anonymity_ds_es != null && anon.anonymity_ds_es != "")
                        {
                            temp.anonymity_description = string.Format(anon.anonymity_ds_es, company.company_nm);
                        }
                        else
                        {
                            temp.anonymity_description = string.Format(anon.anonymity_ds_en, company.company_nm);
                        }
                        anonimityView.Add(temp);
                    }
                    break;
            }
            return anonimityView;
        }
    }
}