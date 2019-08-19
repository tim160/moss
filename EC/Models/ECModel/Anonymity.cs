using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Constants;

namespace EC.Models.ECModel
{
    public class Anonymity : BaseEntity
    {
        #region Properties
        public int id
        {
            get;
            set;
        }
        public string anonymity
        {
            get;
            set;
        }
        public string anonymity_for_company
        {
            get;
            set;
        }
        public string anonymity_description
        {
            get;
            set;
        }
        #endregion

        #region constructor - anonymity by id

        public Anonymity() { }
        public Anonymity(int anonymity_id, int? language_id)
        {
            Database.anonymity _anonymity = db.anonymity.First(a => a.id == anonymity_id);

            if ((_anonymity.id != null) && (_anonymity.id != 0))
            {
                id = _anonymity.id;
                anonymity = _anonymity.anonymity_en;
                anonymity_for_company = _anonymity.anonymity_en_company;
                anonymity_description = _anonymity.anonymity_ds_en;

                #region Region Languages
                if ((language_id.HasValue) && (language_id.Value.ToString() == ECLanguageConstants.LanguageFrench))
                {
                    anonymity = _anonymity.anonymity_fr;
                    anonymity_for_company = _anonymity.anonymity_fr_company;
                    anonymity_description = _anonymity.anonymity_ds_fr;
                }
                else if ((language_id.HasValue) && (language_id.Value.ToString() == ECLanguageConstants.LanguageSpanish))
                {
                    anonymity = _anonymity.anonymity_es;
                    anonymity_for_company = _anonymity.anonymity_es_company;
                    anonymity_description = _anonymity.anonymity_ds_es;
                }
                else if ((language_id.HasValue) && (language_id.Value.ToString() == ECLanguageConstants.LanguageRussian))
                {
                    anonymity = _anonymity.anonymity_ru;
                    anonymity_for_company = _anonymity.anonymity_ru_company;
                    anonymity_description = _anonymity.anonymity_ds_ru;
                }
                else if ((language_id.HasValue) && (language_id.Value.ToString() == ECLanguageConstants.LanguageArabic))
                {
                    anonymity = _anonymity.anonymity_ar;
                    anonymity_for_company = _anonymity.anonymity_ar_company;
                    anonymity_description = _anonymity.anonymity_ds_ar;
                } 
                #endregion
            }
            else
            {
                id = 0;
                anonymity = "";
                anonymity_for_company = "";
                anonymity_description = "";
            }

        }
        #endregion
    }
}