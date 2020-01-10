using System;
using System.Data.Entity;
using System.Threading.Tasks;
using EC.Models.API.v1.Company;
using EC.Models.Database;

namespace EC.Services.API.v1.CompanyServices
{
	internal static class CompanyActions
	{
		public static company Add(this DbSet<company> companies, ModifyCompanyModel modifyCompanyModel)
		{
			return companies.Add(modifyCompanyModel, company =>
			{
				company.registration_dt = DateTime.UtcNow; // TODO: Это время UTC?
				company.last_update_dt = DateTime.UtcNow; // TODO: Это время UTC?

				////////////////////////////////////////////////////////
				// TODO: Необходимо определиться с заполнением обязательных полей и просто полей из модели.
				company.client_id = 1;
				company.address_id = 1;
				company.billing_info_id = 1;
				company.status_id = 1;
				company.language_id = 1;
				company.employee_quantity = "1";
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
				company.onboard_sessions_paid = 1;
				////////////////////////////////////////////////////////
			});
		}

		public static Task<company> Update(this DbSet<company> companies, int id, ModifyCompanyModel modifyCompanyModel)
		{
			return companies.Update(id, modifyCompanyModel, company =>
			{
				company.last_update_dt = DateTime.UtcNow; // TODO: Это время UTC?
			});
		}
	}
}