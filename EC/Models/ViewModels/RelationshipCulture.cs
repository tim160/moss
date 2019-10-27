using EC.Constants;
using EC.Models.Database;
using EC.Models.ECModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
    public class RelationshipCulture
    {
        private List<EC.Models.ECModel.Relationship> relationshipViewModel;
        private CompanyModel companyModel;
        public RelationshipCulture(CompanyModel companyModel)
        {
            relationshipViewModel = new List<Relationship>();
            this.companyModel = companyModel;
        }

        public List<EC.Models.ECModel.Relationship> getRelationshipCulture()
        {
            var allRelationships = companyModel.getRelationships();

            foreach(var relation in allRelationships)
            {
                Relationship temp = new Relationship();
                temp.id = relation.id;
                temp.relationship_nm = relation.relationship_en;
                temp.relationship_nm = getCulturyRelation(relation);
                temp.relationship_nm_val = relation.relationship_en;
                relationshipViewModel.Add(temp);
            }

            return relationshipViewModel;
        }

        private string getCulturyRelation(relationship relationship)
        {
            switch (Localization.LocalizationGetter.Culture.TwoLetterISOLanguageName)
            {
                case ECLanguageConstants.LanguageArabic:
                    return relationship.relationship_ar != null ? relationship.relationship_ar : relationship.relationship_en;
                case ECLanguageConstants.LanguageFrench:
                    return relationship.relationship_fr != null ? relationship.relationship_fr : relationship.relationship_en;
                case ECLanguageConstants.LanguageRussian:
                    return relationship.relationship_ru != null ? relationship.relationship_ru : relationship.relationship_en;
                case ECLanguageConstants.LanguageSpanish:
                    return relationship.relationship_es != null ? relationship.relationship_es : relationship.relationship_en;
            }
            return relationship.relationship_en;
        }
        private string getCulturyRelationOther(company_relationship relationship)
        {
            switch (Localization.LocalizationGetter.Culture.TwoLetterISOLanguageName)
            {
                case ECLanguageConstants.LanguageArabic:
                    return relationship.relationship_ar != null ? relationship.relationship_ar : relationship.relationship_en;
                case ECLanguageConstants.LanguageFrench:
                    return relationship.relationship_fr != null ? relationship.relationship_fr : relationship.relationship_en;
                case ECLanguageConstants.LanguageRussian:
                    return relationship.relationship_ru != null ? relationship.relationship_ru : relationship.relationship_en;
                case ECLanguageConstants.LanguageSpanish:
                    return relationship.relationship_es != null ? relationship.relationship_es : relationship.relationship_en;
            }
            return relationship.relationship_en;
        }
        public List<EC.Models.ECModel.Relationship> getOtherRelationshipCulture(ReportModel reportModel, int companyId)
        {
            var allRelationships = reportModel.getCustomRelationshipCompany(companyId);

            foreach (var relation in allRelationships)
            {
                Relationship temp = new Relationship();
                temp.id = relation.id;
                temp.relationship_nm = relation.relationship_en;
                temp.relationship_nm = getCulturyRelationOther(relation);
                temp.relationship_nm_val = relation.relationship_en;
                relationshipViewModel.Add(temp);
            }


            return relationshipViewModel;
        }
    }
}