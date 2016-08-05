using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Controllers.Utils
{
    public class ListEntety : Attribute
    {
        public ListEntety(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public Type Type { get; set; }

        public string Name { get; set; }
    }
}