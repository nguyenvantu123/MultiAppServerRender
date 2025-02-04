// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using BlazorIdentityApi.Dtos.Identity.Interfaces;

namespace BlazorIdentityApi.Dtos.Identity.Base
{
    public class BaseRoleClaimDto<TRoleId> : IBaseRoleClaimDto
    {
        public int ClaimId { get; set; }

        public TRoleId RoleId { get; set; }

        object IBaseRoleClaimDto.RoleId => RoleId;
    }
}