﻿using EC.Common.Base;
using EC.Common.Util;
using EC.Common.Util.Filters;
using EC.Errors.CommonExceptions;
using EC.Models.API.v1.User;
using EC.Models.Database;
using EC.Services.API.v1.UserService;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EC.Constants;
using EC.Utils.Auth;

namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/users")]
    public class UserController : BaseApiController
    {
        private readonly UserService _userService;
        protected readonly ILog _logger;

        public UserController()
        {
            _logger = LogManager.GetLogger(GetType());
            _userService = new UserService();
        }

        // should be 1 item, not list
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(UserModel))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            var result =  await DB.user.FirstOrDefaultAsync(u => u.id == id);

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

//        [HttpPut]
//        [Route("{id}")]
//        public async Task<IHttpActionResult> UpdateExternalUser(string id, UpdateUserModel updateUserModel)
//        {
//            _logger.Debug($"id={id}");
//
//            if (updateUserModel == null)
//            {
//                ModelState.AddModelError(nameof(updateUserModel), "User data required.");
//            }
//            if (String.IsNullOrEmpty(id))
//            {
//                ModelState.AddModelError(nameof(id), "User ID required.");
//            }
//
//            int idFromDb = DB.user.Where(user => user.partner_api_id.Equals(id)).Select(user => user.id).FirstOrDefault();
//            if (idFromDb == 0)
//            {
//                ModelState.AddModelError(nameof(id), "User not found.");
//            }
//
//            if (!ModelState.IsValid)
//            {
//                return ApiBadRequest(ModelState);
//            }
//
//            try
//            {
//                await _userService
//                    .UpdateAsync(updateUserModel, idFromDb)
//                    .ConfigureAwait(false);
//            }
//            catch (NotFoundException exception)
//            {
//                return ApiNotFound(exception.Message);
//            }
//
//            return ApiOk();
//        }

        // do not do it now
        [HttpDelete]
        [Route("internal/{id}")]
        private async Task<IHttpActionResult> Delete(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError(nameof(id), "User ID required.");
            }

            try
            {
                await _userService
                    .DeleteAsync(id)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ModelState.AddModelError(nameof(id), "User ID required.");
            }

            int idFromDb = DB.user.Where(user => user.partner_api_id.Equals(id)).Select(user => user.id).FirstOrDefault();
            if (idFromDb == 0)
            {
                ModelState.AddModelError(nameof(id), "User not found.");
            }

            try
            {
                await _userService
                    .DeleteAsync(idFromDb)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException exception)
            {
                return ApiNotFound(exception.Message);
            }

            return ApiOk();
        }

        [HttpGet]
        [Route("{id}/unreadCounters")]
        [ResponseType(typeof(usersUnreadEntitiesNumberViewModel))]
        public async Task<IHttpActionResult> UnreadCounters(string id)
        {
            usersUnreadEntitiesNumberViewModel unread_entites = new usersUnreadEntitiesNumberViewModel();
            var statusModel = new Models.ReadStatusModel();
            usersUnreadEntitiesNumberViewModel result = new usersUnreadEntitiesNumberViewModel();
            ///  var statusModel = new Models.ReadStatusModel();
            ///  result.Items.ForEach(entity =>
            //    {
            ///        entity.usersUnreadEntities = statusModel.GetUserUnreadEntitiesNumbers(entity.id);
            //   });
            return ApiOk(result);

        }

        //to do - merge with using EC.Models.ViewModels;   usersUnreadEntitiesNumberViewModel
        public class usersUnreadEntitiesNumberViewModel
        {
            public int unreadNewReports { get; set; }

            public int unreadMessages { get; set; }

            public int unreadTasks { get; set; }
        }

        [HttpPatch]
        [Route("{id}/activate")]
        public async Task<IHttpActionResult> UserActivate(int id)
        {
            var user = await DB.user.FirstOrDefaultAsync(u => u.id == id);
            if (user != null)
            {
                user.status_id = 2;
                await DB.SaveChangesAsync();
                return ApiOk();
            }

            return ApiNotFound();
        }

        [HttpPatch]
        [Route("{id}/deactivate")]
        public async Task<IHttpActionResult> UserDeactivate(int id)
        {
            var user = await DB.user.FirstOrDefaultAsync(u => u.id == id);
            if (user != null)
            {
                user.status_id = 3;
                await DB.SaveChangesAsync();
                return ApiOk();
            }

            return ApiNotFound();
        }
    }
}