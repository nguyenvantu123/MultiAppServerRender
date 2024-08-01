
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel;
using MultiAppServer.ServiceDefaults;
using Google.Protobuf.WellKnownTypes;
using Minio.DataModel.Encryption;
using BlazorWebApiFiles.Application.Commands;
using BlazorWebApi.Files.Entities;
using BlazorWebApi.Files.Exceptions;
using BetkingLol.DataAccess.UnitOfWork;
using SixLabors.ImageSharp;
using BlazorWebApi.Repository;
using BetkingLol.Domain.Request.Queries.BankInfoByUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public static class FileApi
{
    public static RouteGroupBuilder MapFilesApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/files").HasApiVersion(1.0);

        api.MapGet("/getPresignedUrl", GetPresignedAsync);
        api.MapPost("/uploadFile", UploadFile);
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
         IMinioClient minioClient,
         IHttpContextAccessor HttpContextAccessor
        )
    {
        var getObjectName = services.UnitOfWork.Repository<FileMapWithEntity>().GetQuery(x => x.RelationType == queries.RelationType && x.FileName == queries.ObjectName);
        if (queries.RelationType == "UserProfile")
        {

            string userIdString = HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

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

        string file = await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket("multiappbucket").WithObject(objectName));

        return new ApiResponseDto<string>(200, "Success", "");
    }

    public static async Task<ApiResponseDto<bool>> UploadFile(
        [AsParameters] UploadFileCommand command,
        [FromServices] FileServices services,
        IMinioClient minioClient)
    {

        if (command.FormFile != null && command.FormFile.Length > 0)
        {

            var memoryStream = new MemoryStream();
            await command.FormFile.CopyToAsync(memoryStream);

            PutObjectArgs putObjectArgs = new PutObjectArgs()
                                      .WithBucket("multiapp")
                                      .WithStreamData(memoryStream)
                                      .WithObject(command.FormFile.FileName)
                                      .WithFileName(command.FormFile.FileName)
                                      .WithContentType(command.FormFile.ContentType);

            var dataUpload = await minioClient.PutObjectAsync(putObjectArgs);

            FileData fileData = new FileData();

            fileData.Name = command.FormFile.FileName;
            fileData.Size = command.FormFile.Length;
            if (HttpPostedFileBaseExtensions.IsImage(command.FormFile))
            {
                var fileBytes = memoryStream.ToArray();

                using Image image = Image.Load(fileBytes);

                fileData.Width = image.Width;
                fileData.Height = image.Height;
            }

            fileData.FileTypeData = command.FileType;
            fileData.Ext = Path.GetExtension(command.FormFile.FileName);

            fileData.Mime = HttpPostedFileBaseExtensions.GetMimeType(fileData.Ext);

            if (command.FolerId.HasValue)
            {
                fileData.FolderId = command.FolerId;
            }

            services.UnitOfWork.Repository<FileData>().Add(fileData);


            FileMapWithEntity fileMapWithEntity = new FileMapWithEntity();
            fileMapWithEntity.RelationType = command.RelationType;
            fileMapWithEntity.RelationId = command.RelationId;
            fileMapWithEntity.FileName = dataUpload.ObjectName;
            fileMapWithEntity.FileId = fileData.Id;

            await services.UnitOfWork.SaveEntitiesAsync();

            return new ApiResponseDto<bool>(200, "Success!!!", true);
        }

        return new ApiResponseDto<bool>(400, "File Is Require!!!", false);
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
