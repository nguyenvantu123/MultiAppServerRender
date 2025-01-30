using BlazorIdentity.Files.Data;
using Minio;
using Minio.DataModel.Args;

namespace BlazorIdentityFiles.Application.Queries;

public class FilesQueries()
    : IFilesQueries
{
    public async Task<string> GetPresignedUserProfileAsync()
    {

        return "";
    }
}
