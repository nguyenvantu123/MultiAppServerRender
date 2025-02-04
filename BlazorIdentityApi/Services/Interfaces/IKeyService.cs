// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading;
using System.Threading.Tasks;
using BlazorIdentityApi.Dtos.Grant;
using BlazorIdentityApi.Dtos.Key;

namespace BlazorIdentityApi.Services.Interfaces
{
    public interface IKeyService
    {
        Task<KeysDto> GetKeysAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<KeyDto> GetKeyAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsKeyAsync(string id, CancellationToken cancellationToken = default);
        Task DeleteKeyAsync(string id, CancellationToken cancellationToken = default);
    }
}