

using BlazorIdentity.Data;
using MediatR;

public class RolesServices(
    IMediator mediator,
    ApplicationDbContext context,
    ILogger<RolesServices> logger)
{
    public IMediator Mediator { get; } = mediator;
    public ILogger<RolesServices> Logger { get; } = logger;

    public ApplicationDbContext Context { get; } = context;
}
