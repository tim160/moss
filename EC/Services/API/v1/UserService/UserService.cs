using EC.Models.API.v1.User;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EC.Services.API.v1.UserService
{
    internal class UserService : ServiceBase<user>
    {
        public async Task<int> CreateAsync(CreateUserModel createUserModel, bool isCC)
        {
            //if (createUserModel == null)
            //{
            //    throw new ArgumentNullException(nameof(createUserModel));
            //}

            //List<Exception> errors = new List<Exception>();

            //Models.GenerateRecordsModel generateRecordsModel = new Models.GenerateRecordsModel();
            //string messageCompanyInUse = LocalizationGetter.GetString("CompanyInUse", isCC);

            //if (generateRecordsModel.isCompanyInUse(createUserModel.Name))
            //{
            //    errors.Add(new AlreadyExistsException(messageCompanyInUse, createUserModel.Name));
            //}

            //string shortName = StringUtil.ShortString(createUserModel.Name);
            //shortName = await generateRecordsModel.GenerateUnusedCompanyShortName(shortName);
            //if (string.IsNullOrWhiteSpace(shortName))
            //{
            //    errors.Add(new AlreadyExistsException(messageCompanyInUse, createUserModel.Name));
            //}

            //IEmailAddressHelper emailAddressHelper = new EmailAddressHelper();
            //if (!emailAddressHelper.IsValidEmail(createUserModel.Email, false))
            //{
            //    errors.Add(new EmailFormatException(LocalizationGetter.GetString("EmailInvalid"), createUserModel.Email));
            //}

            //var companyInvitation = await _appContext.company_invitation
            //    .AsNoTracking()
            //    .Where(item =>
            //        item.is_active == 1
            //        && item.invitation_code.Trim().ToLower() == createUserModel.InvitationCode.Trim().ToLower())
            //    .Select(item => new
            //    {
            //        item.id,
            //        item.created_by_company_id
            //    })
            //    .FirstOrDefaultAsync();
            //if (companyInvitation == null)
            //{
            //    errors.Add(new ParameterValidationException(nameof(createUserModel.InvitationCode), LocalizationGetter.GetString("InvalidCode")));
            //}

            //if (errors.Count > 0)
            //{
            //    throw new AggregateException(errors);
            //}

            //var varInfo = await _appContext.var_info
            //    .AsNoTracking()
            //    .Where(item => item.emailed_code_to_customer == createUserModel.EmailedCodeToCustomer)
            //    .Select(item => new
            //    {
            //        item.employee_no,
            //        item.customers_no,
            //        item.onboarding_session_numbers
            //    })
            //    .FirstOrDefaultAsync();

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
            //await _appContext
            //    .SaveChangesAsync()
            //    .ConfigureAwait(false);
            //return newCompany.id;
            return 0;
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
    }
}