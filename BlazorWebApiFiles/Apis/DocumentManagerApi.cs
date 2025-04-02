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
using System.Net.Http.Headers;

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
            api.MapPost("/document-type", CreateDocumentType); // Add this line
            api.MapPut("/document-type", UpdateDocumentType); // Add this line


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
         [FromBody] CreateDocumentTypeCommand command, [FromServices] IMediator mediator)
        {
            return await mediator.Send(command);
        }
    }
}
