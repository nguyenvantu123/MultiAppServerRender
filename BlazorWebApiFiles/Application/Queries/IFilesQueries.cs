using BlazorWebApi.Files.Entities;
using MultiAppServer.ServiceDefaults;

namespace BlazorWebApiFiles.Application.Queries;

public interface IFilesQueries
{
    Task<string> GetPresignedUserProfileAsync(GetPresignedUserProfileUrl getPresignedUrl);
}
