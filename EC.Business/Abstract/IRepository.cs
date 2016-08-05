using System;
using System.Collections.Generic;
using System.Text;
using EC.Framework.Data;

namespace EC.Business.Abstract
{
    public interface IRepository<T>
    {
        T GetEntityByKey(Guid key);
        T[] GetEntities(FilterCriteria filterCriteria);
        T[] GetEntities(FilterCriteria filterCriteria, OrderCriteria orderCriteria);
        void SaveEntities(T[] entities);
        void DeepSaveEntities(T[] entities);
    }
}
