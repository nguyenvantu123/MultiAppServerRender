// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using BlazorIdentityApi.Common;
using BlazorIdentityApi.Dtos.Identity.Base;
using BlazorIdentityApi.Dtos.Identity.Interfaces;

namespace BlazorIdentityApi.Dtos.Identity
{
    public class UserRolesDto<TRoleDto, TKey> : BaseUserRolesDto<TKey>, IUserRolesDto
        where TRoleDto : RoleDto<TKey>
    {
        public UserRolesDto()
        {
           Roles = new List<TRoleDto>(); 
        }

        public string UserName { get; set; }

        public List<SelectItemDto> RolesList { get; set; }

        public List<TRoleDto> Roles { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        List<IRoleDto> IUserRolesDto.Roles => Roles.Cast<IRoleDto>().ToList();
    }
}
