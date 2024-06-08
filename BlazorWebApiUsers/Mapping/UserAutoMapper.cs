using AutoMapper;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Request.User;
using BlazorWebApi.Users.Response.User;
using eShop.Identity.API.Models;

namespace BlazorWebApi.Users.Mapping
{
    public class UserAutoMapper : Profile
    {
        public UserAutoMapper()
        {
            CreateMap<ApplicationUser, RegisterUserRequest>()
                .ForMember(dts => dts.Email, opts => opts.MapFrom(src => src.Email))
                .ForMember(dts => dts.UserName, opts => opts.MapFrom(src => src.UserName))
                .ForMember(dts => dts.FirstName, opts => opts.MapFrom(src => src.FirstName))
                .ForMember(dts => dts.LastName, opts => opts.MapFrom(src => src.LastName))
                .ForMember(dts => dts.PhoneNumber, opts => opts.MapFrom(src => src.PhoneNumber));

            CreateMap<ApplicationUser, UserProfileResponse>()
                 .ForMember(dts => dts.Email, opts => opts.MapFrom(src => src.Email))
                 .ForMember(dts => dts.UserName, opts => opts.MapFrom(src => src.UserName))
                 .ForMember(dts => dts.FirstName, opts => opts.MapFrom(src => src.FirstName))
                 .ForMember(dts => dts.LastName, opts => opts.MapFrom(src => src.LastName))
                 .ForMember(dts => dts.PhoneNumber, opts => opts.MapFrom(src => src.PhoneNumber));
        }
    }
}