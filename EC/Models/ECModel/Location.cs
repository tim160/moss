using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ECModel
{
    public class Location : BaseEntity
    {
        #region Properties
        public int id
        {
            get;
            set;
        }
        public string location_nm
        {
            get;
            set;
        }
        #endregion

        #region constructor - location by id
        public Location(int location_id, int? language_id)
        {
            Database.company_location _location = db.company_location.First(a => a.id == location_id);

            if ((_location.id != null) && (_location.id != 0))
            {
                id = _location.id;
                location_nm = _location.location_en;

                #region Region Languages
                if ((language_id.Value != null) && (language_id.Value.ToString() == Constant.LanguageFrench))
                {
                    location_nm = _location.location_fr;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == Constant.LanguageSpanish))
                {
                    location_nm = _location.location_es;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == Constant.LanguageRussian))
                {
                    location_nm = _location.location_ru;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == Constant.LanguageArabic))
                {
                    location_nm = _location.location_ar;
                }
                #endregion
            }
            else
            {
                id = 0;
                location_nm = "";
            }
        }
        #endregion
    }
}