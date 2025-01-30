using AutoMapper;
using BlazorIdentity.Models;
using BlazorIdentity.Users.Models;
using Shared.Models;


namespace BlazorApiUser.MapperProfile
{
    public class AutoMapperConfig : Profile
    {

        public AutoMapperConfig()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<ApplicationUser, UserDataViewModel>();
            CreateMap<AppTenantInfo, TenantDto>();

            CreateMap<UserProfile, UserProfileViewModel>();
			//CreateMap<UserDto, User>();
		}

        //public static IMapper Initialize()
        //{
        //    var config = new MapperConfiguration(cfg =>
        //    {
        //        cfg.CreateMap<ApplicationUser, UserDataViewModel>().ReverseMap();
        //    });

        //    return config.CreateMapper();
        //}
    }
}