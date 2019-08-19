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
            relationshipViewModel = new List<ECModel.Relationship>();
            this.companyModel = companyModel;
        }

        public List<EC.Models.ECModel.Relationship> getRelationshipCulture()
        {
            var allRelationships = companyModel.getRelationships();

            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var relationship in allRelationships)
                    {
                        EC.Models.ECModel.Relationship temp = new ECModel.Relationship();
                        temp.id = relationship.id;
                        temp.relationship_nm = relationship.relationship_en;
                        relationshipViewModel.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var relationship in allRelationships)
                    {
                        EC.Models.ECModel.Relationship temp = new ECModel.Relationship();
                        temp.id = relationship.id;
                        temp.relationship_nm = relationship.relationship_es;
                        relationshipViewModel.Add(temp);
                    }
                    break;
            }
            return relationshipViewModel;
        }
        public List<EC.Models.ECModel.Relationship> getOtherRelationshipCulture(ReportModel reportModel, int companyId)
        {
            var allRelationships = reportModel.getCustomRelationshipCompany(companyId);

            switch (Localization.LocalizationGetter.Culture.Name)
            {
                case "en-US":
                    foreach (var relationship in allRelationships)
                    {
                        EC.Models.ECModel.Relationship temp = new ECModel.Relationship();
                        temp.id = relationship.id;
                        temp.relationship_nm = relationship.relationship_en;
                        relationshipViewModel.Add(temp);
                    }
                    break;

                case "es-ES":
                    foreach (var relationship in allRelationships)
                    {
                        EC.Models.ECModel.Relationship temp = new ECModel.Relationship();
                        temp.id = relationship.id;
                        if(relationship.relationship_es != null && relationship.relationship_es != "")
                        {
                            temp.relationship_nm = relationship.relationship_es;
                        } else
                        {
                            temp.relationship_nm = relationship.relationship_en;
                        }
                        relationshipViewModel.Add(temp);
                    }
                    break;
            }


            return relationshipViewModel;
        }
    }
}