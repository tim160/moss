using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Common.Util
{
    public class ListEntity : Attribute
    {
        public ListEntity(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public Type Type { get; set; }

        public string Name { get; set; }
    }
}