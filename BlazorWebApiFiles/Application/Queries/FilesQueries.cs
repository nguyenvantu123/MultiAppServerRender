using BetkingLol.Domain.Request.Queries.BankInfoByUser;
using BlazorWebApi.Files.Data;
using Minio;
using Minio.DataModel.Args;

namespace eShop.Ordering.API.Application.Queries;

public class FilesQueries([AsParameters] FileServices services,
        IMinioClient minioClient)
    : IFilesQueries
{
    public async Task<string> GetPresignedUserProfileAsync(GetPresignedUserProfileUrl getPresignedUrl)
    {

        string data = await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                 .WithObject(getPresignedUrl.ObjectName));

        return data;
    }
}
