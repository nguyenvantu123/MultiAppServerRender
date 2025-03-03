
namespace Shared;

public interface ISessionRevocationService
{
    /// <summary>
    /// Revokes a user session
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns></returns>
    Task RevokeSessionsAsync(UserSessionsFilter filter, CancellationToken cancellationToken = default);
}
