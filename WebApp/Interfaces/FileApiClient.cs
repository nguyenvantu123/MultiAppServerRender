using Azure.Core;
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

        public FileApiClient(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponseDto<string>> UploadFile(Guid? folderId, MultipartFormDataContent content)
        {

            //_httpClient.DefaultRequestHeaders
            //            .Accept
            //            .Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

            //_httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            //_httpClient.DefaultRequestHeaders.Add("Content-Type", "multipart/form-data");
            //_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkY3MzgzQTBFRDdDRDk2MEVGNDczRkQyODUxODAzOTM4IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3MjI5MzM1NjQsImV4cCI6MTcyMjkzNzE2NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzAyMSIsImNsaWVudF9pZCI6ImZpbGVzd2FnZ2VydWkiLCJzdWIiOiJkYjRiNmZkNi00Y2Q3LTRhY2YtYzVjMi0wOGRjOWIxOWU0MjciLCJhdXRoX3RpbWUiOjE3MjI5MTUwMDksImlkcCI6ImxvY2FsIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiYWRtaW4iLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJuZ3V5ZW52YW50dTAyMDc5NEBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwicGhvbmVfbnVtYmVyIjoiMDMzNDMzNjIzMiIsInBob25lX251bWJlcl92ZXJpZmllZCI6ZmFsc2UsImp0aSI6IkNDOTQ4N0M4NDlBNTA4M0QyOTM2Q0U2Rjc3MEE5QjY2Iiwic2lkIjoiNjMzNzhDNzU1NUIzOTA2MzhFQjMyMUY3QjM0NzU0MTUiLCJpYXQiOjE3MjI5MzM1NjQsInNjb3BlIjpbImZpbGVzIl0sImFtciI6WyJwd2QiXX0.mif-na7OfrjSMas1kk_x99JPK4A1nBxobBnYRTjMXlYsJiW4Zd7bHM0wVLetSZbpNJeuMtdY8ntIOiVu7PwA73Dh0GvYZyw75ZB9bKRvbrh-aJjc-gj11JSDlfT43MTJpuPt6r6Dcc_vnBouuQGPp57kfbzNrrhO4Lamiqz-Fvf5NCibIMfyPhz3DjcdNO0SvrRjkeYpU1IhpdZHwng602cZ5UjptuTb7eWUt7PZ7eqjn58BczNp5Q7ZKWxBTQP02NVyfyGUYJ3vzXf2Kr2SZY0l8Lu76RlL5mc4uycMBV8SVnguDg1yUIZe9yF3S_8ehGJlPHMNe--vJZCGbfdxtA");

            //Content - Type: multipart / form - data
            //var content1 = new MultipartFormDataContent();
            //content.Add(new StringContent("string"), "FileType");
            //content.Add(new StringContent("string"), "RelationType");
            //content.Add(new StringContent("3fa85f64-5717-4562-b3fc-2c963f66afa6"), "RelationId");

            return await _httpClient.PostFileAsync<ApiResponseDto<string>>($"api/files/upload-file", content);
        }

        public async Task<ApiResponseDto<string>> GetPresignedUrl(GetPresignedUserProfileModel presignedUserProfileUrl)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponseDto<string>>($"/api/files/get-presigned-url?RelationType={presignedUserProfileUrl.RelationType}&ObjectName={presignedUserProfileUrl.ObjectName}&RelationId={(presignedUserProfileUrl.RelationId.HasValue ? presignedUserProfileUrl.RelationId.Value.ToString() : "")}");
        }
    }
}
