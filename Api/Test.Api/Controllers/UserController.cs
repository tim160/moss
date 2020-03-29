using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EC.Errors.CommonExceptions;
using EC.Models.API.v1.User;
using EC.Services.API.v1.UserService;
using EC.Services.API.v1.CompanyServices;

using TestApi.Utils;
using EC.Constants;

namespace TestApi.Controllers
{
    [RoutePrefix("api/v1/users")]
    [CustomAuthorization]
    public class UserController : BaseApiController
    {
        private readonly UserService _userService;
        private readonly CompanyService _companyService;



    public UserController()
    {
            _userService = new UserService();
          _companyService = new CompanyService();

    }

    [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(UserModel))]
        public async Task<IHttpActionResult> GetUser(string id)
        {

          if (string.IsNullOrEmpty(id))
            return ApiBadRequest(ModelState);

          var idFromDb = await _userService.GetInternalIDfromExternal(id);
          if (idFromDb == 0)
          {
            return ApiNotFound("User not found.");
          }


          //var result = await DB.user.FirstOrDefaultAsync(u => u.id == idFromDb);
            var result = await _userService.GetUserById(idFromDb);
      if (result != null)
      {
       // result.usersUnreadEntities = new TestApi.Controllers.usersUnreadEntitiesNumberViewModel { unreadMessages = 0, unreadNewReports = 0, unreadTasks = 0 }
        return ApiOk(result);
      }

            return ApiNotFound();
        }

        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Create(List<CreateUserModel> createUserModel)
        {
            if (createUserModel == null || !createUserModel.Any())
                ModelState.AddModelError(nameof(createUserModel), "User data required.");

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

            try
            {

              foreach (var t in createUserModel)
              {
                if (string.IsNullOrWhiteSpace(t.PartnerUserId))
                  return ApiBadRequest("PartnerUserId required.");
                if (string.IsNullOrWhiteSpace(t.PartnerCompanyId))
                  return ApiBadRequest("PartnerCompanyId required.");
              }


              var createdUsers = await _userService
                    .CreateAsync(createUserModel)
                    .ConfigureAwait(false);

                return ApiCreated(createdUsers);
            }
            catch (AggregateException exception)
            {
                return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(string id, UpdateUserModel updateUserModel)
        {
            if (updateUserModel == null)
                ModelState.AddModelError(nameof(updateUserModel), "User data required.");

            if (id.Length == 0)
                ModelState.AddModelError(nameof(id), "User ID required.");

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

            var idFromDb = await _userService.GetInternalIDfromExternal(id);
            var idCompanyFromDb = await _companyService.GetInternalIDfromExternal(updateUserModel.PartnerCompanyId);

            if (idFromDb == 0)
            {
              return ApiNotFound("User not found.");
            }
            if (idCompanyFromDb == 0)
            {
              return ApiNotFound("Company not found.");
            }

      try
      {
        var user = DB.user.FirstOrDefault(c => c.id == idFromDb);


        if (user != null)
        {
          user.email = updateUserModel.Email;
          //user.depar = updateUserModel.Department;
          user.first_nm = updateUserModel.FirstName;
          user.last_nm = updateUserModel.LastName;
          user.photo_path = updateUserModel.PhotoPath;
          user.title_ds = updateUserModel.Title;
          user.company_id = idCompanyFromDb;

 


          await DB.SaveChangesAsync();


          return ApiOk();
        }
      }
      catch (NotFoundException exception)
      { return ApiNotFound(exception.Message); }


      ////try
      ////{
      ////    await _userService
      ////        .UpdateAsync(updateUserModel, id)
      ////        .ConfigureAwait(false);
      ////}
      ////catch (NotFoundException exception)
      ////{
      ////    return ApiNotFound(exception.Message);
      ////}

      return ApiOk();
        }

        //[HttpDelete]
        //[Route("internal/{id}")]
        //private async Task<IHttpActionResult> Delete(int id)
        //{
        //    if (id == 0)
        //    {
        //        ModelState.AddModelError(nameof(id), "User ID required.");
        //    }

        //    try
        //    {
        //        await _userService
        //            .DeleteAsync(id)
        //            .ConfigureAwait(false);
        //    }
        //    catch (NotFoundException exception)
        //    {
        //        return ApiNotFound(exception.Message);
        //    }

        //    return ApiOk();
        //}

        //[HttpDelete]
        //[Route("{id}")]
        //public async Task<IHttpActionResult> Delete(string id)
        //{
        //    if (String.IsNullOrEmpty(id))
        //    {
        //        ModelState.AddModelError(nameof(id), "User ID required.");
        //    }

        //    int idFromDb = DB.user.Where(user => user.partner_api_id.Equals(id)).Select(user => user.id).FirstOrDefault();
        //    if (idFromDb == 0)
        //    {
        //        ModelState.AddModelError(nameof(id), "User not found.");
        //    }

        //    try
        //    {
        //        await _userService
        //            .DeleteAsync(idFromDb)
        //            .ConfigureAwait(false);
        //    }
        //    catch (NotFoundException exception)
        //    {
        //        return ApiNotFound(exception.Message);
        //    }

        //    return ApiOk();
        //}

        [HttpGet]
        [Route("{id}/unreadCounters")]
        [ResponseType(typeof(usersUnreadEntitiesNumberViewModel))]
        public async Task<IHttpActionResult> UnreadCounters(string id)
        {
            usersUnreadEntitiesNumberViewModel unread_entites = new usersUnreadEntitiesNumberViewModel();
            var statusModel = new EC.Models.ReadStatusModel();
            usersUnreadEntitiesNumberViewModel result = new usersUnreadEntitiesNumberViewModel();
      ///  var statusModel = new Models.ReadStatusModel();
      ///  result.Items.ForEach(entity =>
      //    {
      ///        entity.usersUnreadEntities = statusModel.GetUserUnreadEntitiesNumbers(entity.id);
      //   });

      result.unreadMessages = 0;
      result.unreadNewReports = 0;
      result.unreadTasks = 0;

      return ApiOk(result);

        }

        [HttpPatch]
        [Route("{id}/activate")]
        public async Task<IHttpActionResult> UserActivate(string id)
        {
          if (String.IsNullOrEmpty(id))
          {
            ModelState.AddModelError(nameof(id), "User ID required.");
          }
          int idFromDb = DB.user.Where(u => u.partner_api_id.Equals(id)).Select(u => u.id).FirstOrDefault();
          if (idFromDb == 0)
          {
            ModelState.AddModelError(nameof(id), "User not found.");
          }

          var user = await DB.user.FirstOrDefaultAsync(u => u.id == idFromDb);
          if (user != null)
          {
            user.status_id = ECStatusConstants.Active_Value;
            await DB.SaveChangesAsync();
            return ApiOk();
          }

          return ApiNotFound();
    }

        [HttpPatch]
        [Route("{id}/deactivate")]
        public async Task<IHttpActionResult> UserDeactivate(string id)
        {
          if (String.IsNullOrEmpty(id))
          {
            ModelState.AddModelError(nameof(id), "User ID required.");
          }
          int idFromDb = DB.user.Where(u => u.partner_api_id.Equals(id)).Select(u => u.id).FirstOrDefault();
          if (idFromDb == 0)
          {
            ModelState.AddModelError(nameof(id), "User not found.");
          }

          var user = await DB.user.FirstOrDefaultAsync(u => u.id == idFromDb);
          if (user != null)
          {
            user.status_id = ECStatusConstants.Inactive_Value;
            await DB.SaveChangesAsync();
            return ApiOk();
          }

          return ApiNotFound();
        }
    }
  // tp d0 - merge it with one in service/
    public class usersUnreadEntitiesNumberViewModel
    {
        public int unreadNewReports { get; set; }

        public int unreadMessages { get; set; }

        public int unreadTasks { get; set; }
    }
}