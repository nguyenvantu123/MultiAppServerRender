// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using BlazorIdentityApi.Helpers;

namespace BlazorIdentityApi.Resources
{
    public class ClientServiceResources : IClientServiceResources
    {
        public virtual ResourceMessage ClientClaimDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientClaimDoesNotExist),
                Description = "ClientClaimDoesNotExist"
            };
        }

        public virtual ResourceMessage ClientDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientDoesNotExist),
                Description = "ClientDoesNotExist"
            };
        }

        public virtual ResourceMessage ClientExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientExistsKey),
                Description = "ClientExistsKey"
            };
        }

        public virtual ResourceMessage ClientExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientExistsValue),
                Description = "ClientExistsValue"
            };
        }

        public virtual ResourceMessage ClientPropertyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientPropertyDoesNotExist),
                Description = "ClientPropertyDoesNotExist"
            };
        }

        public virtual ResourceMessage ClientSecretDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientSecretDoesNotExist),
                Description = "ClientSecretDoesNotExist"
            };
        }
    }
}