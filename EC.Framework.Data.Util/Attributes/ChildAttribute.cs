using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Framework.Data
{
    [AttributeUsage(AttributeTargets.Property,
         AllowMultiple = false)]
    public sealed class ChildAttribute : Attribute
    {
        public bool UseIdAsPrimaryKey
        {
            get;
            set;
        }

        public ChildAttribute(bool useIdAsPrimaryKey = false)
        {
            UseIdAsPrimaryKey = useIdAsPrimaryKey;
        }
    }
}