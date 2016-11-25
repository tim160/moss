using System;
using System.Collections.Generic;
using System.ServiceModel;
using EC.Service.DTO;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// WCF Implementation for CPMS Admin Service
    /// </summary>
    
    [ServiceContract]
    public interface ICPMSServiceAdmin
    {
        #region User Queries

        [OperationContract]
        List<string> GetUserCapabilities(string path, Guid? userId = null);

        #endregion

        #region Competency Collection

        /// <summary>
        /// Add CompetemcyTemplateCollection for a NavPage
        /// </summary>
        /// <remarks>
        /// -  Requires System.Capability.CanCreateCompetencyTemplateCollection
        /// -  Root Category is created
        /// </remarks>
        /// <param name="path">string path to NavPage</param>
        /// <returns>Guid Id of Collection</returns>

        [OperationContract]
        Guid? AddCompetencyTemplateCollectionCmd(string path);

        /// <summary>
        /// Delete CompetencyTemplateCollection for a NavPage
        /// </summary>
        /// <remarks>
        /// -  System.Capability.CanDeleteCompetencyTemplateCollection
        /// -  Collection is not deleted but all child objects are recursively set with 'Orphaned' state
        /// </remarks>
        /// <param name="path">string path to NavPage</param>

        [OperationContract]
        void DeleteCompetencyTemplateCollectionCmd(string path);

        /// <summary>
        /// Get CompetencyTemplateCollection for a NavPage
        /// </summary>
        /// <remarks>
        /// - Capability System.Capability.CanViewCompetencyTemplateCollection
        /// - If detailLevel is null, defaults to:
        ///     CompetencyTemplateCollection.RootCategory
        ///     CompetencyTemplateCollection.RootCategory.CompetencyCategory.CompetencyCategory
        ///     CompetencyTemplateCollection.RootCategory.CompetencyCategory.CompetencyTemplate
        ///     CompetencyTemplateCollection.CompetencyCategory 
        ///     CompetencyTemplateCollection.CompetencyTemplate
        /// </remarks>
        /// <param name="path">string path to NavPage</param>
        /// <param name="detailLevel">specifies Model elements to serialize to DTO; see remarks for default</param>
        /// <returns>CompetencyTemplateCollectionDTO</returns>

        //////[OperationContract]
        //////CompetencyTemplateCollectionDTO CompetencyTemplateCollectionByPathQuery(string path, string detailLevel = null);

        #endregion

        #region Competency Category

        /// <summary>
        /// Add CompetenyCategory
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="parentCategoryId">null if adding to Root Category or id of parent Sub-Category</param>
        /// <param name="name">string name</param>
        /// <returns>Guid Id of Category</returns>

        [OperationContract]
        Guid? AddCompetencyCategoryCmd(string path, Guid? parentCategoryId, string name);

        /// <summary>
        /// Update CompetencyCategory
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="ccDto">CompetencyCategory DTO</param>

        ////////[OperationContract]
        ////////void UpdateCompetencyCategoryCmd(string path, CompetencyCategoryDTO ccDto);

        /// <summary>
        /// Delete CompetencyCategory
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="categoryId">id of CompetencyCategory</param>
        /// <returns></returns>

        [OperationContract]
        void DeleteCompetencyCategoryCmd(string path, Guid categoryId);

        /// <summary>
        /// Get CompetencyCategoryTree for a NavPage
        /// </summary>
        /// <remarks>
        /// - Capability System.Capability.CanViewCompetencyCategory
        /// - If detailLevel is null, defaults to:
        ///     CompetencyCategory.CompetencyCategory
        ///     CompetencyCategory.CompetencyTemplate CompetencyCategory.CompetencyTemplate.Tag
        ///     CompetencyCategory.CompetencyTemplate.CompetencyInstance CompetencyCategory.CompetencyTemplate.CompetencyInstance.Tag
        /// </remarks>
        /// <param name="path">string path to NavPage</param>
        /// <param name="detailLevel">specifies Model elements to serialize to DTO; see remarks for default</param>
        /// <returns>CompetencyCategoryDTO</returns>

        ////////[OperationContract]
        ////////CompetencyCategoryDTO CompetencyCategoryByPathQuery(string path, string detailLevel = null);

        #endregion

        #region Competency Template

        /// <summary>
        /// Add CompetencyTemplate
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="categoryId">Null if adding to Root Category or id of Parent Sub-Category</param>
        /// <param name="ctDto">CompetencyTemplate DTO</param>
        ///////////// <returns>Guid of newly created CompetencyTemplate</returns>

        //////////[OperationContract]
        //////////Guid? AddCompetencyTemplateCmd(string path, Guid? categoryId, CompetencyTemplateDTO ctDto);

        ///////////// <summary>
        ///////////// Update CompetencyTemplate
        ///////////// </summary>
        ///////////// <param name="path">string path to NavPage</param>
        ///////////// <param name="competencyTemplateDto">CompetencyTemplate DTO</param>

        //////////[OperationContract]
        //////////void UpdateCompetencyTemplateCmd(string path, CompetencyTemplateDTO competencyTemplateDto);

        /// <summary>
        /// Delete CompetencyTemplate
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="competencyTemplateId">id of CompetencyTemplate</param>

        [OperationContract]
        void DeleteCompetencyTemplateCmd(string path, Guid competencyTemplateId);

        /// <summary>
        /// Get CompetencyTemplate
        /// </summary>
        /// <remarks>
        /// - Capability System.Capability.CanViewCompetencyTemplate
        /// - If detailLevel is null, defaults to:
        ///     CompetencyTemplate.Tag CompetencyTemplate.CompetencyInstance CompetencyTemplate.CompetencyInstance.Tag
        /// </remarks>
        /// <param name="path">string Path to NavPage</param>
        /// <param name="competencyTemplateId">id of CompetencyTemplate</param>
        /// <param name="detailLevel">specifies Model elements to serialize to DTO; see remarks for default</param>

        ////////[OperationContract]
        ////////CompetencyTemplateDTO GetCompetencyTemplateCmd(string path, Guid competencyTemplateId, string detailLevel = null);

        /// <summary>
        /// Assign Tag to a CompetencyTemplate
        /// </summary>
        /// <param name="path">string Path to NavPage</param>
        /// <param name="competencyTemplateId">id of CompetencyTemplate</param>
        /// <param name="tagId">id of Tag</param>

        [OperationContract]
        void AssignCompetencyTemplateTagCmd(string path, Guid competencyTemplateId, Guid tagId);

        #endregion

        #region Competency Instance

        /// <summary>
        /// Assign Competency
        /// - If Active CompetencyInstance exists and new requiredBy is later, 
        ///   existing Instance date is updated, new Instance created and both are returned
        /// </summary>
        /// <remarks>
        /// Capability CAN_ASSIGN_COMPETENCYTEMPLATE
        /// </remarks>
        /// <param name="path">String path of NavPage</param>
        /// <param name="competencyTemplateId">Guid Id of CompetencyTemplate</param>
        /// <param name="ciDto">CompetencyInstance DTO</param>
        /// <returns>List of Guid ids of new CompetencyInstances</returns>

        //////////[OperationContract]
        //////////List<Guid> AssignCompetencyCmd(string path, Guid competencyTemplateId, CompetencyInstanceDTO ciDto);

        /// <summary>
        /// Update CompetencyInstance
        /// </summary>
        /// <remarks>
        /// Capability CAN_EDIT_COMPETENCYINSTANCE
        /// </remarks>
        /// <param name="path">string path to NavPage</param>
        /// <param name="competencyInstanceDto">CompetencyInstance DTO</param>

        //////////[OperationContract]
        //////////void UpdateCompetencyInstanceCmd(string path, CompetencyInstanceDTO competencyInstanceDto);

        /// <summary>
        /// Delete competeny Instance
        /// </summary>
        /// <remarks>
        /// Capability CAN_DELETE_COMPETENCYINSTANCE
        /// </remarks>
        /// <param name="path">string path to NavPage</param>
        /// <param name="competencyInstanceId">id of CompetencyInstance</param>

        [OperationContract]
        void DeleteCompetencyInstanceCmd(string path, Guid competencyInstanceId);

        ///////////// <summary>
        ///////////// Get CompetencyInstance
        ///////////// </summary>
        ///////////// <remarks>
        ///////////// - Capability System.Capability.CanViewCompetencyInstance
        ///////////// - If detailLevel is null, defaults to:
        /////////////     CompetencyInstance.Tag
        ///////////// </remarks>
        ///////////// <param name="path">string Path to NavPage</param>
        ///////////// <param name="competencyInstanceId">id of CompetencyInstance</param>
        ///////////// <param name="detailLevel">specifies Model elements to serialize to DTO; see remarks for default</param>

        //////////[OperationContract]
        //////////CompetencyInstanceDTO GetCompetencyInstanceCmd(string path, Guid competencyInstanceId, string detailLevel = null);

        /// <summary>
        /// Mark Competency Complete (Set InstanceStatus to 'Held')
        /// </summary>
        /// <remarks>
        /// Capability CAN_CHANGE_COMPETENCYSTATE
        /// </remarks>
        /// <param name="path">string path of NavPage</param>
        /// <param name="ciId">CompetencyInstance Guid</param>

        [OperationContract]
        void MarkCompetencyComplete(string path, Guid ciId);

        /// <summary>
        /// Mark Competency Incomplete (Set InstanceStatus to 'Assigned')
        /// </summary>
        /// <remarks>
        /// Capability CAN_CHANGE_COMPETENCYSTATE
        /// </remarks>
        /// <param name="path">string path of NavPage</param>
        /// <param name="ciId">CompetencyInstance Guid</param>

        [OperationContract]
        void MarkCompetencyAssigned(string path, Guid ciId);

        /// <summary>
        /// Mark Competency Incomplete (Set InstanceStatus to 'InProgress')
        /// </summary>
        /// <remarks>
        /// Capability CAN_CHANGE_COMPETENCYSTATE
        /// </remarks>
        /// <param name="path">string path of NavPage</param>
        /// <param name="ciId">CompetencyInstance Guid</param>

        [OperationContract]
        void MarkCompetencyInProgress(string path, Guid ciId);

        /// <summary>
        /// Mark Competency Incomplete (Set InstanceStatus to 'Failed')
        /// </summary>
        /// <remarks>
        /// Capability CAN_CHANGE_COMPETENCYSTATE
        /// </remarks>
        /// <param name="path">string path of NavPage</param>
        /// <param name="ciId">CompetencyInstance Guid</param>

        [OperationContract]
        void MarkCompetencyFailed(string path, Guid ciId);

        /// <summary>
        /// Mark Competency Incomplete (Set InstanceStatus to 'Expired')
        /// </summary>
        /// <remarks>
        /// Capability CAN_CHANGE_COMPETENCYSTATE
        /// </remarks>
        /// <param name="path">string path of NavPage</param>
        /// <param name="ciId">CompetencyInstance Guid</param>

        [OperationContract]
        void MarkCompetencyExpired(string path, Guid ciId);

        #endregion

        #region Training Collection

        /// <summary>
        /// Add TrainingTemplateCollection for a NavPage
        /// </summary>
        /// <remarks>
        /// -  Requires System.Capability.CanCreateTrainingTemplateCollection
        /// -  Root Category is created
        /// </remarks>
        /// <param name="path">string Path to NavPage</param>
        /// <returns>Guid Id of Collection</returns>

        [OperationContract]
        Guid? AddTrainingTemplateCollectionCmd(string path);

        /// <summary>
        /// Delete TrainingTemplateCollection for a NavPage
        /// </summary>
        /// <remarks>
        /// -  Requires System.Capability.CanDeleteTrainingTemplateCollection
        /// -  Collection is not deleted but all child objects are recursively set with 'Orphaned' state
        /// </remarks>
        /// <param name="path">string path to NavPage</param>

        [OperationContract]
        void DeleteTrainingTemplateCollectionCmd(string path);



        #endregion

        #region Training Category

        /// <summary>
        /// Add TrainingCategory
        /// </summary>
        /// <param name="path">string Path to NavPage</param>
        /// <param name="parentCategoryId">null if adding to Root or id of Parent for Sub-Category</param>
        /// <param name="name">string name of Category</param>
        /// <returns>Guid Id of Category</returns>

        [OperationContract]
        Guid? AddTrainingCategoryCmd(string path, Guid? parentCategoryId, string name);



        /// <summary>
        /// Delete TrainingCategory
        /// </summary>
        /// <param name="path">string Path to NavPage</param>
        /// <param name="trainingCategoryId">id of TrainingCategory to delete</param>
    
        [OperationContract]
        void DeleteTrainingCategoryCmd(string path, Guid parentCategoryId);

        #endregion

        #region Training Template

        /// <summary>
        /// Delete TrainingTemplate
        /// </summary>
        /// <param name="path">string Path to NavPage</param>
        /// <param name="trainingTemplateId">id of TrainingTemplate</param>

        [OperationContract]
        void DeleteTrainingTemplateCmd(string path, Guid trainingTemplateId);


        /// <summary>
        /// Bind TrainingTemplate to a CompetencyTemplate
        /// </summary>
        /// <param name="path">string Path to NavPage</param>
        /// <param name="trainingTemplateId">id of TrainingTemplate</param>
        /// <param name="competencyTemplateId">id of CompetencyTemplate</param>
        
        [OperationContract]
        void BindTrainingTemplateToCompetencyTemplateCmd(string path, Guid trainingTemplateId, Guid competencyTemplateId);

        #endregion

        #region Training Instance

        /// <summary>
        /// Delete TrainingInstance
        /// </summary>
        /// <remarks>
        /// Capability CAN_DELETE_TRAININGINSTANCE
        /// </remarks>
        /// <param name="path">string Path to NavPage</param>
        /// <param name="trainingInstanceId">id of TrainingInstance</param>

        [OperationContract]
        void DeleteTrainingInstanceCmd(string path, Guid trainingInstanceId);

        #endregion
    }
}
