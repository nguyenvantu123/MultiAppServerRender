// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Threading.Tasks;
using Skoruba.AuditLogging.Services;
using BlazorIdentityApi.Dtos.Log;
using BlazorIdentityApi.Events.Log;
using BlazorIdentityApi.Mappers;
using BlazorIdentityApi.Services.Interfaces;
using BlazorIdentityApi.Repositories.Interfaces;

namespace BlazorIdentityApi.Services
{
    public class LogService : ILogService
    {
        protected readonly ILogRepository Repository;
        protected readonly IAuditEventLogger AuditEventLogger;

        public LogService(ILogRepository repository, IAuditEventLogger auditEventLogger)
        {
            Repository = repository;
            AuditEventLogger = auditEventLogger;
        }

        public virtual async Task<LogsDto> GetLogsAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await Repository.GetLogsAsync(search, page, pageSize);
            var logs = pagedList.ToModel();

            await AuditEventLogger.LogEventAsync(new LogsRequestedEvent());

            return logs;
        }

        public virtual async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
        {
            await Repository.DeleteLogsOlderThanAsync(deleteOlderThan);

            await AuditEventLogger.LogEventAsync(new LogsDeletedEvent(deleteOlderThan));
        }
    }
}
