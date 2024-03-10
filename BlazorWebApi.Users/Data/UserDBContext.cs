
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BlazorWebApi.Data
{

    public partial class UserDBContext : IdentityDbContext<User, UserRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, UserRoleClaim, IdentityUserToken<string>>
    {

        //private readonly ICurrentUserService _currentUserService;
        //private readonly IDateTimeService _dateTimeService;


        //public UserDBContext(DbContextOptions<UserDBContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
        //    : base(options)
        //{
        //}

        public UserDBContext(DbContextOptions<UserDBContext> options) : base(options)
        { }

        //protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        //{
        //    if (entityEntry != null && entityEntry.State == EntityState.Added)
        //    {
        //        var errors = new List<DbValidationError>();
        //        var user = entityEntry.Entity as ApplicationUser;
        //        //check for uniqueness of phone number
        //        if (user != null)
        //        {
        //            if (Users.Any(u => String.Equals(u.PhoneNumber, user.PhoneNumber)))
        //            {
        //                errors.Add(new DbValidationError("User", user.PhoneNumber + " is already registered."));
        //            }
        //        }
        //    }
        //    return base.ValidateEntity(entityEntry, items); //check for uniqueness of user name and email and return result
        //}

        //public DbSet<Audit> AuditTrails { get; set; }

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        //{
        //    foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        //    {
        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //                entry.Entity.CreatedOn = _dateTimeService.NowUtc;
        //                entry.Entity.CreatedBy = _currentUserService.UserId;
        //                break;

        //            case EntityState.Modified:
        //                entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
        //                entry.Entity.LastModifiedBy = _currentUserService.UserId;
        //                break;
        //        }
        //    }

        //    var auditEntries = OnBeforeSaveChanges(_currentUserService.UserId);
        //    var result = await base.SaveChangesAsync(cancellationToken);
        //    await OnAfterSaveChanges(auditEntries, cancellationToken);
        //    return result;
        //}

        //public interface IAuditableEntity : IEntity
        //{
        //    string CreatedBy { get; set; }

        //    DateTime CreatedOn { get; set; }

        //    string LastModifiedBy { get; set; }

        //    DateTime? LastModifiedOn { get; set; }
        //}



        //private List<AuditEntry> OnBeforeSaveChanges(string userId)
        //{
        //    ChangeTracker.DetectChanges();
        //    var auditEntries = new List<AuditEntry>();
        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
        //            continue;

        //        var auditEntry = new AuditEntry(entry)
        //        {
        //            TableName = entry.Entity.GetType().Name,
        //            UserId = userId
        //        };
        //        auditEntries.Add(auditEntry);
        //        foreach (var property in entry.Properties)
        //        {
        //            if (property.IsTemporary)
        //            {
        //                auditEntry.TemporaryProperties.Add(property);
        //                continue;
        //            }

        //            string propertyName = property.Metadata.Name;
        //            if (property.Metadata.IsPrimaryKey())
        //            {
        //                auditEntry.KeyValues[propertyName] = property.CurrentValue;
        //                continue;
        //            }

        //            switch (entry.State)
        //            {
        //                case EntityState.Added:
        //                    auditEntry.AuditType = AuditType.Create;
        //                    auditEntry.NewValues[propertyName] = property.CurrentValue;
        //                    break;

        //                case EntityState.Deleted:
        //                    auditEntry.AuditType = AuditType.Delete;
        //                    auditEntry.OldValues[propertyName] = property.OriginalValue;
        //                    break;

        //                case EntityState.Modified:
        //                    if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
        //                    {
        //                        auditEntry.ChangedColumns.Add(propertyName);
        //                        auditEntry.AuditType = AuditType.Update;
        //                        auditEntry.OldValues[propertyName] = property.OriginalValue;
        //                        auditEntry.NewValues[propertyName] = property.CurrentValue;
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //    foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
        //    {
        //        AuditTrails.Add(auditEntry.ToAudit());
        //    }
        //    return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        //}

        //private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken = new())
        //{
        //    if (auditEntries == null || auditEntries.Count == 0)
        //        return Task.CompletedTask;

        //    foreach (var auditEntry in auditEntries)
        //    {
        //        foreach (var prop in auditEntry.TemporaryProperties)
        //        {
        //            if (prop.Metadata.IsPrimaryKey())
        //            {
        //                auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
        //            }
        //            else
        //            {
        //                auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
        //            }
        //        }
        //        AuditTrails.Add(auditEntry.ToAudit());
        //    }
        //    return SaveChangesAsync(cancellationToken);
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {

            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
            {
                property.SetColumnType("nvarchar(128)");
            }

            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable(name: "Roles", "Identity");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles", "Identity");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", "Identity");
            });

            builder.Entity<UserRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "Identity");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", "Identity");
            });
        }

    }
}
