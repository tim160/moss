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
    /// Exception if the system might be in an inconsistent state.
    /// </summary>

    public class DataConsistencyException : FaultableException<DataConsistencyFault>
    {
        public override DataConsistencyFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new DataConsistencyFault(Message, reqPath, userInfo);
            return f;
        }

        public DataConsistencyException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Thrown if there is an inconsistency in the DB or inconsistency was about to be saved into the DB.
    /// </summary>

    [DataContract]
    public class DataConsistencyFault : BasicFault
    {
        public DataConsistencyFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }
    }
}
