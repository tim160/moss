using System.Collections.Generic;

namespace EC.Models.API.v1.Company
{
    public class CompanyViewModel
  {
        public int Total { get; set; }
        public List<CompanyModel> Items { get; set; }
    }
}