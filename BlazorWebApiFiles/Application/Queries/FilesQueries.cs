using BlazorWebApi.Files.Data;
using Minio;
using Minio.DataModel.Args;

namespace BlazorWebApiFiles.Application.Queries;

public class FilesQueries(IMinioClient minioClient)
    : IFilesQueries
{
    public async Task<string> GetPresignedUserProfileAsync(GetPresignedUserProfileUrl getPresignedUrl)
    {

        string data = await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                 .WithObject(getPresignedUrl.ObjectName));

        return data;
    }
}
