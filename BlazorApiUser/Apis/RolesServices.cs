

using BlazorIdentity.Data;
using MediatR;

public class UserServices(
    IMediator mediator,
    ApplicationDbContext context,
    ILogger<UserServices> logger)
{
    public IMediator Mediator { get; } = mediator;
    public ILogger<UserServices> Logger { get; } = logger;

    public ApplicationDbContext Context { get; } = context;
}
