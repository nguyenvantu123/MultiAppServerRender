
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel;
using MultiAppServer.ServiceDefaults;

public static class FileApi
{
    public static RouteGroupBuilder MapFilesApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/files").HasApiVersion(1.0);

        api.MapGet("/getPresignedUrl", GetPresignedAsync);
        //api.MapPut("/ship", ShipOrderAsync);
        //api.MapGet("{orderId:int}", GetOrderAsync);
        //api.MapGet("/", GetOrdersByUserAsync);
        //api.MapGet("/cardtypes", GetCardTypesAsync);
        //api.MapPost("/draft", CreateOrderDraftAsync);
        //api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<ApiResponseDto<string>> GetPresignedAsync(
        string objectName,
        [AsParameters] FileServices services,
        IMinioClient minioClient)
    {
        //    return Ok(await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
        //           .WithBucket(bucketID))
        //       .ConfigureAwait(false));

        string file = await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket("multiappbucket").WithObject(objectName));

        return new ApiResponseDto<string>(200, "Success", file);
    }

    //public static async Task<Results<Ok, BadRequest<string>, ProblemHttpResult>> ShipOrderAsync(
    //    [FromHeader(Name = "x-requestid")] Guid requestId,
    //    ShipOrderCommand command,
    //    [AsParameters] FileServices services)
    //{
    //    if (requestId == Guid.Empty)
    //    {
    //        return TypedResults.BadRequest("Empty GUID is not valid for request ID");
    //    }

    //    var requestShipOrder = new IdentifiedCommand<ShipOrderCommand, bool>(command, requestId);

    //    services.Logger.LogInformation(
    //        "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
    //        requestShipOrder.GetGenericTypeName(),
    //        nameof(requestShipOrder.Command.OrderNumber),
    //        requestShipOrder.Command.OrderNumber,
    //        requestShipOrder);

    //    var commandResult = await services.Mediator.Send(requestShipOrder);

    //    if (!commandResult)
    //    {
    //        return TypedResults.Problem(detail: "Ship order failed to process.", statusCode: 500);
    //    }

    //    return TypedResults.Ok();
    //}

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
