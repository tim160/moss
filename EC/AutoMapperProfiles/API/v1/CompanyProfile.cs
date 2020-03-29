using AutoMapper;
using EC.Models.API.v1.Company;
using EC.Models.Database;

namespace EC.AutoMapperProfiles.API.v1
{
	public class CompanyProfile : Profile
	{
		public CompanyProfile()
		{
            CreateMap<CreateCompanyModel, company>()
                 .ForMember(
                    destinationMember => destinationMember.client_id,
                    options => options.UseValue(1))
                .ForMember(
                    destinationMember => destinationMember.address_id,
                    options => options.UseValue(1))
                .ForMember(
                    destinationMember => destinationMember.billing_info_id,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.status_id,
                    options => options.UseValue(2)
                    ).ForMember(
                    destinationMember => destinationMember.language_id,
                    options => options.UseValue(2)
                    ).ForMember(
                    destinationMember => destinationMember.employee_quantity,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.implicated_title_name_id,
                    options => options.UseValue(2)
                    ).ForMember(
                    destinationMember => destinationMember.witness_show_id,
                    options => options.UseValue(2))
                    .ForMember(
                    destinationMember => destinationMember.show_location_id,
                    options => options.UseValue(2))
                    .ForMember(
                    destinationMember => destinationMember.show_department_id,
                    options => options.UseValue(2))
                    .ForMember(
                    destinationMember => destinationMember.default_anonymity_id,
                    options => options.UseValue(2))
                                        .ForMember(
                    destinationMember => destinationMember.user_id,
                    options => options.UseValue(2))
                                        .ForMember(
                    destinationMember => destinationMember.time_zone_id,
                    options => options.UseValue(2))
                                        .ForMember(
                    destinationMember => destinationMember.step1_delay,
                    options => options.UseValue(2))
                                        .ForMember(
                    destinationMember => destinationMember.step1_postpone,
                    options => options.UseValue(2))
                                        .ForMember(
                    destinationMember => destinationMember.step2_delay,
                    options => options.UseValue(2))
                                                            .ForMember(
                    destinationMember => destinationMember.step2_postpone,
                    options => options.UseValue(2))
                                                            .ForMember(
                    destinationMember => destinationMember.step3_delay,
                    options => options.UseValue(2))
                                                            .ForMember(
                    destinationMember => destinationMember.step3_postpone,
                    options => options.UseValue(2))
                                                            .ForMember(
                    destinationMember => destinationMember.step4_delay,
                    options => options.UseValue(2))
                                                            .ForMember(
                    destinationMember => destinationMember.step4_postpone,
                    options => options.UseValue(2))
                                                            .ForMember(
                    destinationMember => destinationMember.step5_delay,
                    options => options.UseValue(2))
                                                            .ForMember(
                    destinationMember => destinationMember.step5_postpone,
                    options => options.UseValue(2))
                    .ForMember(
                    destinationMember => destinationMember.step6_delay,
                    options => options.UseValue(2))
                                        .ForMember(
                    destinationMember => destinationMember.step6_postpone,
                    options => options.UseValue(2))
                                        .ForMember(
                    destinationMember => destinationMember.onboard_sessions_paid,
                    options => options.UseValue(2)).ForMember(
                    destinationMember => destinationMember.controls_client,
                    options => options.UseValue(false))
                    ;
            //CreateMap<CreateCompanyModel, company>();
			CreateMap<UpdateCompanyModel, company>();
      CreateMap<company, CompanyModel>();

    }
	}
}