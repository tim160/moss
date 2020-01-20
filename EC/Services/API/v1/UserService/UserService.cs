using EC.Common.Interfaces;
using EC.Constants;
using EC.Core.Common;
using EC.Errors.CommonExceptions;
using EC.Localization;
using EC.Models;
using EC.Models.API.v1.User;
using EC.Models.Database;
using EC.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Services.API.v1.UserService
{
    internal class UserService : ServiceBase<user>
    {
        public async Task<int> CreateAsync(CreateUserModel createUserModel, bool isCC)
        {
            if (createUserModel == null)
            {
                throw new ArgumentNullException(nameof(createUserModel));
            }

            List<Exception> errors = new List<Exception>();

            if (!String.IsNullOrEmpty(createUserModel.PartnerInternalID))
            {
                var partnerInternal = _appContext.user.Where(user => user.partner_api_id.Equals(createUserModel.PartnerInternalID)).FirstOrDefault();
                if (partnerInternal != null)
                {
                    errors.Add(new Exception("PartnerInternalID already exists"));
                }
            }

            if (errors.Count > 0)
            {
                throw new AggregateException(errors);
            }

            var generateModel = new GenerateRecordsModel();
            string login = generateModel.GenerateLoginName(createUserModel.first_nm, createUserModel.last_nm);
            string pass = generateModel.GeneretedPassword().Trim();

            user newUser = _set.Add(createUserModel, user => {
                user.company_id = createUserModel.company_id;
                user.role_id = createUserModel.role_id;
                user.status_id = createUserModel.status_id;
                user.first_nm = createUserModel.first_nm;
                user.last_nm = createUserModel.last_nm;
                user.login_nm = login;
                user.password = PasswordUtils.GetHash(pass);
                user.photo_path = createUserModel.photo_path;
                user.last_update_dt = DateTime.Now;
                user.is_api = true;
                user.api_source_id = null;
                user.partner_api_id = createUserModel.PartnerInternalID;
            });

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            try
            {
                var currentCreateduser = _appContext.user.Find(newUser.id);
                var sessionUser = _appContext.user.Find(currentCreateduser.user_id);

                if (sessionUser != null)
                {
                    var company = _appContext.company.Find(sessionUser.company_id);
                    if (company != null)
                    {
                        EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(isCC);
                        EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, "");
                        eb.NewMediator(
                            $"{sessionUser.first_nm} {sessionUser.last_nm}",
                            $"{company.company_nm}",
                            System.Configuration.ConfigurationManager.AppSettings["SiteRoot"] + "/settings/index",
                            System.Configuration.ConfigurationManager.AppSettings["SiteRoot"] + "/settings/index",
                            $"{currentCreateduser.login_nm}",
                            $"{pass}");
                        string body = eb.Body;
                        EmailNotificationModel emailNotificationModel = new EmailNotificationModel();
                        emailNotificationModel.SaveEmailBeforeSend(sessionUser.id, currentCreateduser.id, sessionUser.company_id, currentCreateduser.email, System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", "You have been added as a Case Administrator", body, false, 0);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return newUser.id;
        }

        public async Task<int> UpdateAsync(UpdateUserModel updateUserModel, int id)
        {
            if (updateUserModel == null)
            {
                throw new ArgumentNullException(nameof(updateUserModel));
            }
            if (id == 0)
            {
                throw new ArgumentException("The ID can't be empty.", nameof(id));
            }

            user user = await _set
                .UpdateAsync(id, updateUserModel)
                .ConfigureAwait(false);

            user.last_update_dt = DateTime.Now;

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
            return user.id;
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
    }
}