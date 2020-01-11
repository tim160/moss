using AutoMapper;
using EC.Models.API.v1.Company;
using EC.Models.Database;

namespace EC.AutoMapperProfiles.API.v1
{
	public class CompanyProfile : Profile
	{
		public CompanyProfile()
		{
			CreateMap<company, CompanyModel>()
				.ForMember(
					destinationMember => destinationMember.Name,
					options => options.MapFrom(sourceMember => sourceMember.company_nm)
					);
			CreateMap<CreateCompanyModel, company>()
				.ForMember(
					destinationMember => destinationMember.company_nm,
					options => options.MapFrom(sourceMember => sourceMember.Name.Trim())
					);
			CreateMap<UpdateCompanyModel, company>();
		}
	}
}