using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Business.Entities
{
    public class DatabaseConstraintException : Exception
    {
        //public WorkOrder Order
        //{
        //    get
        //    {
        //        return Data["WorkOrder"] as WorkOrder;  
        //    }
        //}
        public DatabaseConstraintException() { }
        public DatabaseConstraintException(string message) : base(message) { }
        //public DatabaseConstraintException(string message, WorkOrder order)
        //    : base(message)
        //{
        //    Data.Add("WorkOrder", order);
        //}
        public DatabaseConstraintException(string message, Exception inner) : base(message, inner) { }
        public DatabaseConstraintException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
