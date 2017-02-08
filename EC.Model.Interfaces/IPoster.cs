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
        string fileName { get; set; }
        int Id { get; set; }
        List<IPosterCategory> posterCategoryNames { get; set; }
        IPosterMessage posterMessage { get; set; }
        string imageName { get; set; }
        string imagePath { get; set; }
    }

    public interface IPosterCategory
    {
        int Id { get; set; }
        string posterCategoryName { get; set; }
    }

    public interface IPosterMessage
    {
        int Id { get; set; }
        string posterMessageName { get; set; }
    }
}
