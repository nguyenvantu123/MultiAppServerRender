using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorWebApi.Files.Data;
using BlazorWebApiFiles.Entity._base;
using BlazorWebApiFiles.Mediatr;
using BlazorWebApiFiles.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace BetkingLol.DataAccess.UnitOfWork
{
    //
    // Summary:
    //     A abstract class provide functions/DbSets to work with database
    public class UnitOfWork : IUnitOfWork
    {
        protected FileDbContext DataContext { get; set; }
        protected IHttpContextAccessor HttpContextAccessor;
        private readonly IMediator _mediator;

        public UnitOfWork(FileDbContext dataContext, IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            DataContext = dataContext;
            HttpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        public FileDbContext GetDatabaseContext()
        {
            return DataContext;
        }

        public int SaveChanges()
        {
            try
            {
                SaveChangesDetail();
                return DataContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbUpdateConcurrencyException(e.Message, new List<IUpdateEntry>());
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                SaveChangesDetail(cancellationToken);
                return DataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbUpdateConcurrencyException(e.Message, new List<IUpdateEntry>());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
                DataContext?.Dispose();
        }

        private void SaveChangesDetail(CancellationToken cancellationToken = default)
        {

            Guid userId;

            string userIdString = HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            string userName = HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Name)!.Value ?? "System";

            if (!string.IsNullOrEmpty(userIdString))
            {
                Guid.TryParse(userIdString, out userId);
            }
            else
            {
                userId = Guid.Empty;
            }

            var entries = DataContext.ChangeTracker.Entries();
            foreach (var e in entries)
            {
                if (e.Entity is IEntityBase entity)
                {
                    switch (e.State)
                    {
                        case EntityState.Added:
                            entity.InsertedById = userId;
                            entity.InsertedAt = DateTime.UtcNow;
                            entity.UpdatedById = userId;
                            entity.UpdatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            entity.UpdatedById = userId;
                            entity.UpdatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Detached:
                            break;
                        case EntityState.Unchanged:
                            break;
                        case EntityState.Deleted:
                            entity.DeletedById = userId;
                            entity.DeletedBy = userName;
                            entity.DeletedAt = DateTime.UtcNow;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        //public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{

        //}

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(DataContext);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            await SaveChangesAsync(cancellationToken);

            return true;
        }

        public virtual IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntityBase
        {
            string key = typeof(TEntity).ToString();
            var repository = new Repository<TEntity>(DataContext);

            return (IRepository<TEntity>)repository;
        }

    }
}
