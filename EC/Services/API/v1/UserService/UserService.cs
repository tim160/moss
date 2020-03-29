using EC.Common.Base;
using EC.Constants;
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
    public class UserService : ServiceBase<user>
    {

        public async Task<int> GetInternalIDfromExternal(string id)
        {
          int idFromDb = 0;
          var users = await _appContext.user.ToListAsync();
          var _users = users.Where(c => c.partner_api_id == id).ToList();
          if (_users.Count() > 0)
            idFromDb = _users[0].id;

          return idFromDb;
        }

    public async Task<UserModel> GetUserById(int id)
    {

      var user = await _appContext.user.FindAsync(id);
      if (user != null)
      {

        return new EC.Models.API.v1.User.UserModel()
        {
          partnerUserID = user.partner_api_id,


          // partnerCompanyID = user.partner_api_id, to do
          firstName = user.first_nm,
          lastName = user.last_nm,
          email = user.email,
          photoPath = user.photo_path,
         

      };
      }

      return null;
    }



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

            var allCompanies = _appContext.company.ToList();
 

            foreach (var userToCreate in createUserModels)
            {
                var generateModel = new Models.GenerateRecordsModel();
                string login = generateModel.GenerateLoginName(userToCreate.FirstName, userToCreate.LastName);
                string pass = generateModel.GeneretedPassword().Trim();

                int idFromDb = 0;
                var _companies = allCompanies.Where(c => c.partner_api_id == userToCreate.PartnerCompanyId).ToList();
                if (_companies.Count() > 0)
                  idFromDb = _companies[0].id;

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
                    company_id = idFromDb,
                    answer_ds = string.Empty,
                    question_ds = string.Empty,
                    status_id = ECStatusConstants.Active_Value 
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
                            var emailNotificationModel = new Models.EmailNotificationModel();
      ///  will test later                    emailNotificationModel.SaveEmailBeforeSend(sessionUser.id, currentCreatedUser.id, sessionUser.company_id, currentCreatedUser.email, System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", "You have been added as a Case Administrator", body, false, 0);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogManager.GetLogger(GetType()).Error($"Send email failed, userId = {user.id}");
                }
            }
        }


    

      public async Task<List<UserModel>> GetUsersByCompanyId(int id, string originalId)
    {

      var allUsers = await _appContext.user.ToListAsync();
      var _users = allUsers.Where(u => u.company_id == id && new[] { ECLevelConstants.level_ec_mediator, ECLevelConstants.level_escalation_mediator, ECLevelConstants.level_supervising_mediator }.Contains(u.role_id)).ToList();

      var companyModel = _users.Select(item => new UserModel()
      {
        partnerUserID = item.partner_api_id,


        partnerCompanyID = originalId,
        firstName = item.first_nm,
        lastName = item.last_nm,
        email = item.email,
        photoPath = item.photo_path,
 
      }).ToList();


      return companyModel;
    }
  }

      public class usersUnreadEntitiesNumberViewModel1
      {
        public int unreadNewReports { get; set; }

        public int unreadMessages { get; set; }

        public int unreadTasks { get; set; }
      }
}