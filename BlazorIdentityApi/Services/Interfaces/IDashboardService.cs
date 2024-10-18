using System.Threading;
using System.Threading.Tasks;
using BlazorIdentityApi.Dtos.Dashboard;

namespace BlazorIdentityApi.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardIdentityServerAsync(int auditLogsLastNumberOfDays,
        CancellationToken cancellationToken = default);
}