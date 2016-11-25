using System;
using System.ServiceModel;
using System.Collections.Generic;
using EC.Service.DTO;
using EC.Errors;
using EC.Errors.ECExceptions;

namespace EC.Service.Interfaces
{
    [ServiceContract]
    public interface ICPMSServiceSecure
    {
        #region System Service Operations

        [OperationContract]
        int Ping(int sequenceNumber);

        #endregion

        #region User Queries

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        List<string> GetCurrentUserCapabilities(string path);

        #endregion

        #region Competency Service Operations

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        Guid? AddCompetencyCategory(string path, Guid? parentCategoryId, string name);

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        void DeleteCompetencyCategory(string path, Guid competencyCategoryId);

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        void DeleteCompetencyTemplate(string path, Guid competencyTemplateId);

        #endregion

        #region Training Service Operations

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        Guid? AddTrainingCategory(string path, Guid? parentCategoryId, string name);

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        void DeleteTrainingCategory(string path, Guid trainingCategoryId);

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        void DeleteTrainingTemplate(string path, Guid trainingTemplateId);

        #endregion
    }
}
