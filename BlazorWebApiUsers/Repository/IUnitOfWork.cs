using System;
using System.Threading;
using System.Threading.Tasks;
using BlazorWeb.Contracts;

namespace BlazorWebApi.Users.Repository
{
    public interface IUnitOfWork<TId> : IDisposable
    {
        IRepositoryAsync<T, TId> Repository<T>() where T : class;

        Task<int> Commit(CancellationToken cancellationToken);

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}