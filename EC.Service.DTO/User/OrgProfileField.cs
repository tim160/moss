using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// OrgProfileField DTO.
    /// </summary>

    [DataContract]
    [KnownType(typeof(OrgProfileFieldString))]
    [KnownType(typeof(OrgProfileFieldInteger))]
    [KnownType(typeof(OrgProfileFieldSingleChoiceString))]
    public abstract class OrgProfileField
    {
        /// <summary>
        /// Gets or sets the identifier. If null, it wont get update
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>

        [DataMember]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>

        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user editable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user editable]; otherwise, <c>false</c>.
        /// </value>

        [DataMember]
        public bool UserEditable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user visible]; otherwise, <c>false</c>.
        /// </value>

        [DataMember]
        public bool UserVisible { get; set; }

        ///////// <summary>
        ///////// Gets or sets the organization.
        ///////// </summary>
        ///////// <value>
        ///////// The organization.
        ///////// </value>

        //////[DataMember]
        //////public NavPage Organization { get; set; }
    }

    /// <summary>
    /// OrgProfileFieldString DTO with a non-nullable string for its values.
    /// </summary>

    [DataContract]
    public class OrgProfileFieldString : OrgProfileField
    {
    }

    /// <summary>
    /// OrgProfileFieldInteger DTO with a nullable integer for its values.
    /// </summary>

    [DataContract]
    public class OrgProfileFieldInteger : OrgProfileField
    {
    }

    /// <summary>
    /// OrgProfileFieldSingleChoiceString DTO with a non-nullable string for its values, which must be one of the choices.
    /// </summary>

    [DataContract]
    public class OrgProfileFieldSingleChoiceString : OrgProfileField
    {
        [DataMember]
        public List<string> Choices { get; set; }
    }
}
