using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.Constants;

namespace EC.Models.ECModel
{
    public class Department : BaseEntity
    {
        #region Properties
        public int id
        {
            get;
            set;
        }
        public string department_nm
        {
            get;
            set;
        }
        #endregion

        public Department() { }
        #region constructor - department by id
        public Department(int department_id, int? language_id)
        {
            
            Database.company_department _department = db.company_department.First(a => a.id == department_id);

            if ((_department.id != null) && (_department.id != 0))
            {
                id = _department.id;
                department_nm = _department.department_en;

                #region Region Languages
                if ((language_id.Value != null) && (language_id.Value.ToString() == ECLanguageConstants.LanguageFrench))
                {
                    department_nm = _department.department_fr;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == ECLanguageConstants.LanguageSpanish))
                {
                    department_nm = _department.department_es;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == ECLanguageConstants.LanguageRussian))
                {
                    department_nm = _department.department_ru;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == ECLanguageConstants.LanguageArabic))
                {
                    department_nm = _department.department_ar;
                }
                #endregion
            }
            else
            {
                id = 0;
                department_nm = "";
            }
        }
        #endregion
    }
}