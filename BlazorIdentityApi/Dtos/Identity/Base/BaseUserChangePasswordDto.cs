// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using BlazorIdentityApi.Dtos.Identity.Interfaces;

namespace BlazorIdentityApi.Dtos.Identity.Base
{
    public class BaseUserChangePasswordDto<TUserId> : IBaseUserChangePasswordDto
    {
        public TUserId UserId { get; set; }

        object IBaseUserChangePasswordDto.UserId => UserId;
    }
}