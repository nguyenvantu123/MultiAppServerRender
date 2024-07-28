using AutoMapper;
using AutoMapper.Configuration;
using BlazorWebApi.Users.Models;
using Finbuckle.MultiTenant;
using WebApp.Models;

namespace WebApp.Mapping
{
    public class MappingModel : MapperConfigurationExpression
    {
        /// <summary>
        /// Create automap mapping profiles
        /// </summary>
        public MappingModel()
        {
            CreateMap<AppTenantInfo, TenantModel>().ReverseMap();

            CreateMap<ApplicationUser, UserViewModel>();

            CreateMap<ApplicationRoleClaim, RoleClaimModel>()
                .ForMember(x => x.RoleId, opt => opt.MapFrom(map => map.RoleId))
                .ForMember(x => x.Id, opt => opt.MapFrom(map => map.Id))
                //.ForMember(x => x.Type, opt => opt.MapFrom(map => map.Type))
                .ForMember(x => x.Value, opt => opt.MapFrom(map => map.ClaimValue))
                .ForMember(x => x.Description, opt => opt.MapFrom(map => map.Description))
                .ForMember(x => x.Group, opt => opt.MapFrom(map => map.Group));

            //CreateMap<Message, MessageDto>().ReverseMap();
        }
    }
}
