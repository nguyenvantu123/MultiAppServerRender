using AutoMapper;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Request.User;
using BlazorWebApi.Users.Response.User;

namespace BlazorWebApi.Users.Mapping
{
    public class UserAutoMapper : Profile
    {
        public UserAutoMapper()
        {
            CreateMap<User, RegisterUserRequest>()
                .ForMember(dts => dts.Email, opts => opts.MapFrom(src => src.Email))
                .ForMember(dts => dts.UserName, opts => opts.MapFrom(src => src.UserName))
                .ForMember(dts => dts.FirstName, opts => opts.MapFrom(src => src.FirstName))
                .ForMember(dts => dts.LastName, opts => opts.MapFrom(src => src.LastName))
                .ForMember(dts => dts.PhoneNumber, opts => opts.MapFrom(src => src.PhoneNumber));

            CreateMap<User, UserProfileResponse>()
                 .ForMember(dts => dts.Email, opts => opts.MapFrom(src => src.Email))
                 .ForMember(dts => dts.UserName, opts => opts.MapFrom(src => src.UserName))
                 .ForMember(dts => dts.FirstName, opts => opts.MapFrom(src => src.FirstName))
                 .ForMember(dts => dts.LastName, opts => opts.MapFrom(src => src.LastName))
                 .ForMember(dts => dts.PhoneNumber, opts => opts.MapFrom(src => src.PhoneNumber));
        }
    }
}