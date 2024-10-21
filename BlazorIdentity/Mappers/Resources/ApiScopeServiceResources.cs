// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using BlazorIdentityApi.Helpers;

namespace BlazorIdentityApi.Resources
{
    public class ApiScopeServiceResources : IApiScopeServiceResources
    {
        public virtual ResourceMessage ApiScopeDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopeDoesNotExist),
                Description = "ApiScopeDoesNotExist"
            };
        }

        public virtual ResourceMessage ApiScopeExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopeExistsValue),
                Description = "ApiScopeExistsValue"
            };
        }

        public virtual ResourceMessage ApiScopeExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopeExistsKey),
                Description = "ApiScopeExistsKey"
            };
        }

        public ResourceMessage ApiScopePropertyExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopePropertyExistsValue),
                Description = "ApiScopePropertyExistsValue"
            };
        }

        public ResourceMessage ApiScopePropertyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopePropertyDoesNotExist),
                Description = "ApiScopePropertyDoesNotExist"
            };
        }

        public ResourceMessage ApiScopePropertyExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopePropertyExistsKey),
                Description = "ApiScopePropertyExistsKey"
            };
        }
    }
}