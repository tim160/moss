using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Audit actions that are common to all audits for model objects.
    /// </summary>
    
    public enum ModelAuditActions
    {
        Created = 0
    }

    /// <summary>
    /// These are the subtypes of the tracking audit.
    /// </summary>
    
    public enum TrackingAuditActions
    {
        AccessedPage = 0,
        AccessedSimple = 1
    }

    /// <summary>
    /// These are the subtypes of the case instance audit.
    /// </summary>
    
    public enum CaseInstanceAuditActions
    {
        None = 0,
        CreateInitial = 1
    }

    /// <summary>
    /// These are the subtypes of the session audit. Marked as DataContract so that they
    /// can be used in the model and the service layer.
    /// </summary>
    
    [DataContract]
    public enum SessionAuditActions
    {
        [Description("BeginSession")]
        [EnumMember]
        BeginSession = 0,
       
        [Description("EndSession")]
        [EnumMember]
        EndSession = 1,
       
        [Description("FailedLogin")]
        [EnumMember]
        FailedLogin = 2
    }

    /// <summary>
    /// 
    /// </summary>
    
    [DataContract]
    public enum CaseAdministratorAuditStateEnum
    {
        [Description("Added")]
        [EnumMember]
        Added = 1,
        
        [Description("Removed")]
        [EnumMember]
        Removed = 2
    }


    /// <summary>
    /// Types of RelationAudit changes
    /// </summary>

    public enum RelationAuditActions
    { 
        Add, 
        Remove 
    };
}
