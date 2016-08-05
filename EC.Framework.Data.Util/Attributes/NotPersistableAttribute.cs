using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Framework.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class NotPersistableAttribute : Attribute
    {
    }
}
