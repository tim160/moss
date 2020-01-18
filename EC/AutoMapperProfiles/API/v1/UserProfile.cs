using AutoMapper;
using EC.Models.API.v1.Company;
using EC.Models.API.v1.User;
using EC.Models.Database;

namespace EC.AutoMapperProfiles.API.v1
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //CreateMap<user, UserModel>()
            //    .ForMember(
            //        destinationMember => destinationMember.Name,
            //        options => options.MapFrom(sourceMember => sourceMember.company_nm)
            //        );
            //CreateMap<CreateCompanyModel, user>()
            //    .ForMember(
            //        destinationMember => destinationMember.company_nm,
            //        options => options.MapFrom(sourceMember => sourceMember.Name.Trim())
            //        );
            //CreateMap<UpdateCompanyModel, user>();
        }
    }
}