using EC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ECModel
{
    public class InjuryDamage : BaseEntity
    {

        #region Properties
        public int id { get; set; }
        public string InjuryDamageText { get; set; }
        public string InjuryDamageVal { get; set; }
        #endregion
        public InjuryDamage(int injuryDamage_id, int? language_id)
        {
            Database.injury_damage injuryDamage = db.injury_damage.First(a => a.id == injuryDamage_id);
            if(injuryDamage != null)
            {
                InjuryDamageText = injuryDamage.text_en;
                InjuryDamageVal = injuryDamage.text_en;
                id = injuryDamage.id;

                if (injuryDamage != null && injuryDamage_id > 0)
                {
                    #region Region Languages
                    if (language_id.HasValue)
                    {
                        switch (language_id.Value.ToString())
                        {
                            case ECLanguageConstants.LanguageFrench:
                                InjuryDamageText = injuryDamage.text_fr;
                                break;
                            case ECLanguageConstants.LanguageSpanish:
                                InjuryDamageText = injuryDamage.text_es;

                                break;
                            case ECLanguageConstants.LanguageRussian:
                                InjuryDamageText = injuryDamage.text_ru;

                                break;
                            case ECLanguageConstants.LanguageArabic:
                                InjuryDamageText = injuryDamage.text_ar;
                                break;
                        }
                    }
                    #endregion
                }
            }
        }
    }
}