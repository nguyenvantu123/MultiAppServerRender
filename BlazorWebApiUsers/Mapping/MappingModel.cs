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
            //CreateMap<Message, MessageDto>().ReverseMap();
        }
    }
}
