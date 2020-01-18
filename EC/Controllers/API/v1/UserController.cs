using EC.Common.Util;
using EC.Errors.CommonExceptions;
using EC.Models.API.v1.User;
using EC.Services.API.v1.UserService;
using log4net;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace EC.Controllers.API.v1
{
    [RoutePrefix("api/v1/user")]
    public class UserController : BaseApiController
    {
        private readonly UserService _userService;
        protected readonly ILog _logger;

        public UserController()
        {
            _logger = LogManager.GetLogger(GetType());
            _userService = new UserService();
        }

        [HttpPost]
        [Route("new")]
        public async Task<IHttpActionResult> Create(CreateUserModel createUserModel)
        {
            if (createUserModel == null)
            {
                ModelState.AddModelError(nameof(createUserModel), "Company data required.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            bool isCC = DomainUtil.IsCC(Request.RequestUri.AbsoluteUri);
            int id = 0;
            try
            {
                id = await _userService
                    .CreateAsync(createUserModel, isCC)
                    .ConfigureAwait(false);
            }
            catch (AggregateException exception)
            {
                return ApiBadRequest(string.Join(Environment.NewLine, exception.InnerExceptions.Select(item => item.Message)));
            }

            return ApiCreated(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(int id, UpdateUserModel updateUserModel)
        {
            _logger.Debug($"id={id}");

            if (updateUserModel == null)
            {
                ModelState.AddModelError(nameof(updateUserModel), "Company data required.");
            }
            if (id == 0)
            {
                ModelState.AddModelError(nameof(id), "Company ID required.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

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

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError(nameof(id), "Company ID required.");
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
    }
}