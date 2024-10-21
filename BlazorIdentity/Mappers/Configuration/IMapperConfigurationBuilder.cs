// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BlazorIdentityApi.Mappers.Configuration
{
    public interface IMapperConfigurationBuilder
    {
        HashSet<Type> ProfileTypes { get; }

        IMapperConfigurationBuilder AddProfilesType(HashSet<Type> profileTypes);

        IMapperConfigurationBuilder UseIdentityMappingProfile();
    }
}
