
using BlazorWebApi.Repository;

namespace BlazorWebApiFiles.Application.Commands;

// Regular CommandHandler
public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, bool>
{
    private readonly IFileRepository _fileRepository;

    public UploadFileCommandHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<bool> Handle(UploadFileCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _fileRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetCancelledStatus();
        return await _fileRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}


// Use for Idempotency in Command process
public class CancelOrderIdentifiedCommandHandler : IdentifiedCommandHandler<UploadFileCommand, bool>
{
    public CancelOrderIdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<UploadFileCommand, bool>> logger)
        : base(mediator, requestManager, logger)
    {
    }

    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}
