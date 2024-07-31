

using BlazorWebApi.Files.Data;
using BlazorWebApiFiles.Entity._base;

namespace BlazorWebApiFiles.SeedWork;

public class Repository<T> : RepositoryBase<T, FileDbContext> where T : class, IEntityBase
{
    public Repository(FileDbContext dataContext) : base(dataContext)
    {
    }
}
