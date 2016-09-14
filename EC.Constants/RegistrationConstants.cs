using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// Reasons why a user has been deregistered from an offering.
    /// </summary>

    [DataContract]
    public enum DeRegistrationReasonsEnum
    {
        /// <summary>
        /// This is the default if the user still is registered in the course offering.
        /// </summary>
        
        [EnumMember]
        None = 0,

        /// <summary>
        /// If the user has been directly deleted from an course offering.
        /// </summary>

        [EnumMember]
        DeletedByUser = 1,

        /// <summary>
        /// If the user has been removed from the organization the registration was located in.
        /// </summary>

        [EnumMember]
        RemovedFromOrg = 2,

        /// <summary>
        /// If the user has been de-registered by the RTA synchronization process.
        /// </summary>

        [EnumMember]
        DeletedByRTASync = 3
    }

    /// <summary>
    /// These are the state a Registration can be in 
    /// </summary>

    [DataContract]
    public enum RegistrationState
    {
        [EnumMember]
        Inactive = 0,
        
        [EnumMember]
        Active = 1,
        
        [EnumMember]
        Deleted = 2,
        
        [EnumMember]
        Completed = 3,
        
        [EnumMember]
        Historical = 4
    }

    /// <summary>
    /// Columns to sort the registration by.
    /// </summary>

    [DataContract]
    public enum RegistrationSortTypesEnum
    {
        [Description("UserName")]
        [EnumMember]
        UserName = 0,

        [Description("FirstName")]
        [EnumMember]
        FirstName = 1,

        [Description("LastName")]
        [EnumMember]
        LastName = 2,

        [Description("RegistrationDate")]
        [EnumMember]
        RegistrationDate = 3,

        [Description("RegistrationState")]
        [EnumMember]
        RegistrationState = 4,

        [Description("OfferingShortId")]
        [EnumMember]
        OfferingShortId = 5
    }
}
