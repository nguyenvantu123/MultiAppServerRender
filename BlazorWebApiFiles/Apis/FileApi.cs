
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel;
using MultiAppServer.ServiceDefaults;
using Google.Protobuf.WellKnownTypes;
using Minio.DataModel.Encryption;
using BetkingLol.DataAccess.UnitOfWork;
using SixLabors.ImageSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BlazorIdentity.Files.Entities;
using BlazorIdentity.Files.Constant;
using BlazorIdentity.Files.Exceptions;

public static class FileApi
{
    public static RouteGroupBuilder MapFilesApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/files").HasApiVersion(1.0);

        api.MapGet("/get-presigned-url", GetPresignedAsync);
        api.MapPost("/upload-file", UploadFile).DisableAntiforgery();
        //api.MapGet("{orderId:int}", GetOrderAsync);
        //api.MapGet("/", GetOrdersByUserAsync);
        //api.MapGet("/cardtypes", GetCardTypesAsync);
        //api.MapPost("/draft", CreateOrderDraftAsync);
        //api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<ApiResponseDto<string>> GetPresignedAsync(
         [AsParameters] GetPresignedUserProfileUrl queries,
         [FromServices] FileServices services,
         [FromServices] IMinioClient minioClient,
         IHttpContextAccessor HttpContextAccessor
        )
    {
        var getObjectName = services.UnitOfWork.Repository<FileMapWithEntity>().GetQuery(x => x.RelationType == queries.RelationType && x.FileName == queries.ObjectName);
        if (queries.RelationType == "UserProfile")
        {

            string userIdString = HttpContextAccessor.HttpContext!.User.FindFirst("sub")!.Value;

            getObjectName = getObjectName.Where(x => x.RelationId.ToString() == userIdString);
        }
        else
        {
            getObjectName = getObjectName.Where(x => x.RelationId == queries.RelationId);
        }

        string objectName = (await getObjectName.FirstOrDefaultAsync())!.FileName;

        if (objectName.Equals(null) || string.IsNullOrEmpty(objectName))
        {
            return new ApiResponseDto<string>(400, "Not Found File", string.Empty);
        }

        string file = await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket("multiappserver").WithObject(objectName));

        return new ApiResponseDto<string>(200, "Success", "");
    }

    public static async Task<ApiResponseDto<bool>> UploadFile(
         IFormFile FormFile,
         [FromForm] FileType FileType,
         Guid? FolerId,
         [FromForm] string RelationType,
         [FromForm] Guid RelationId,
         FileServices services,
       [FromServices] IMinioClient minioClient)
    {

        if (FormFile != null && FormFile.Length > 0)
        {

            var memoryStream = new MemoryStream();
            await FormFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            PutObjectArgs putObjectArgs = new PutObjectArgs()
                                      .WithBucket("multiappserver")
                                      .WithStreamData(memoryStream)
                                      .WithObject(FormFile.FileName)
                                      .WithObjectSize(memoryStream.Length)
                                      .WithContentType(FormFile.ContentType);

            var dataUpload = await minioClient.PutObjectAsync(putObjectArgs);

            FileData fileData = new FileData();

            fileData.Name = FormFile.FileName;
            fileData.Size = FormFile.Length;
            if (HttpPostedFileBaseExtensions.IsImage(FormFile))
            {
                var fileBytes = memoryStream.ToArray();

                using Image image = Image.Load(fileBytes);

                fileData.Width = image.Width;
                fileData.Height = image.Height;
            }

            fileData.FileTypeData = FileType;
            fileData.Ext = Path.GetExtension(FormFile.FileName);

            fileData.Mime = HttpPostedFileBaseExtensions.GetMimeType(fileData.Ext);

            if (FolerId != Guid.Empty)
            {
                fileData.FolderId = FolerId;
            }
            else
            {
                fileData.FolderId = null;
            }

            services.UnitOfWork.Repository<FileData>().Add(fileData);


            FileMapWithEntity fileMapWithEntity = new FileMapWithEntity();
            fileMapWithEntity.RelationType = RelationType;
            fileMapWithEntity.RelationId = RelationId;
            fileMapWithEntity.FileName = dataUpload.ObjectName;
            fileMapWithEntity.FileId = fileData.Id;

            services.UnitOfWork.Repository<FileMapWithEntity>().Add(fileMapWithEntity);

            await services.UnitOfWork.SaveEntitiesAsync();

            return new ApiResponseDto<bool>(200, "Success!!!", true);
        }

        return new ApiResponseDto<bool>(400, "File Is Require!!!", false);
    }

    public static async Task<ApiResponseDto<string>> UploadProfile(
         IFormFile FormFile,
       [FromServices] IMinioClient minioClient)
    {

        if (FormFile != null && FormFile.Length > 0)
        {

            var memoryStream = new MemoryStream();
            await FormFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            PutObjectArgs putObjectArgs = new PutObjectArgs()
                                      .WithBucket("multiappserver")
                                      .WithStreamData(memoryStream)
                                      .WithObject(FormFile.FileName)
                                      .WithObjectSize(memoryStream.Length)
                                      .WithContentType(FormFile.ContentType);

            var dataUpload = await minioClient.PutObjectAsync(putObjectArgs);

            return new ApiResponseDto<string>(200, "Success!!!", "");
        }

        return new ApiResponseDto<string>(400, "File Is Require!!!", "");
    }
}
