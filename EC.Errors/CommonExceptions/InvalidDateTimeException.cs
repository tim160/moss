using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// If the DateTime is invalid (i.e. it is before the SQL min date, which can't be stored in SQL).
    /// </summary>

    public class InvalidDateTimeException : FaultableException<InvalidDateTimeFault>
    {
        public override InvalidDateTimeFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new InvalidDateTimeFault(Message, path, userInfo);
            return f;
        }

        public InvalidDateTimeException(string msg, Exception innerException = null) : base(msg, innerException)
        {
        }
    }

    /// <summary>
    /// If the DateTime is invalid (i.e. it is before the SQL min date, which can't be stored in SQL).
    /// </summary>

    [DataContract]
    public class InvalidDateTimeFault : BasicFault
    {
        public InvalidDateTimeFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
