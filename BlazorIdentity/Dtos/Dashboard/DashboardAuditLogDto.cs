using System;

namespace BlazorIdentityApi.Dtos.Dashboard;

public class DashboardAuditLogDto
{
    public int Total { get; set; }

    public DateTime Created { get; set; }
}