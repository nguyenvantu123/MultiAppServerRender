public class FileServices(
    IMediator mediator,
    IOrderQueries queries,
    IIdentityService identityService,
    ILogger<FileServices> logger)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<FileServices> Logger { get; } = logger;
    public IOrderQueries Queries { get; } = queries;
    public IIdentityService IdentityService { get; } = identityService;
}
