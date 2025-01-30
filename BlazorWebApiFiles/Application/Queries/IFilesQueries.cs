using BlazorIdentity.Files.Entities;
using MultiAppServer.ServiceDefaults;

namespace BlazorIdentityFiles.Application.Queries;

public interface IFilesQueries
{
    Task<string> GetPresignedUserProfileAsync();
}
