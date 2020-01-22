using AutoMapper;
using EC.Models.API.v1.User;
using EC.Models.Database;

namespace EC.AutoMapperProfiles.API.v1
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserModel, user>()
                .ForMember(
                    destinationMember => destinationMember.company_id,
                    options => options.MapFrom(sourceMember => sourceMember.company_id)
                    ).ForMember(
                    destinationMember => destinationMember.role_id,
                    options => options.MapFrom(sourceMember => sourceMember.role_id)
                    ).ForMember(
                    destinationMember => destinationMember.status_id,
                    options => options.UseValue(3)
                    ).ForMember(
                    destinationMember => destinationMember.first_nm,
                    options => options.MapFrom(sourceMember => sourceMember.first_nm)
                    ).ForMember(
                    destinationMember => destinationMember.last_nm,
                    options => options.MapFrom(sourceMember => sourceMember.last_nm)
                    ).ForMember(
                    destinationMember => destinationMember.login_nm,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.password,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.photo_path,
                    options => options.MapFrom(sourceMember => sourceMember.photo_path)
                    ).ForMember(
                    destinationMember => destinationMember.preferred_contact_method_id,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.question_ds,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.answer_ds,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.user_id,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.preferred_email_language_id,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.notification_messages_actions_flag,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.notification_new_reports_flag,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.notification_marketing_flag,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.notification_summary_period,
                    options => options.UseValue(1)
                    );

            CreateMap<UpdateUserModel, user>()
                                .ForMember(
                    destinationMember => destinationMember.company_id,
                    options => options.MapFrom(sourceMember => sourceMember.company_id)
                    ).ForMember(
                    destinationMember => destinationMember.role_id,
                    options => options.MapFrom(sourceMember => sourceMember.role_id)
                    ).ForMember(
                    destinationMember => destinationMember.status_id,
                    options => options.UseValue(3)
                    ).ForMember(
                    destinationMember => destinationMember.first_nm,
                    options => options.MapFrom(sourceMember => sourceMember.first_nm)
                    ).ForMember(
                    destinationMember => destinationMember.last_nm,
                    options => options.MapFrom(sourceMember => sourceMember.last_nm)
                    ).ForMember(
                    destinationMember => destinationMember.login_nm,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.password,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.photo_path,
                    options => options.MapFrom(sourceMember => sourceMember.photo_path)
                    ).ForMember(
                    destinationMember => destinationMember.preferred_contact_method_id,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.question_ds,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.answer_ds,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.user_id,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.preferred_email_language_id,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.notification_messages_actions_flag,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.notification_new_reports_flag,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.notification_marketing_flag,
                    options => options.UseValue(1)
                    ).ForMember(
                    destinationMember => destinationMember.notification_summary_period,
                    options => options.UseValue(1)
                    );
        }
    }
}