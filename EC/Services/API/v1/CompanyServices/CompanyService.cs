using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EC.Common.Base;
using EC.Common.Interfaces;
using EC.Common.Util;
using EC.Constants;
using EC.Core.Common;
using EC.Errors.CommonExceptions;
using EC.Localization;
using EC.Models.API.v1.Company;
using EC.Models.Database;
using static EC.Constants.LanguageConstants;

namespace EC.Services.API.v1.CompanyServices
{
	internal class CompanyService : ServiceBase<company>
	{
		public Task<PagedList<CompanyModel>> GetPagedAsync(
			int page,
			int pageSize,
			Expression<Func<company, bool>> filter = null)
		{
			return GetPagedAsync<string, CompanyModel>(page, pageSize, filter, company => company.company_nm);
		}

		public async Task<int> CreateAsync(CreateCompanyModel createCompanyModel, bool isCC)
		{
			if (createCompanyModel == null)
			{
				throw new ArgumentNullException(nameof(createCompanyModel));
			}

			List<Exception> errors = new List<Exception>();

			Models.GenerateRecordsModel generateRecordsModel = new Models.GenerateRecordsModel();
			string messageCompanyInUse = LocalizationGetter.GetString("CompanyInUse", isCC);

			if (generateRecordsModel.isCompanyInUse(createCompanyModel.Name))
			{
				errors.Add(new AlreadyExistsException(messageCompanyInUse, createCompanyModel.Name));
			}

			string shortName = StringUtil.ShortString(createCompanyModel.Name);
			shortName = await generateRecordsModel.GenerateUnusedCompanyShortName(shortName);
			if (string.IsNullOrWhiteSpace(shortName))
			{
				errors.Add(new AlreadyExistsException(messageCompanyInUse, createCompanyModel.Name));
			}

			IEmailAddressHelper emailAddressHelper = new EmailAddressHelper();
			if (!emailAddressHelper.IsValidEmail(createCompanyModel.Email, false))
			{
				errors.Add(new EmailFormatException(LocalizationGetter.GetString("EmailInvalid"), createCompanyModel.Email));
			}

			var companyInvitation = await _appContext.company_invitation
				.AsNoTracking()
				.Where(item =>
					item.is_active == 1
					&& item.invitation_code.Trim().ToLower() == createCompanyModel.InvitationCode.Trim().ToLower())
				.Select(item => new
				{
					item.id,
					item.created_by_company_id
				})
				.FirstOrDefaultAsync();
			if (companyInvitation == null)
			{
				errors.Add(new ParameterValidationException(nameof(createCompanyModel.InvitationCode), LocalizationGetter.GetString("InvalidCode")));
			}

			if (errors.Count > 0)
			{
				throw new AggregateException(errors);
			}

			var varInfo = await _appContext.var_info
				.AsNoTracking()
				.Where(item => item.emailed_code_to_customer == createCompanyModel.EmailedCodeToCustomer)
				.Select(item => new
				{
					item.employee_no,
					item.customers_no,
					item.onboarding_session_numbers
				})
				.FirstOrDefaultAsync();

			company newCompany = _set.Add(createCompanyModel, company =>
			{
				company.registration_dt = DateTime.Now;
				company.last_update_dt = DateTime.Now;

				company.company_invitation_id = companyInvitation.id;
				company.client_id = companyInvitation.created_by_company_id;
				company.status_id = ECSessionConstants.status_active;
				company.company_code = generateRecordsModel.GenerateCompanyCode(company.company_nm);
				company.language_id = (int)Language_Values.English;
				company.company_short_name = shortName;

				if (varInfo != null)
				{
					company.contractors_number = varInfo.employee_no;
					company.customers_number = varInfo.customers_no;
					company.onboard_sessions_paid = varInfo.onboarding_session_numbers;
					if (varInfo.onboarding_session_numbers > 0)
					{
						company.onboard_sessions_expiry_dt = DateTime.Today.AddYears(1);
					}
				}

				////////////////////////////////////////////////////////
				// TODO: Необходимо определиться с заполнением обязательных полей и просто полей из модели.
				company.address_id = 1;
				company.billing_info_id = 1;
				company.implicated_title_name_id = 1;
				company.witness_show_id = 1;
				company.show_location_id = 1;
				company.show_department_id = 1;
				company.default_anonymity_id = 1;
				company.user_id = 1;
				company.time_zone_id = 1;
				company.step1_delay = 1;
				company.step1_postpone = 1;
				company.step2_delay = 1;
				company.step2_postpone = 1;
				company.step3_delay = 1;
				company.step3_postpone = 1;
				company.step4_delay = 1;
				company.step4_postpone = 1;
				company.step5_delay = 1;
				company.step5_postpone = 1;
				company.step6_delay = 1;
				company.step6_postpone = 1;
				////////////////////////////////////////////////////////
			});
			await _appContext
				.SaveChangesAsync()
				.ConfigureAwait(false);
			return newCompany.id;
		}

		public async Task<int> UpdateAsync(UpdateCompanyModel updateCompanyModel, int id)
		{
			if (updateCompanyModel == null)
			{
				throw new ArgumentNullException(nameof(updateCompanyModel));
			}
			if (id == 0)
			{
				throw new ArgumentException("The ID can't be empty.", nameof(id));
			}

			company company = await _set
				.UpdateAsync(id, updateCompanyModel)
				.ConfigureAwait(false);

			await _appContext
				.SaveChangesAsync()
				.ConfigureAwait(false);
			return company.id;
		}

        public async Task<int> DeleteAsync(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("The ID can't be empty.", nameof(id));
            }
            company companyForDelete = await _set.FindAsync(id);
            if (companyForDelete == null)
            {
                throw new ArgumentException("Client not found.", nameof(id));
            }
            companyForDelete.status_id = ECStatusConstants.Inactive_Value;

            company client = await _set
                .UpdateAsync(id, companyForDelete)
                .ConfigureAwait(false);
            return client.id;
        }
    }
}