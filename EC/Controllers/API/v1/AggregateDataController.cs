using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EC.Common.Base;
using EC.Common.Util;
using EC.Common.Util.Filters;
using EC.Errors.CommonExceptions;
using EC.Models.API.v1.Company;
using EC.Services.API.v1.CompanyServices;
using log4net;

namespace EC.Controllers.API.v1
{
  [JwtAuthentication]
  [Authorize]
  [RoutePrefix("api/v1/aggregate")]
  public class AggregateDataController : BaseApiController
  {
    private readonly CompanyService _companyService;
    protected readonly ILog _logger;

    public class AggregateData
    {
      public int Name { get; set; }
      public int Quantity { get; set; }

      public decimal Percentage { get; set; }

    }

    [HttpGet]
    [Route("Departments")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> Departments()
    {
     // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();  
      return ApiOk(result);
    }

    [HttpGet]
    [Route("Locations")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> Locations()
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }

    [HttpGet]
    [Route("IncidentTypes")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> IncidentTypes()
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }
    [HttpGet]
    [Route("ReporterTypes")]
    [ResponseType(typeof(AggregateData))]
    public async Task<IHttpActionResult> ReporterTypes()
    {
      // _logger.Debug($"page={page}; pageSize={pageSize}");

      AggregateData result = new AggregateData();
      return ApiOk(result);
    }
  }
}