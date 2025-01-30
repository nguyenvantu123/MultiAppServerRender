

using BlazorIdentity.Files.Data;
using BlazorIdentityFiles.Entity._base;

namespace BlazorIdentityFiles.SeedWork;

public class Repository<T> : RepositoryBase<T, FileDbContext> where T : class, IEntityBase
{
    public Repository(FileDbContext dataContext) : base(dataContext)
    {
    }
}
