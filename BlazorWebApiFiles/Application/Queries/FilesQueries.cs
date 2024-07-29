using BlazorWebApi.Files.Data;

namespace eShop.Ordering.API.Application.Queries;

public class FilesQueries(FileDbContext context)
    : IFilesQueries
{
    public Task<string> GetPresignedAsync(string objectName)
    {
        throw new NotImplementedException();
    }
}
