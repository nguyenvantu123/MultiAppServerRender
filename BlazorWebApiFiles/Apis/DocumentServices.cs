using BlazorIdentityFiles.SeedWork;

namespace BlazorIdentity.Files.Apis
{

    public class DocumentServices(
    IFilesQueries queries,
    IIdentityService identityService,
    ILogger<DocumentServices> logger,
    IUnitOfWork unitOfWork)
    {
        public ILogger<DocumentServices> Logger { get; } = logger;
        public IFilesQueries Queries { get; } = queries;
        public IIdentityService IdentityService { get; } = identityService;

        public IUnitOfWork UnitOfWork { get; set; } = unitOfWork;
    }

}
