using BlazorIdentityFiles.Entity._base;
using System.Linq.Expressions;

namespace BlazorIdentityFiles.SeedWork;

public interface IRepository<T> where T : IEntityBase
{

    void Add(T entity);
    void Add(IList<T> entity);

    void Update(T entity);

    void Delete(T entity);
    void Delete(IList<T> entities);
    void Delete(Expression<Func<T, bool>> where);

    T? GetById(Guid id);
    ValueTask<T?> GetByIdAsync(Guid id);
    IQueryable<T?> GetQueryById(Guid id);
    Task<TResult?> GetPropertyByRecordId<TResult>(Guid id, Expression<Func<T, TResult?>> selector);

    IQueryable<T> GetQuery(bool withDeleted = false);
    IQueryable<T> GetQuery(Expression<Func<T, bool>> where);


    IQueryable<T> GetQueryCustomQuery();
    IQueryable<T> GetQueryCustomQuery(Expression<Func<T, bool>> where);

    T? Refresh(T entity);

    Task<T?> FindAsync(Expression<Func<T?, bool>> predicate);

}
