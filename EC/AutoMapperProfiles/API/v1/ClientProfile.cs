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
                    options => options.MapFrom(sourceMember => sourceMember.address_id)
                    ).ForMember(
                    destinationMember => destinationMember.status_id,
                    options => options.MapFrom(sourceMember => sourceMember.status_id)
                    ).ForMember(
                    destinationMember => destinationMember.client_nm,
                    options => options.MapFrom(sourceMember => sourceMember.client_nm.Trim())
                    ).ForMember(
                    destinationMember => destinationMember.client_ds,
                    options => options.MapFrom(sourceMember => sourceMember.client_ds.Trim())
                    ).ForMember(
                    destinationMember => destinationMember.notepad_tx,
                    options => options.MapFrom(sourceMember => sourceMember.notepad_tx.Trim())
                    ).ForMember(
                    destinationMember => destinationMember.user_id,
                    options => options.MapFrom(sourceMember => sourceMember.user_id)
                    );


            CreateMap<UpdateClientModel, client>()
                .ForMember(
                    destinationMember => destinationMember.address_id,
                    options => options.MapFrom(sourceMember => sourceMember.address_id)
                    ).ForMember(
                    destinationMember => destinationMember.status_id,
                    options => options.MapFrom(sourceMember => sourceMember.status_id)
                    ).ForMember(
                    destinationMember => destinationMember.client_nm,
                    options => options.MapFrom(sourceMember => sourceMember.client_nm.Trim())
                    ).ForMember(
                    destinationMember => destinationMember.client_ds,
                    options => options.MapFrom(sourceMember => sourceMember.client_ds.Trim())
                    ).ForMember(
                    destinationMember => destinationMember.notepad_tx,
                    options => options.MapFrom(sourceMember => sourceMember.notepad_tx.Trim())
                    ).ForMember(
                    destinationMember => destinationMember.user_id,
                    options => options.MapFrom(sourceMember => sourceMember.user_id)
                    );
        }
    }
}