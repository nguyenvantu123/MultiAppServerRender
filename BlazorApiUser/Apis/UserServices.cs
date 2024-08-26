

using MediatR;

public class UserServices(
    IMediator mediator,
    ILogger<UserServices> logger)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<UserServices> Logger { get; } = logger;
}
