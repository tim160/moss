using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Business
{
    [Flags]
    public enum PasswordRules
    {
        None = 0,
        Digit = 1,
        UpperCase = 2,
        LowerCase = 4,
        Special = 8,
        All = 15
    }
}
