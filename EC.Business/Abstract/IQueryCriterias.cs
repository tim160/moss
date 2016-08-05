using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EC.Business.Entities;

namespace EC.Business.Actions
{
    public interface IQueryCriterias
    {
        string GetNameFromKey(Guid key);
        FilterQueryCriteria GetFilterQueryCriteria(Guid objectKey);
        FilterQueryCriteria GetFilterQueryCriteria(string name);
        Tuple<string, string, string>[] GetFilterQueryCriteria(long userId, long[] bgIds);
        QueryCriteria GetQueryCriteria(Guid objectKey);

    }
}
