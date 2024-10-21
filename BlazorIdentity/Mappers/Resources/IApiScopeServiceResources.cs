// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using BlazorIdentityApi.Helpers;

namespace BlazorIdentityApi.Resources
{
    public interface IApiScopeServiceResources
    {
        ResourceMessage ApiScopeDoesNotExist();
        ResourceMessage ApiScopeExistsValue();
        ResourceMessage ApiScopeExistsKey();
        ResourceMessage ApiScopePropertyExistsValue();
        ResourceMessage ApiScopePropertyDoesNotExist();
        ResourceMessage ApiScopePropertyExistsKey();
    }
}