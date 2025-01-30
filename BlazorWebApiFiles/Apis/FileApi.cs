
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel;
using MultiAppServer.ServiceDefaults;
using Google.Protobuf.WellKnownTypes;
using Minio.DataModel.Encryption;
using BlazorIdentityFiles.Application.Commands;
using BlazorIdentity.Files.Entities;
using BlazorIdentity.Files.Exceptions;
using BetkingLol.DataAccess.UnitOfWork;
using SixLabors.ImageSharp;
using BlazorIdentity.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BlazorIdentity.Files.Constant;

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

        //string file = await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket("multiappbucket").WithObject(objectName).WithExpiry(60 * 60));

        return new ApiResponseDto<string>(200, "Success", "");
    }

    public static async Task<ApiResponseDto<string>> UploadFile(
         //IFormFile FormFile,
         [FromForm] string FileType,
         Guid? FolerId,
         [FromForm] string RelationType,
         [FromForm] Guid RelationId,
         FileServices services)
    {

        //if (FormFile != null && FormFile.Length > 0)
        //{

            var memoryStream = new MemoryStream();
            //await FormFile.CopyToAsync(memoryStream);
            //memoryStream.Position = 0;

            //string fileUrl = "img/file/" + FormFile.FileName;

            //PutObjectArgs putObjectArgs = new PutObjectArgs()
            //                          .WithBucket("multiappbucket")
            //                          .WithStreamData(memoryStream)
            //                          .WithObject(fileUrl)
            //                          .WithObjectSize(memoryStream.Length)
            //                          .WithVersionId("1.0")
            //                          .WithContentType(FormFile.ContentType);

            //var dataUpload = await minioClient.PutObjectAsync(putObjectArgs);

            //FileData fileData = new FileData();

            //fileData.Name = FormFile.FileName;
            //fileData.Size = FormFile.Length;
            //fileData.FileUrl = fileUrl;
            //if (HttpPostedFileBaseExtensions.IsImage(FormFile))
            //{
            //    var fileBytes = memoryStream.ToArray();

            //    using Image image = Image.Load(fileBytes);

            //    fileData.Width = image.Width;
            //    fileData.Height = image.Height;
            //}

            //fileData.FileTypeData = FileType;
            //fileData.Ext = Path.GetExtension(FormFile.FileName);

            //fileData.Mime = HttpPostedFileBaseExtensions.GetMimeType(fileData.Ext);

            //if (FolerId.HasValue)
            //{
            //    fileData.FolderId = FolerId;
            //}
            //else
            //{
            //    fileData.FolderId = null;
            //}

            //services.UnitOfWork.Repository<FileData>().Add(fileData);


            //FileMapWithEntity fileMapWithEntity = new FileMapWithEntity();
            //fileMapWithEntity.RelationType = RelationType;
            //fileMapWithEntity.RelationId = RelationId;
            //fileMapWithEntity.FileName = fileUrl;
            //fileMapWithEntity.FileId = fileData.Id;

            //services.UnitOfWork.Repository<FileMapWithEntity>().Add(fileMapWithEntity);

            //await services.UnitOfWork.SaveEntitiesAsync();

            return new ApiResponseDto<string>(200, "Success!!!", "");
        //}

        //return new ApiResponseDto<string>(400, "File Is Require!!!", "");
    }

    //public static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(int orderId, [AsParameters] FileServices services)
    //{
    //    try
    //    {
    //        var order = await services.Queries.GetOrderAsync(orderId);
    //        return TypedResults.Ok(order);
    //    }
    //    catch
    //    {
    //        return TypedResults.NotFound();
    //    }
    //}

    //public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync([AsParameters] FileServices services)
    //{
    //    var userId = services.IdentityService.GetUserIdentity();
    //    var orders = await services.Queries.GetOrdersFromUserAsync(userId);
    //    return TypedResults.Ok(orders);
    //}

    //public static async Task<Ok<IEnumerable<CardType>>> GetCardTypesAsync(IOrderQueries orderQueries)
    //{
    //    var cardTypes = await orderQueries.GetCardTypesAsync();
    //    return TypedResults.Ok(cardTypes);
    //}

    //public static async Task<OrderDraftDTO> CreateOrderDraftAsync(CreateOrderDraftCommand command, [AsParameters] FileServices services)
    //{
    //    services.Logger.LogInformation(
    //        "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
    //        command.GetGenericTypeName(),
    //        nameof(command.BuyerId),
    //        command.BuyerId,
    //        command);

    //    return await services.Mediator.Send(command);
    //}

    //public static async Task<Results<Ok, BadRequest<string>>> CreateOrderAsync(
    //    [FromHeader(Name = "x-requestid")] Guid requestId,
    //    CreateOrderRequest request,
    //    [AsParameters] FileServices services)
    //{

    //    //mask the credit card number

    //    services.Logger.LogInformation(
    //        "Sending command: {CommandName} - {IdProperty}: {CommandId}",
    //        request.GetGenericTypeName(),
    //        nameof(request.UserId),
    //        request.UserId); //don't log the request as it has CC number

    //    if (requestId == Guid.Empty)
    //    {
    //        services.Logger.LogWarning("Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}", request);
    //        return TypedResults.BadRequest("RequestId is missing.");
    //    }

    //    using (services.Logger.BeginScope(new List<KeyValuePair<string, object>> { new("IdentifiedCommandId", requestId) }))
    //    {
    //        var maskedCCNumber = request.CardNumber.Substring(request.CardNumber.Length - 4).PadLeft(request.CardNumber.Length, 'X');
    //        var createOrderCommand = new CreateOrderCommand(request.Items, request.UserId, request.UserName, request.City, request.Street,
    //            request.State, request.Country, request.ZipCode,
    //            maskedCCNumber, request.CardHolderName, request.CardExpiration,
    //            request.CardSecurityNumber, request.CardTypeId);

    //        var requestCreateOrder = new IdentifiedCommand<CreateOrderCommand, bool>(createOrderCommand, requestId);

    //        services.Logger.LogInformation(
    //            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
    //            requestCreateOrder.GetGenericTypeName(),
    //            nameof(requestCreateOrder.Id),
    //            requestCreateOrder.Id,
    //            requestCreateOrder);

    //        var result = await services.Mediator.Send(requestCreateOrder);

    //        if (result)
    //        {
    //            services.Logger.LogInformation("CreateOrderCommand succeeded - RequestId: {RequestId}", requestId);
    //        }
    //        else
    //        {
    //            services.Logger.LogWarning("CreateOrderCommand failed - RequestId: {RequestId}", requestId);
    //        }

    //        return TypedResults.Ok();
    //    }
    //}
}
