using BlazorIdentity.Files.Data;
using BlazorIdentityFiles.Entity._base;

namespace BlazorIdentityFiles.Mediatr;

static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, FileDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<EntityBase>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
