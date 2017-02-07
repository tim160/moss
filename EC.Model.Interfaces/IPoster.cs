using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Model.Interfaces
{
    public interface IPoster
    {
        string posterName { get; set; }
        int Id { get; set; }
        List<IPosterCategory> posterCategoryNames { get; set; }
    }
}
