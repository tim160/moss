using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Model.Interfaces;

namespace EC.Model.Impl
{
    public class PosterItem : IPoster
    {
        public virtual Int32 Id { get; set; }
        public virtual string posterName { get; set; }
        public virtual string fileName { get; set; }

        public virtual List<IPosterCategory> posterCategoryNames { get; set; }
        public virtual IPosterMessage posterMessage { get; set; }
    }
    public class PosterCategory : IPosterCategory
    {
        public virtual Int32 Id { get; set; }
        public virtual string posterCategoryName { get; set; }
    }

    public class PosterMessage : IPosterMessage
    {
        public virtual Int32 Id { get; set; }
        public virtual string posterMessageName { get; set; }
    }
}
