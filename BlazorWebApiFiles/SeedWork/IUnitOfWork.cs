using BlazorWebApi.Files.Data;
using BlazorWebApiFiles.Entity._base;

namespace BlazorWebApiFiles.SeedWork;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    FileDbContext GetDatabaseContext();

    IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntityBase;

}
