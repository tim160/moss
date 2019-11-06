using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using EC.Controllers.Utils;
using EC.Models.Database;
using EC.Common.Util;

namespace EC.Controllers.ViewModel
{
    public class BaseViewModel
    {
        public ECEntities db = new ECEntities();
        protected Type type;
        public HttpFileCollectionBase files;
        public BaseViewModel()
        {
            type = GetType();
        }

        public void Process(NameValueCollection form, HttpFileCollectionBase files)
        {
            this.files = files;
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ListEntity), false)).ToList();
            foreach (var property in properties)
            {
                var attr = ((ListEntity[])property.GetCustomAttributes(typeof(ListEntity), false))[0];
                var name = attr.Name;
                //namesFile.Add(name);
                var itemType = attr.Type;
                var list = new List<object>();
                int count;
                if (form[name + "Num"] != "" && int.TryParse(form[name + "Num"], out count))
                {
                    for (var i = 1; i <= count; i++)
                    {
                        if (form[name + i] != null) list.Add(form[name + i]);
                    }

                    if (itemType == typeof(string))
                    {
                        property.SetValue(this, list.OfType<string>().ToList());

                    }
                    else if (itemType == typeof(int))
                    {
                        property.SetValue(this, list.OfType<int>().ToList());
                    }
                }
            }
        }
    }
}