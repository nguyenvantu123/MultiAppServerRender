using BlazorIdentity.Users.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using BlazorIdentity.Users.Constants;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlazorIdentity.Data
{

    public class ApplicationDbContext : MultiTenantIdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole, IdentityUserLogin<Guid>,
        ApplicationRoleClaim, IdentityUserToken<Guid>>, IMultiTenantDbContext
    {
        public ApplicationDbContext(IMultiTenantContextAccessor multiTenantContextAccessor, DbContextOptions options) : base(multiTenantContextAccessor, options)
        {
            TenantInfo = (AppTenantInfo)multiTenantContextAccessor.MultiTenantContext.TenantInfo ?? new AppTenantInfo { Id = DefaultTenant.DefaultTenantId, Identifier = DefaultTenant.DefaultTenantId, Name = DefaultTenant.DefaultTenantId };
            TenantNotSetMode = TenantNotSetMode.Overwrite;
            TenantMismatchMode = TenantMismatchMode.Overwrite;
        }


        private IDbContextTransaction _currentTransaction;

        public new ITenantInfo TenantInfo { get; }

        public DbSet<ApiLogItem> ApiLogs { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<TenantSetting> TenantSettings { get; set; }

        private IUserSession UserSession { get; set; }

        public bool HasActiveTransaction => _currentTransaction != null;


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync();

            return _currentTransaction;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            //builder.ConfigureMultiTenant();

            builder.HasDefaultSchema("Identity");

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

            builder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many entries in the UserRole join table
                builder.Entity<ApplicationUser>(b =>
                {
                    b.HasOne(a => a.Profile)
                    .WithOne(b => b.ApplicationUser)
                    .HasForeignKey<UserProfile>(b => b.UserId);

                    b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
                });

            });

            builder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many entries in the RoleClaim join table
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            builder.Entity<IdentityUserLogin<Guid>>().IsMultiTenant();

            builder.Entity<IdentityUserToken<Guid>>().IsMultiTenant();


            builder.Entity<ApiLogItem>(b =>
              {
                  b.HasOne(e => e.ApplicationUser)
                      .WithMany(e => e.ApiLogItems)
                      .HasForeignKey(e => e.ApplicationUserId)
                      .OnDelete(DeleteBehavior.Cascade);
              });

            builder.Entity<Message>().ToTable("Messages");

            builder.Entity<TenantSetting>().ToTable("TenantSettings").HasKey(i => new { i.TenantId, i.Key }); ;


            //builder.Ignore<IdentityRole<Guid>>();
            //builder.Ignore<IdentityRoleClaim<Guid>>();
            //builder.Ignore<IdentityUser<Guid>>();
            //builder.Ignore<IdentityUserClaim<Guid>>();
            //builder.Ignore<IdentityUserRole<Guid>>();
            //builder.Ignore<IdentityUserLogin<Guid>>();
            //builder.Ignore<IdentityUserLogin<Guid>>();


            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}

