using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EC.Business.Entities;
using EC.Common.Util;

namespace EC.Business.Actions
{
    public interface IEmailer
    {
        ActionResultExtended Send(List<string> to, List<string> CC, string messageSubject, string messageBody);
        ActionResultExtended Send(string to, string messageSubject, string messageBody);
    }
}
