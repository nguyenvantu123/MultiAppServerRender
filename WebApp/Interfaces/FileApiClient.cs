
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MultiAppServer.ServiceDefaults;
using System.Net.Http.Headers;
using WebApp.Extensions;
using WebApp.Models;
using static WebApp.Components.Pages.Document.DocumentType;

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

        public async Task<HttpResponseMessage> UpdateDocumentType(DocumentsTypes documentType)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admins/document-type/{documentType.Id}", documentType);
            return response;
        }

        public async Task<ApiResponseDto<List<UploadHistoryModel>>> GetUploadHistory(Guid documentId)
        {
            return await _httpClient.GetJsonAsync<ApiResponseDto<List<UploadHistoryModel>>>($"api/admins/document-type/{documentId}/history");
        }

        public async Task<ApiResponseDto<string>> UploadFileAgain(Guid documentId, IBrowserFile file)
        {
            var content = new MultipartFormDataContent();
            if (file != null)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);
            }

            var response = await _httpClient.PostAsync($"api/admin/document-type/{documentId}/upload-again", content);
            var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>();

            return responseData ?? new ApiResponseDto<string>(500, "Error", "");
        }

        public async Task<ApiResponseDto<string>> GetContentDocx(string fileUrl)
        {
         /// document - type /{ fileUrl}/ get - content - file
            var response = await _httpClient.GetAsync($"/api/admins/document-type/{Uri.EscapeDataString(fileUrl)}/get-content-file");
            var responseData = await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>();

            return responseData ?? new ApiResponseDto<string>(500, "Error", "");
        }
    }
}
