using EC.Common.Base;
using EC.Constants;
using EC.Models;
using EC.Models.API.v1.User;
using EC.Models.Database;
using EC.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EC.Model.Impl;
using log4net;

namespace EC.Services.API.v1.UserService
{
    internal class UserService : ServiceBase<user>
    {
        public Task<PagedList<EC.Models.API.v1.User.UserModel>> GetPagedAsync(int page, int pageSize, Expression<Func<user, bool>> filter = null)
        {
            return GetPagedAsync<string, EC.Models.API.v1.User.UserModel>(page, pageSize, filter, null);
        }

        public async Task<IEnumerable<user>> CreateAsync(List<CreateUserModel> createUserModels)
        {
            List<Exception> errors = await CheckPartnerUserId(createUserModels);

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var users = GetUsersFromCreatedModel(createUserModels);

            _appContext.user.AddRange(users);
            
            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            SendEmailToCreatedUsers(users);

            return users;
        }

        public async Task<user> UpdateAsync(UpdateUserModel updateUserModel, int id)
        {
            user user = await _set
                .UpdateAsync(id, updateUserModel)
                .ConfigureAwait(false);

            user.last_update_dt = DateTime.Now;

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return user;
        }

        public async Task<int> DeleteAsync(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("The ID can't be empty.", nameof(id));
            }
            user userForDelete = await _set.FindAsync(id);
            if (userForDelete == null)
            {
                throw new ArgumentException("User not found.", nameof(id));
            }

            userForDelete.status_id = ECStatusConstants.Inactive_Value;

            user user = await _set
                .UpdateAsync(id, userForDelete)
                .ConfigureAwait(false);

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
            return user.id;
        }

        private async Task<List<Exception>> CheckPartnerUserId(List<CreateUserModel> data)
        {
            List<Exception> errors = new List<Exception>();
            var usersInDb = await _appContext.user.ToListAsync();
            foreach (var item in data)
            {
                var partnerInternal = usersInDb
                    .FirstOrDefault(user => user.partner_api_id != null && user.partner_api_id.Equals(item.PartnerUserId));
                if (partnerInternal != null)
                    errors.Add(new Exception($"PartnerInternalID = {item.PartnerUserId} already exists"));
            }

            return errors;
        }

        private List<user> GetUsersFromCreatedModel(List<CreateUserModel> createUserModels)
        {
            List<user> users = new List<user>();

            foreach (var userToCreate in createUserModels)
            {
                var generateModel = new GenerateRecordsModel();
                string login = generateModel.GenerateLoginName(userToCreate.FirstName, userToCreate.LastName);
                string pass = generateModel.GeneretedPassword().Trim();

                users.Add(new user()
                {
                    first_nm = userToCreate.FirstName,
                    last_nm = userToCreate.LastName,
                    login_nm = login,
                    password = PasswordUtils.GetHash(pass),
                    photo_path = userToCreate.PhotoPath,
                    last_update_dt = DateTime.Now,
                    is_api = true,
                    api_source_id = null,
                    partner_api_id = userToCreate.PartnerUserId,
                    company_id = Int32.TryParse(userToCreate.PartnerCompanyId, out int compId)? compId: 0,
                    answer_ds = string.Empty,
                    question_ds = string.Empty
                });
            }

            return users;
        }

        private void SendEmailToCreatedUsers(List<user> users)
        {
            foreach (var user in users)
            {
                try
                {
                    var currentCreatedUser = _appContext.user.Find(user.id);
                    var sessionUser = _appContext.user.Find(currentCreatedUser.user_id);
                    if (sessionUser != null)
                    {
                        var company = _appContext.company.Find(sessionUser.company_id);
                        if (company != null)
                        {
                            EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, "");
                            eb.NewMediator(
                                $"{sessionUser.first_nm} {sessionUser.last_nm}",
                                $"{company.company_nm}",
                                System.Configuration.ConfigurationManager.AppSettings["SiteRoot"] + "/settings/index",
                                System.Configuration.ConfigurationManager.AppSettings["SiteRoot"] + "/settings/index",
                                $"{currentCreatedUser.login_nm}",
                                $"{sessionUser.password}");
                            string body = eb.Body;
                            EmailNotificationModel emailNotificationModel = new EmailNotificationModel();
                            emailNotificationModel.SaveEmailBeforeSend(sessionUser.id, currentCreatedUser.id, sessionUser.company_id, currentCreatedUser.email, System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", "You have been added as a Case Administrator", body, false, 0);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogManager.GetLogger(GetType()).Error($"Send email failed, userId = {user.id}");
                }
            }
        }
    }
}