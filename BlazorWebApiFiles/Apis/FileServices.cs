public class FileServices(
    IMediator mediator,
    IFilesQueries queries,
    IIdentityService identityService,
    ILogger<FileServices> logger)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<FileServices> Logger { get; } = logger;
    public IFilesQueries Queries { get; } = queries;
    public IIdentityService IdentityService { get; } = identityService;
}
