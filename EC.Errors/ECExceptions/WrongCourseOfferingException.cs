using EC.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// If the course offering is found at a wrong path - maybe not matching up with a link path.
    /// </summary>
    /// <example>
    /// If a student evaluation link and an offeringId are passed in to a service endpoint we expect 
    /// the offeringId to be found along the student evaluation link. If that is not the case this exception
    /// is thrown.
    /// </example>

    public class WrongCourseOfferingException: FaultableException<WrongCourseOfferingFault>
    {
        public override WrongCourseOfferingFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new WrongCourseOfferingFault(Message, reqPath, userInfo);
            f.OfferingId = this.OfferingId;
            f.OfferingPath = this.OfferingPath;
            return f;
        }

        public WrongCourseOfferingException (string message, Guid offeringId, string offeringPath, Exception innerException = null) : base(message, innerException)
        {
            OfferingId = offeringId;
            OfferingPath = OfferingPath;
        }

        public Guid OfferingId { get; set; }
        public string OfferingPath { get; set; }
    }

     /// <summary>
    /// If the course offering is found at a wrong path - maybe not matching up with a link path.
    /// </summary>
    /// <example>
    /// If a student evaluation link and an offeringId are passed in to a service endpoint we expect 
    /// the offeringId to be found along the student evaluation link. If that is not the case this fault
    /// is thrown.
    /// </example>

    [DataContract]
    public class WrongCourseOfferingFault : BasicFault
    {
        public WrongCourseOfferingFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid OfferingId { get; set; }
       
        [DataMember]
        public string OfferingPath { get; set; }
    }
}
