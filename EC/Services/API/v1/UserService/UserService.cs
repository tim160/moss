using EC.Common.Interfaces;
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

            IEmailAddressHelper emailAddressHelper = new EmailAddressHelper();
            if (!emailAddressHelper.IsValidEmail(createUserModel.Email, false))
            {
                errors.Add(new EmailFormatException(LocalizationGetter.GetString("EmailInvalid"), createUserModel.Email));
            }

            if (errors.Count > 0)
            {
                throw new AggregateException(errors);
            }
            user newUser = _set.Add(createUserModel, user => {
                user.email = createUserModel.Email;
            });
            //company newCompany = _set.Add(createUserModel, company =>
            //{
            //    company.registration_dt = DateTime.Now;
            //    company.last_update_dt = DateTime.Now;

            //    company.company_invitation_id = companyInvitation.id;
            //    company.client_id = companyInvitation.created_by_company_id;
            //    company.status_id = ECSessionConstants.status_active;
            //    company.company_code = generateRecordsModel.GenerateCompanyCode(company.company_nm);
            //    company.language_id = (int)Language_Values.English;
            //    company.company_short_name = shortName;

            //    if (varInfo != null)
            //    {
            //        company.contractors_number = varInfo.employee_no;
            //        company.customers_number = varInfo.customers_no;
            //        company.onboard_sessions_paid = varInfo.onboarding_session_numbers;
            //        if (varInfo.onboarding_session_numbers > 0)
            //        {
            //            company.onboard_sessions_expiry_dt = DateTime.Today.AddYears(1);
            //        }
            //    }

            //    ////////////////////////////////////////////////////////
            //    // TODO: Необходимо определиться с заполнением обязательных полей и просто полей из модели.
            //    company.address_id = 1;
            //    company.billing_info_id = 1;
            //    company.implicated_title_name_id = 1;
            //    company.witness_show_id = 1;
            //    company.show_location_id = 1;
            //    company.show_department_id = 1;
            //    company.default_anonymity_id = 1;
            //    company.user_id = 1;
            //    company.time_zone_id = 1;
            //    company.step1_delay = 1;
            //    company.step1_postpone = 1;
            //    company.step2_delay = 1;
            //    company.step2_postpone = 1;
            //    company.step3_delay = 1;
            //    company.step3_postpone = 1;
            //    company.step4_delay = 1;
            //    company.step4_postpone = 1;
            //    company.step5_delay = 1;
            //    company.step5_postpone = 1;
            //    company.step6_delay = 1;
            //    company.step6_postpone = 1;
            //    ////////////////////////////////////////////////////////
            //});
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
            userForDelete.status_id = 2;
            user user = await _set
                .UpdateAsync(id, userForDelete)
                .ConfigureAwait(false);
            return user.id;
        }
    }
}