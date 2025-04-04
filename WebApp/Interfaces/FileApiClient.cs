
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MultiAppServer.ServiceDefaults;
using System.Net.Http.Headers;
using WebApp.Extensions;
using WebApp.Models;

namespace WebApp.Interfaces
{
    public class FileApiClient
    {

        private readonly HttpClient _httpClient;

        public FileApiClient(IHttpClientFactory httpClientFactory, HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClientFactory.CreateClient("FileApi");
        }

        public async Task<ApiResponseDto<string>> GetPresignedUrl(GetPresignedUserProfileModel presignedUserProfileUrl)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponseDto<string>>($"/api/files/get-presigned-url?RelationType={presignedUserProfileUrl.RelationType}&ObjectName={presignedUserProfileUrl.ObjectName}&RelationId={(presignedUserProfileUrl.RelationId.HasValue ? presignedUserProfileUrl.RelationId.Value.ToString() : "")}");
        }

        public async Task<ApiResponseDto<List<DocumentsTypes>>> GetDocumentType(int pageSize, int currentPage, string search)
        {
            return await _httpClient.GetJsonAsync<ApiResponseDto<List<DocumentsTypes>>>($"api/admins/document-type?pageSize={pageSize}&pageNumber={currentPage}&search={search}");
        }

        public async Task<ApiResponseDto<bool>> CreateDocumentType(IBrowserFile file, string name, string description, bool isActive)
        {
            var content = new MultipartFormDataContent();
            if (file != null)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);
            }
            content.Add(new StringContent(name), "name");
            content.Add(new StringContent(description), "description");
            content.Add(new StringContent(isActive.ToString()), "isActive");

            var response = await _httpClient.PostAsync("/api/admins/document-type", content);
            var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

            return responseData ?? new ApiResponseDto<bool>(500, "Error", false);
        }
    }
}
