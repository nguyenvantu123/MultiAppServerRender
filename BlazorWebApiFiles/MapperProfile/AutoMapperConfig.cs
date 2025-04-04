
using AutoMapper;
using BlazorIdentity.Files.Entities;
using BlazorIdentity.Files.Response;


namespace BlazorApiUser.MapperProfile
{
    public class AutoMapperConfig : Profile
    {

        public AutoMapperConfig()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<DocumentsType, DocumentResponse>();

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