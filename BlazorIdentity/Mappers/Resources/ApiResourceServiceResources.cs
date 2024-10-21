// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.


using BlazorIdentityApi.Helpers;

namespace BlazorIdentityApi.Resources
{
    public class ApiResourceServiceResources : IApiResourceServiceResources
    {
        public virtual ResourceMessage ApiResourceDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourceDoesNotExist),
                Description = "ApiResourceDoesNotExist"
            };
        }

        public virtual ResourceMessage ApiResourceExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourceExistsValue),
                Description = "ApiResourceExistsValue"
            };
        }

        public virtual ResourceMessage ApiResourceExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourceExistsKey),
                Description = "ApiResourceExistsKey"
            };
        }

        public virtual ResourceMessage ApiSecretDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiSecretDoesNotExist),
                Description = "ApiSecretDoesNotExist"
            };
        }

        public virtual ResourceMessage ApiResourcePropertyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourcePropertyDoesNotExist),
                Description = "ApiResourcePropertyDoesNotExist"
            };
        }

        public virtual ResourceMessage ApiResourcePropertyExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourcePropertyExistsKey),
                Description = "ApiResourcePropertyDoesNotExist"
            };
        }

        public virtual ResourceMessage ApiResourcePropertyExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourcePropertyExistsValue),
                Description = "ApiResourcePropertyExistsValue"
            };
        }
    }
}
