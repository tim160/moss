using AutoMapper;
using EC.Models.API.v1.Client;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.AutoMapperProfiles.API.v1
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<client, ClientModel>()
                .ForMember(
                    destinationMember => destinationMember.Name,
                    options => options.MapFrom(sourceMember => sourceMember.company_nm)
                    );
            CreateMap<CreateClientModel, client>()
                .ForMember(
                    destinationMember => destinationMember.company_nm,
                    options => options.MapFrom(sourceMember => sourceMember.Name.Trim())
                    );
            CreateMap<CreateClientModel, client>();
        }
    }
}