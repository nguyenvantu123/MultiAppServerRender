namespace BlazorApiUser.Repository;

public interface IRepository<T>
{
    IUnitOfWork UnitOfWork { get; }
}
