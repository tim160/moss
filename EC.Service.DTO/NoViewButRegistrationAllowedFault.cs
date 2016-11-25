using EC.Errors;
using EC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Service.DTO
{
    /// <summary>
    /// This exception is thrown if a user can't view an index page, but he/she would be allowed
    /// to register to a course. Also include all active course offerings in case we want to 
    /// register right away.
    /// </summary>

    [DataContract]
    public class NoViewButRegistrationAllowedFault : BasicFault
    {
        /// <summary>
        /// Course path (which is an index page).
        /// </summary>

        [DataMember]
        public string CoursePath { get; set; }

        /// <summary>
        /// All active course offerings for the course to which the user is able to register for.
        /// </summary>

        //////[DataMember]
        //////public List<CourseOffering> ActiveCourseOfferings { get; set; }
    }
}
