using AutoMapper;
using BlazorIdentity.Users.Models;


namespace BlazorApiUser.MapperProfile
{
    public class AutoMapperConfig : Profile
    {
        public static IMapper Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, UserDataViewModel>();
            });

            return config.CreateMapper();
        }
    }
}