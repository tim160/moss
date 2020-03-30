using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EC.Common.Base;
using EC.Common.Util;
using EC.Constants;
using EC.Errors.CommonExceptions;
using EC.Models.API.v1.Company;
using EC.Models.API.v1.User;
using EC.Models.API.v1.Client;
using EC.Models.Database;
using EC.Services.API.v1.CompanyServices;
using EC.Services.API.v1.UserService;
using EC.Services.API.v1.ClientService;

using TestApi.Utils;


namespace TestApi.Controllers
{
    [RoutePrefix("api/v1/companies")]
    [CustomAuthorization]
    public class CompanyController : BaseApiController
    {
        private readonly ClientService _clientService;

        private readonly CompanyService _companyService;
        private readonly UserService _userService;

        public CompanyController()
        {
            _companyService = new CompanyService();
            _userService = new UserService();
            _clientService = new ClientService();
        }

        [HttpGet]
        [Route("{id}/analytics/departments")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsDepartments(int id, string startDate = "", string endDate = "")
        {
            var departmentsAnalytics = await _companyService.GetCompanyDepartmentsAnalytics(id, startDate, endDate);

            var result = new DepartmentAnalyticViewModel()
            {
                DepartmentTable = departmentsAnalytics
            };

            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/locations")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsLocations(int id, string startDate = "", string endDate = "")
        {
            var locationsAnalytics = await _companyService.GetCompanyLocationsAnalytics(id, startDate, endDate);

            var result = new LocationAnalyticViewModel()
            {
                LocationTable = locationsAnalytics
            };

            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/incidentTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsIncidentTypes(int id, string startDate = "", string endDate = "")
        {
            var incidentsAnalytics = await _companyService.GetCompanyIncidentsAnalytics(id, startDate, endDate);

            var result = new IncidentAnalyticViewModel()
            {
                SecondaryTypeTable = incidentsAnalytics
            };

            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/reporterTypes")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsReporterTypes(int id, string startDate = "", string endDate = "")
        {
            var reporterTypesAnalytics = await _companyService.GetCompanyReporterTypeAnalytics(id, startDate, endDate);

            var result = new ReporterTypeAnalyticViewModel()
            {
                RelationTable = reporterTypesAnalytics
            };

            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/behavioralFactors")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsBehavioralFactors(string startDate, string endDate)
        {
            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/analytics/externalInfluences")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsExternalInfluences(string startDate, string endDate)
        {
            AggregateData result = new AggregateData();
            return ApiOk(result);
        }
        [HttpGet]
        [Route("{id}/analytics/organizationalInfluences")]
        [ResponseType(typeof(List<AggregateData>))]
        public async Task<IHttpActionResult> AnalyticsOrganizationalInfluences(string startDate, string endDate)
        {
            AggregateData result = new AggregateData();
            return ApiOk(result);
        }

        [HttpGet]
        [Route("{id}/users")]
        [ResponseType(typeof(List<UserModel>))]
        public async Task<IHttpActionResult> GetUsersList(string id)
        {
          if (string.IsNullOrEmpty(id))
            return ApiBadRequest(ModelState);

          var idFromDb = await _companyService.GetInternalIDfromExternal(id);
          if (idFromDb == 0)
          {
            return ApiNotFound("Company not found.");
          }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            List<UserModel> result = await _userService.GetUsersByCompanyId(idFromDb, id);

            /*
            Expression<Func<user, bool>> filterApp = u => new[] { ECLevelConstants.level_ec_mediator, ECLevelConstants.level_escalation_mediator, ECLevelConstants.level_supervising_mediator }.Contains(u.role_id);
            PagedList<UserModel> result = await _userService
                .GetPagedAsync(page, pageSize, filterApp)
                .ConfigureAwait(false);
            var statusModel = new EC.Models.ReadStatusModel();
            result.Items.ForEach(entity =>
            {
                entity.usersUnreadEntities = statusModel.GetUserUnreadEntitiesNumbers(entity.id);
            });
            */
      return ApiOk(result);
        }

        [HttpPatch]
        [Route("{id}/deactivate")]
        public async Task<IHttpActionResult> CompanyDeactivate(string id)
        {
            if (string.IsNullOrEmpty(id))
              return ApiBadRequest(ModelState);

            var idFromDb = await _companyService.GetInternalIDfromExternal(id);
            if (idFromDb == 0)
            {
              return ApiNotFound("Company not found.");
            }

            var company = await DB.company.FirstOrDefaultAsync(c => c.id == idFromDb);
            if (company != null)
            {
                company.status_id = ECStatusConstants.Inactive_Value;
                await DB.SaveChangesAsync();
                return ApiOk();
            }

            return ApiNotFound();
        }

        [HttpPatch]
        [Route("{id}/activate")]
        public async Task<IHttpActionResult> CompanyActivate(string id)
        {
            if (string.IsNullOrEmpty(id))
              return ApiBadRequest(ModelState);

            var idFromDb = await _companyService.GetInternalIDfromExternal(id);
            if (idFromDb == 0)
            {
              return ApiNotFound("Company not found.");
            }

          var company = await DB.company.FirstOrDefaultAsync(c => c.id == idFromDb);
          if (company != null)
          {
            company.status_id = ECStatusConstants.Active_Value;
            await DB.SaveChangesAsync();
            return ApiOk();
          }

          return ApiNotFound();
        }

        //[HttpDelete]
        //[Route("{id}")]
        //public async Task<IHttpActionResult> Delete(string id)
        //{
        //    if (String.IsNullOrEmpty(id))
        //    {
        //        ModelState.AddModelError(nameof(id), "Company ID required.");
        //    }

        //    int idFromDb = DB.company.Where(company => company.partner_api_id.Equals(id)).Select(company => company.id).FirstOrDefault();
        //    if (idFromDb == 0)
        //    {
        //        ModelState.AddModelError(nameof(id), "Company not found.");
        //    }

        //    try
        //    {
        //        await _companyService
        //            .DeleteAsync(idFromDb)
        //            .ConfigureAwait(false);
        //    }
        //    catch (NotFoundException exception)
        //    {
        //        return ApiNotFound(exception.Message);
        //    }

        //    return ApiOk();
        //}

        //[HttpDelete]
        //[Route("internal/{id}")]
        //private async Task<IHttpActionResult> Delete(int id)
        //{
        //    if (id == 0)
        //    {
        //        ModelState.AddModelError(nameof(id), "Company ID required.");
        //    }

        //    try
        //    {
        //        await _companyService
        //            .DeleteAsync(id)
        //            .ConfigureAwait(false);
        //    }
        //    catch (NotFoundException exception)
        //    {
        //        return ApiNotFound(exception.Message);
        //    }

        //    return ApiOk();
        //}

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(string id, UpdateCompanyModel updateCompanyModel)
        {

            if (updateCompanyModel == null)
                ModelState.AddModelError(nameof(updateCompanyModel), "Company data required.");
 

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

            if (!ModelState.IsValid)
              return ApiBadRequest(ModelState);

            if (string.IsNullOrEmpty(id))
              return ApiBadRequest(ModelState);

            var idFromDb = await _companyService.GetInternalIDfromExternal(id);
            var idClientFromDb = await _clientService.GetInternalIDfromExternal(updateCompanyModel.PartnerClientId);

            if (idFromDb == 0)
            {
              return ApiNotFound("Company not found.");
            }
            if (idClientFromDb == 0)
            {
              return ApiNotFound("Client not found.");
            }

      try
      {
        var company = DB.company.FirstOrDefault(c => c.id == idFromDb);


        if (company != null)
        {
          company.company_nm = updateCompanyModel.CompanyName;
          //   company.partner_api_id = updateCompanyModel.PartnerCompanyId;  
          company.client_id = idClientFromDb;
          company.employee_quantity = updateCompanyModel.EmployeeQuantity;
          company.path_en = updateCompanyModel.CustomLogoPath;
          /// company.opt = updateCompanyModel.OptinCaseAnalytics;


          await DB.SaveChangesAsync();


          return ApiOk();
        }
      }
      catch (NotFoundException exception)
      { return ApiNotFound(exception.Message); }
        ////   
        ////}

        ////try
        ////{
        ////    await _companyService
        ////        .UpdateAsync(updateCompanyModel, idFromDb)
        ////        .ConfigureAwait(false);
        ////}
        ////catch (NotFoundException exception)
        ////{
        ////    return ApiNotFound(exception.Message);
        ////}

        return ApiOk();
        }

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Create(List<CreateCompanyModel> createCompanyModel)
        {
            if (createCompanyModel == null || !createCompanyModel.Any())
                ModelState.AddModelError(nameof(createCompanyModel), "Company data required.");

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

            try
            {

                foreach (var t in createCompanyModel)
                {
                  if (string.IsNullOrWhiteSpace(t.PartnerClientId))
                    return ApiBadRequest("PartnerClientId required.");
                  if (string.IsNullOrWhiteSpace(t.PartnerCompanyId))
                    return ApiBadRequest("PartnerCompanyId required.");
                }

                foreach (var c in createCompanyModel)
                {

                        var idFromDb = await _clientService.GetInternalIDfromExternal(c.PartnerClientId);
                        if (idFromDb == 0)
                        {
                          return ApiNotFound("Company not found.");
                        }

                        var companyId = (await _companyService
                        .CreateAsync(c, false, idFromDb)
                        .ConfigureAwait(false)).id;
                    c.Users.ForEach(u => u.PartnerCompanyId = companyId.ToString());
                }

                foreach (var c in createCompanyModel)
                {
                    await _userService
                        .CreateAsync(c.Users)
                        .ConfigureAwait(false);
                }

                return ApiCreated(createCompanyModel);

            }
            catch (AggregateException exception)
            {
                return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(CompanyModel))]
        public async Task<IHttpActionResult> GetCompany(string id)
        {
      // to do - check how it is done in client and do in the same way
            if (string.IsNullOrEmpty(id))
              return ApiBadRequest(ModelState);

            var idFromDb = await _companyService.GetInternalIDfromExternal(id);
            if (idFromDb == 0)
            {
              return ApiNotFound("Company not found.");
            }
          var result = await _companyService.GetCompanyById(idFromDb);
         // var result = await DB.company.FirstOrDefaultAsync(u => u.partner_api_id == id);

            if (result != null)
                return ApiOk(result);

            return ApiNotFound();
        }
    }
}
