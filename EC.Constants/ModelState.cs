using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// These are the basic lifestyle states that a model object can be in.
    /// </summary>
    
    public enum ModelState
    {
        Active = 0,

        Deleted = 1,

        Orphaned = 2
    }

    [DataContract]
    public enum OrgProfileFieldState
    {
        [EnumMember]
        Active = ModelState.Active,

        [EnumMember]
        Deleted = ModelState.Deleted,

        [EnumMember]
        Orphaned = ModelState.Orphaned
    }

    [DataContract]
    public enum OrgProfileValueState
    {
        [EnumMember]
        Active = ModelState.Active,

        [EnumMember]
        Deleted = ModelState.Deleted,

        [EnumMember]
        Orphaned = ModelState.Orphaned
    }
}
