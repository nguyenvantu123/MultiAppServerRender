﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using BlazorIdentityApi.Helpers;

namespace BlazorIdentityApi.Resources
{
    public class KeyServiceResources : IKeyServiceResources
    {
        public ResourceMessage KeyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(KeyDoesNotExist),
                Description = "KeyDoesNotExist"
            };
        }
    }
}