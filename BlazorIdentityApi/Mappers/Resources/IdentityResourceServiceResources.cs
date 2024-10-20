// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using BlazorIdentityApi.Helpers;

namespace BlazorIdentityApi.Resources
{
    public class IdentityResourceServiceResources : IIdentityResourceServiceResources
    {
        public virtual ResourceMessage IdentityResourceDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourceDoesNotExist),
                Description = "IdentityResourceDoesNotExist"
            };
        }

        public virtual ResourceMessage IdentityResourceExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourceExistsKey),
                Description = "IdentityResourceExistsKey"
            };
        }

        public virtual ResourceMessage IdentityResourceExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourceExistsValue),
                Description = "IdentityResourceExistsValue"
            };
        }

        public virtual ResourceMessage IdentityResourcePropertyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourcePropertyDoesNotExist),
                Description = "IdentityResourcePropertyDoesNotExist"
            };
        }

        public virtual ResourceMessage IdentityResourcePropertyExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourcePropertyExistsValue),
                Description = "IdentityResourcePropertyExistsValue"
            };
        }

        public virtual ResourceMessage IdentityResourcePropertyExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourcePropertyExistsKey),
                Description = "IdentityResourcePropertyExistsKey"
            };
        }
    }
}
