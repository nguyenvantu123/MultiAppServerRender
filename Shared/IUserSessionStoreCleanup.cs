// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Shared;


/// <summary>
/// User session store cleanup
/// </summary>
public interface IUserSessionStoreCleanup
{
    /// <summary>
    /// Deletes expired sessions
    /// </summary>
    Task DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default);
}
