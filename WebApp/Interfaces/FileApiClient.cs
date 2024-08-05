using BlazorWebApiFiles.Application.Queries;
using Microsoft.JSInterop;
using MultiAppServer.ServiceDefaults;
using WebApp.Extensions;

namespace WebApp.Interfaces
{
    public class FileApiClient
    {

        private readonly HttpClient _httpClient;

        public FileApiClient(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponseDto<string>> UploadFile(Guid? folderId, MultipartFormDataContent content)
        {
            return await _httpClient.PostJsonAsync<ApiResponseDto<string>>($"/api/files/upload-file", content);
        }

        public async Task<ApiResponseDto<string>> GetPresignedUrl(GetPresignedUserProfileUrl presignedUserProfileUrl)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponseDto<string>>($"/api/files/get-presigned-url?RelationType={presignedUserProfileUrl.RelationType}&ObjectName={presignedUserProfileUrl.ObjectName}&RelationId={(presignedUserProfileUrl.RelationId.HasValue ? presignedUserProfileUrl.RelationId.Value.ToString() : "")}");
        }
    }
}
