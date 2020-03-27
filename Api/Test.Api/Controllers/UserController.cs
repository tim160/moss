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
using TestApi.Utils;
using EC.Constants;

namespace TestApi.Controllers
{
    [RoutePrefix("api/v1/users")]
    [CustomAuthorization]
    public class UserController : BaseApiController
    {
        private readonly UserService _userService;

        public UserController()
        {
            _userService = new UserService();
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(UserModel))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            var result = await DB.user.FirstOrDefaultAsync(u => u.id == id);

            if (result != null)
                return ApiOk(result);

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
        public async Task<IHttpActionResult> Update(int id, UpdateUserModel updateUserModel)
        {
            if (updateUserModel == null)
                ModelState.AddModelError(nameof(updateUserModel), "User data required.");

            if (id == 0)
                ModelState.AddModelError(nameof(id), "User ID required.");

            if (!ModelState.IsValid)
                return ApiBadRequest(ModelState);

            try
            {
                await _userService
                    .UpdateAsync(updateUserModel, id)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

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

    public class usersUnreadEntitiesNumberViewModel
    {
        public int unreadNewReports { get; set; }

        public int unreadMessages { get; set; }

        public int unreadTasks { get; set; }
    }
}