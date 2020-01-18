using EC.Common.Interfaces;
using EC.Constants;
using EC.Core.Common;
using EC.Errors.CommonExceptions;
using EC.Localization;
using EC.Models;
using EC.Models.API.v1.User;
using EC.Models.Database;
using System;
using System.Collections.Generic;
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


            if (errors.Count > 0)
            {
                throw new AggregateException(errors);
            }
            user newUser = _set.Add(createUserModel, user => {
                user.company_id = createUserModel.company_id;
                user.role_id = createUserModel.role_id;
                user.status_id = createUserModel.status_id;
                user.first_nm = createUserModel.first_nm;
                user.last_nm = createUserModel.last_nm;
                user.login_nm = createUserModel.login_nm;
                user.password = createUserModel.password;
                user.photo_path = createUserModel.photo_path;
                user.preferred_contact_method_id = createUserModel.preferred_contact_method_id;
                user.question_ds = createUserModel.question_ds;
                user.answer_ds = createUserModel.answer_ds;
                user.last_update_dt = DateTime.Now;
                user.preferred_email_language_id = createUserModel.preferred_email_language_id;
                user.notification_messages_actions_flag = createUserModel.notification_messages_actions_flag;
                user.notification_new_reports_flag = createUserModel.notification_new_reports_flag;
                user.notification_marketing_flag = createUserModel.notification_marketing_flag;
                user.notification_summary_period = createUserModel.notification_summary_period;
                user.user_id = createUserModel.user_id;
            });
            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
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
            return user.id;
        }
    }
}