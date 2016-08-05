using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using EC.Common;
using EC.Common.Util;
using EC.Framework.Data;

namespace EC.Business.Entities
{
    public interface IEntity
    {
        long Id
        {
            get;
        }

        Guid ObjectKey
        {
            get;
        }

        DataAction ObjectState
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the name of the entity type, this should return a constant from
        /// the TableNames file matching the actual database table name.
        /// </summary>
        /// <returns></returns>
        string GetEntityTypeName();

        /// <summary>
        /// The general EventType that this entity should raise. Could be "Unhandled".
        /// </summary>
        /// <returns></returns>
        List<ReturnProblem> Validate(long? actorId);
        void HandleSaveException(Exception ex);
    }
}
