// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace BlazorIdentityApi.Dtos.Identity.Interfaces
{
    public interface IRoleClaimsDto : IRoleClaimDto
    {
        string RoleName { get; set; }
        List<IRoleClaimDto> Claims { get; }
        int TotalCount { get; set; }
        int PageSize { get; set; }
    }
}
