using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// OrgProfileValue DTO.
    /// </summary>

    [DataContract]
    [KnownType(typeof(OrgProfileValueString))]
    [KnownType(typeof(OrgProfileValueInteger))]
    [KnownType(typeof(OrgProfileValueSingleChoiceString))]
    public abstract class OrgProfileValue
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>

        [DataMember]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>

        [DataMember]
        public OrgProfileField Field { get; set; }

        /// <summary>
        /// Gets or sets the owner of field value
        /// </summary>
        /// <value>
        /// The Owner.
        /// </value>

        [DataMember]
        public User Owner { get; set; }

        /// <summary>
        /// Returns the String Value of the Value of the current profile field
        /// Must be implemented by each Org Profile Value Type
        /// </summary>
        /// <returns></returns>
        public abstract string GetStringValue();

    }

    /// <summary>
    /// OrgProfileValueString DTO with a non-nullable string for its value.
    /// </summary>

    [DataContract]
    public class OrgProfileValueString : OrgProfileValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>

        [DataMember]
        public string Value { get; set; }

        public override string GetStringValue()
        {
            return Value;
        }
    }

    /// <summary>
    /// OrgProfileValueInteger DTO with a nullable integer for its value.
    /// </summary>

    [DataContract]
    public class OrgProfileValueInteger : OrgProfileValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>

        [DataMember]
        public int? Value { get; set; }

        public override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString() : null;
        }
    }

    /// <summary>
    /// OrgProfileValueSingleChoiceString DTO with a non-nullable string for its value, which must be from a list of choices.
    /// </summary>

    [DataContract]
    public class OrgProfileValueSingleChoiceString : OrgProfileValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>

        [DataMember]
        public string Value { get; set; }

        public override string GetStringValue()
        {
            return Value;
        }
    }
}
