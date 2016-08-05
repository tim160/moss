using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using EC.Framework.Data;

namespace EC.Business.Entities
{
    public interface IBaseEntity
    {
        #region Base
        Guid ObjectKey
        {
            get; 
            set;
        }

        DateTime Timestamp
        {
            get;
            set;
        }

        Int64 Id
        {
            get; 
            set;
        }

        DataAction ObjectState
        {
            get; 
            set; 
        }
        #endregion Base
    }
}
