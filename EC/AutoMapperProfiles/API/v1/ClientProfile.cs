using AutoMapper;
using EC.Models.API.v1.Client;
using EC.Models.Database;

namespace EC.AutoMapperProfiles.API.v1
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<CreateClientModel, client>()
                .ForMember(
                    destinationMember => destinationMember.address_id,
                    options => options.UseValue(2)
                    ).ForMember(
                    destinationMember => destinationMember.status_id,
                    options => options.UseValue(2)
                    ).ForMember(
                    destinationMember => destinationMember.client_nm,
                    options => options.MapFrom(sourceMember => sourceMember.client_nm.Trim())
                    ).ForMember(
                    destinationMember => destinationMember.client_ds,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.notepad_tx,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.user_id,
                    options => options.UseValue(2)
                    );


            CreateMap<UpdateClientModel, client>()
                .ForMember(
                    destinationMember => destinationMember.address_id,
                    options => options.UseValue(2)
                    ).ForMember(
                    destinationMember => destinationMember.status_id,
                    options => options.UseValue(2)
                    ).ForMember(
                    destinationMember => destinationMember.client_nm,
                    options => options.MapFrom(sourceMember => sourceMember.client_nm.Trim())
                    ).ForMember(
                    destinationMember => destinationMember.client_ds,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.notepad_tx,
                    options => options.UseValue("")
                    ).ForMember(
                    destinationMember => destinationMember.user_id,
                    options => options.UseValue(2)
                    );
        }
    }
}