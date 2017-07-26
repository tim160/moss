using System.Collections.Generic;
using System.Linq;


namespace EC.Common.Base
{
    public class HtmlDataHelper
    {
        public class SelectItem
        {
            public string Name;
            public string Value;

            public SelectItem() { }

            public SelectItem(string value, string name)
            {
                Value = value;
                Name = name;
            }
        }
        public class SelectViewModel
        {
            public List<SelectItem> Items;
            public string Selected;

            public SelectViewModel(List<SelectItem> items)
            {
                this.Items = items;
                Selected = "";
            }

            public SelectViewModel(string active, List<SelectItem> items)
            {
                Selected = active;
                this.Items = items;
            }
        }
        public delegate SelectItem Convertor<in T>(T item);
        public static SelectViewModel MakeSelect<T>(List<T> data, Convertor<T> make, string selected = "")
        {
            var items = data.Select(item => make(item)).ToList();
            return new SelectViewModel(selected, items);
        }


    }
}
