using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;

namespace EC.Models
{
    public class CountryModel : BaseModel
    {
        public static readonly CountryModel inst = new CountryModel();
 

        protected CountryModel()
        {

        }

        public List<country> Countries()
        {
            return db.country.OrderBy(item=>item.id).ToList();
        }

        public List<company_location> Locations()
        {
            //!!!!! check ---  List<company_location> Locations(int companyId, int? status_id)
            return db.company_location.ToList();
        }
        public country loadById(int idCountry)
        {
            return db.country.Where(item => item.id == idCountry).FirstOrDefault();
        }
    }
}