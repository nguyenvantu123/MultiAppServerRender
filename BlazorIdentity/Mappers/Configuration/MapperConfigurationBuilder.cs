// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BlazorIdentityApi.Mappers.Configuration
{
    public class MapperConfigurationBuilder : IMapperConfigurationBuilder
    {
        public HashSet<Type> ProfileTypes { get; } = new HashSet<Type>();

        public IMapperConfigurationBuilder AddProfilesType(HashSet<Type> profileTypes)
        {
            if (profileTypes == null) return this;

            foreach (var profileType in profileTypes)
            {
                ProfileTypes.Add(profileType);
            }

            return this;
        }

        public IMapperConfigurationBuilder UseIdentityMappingProfile()
        {
            ProfileTypes.Add(typeof(IdentityMapperProfile));

            return this;
        }
    }
}