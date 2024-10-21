// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using BlazorIdentityApi.Helpers;

namespace BlazorIdentityApi.Resources
{
    public class PersistedGrantServiceResources : IPersistedGrantServiceResources
    {
        public virtual ResourceMessage PersistedGrantDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(PersistedGrantDoesNotExist),
                Description = "PersistedGrantDoesNotExist"
            };
        }

        public virtual ResourceMessage PersistedGrantWithSubjectIdDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(PersistedGrantWithSubjectIdDoesNotExist),
                Description = "PersistedGrantWithSubjectIdDoesNotExist"
            };
        }
    }
}
