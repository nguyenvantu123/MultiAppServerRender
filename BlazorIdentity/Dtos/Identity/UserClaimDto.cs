﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;
using BlazorIdentityApi.Dtos.Identity.Base;
using BlazorIdentityApi.Dtos.Identity.Interfaces;

namespace BlazorIdentityApi.Dtos.Identity
{
    public class UserClaimDto<TKey> : BaseUserClaimDto<TKey>, IUserClaimDto
    {
        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}