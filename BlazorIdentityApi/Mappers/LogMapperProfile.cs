// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using AutoMapper;
using Skoruba.AuditLogging.EntityFramework.Entities;
using BlazorIdentityApi.Dtos.Log;
using BlazorIdentityApi.Entities;
using BlazorIdentityApi.Common;

namespace BlazorIdentityApi.Mappers
{
    public class LogMapperProfile : Profile
    {
        public LogMapperProfile()
        {
            CreateMap<Log, LogDto>(MemberList.Destination)
                .ReverseMap();
            
            CreateMap<PagedList<Log>, LogsDto>(MemberList.Destination)
                .ForMember(x => x.Logs, opt => opt.MapFrom(src => src.Data));

            CreateMap<AuditLog, AuditLogDto>(MemberList.Destination)
                .ReverseMap();

            CreateMap<PagedList<AuditLog>, AuditLogsDto>(MemberList.Destination)
                .ForMember(x => x.Logs, opt => opt.MapFrom(src => src.Data));
        }
    }
}
