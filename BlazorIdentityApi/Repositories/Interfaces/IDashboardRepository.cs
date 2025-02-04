using System.Threading;
using System.Threading.Tasks;
using BlazorIdentityApi.Entities;

namespace BlazorIdentityApi.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardDataView> GetDashboardIdentityServerAsync(int auditLogsLastNumberOfDays,
        CancellationToken cancellationToken = default);
}