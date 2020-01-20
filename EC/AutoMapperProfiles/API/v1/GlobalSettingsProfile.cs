using AutoMapper;
using EC.Models.API.v1.GlobalSettings;
using EC.Models.Database;

namespace EC.AutoMapperProfiles.API.v1
{
    public class GlobalSettingsProfile : Profile
    {
        public GlobalSettingsProfile()
        {
            CreateMap<CreateGlobalSettingsModel, global_settings>();
        }
    }
}