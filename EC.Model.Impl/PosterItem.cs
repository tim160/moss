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
        public virtual List<IPosterCategory> posterCategoryNames { get; set; }
    }
}
