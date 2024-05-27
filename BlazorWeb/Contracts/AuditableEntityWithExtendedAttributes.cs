namespace BlazorWeb.Contracts
{
    public sealed class AuditableEntityWithExtendedAttributes<TId, TEntityId, TEntity, TExtendedAttribute>
    {
        public ICollection<TExtendedAttribute> ExtendedAttributes { get; set; } = new HashSet<TExtendedAttribute>();
    }
}