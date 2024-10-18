// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Threading.Tasks;
using BlazorIdentityApi.Entities;
using BlazorIdentityApi.Extensions.Common;

namespace BlazorIdentityApi.Repositories.Interfaces
{
    public interface ILogRepository
    {
        Task<PagedList<Log>> GetLogsAsync(string search, int page = 1, int pageSize = 10);

        Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan);

        bool AutoSaveChanges { get; set; }
    }
}