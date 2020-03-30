using System.Collections.Generic;
using EC.Services.API.v1.CompanyServices;

namespace EC.Models.API.v1.Client
{
    public class ClientCompanyDepartmentAggregateData
    {
        public string CompanyName { get; set; }
        public List<AggregateData> DepartmentTable { get; set; }
    }

    public class ClientCompanyLocationAggregateData
    {
        public string CompanyName { get; set; }
        public List<AggregateData> LocationTable { get; set; }
    }

    public class ClientCompanyIncidentAggregateData
    {
        public string CompanyName { get; set; }
        public List<AggregateData> SecondaryTypeTable { get; set; }
    }

    public class ClientCompanyReporterTypeAggregateData
    {
        public string CompanyName { get; set; }
        public List<AggregateData> RelationTable { get; set; }
    }
}