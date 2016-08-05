using System;
using System.Collections.Generic;
using System.Text;
using EC.Common.Util;

namespace EC.Business.API.Interfaces
{
    public interface IEmailManager
    {
        ActionResult Send(string to, string messageSubject, string messageBody);
    }
}
