﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;

namespace BlazorWebApi.Users.Models
{
    public record ErrorViewModel
    {
        public ErrorMessage Error { get; set; }
    }
}