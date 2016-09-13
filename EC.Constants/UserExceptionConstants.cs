using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// Type of error for a user fault.
    /// </summary>

    [DataContract]
    public enum UserFaultEnum
    {
        /// <summary>
        /// No error for this user occurred.
        /// </summary>

        [EnumMember]
        None = 1,

        /// <summary>
        /// If the user does not exist in the system.
        /// </summary>

        [EnumMember]
        DoesNotExist = 2,

        /// <summary>
        /// If the user is already registered for the course.
        /// </summary>

        [EnumMember]
        AlreadyRegistered = 4,

        /// <summary>
        /// If the user is already registered in an overlapping course.
        /// </summary>

        [EnumMember]
        OverlappingRegistration = 8,

        /// <summary>
        /// If the user is already an instructor for the course.
        /// </summary>

        [EnumMember]
        AlreadyInstructor = 16,

        /// <summary>
        /// Any other error.
        /// </summary>

        [EnumMember]
        Other = 32
    }
}
