using AutoMapper;
using EC.Models;
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
                    destinationMember => destinationMember.status_id,
                    options => options.UseValue(3)
                    ).ForMember(
                    destinationMember => destinationMember.first_nm,
                    options => options.MapFrom(sourceMember => sourceMember.FirstName)
                    ).ForMember(
                    destinationMember => destinationMember.last_nm,
                    options => options.MapFrom(sourceMember => sourceMember.LastName)
                    ).ForMember(
                    destinationMember => destinationMember.login_nm,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.password,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.photo_path,
                    options => options.MapFrom(sourceMember => sourceMember.PhotoPath)
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
                    destinationMember => destinationMember.status_id,
                    options => options.UseValue(3)
                    ).ForMember(
                    destinationMember => destinationMember.first_nm,
                    options => options.MapFrom(sourceMember => sourceMember.FirstName)
                    ).ForMember(
                    destinationMember => destinationMember.last_nm,
                    options => options.MapFrom(sourceMember => sourceMember.LastName)
                    ).ForMember(
                    destinationMember => destinationMember.login_nm,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.password,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.photo_path,
                    options => options.MapFrom(sourceMember => sourceMember.PhotoPath)
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



            CreateMap<user, Models.API.v1.User.UserModel>().BeforeMap((s, d) => d.usersUnreadEntities = new ReadStatusModel().GetUserUnreadEntitiesNumbers(s.id));

        }
    }
}