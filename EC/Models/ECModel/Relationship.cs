using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Constants;

namespace EC.Models.ECModel
{
    public class Relationship : BaseEntity
    {
        #region Properties
        public int id
        {
            get;
            set;
        }
        public string relationship_nm
        {
            get;
            set;
        }
        public string relationship_nm_val { get; set; }
        #endregion

        #region constructor - relationship by id
        public Relationship()
        {
            id = 0;
            relationship_nm = "";
        }

        public Relationship(int relationship_id, int? language_id)
        {
            Database.relationship _relationship = db.relationship.First(a => a.id == relationship_id);

            if ((_relationship.id != null) && (_relationship.id != 0))
            {
                id = _relationship.id;
                relationship_nm = _relationship.relationship_en;

                #region Region Languages
                if ((language_id.Value != null) && (language_id.Value.ToString() == ECLanguageConstants.LanguageFrench))
                {
                    relationship_nm = _relationship.relationship_fr;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == ECLanguageConstants.LanguageSpanish))
                {
                    relationship_nm = _relationship.relationship_es;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == ECLanguageConstants.LanguageRussian))
                {
                    relationship_nm = _relationship.relationship_ru;
                }
                else if ((language_id.Value != null) && (language_id.Value.ToString() == ECLanguageConstants.LanguageArabic))
                {
                    relationship_nm = _relationship.relationship_ar;
                }
                #endregion
            }
            else
            {
                id = 0;
                relationship_nm = "";
            }
        }
        #endregion

        public List<ECModel.Relationship> GetAllRelationships(int? language_id)
        {
            List<ECModel.Relationship> _relationship_list = new List<Relationship>();
            List<Database.relationship> _relationship = db.relationship.ToList();
            Relationship _new_relationship;

            for (int i = 0; i < _relationship.Count; i++)
            {
                _new_relationship = new Relationship(_relationship[i].id, language_id);
                _relationship_list.Add(_new_relationship);
            }

            return _relationship_list;
        }
    }
}