using Azure;
using Azure.Core;
using BlazorIdentity.Files.Application.Commands;
using BlazorIdentity.Files.Application.Queries;
using BlazorIdentity.Files.Constant;
using BlazorIdentity.Files.CQRS.Query;
using BlazorIdentity.Files.Entities;
using BlazorIdentity.Files.Excel;
using BlazorIdentity.Files.Response;
using BlazorIdentityFiles.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using MultiAppServer.ServiceDefaults;
using Newtonsoft.Json;
using Syncfusion.Blazor;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.EJ2.DocumentEditor;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BlazorIdentity.Files.Apis
{
    public static class DocumentManagerApi
    {
        //[Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public static RouteGroupBuilder MapDocumentApiV1(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("api/admins").HasApiVersion(1.0);

            api.MapGet("/document-type", GetListDocument);
            api.MapGet("/export-excel", ExportDocumentListToExcel); // Add this line
            api.MapPost("/document-type", CreateDocumentType).DisableAntiforgery(); // Add this line
            api.MapPut("/document-type/{id}", UpdateDocumentType); // Add this line
            api.MapPost("/document-type/{id}/upload-again", UploadAgain).DisableAntiforgery(); // Add this line
            api.MapGet("/document-type/{id}/history", UploadHistory);

            api.MapGet("/document-type/{fileUrl}/get-content-file", LoadFromPresignedUrl).DisableAntiforgery();
            // Add this line


            return api;
        }

        [Authorize]
        public static async Task<ApiResponseDto<List<DocumentResponse>>> GetListDocument(
             [AsParameters] GetListDocumentQuery queries, [FromServices] IMediator mediator
            )
        {
            return await mediator.Send(queries);
        }

        [Authorize]
        public static async Task<IActionResult> ExportDocumentListToExcel(
             [AsParameters] GetListDocumentQuery queries, [FromServices] IMediator mediator)
        {
            var result = await mediator.Send(queries);

            // Generate the Excel file using the template
            var excelGenerator = new ExcelGenerator();

            var cellDataList = new List<ExcelCellData>
            {
                new ExcelCellData { CellAddress = "A8", Value = "Value1" },
                new ExcelCellData { CellAddress = "B8", Value = "Value2" },
                // Add more cell data as needed
            };

            var templatePath = "path/to/your/template.xlsx"; // Update with the actual template path
            var excelFile = excelGenerator.GenerateExcelFromTemplate<DocumentResponse>(templatePath: templatePath, data: result.Result, startRow: 2, cellDataList, null);

            return new FileContentResult(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "DocumentList.xlsx"
            };
        }

        // create new document type using cqrs
        //[Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        [Authorize]
        public static async Task<ApiResponseDto<bool>> CreateDocumentType(
     IFormFile? file, [FromForm] string name, [FromForm] string? description, [FromForm] bool isActive, [FromServices] IMediator mediator)
        {
            string fileUrl = string.Empty;

            if (file != null)
            {
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);

                var response = await mediator.Send(new UploadFileCommand { Content = content });
                if (response.IsSuccessStatusCode)
                {
                    fileUrl = response.Result;
                }
            }

            var command = new CreateDocumentTypeCommand
            {
                Name = name,
                Description = description,
                IsActive = isActive,
                LinkUrl = fileUrl
            };

            return await mediator.Send(command);
        }

        public static async Task<ApiResponseDto<bool>> UpdateDocumentType(
      Guid id, [FromBody] UpdateDocumentTypeCommand command, [FromServices] IMediator mediator)
        {
            if (id != command.Id)
            {
                return new ApiResponseDto<bool>(400, "Id is not null", false, 0);
            }

            command.Id = id;

            return await mediator.Send(command);
        }

        public static async Task<ApiResponseDto<string>> UploadAgain(
     Guid id, IFormFile? file, [FromServices] IMediator mediator)
        {

            string fileUrl = string.Empty;

            if (file != null)
            {
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);

                var response = await mediator.Send(new UploadFileCommand { Content = content });
                if (response.IsSuccessStatusCode)
                {
                    fileUrl = response.Result;

                    var command = new UploadAgainCommand
                    {
                        Id = id,
                        FilePath = fileUrl
                    };

                    return await mediator.Send(command);
                }
            }

            return new ApiResponseDto<string>(400, "File is not null", string.Empty, 0);

        }

        public static async Task<ApiResponseDto<List<UploadHistoryResponse>>> UploadHistory(Guid id, [FromServices] IMediator mediator)
        {
            var query = new GetUploadHistoryQuery
            {
                Id = id
            };
            var result = await mediator.Send(query);
            if (result.IsSuccessStatusCode)
            {
                return result;
            }
            else
            {
                return new ApiResponseDto<List<UploadHistoryResponse>>(400, "Error", null, 0);
            }
        }


        public static async Task<ApiResponseDto<string>> LoadFromPresignedUrl(string fileUrl)
        {
            //using var httpClient = new HttpClient();

            //string decodedFileName = HttpUtility.UrlDecode(fileUrl);
            //var response = await httpClient.GetAsync(decodedFileName);
            //if (!response.IsSuccessStatusCode)
            //{
            //    return new ApiResponseDto<string>(400, "", null);
            //}

            //await using var inputStream = await response.Content.ReadAsStreamAsync();
            //using var memoryStream = new MemoryStream();
            //await inputStream.CopyToAsync(memoryStream);
            //memoryStream.Position = 0;

            //var wordDocument = WordDocument.Load(memoryStream, FormatType.Docx);
            //string sfdtText = JsonConvert.SerializeObject(wordDocument);
            //wordDocument.Dispose();

            //return new ApiResponseDto<string>(200, "", sfdtText);

            if (string.IsNullOrWhiteSpace(fileUrl))
                return new ApiResponseDto<string>(400, "", "Presigned URL is required");

            try
            {

                string decodedFileName = HttpUtility.UrlDecode(fileUrl);
                //using var httpClient = new HttpClient();
                //// Step 1: Download DOCX from presigned URL
                //var response = await httpClient.GetAsync(decodedFileName);
                //if (!response.IsSuccessStatusCode)
                //    return new ApiResponseDto<string>(400, "", null);

                //var stream = await response.Content.ReadAsStreamAsync();
                //stream.Position = 0;

                //using WordDocument wordDoc = new WordDocument(stream, FormatType.Docx);
                //string sfdtText = WordDocument.SaveAsSfdt(wordDoc); // From Syncfusion.EJ2.DocumentEditor
                //wordDoc.Dispose();
                //return new ApiResponseDto<string>(200, "", sfdtText);

                using var httpClient = new HttpClient();

               var response = await httpClient.GetAsync(decodedFileName);
                if (!response.IsSuccessStatusCode)
                    return new ApiResponseDto<string>(400, "", null);

                var stream = await response.Content.ReadAsStreamAsync();
                if (stream.CanSeek) stream.Position = 0;

                using Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Docx);

                var sfdt = new  SfdtExport().ExportToSfdt(document); // ✅ Works in latest versions

                return new ApiResponseDto<string>(200, "", sfdt);
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>(500, "", ex.Message);

                //return StatusCode(500, $"Error processing file: {ex.Message}");
            }

        }
    }
}
