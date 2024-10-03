using BlazorIdentity.Users.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using BlazorIdentity.Users.Constants;
using Microsoft.EntityFrameworkCore.Storage;
using BlazorIdentity.Interfaces;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlazorIdentity.Data
{

    public class ApplicationDbContext : MultiTenantIdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole, IdentityUserLogin<Guid>,
        ApplicationRoleClaim, IdentityUserToken<Guid>>, IMultiTenantDbContext, IAdminPersistedGrantDbContext, IAdminLogDbContext,
         IAdminConfigurationDbContext
    {
        public ApplicationDbContext(IMultiTenantContextAccessor multiTenantContextAccessor, DbContextOptions options) : base(multiTenantContextAccessor, options)
        {
            TenantInfo = (AppTenantInfo)multiTenantContextAccessor.MultiTenantContext.TenantInfo ?? new AppTenantInfo { Id = DefaultTenant.DefaultTenantId, Identifier = DefaultTenant.DefaultTenantId, Name = DefaultTenant.DefaultTenantId };
            TenantNotSetMode = TenantNotSetMode.Overwrite;
            TenantMismatchMode = TenantMismatchMode.Overwrite;        }



        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public new ITenantInfo TenantInfo { get; }

        public DbSet<ApiLogItem> ApiLogs { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<TenantSetting> TenantSettings { get; set; }

        public bool HasActiveTransaction => _currentTransaction != null;

        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<ServerSideSession> ServerSideSessions { get; set; }
        public DbSet<PushedAuthorizationRequest> PushedAuthorizationRequests { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<ApiResourceSecret> ApiSecrets { get; set; }
        public DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }
        public DbSet<IdentityResourceClaim> IdentityClaims { get; set; }
        public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }
        public DbSet<ClientGrantType> ClientGrantTypes { get; set; }
        public DbSet<ClientScope> ClientScopes { get; set; }
        public DbSet<ClientSecret> ClientSecrets { get; set; }
        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }
        public DbSet<ClientIdPRestriction> ClientIdPRestrictions { get; set; }
        public DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }
        public DbSet<ClientClaim> ClientClaims { get; set; }
        public DbSet<ClientProperty> ClientProperties { get; set; }
        public DbSet<IdentityResourceProperty> IdentityResourceProperties { get; set; }
        public DbSet<ApiResourceProperty> ApiResourceProperties { get; set; }
        public DbSet<ApiScopeProperty> ApiScopeProperties { get; set; }
        public DbSet<ApiResourceScope> ApiResourceScopes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }
        public DbSet<IdentityResource> IdentityResources { get; set; }
        public DbSet<ApiResource> ApiResources { get; set; }
        public DbSet<ApiScope> ApiScopes { get; set; }
        public DbSet<IdentityProvider> IdentityProviders { get; set; }

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

            builder.Entity<TenantSetting>().ToTable("TenantSettings").HasKey(i => new { i.TenantId, i.Key });

            builder.Entity<DeviceFlowCodes>().ToTable("DeviceFlowCodes").HasKey(x => x.DeviceCode);

            SetGlobalQueryFilters(builder);

        }

        private void SetGlobalQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType tp in modelBuilder.Model.GetEntityTypes())
            {
                Type t = tp.ClrType;

                // set Soft Delete Property
                if (typeof(ISoftDelete).IsAssignableFrom(t))
                {
                    MethodInfo method = SetGlobalQueryForSoftDeleteMethodInfo.MakeGenericMethod(t);
                    method.Invoke(this, new object[] { modelBuilder });
                }
            }
        }

        private static readonly MethodInfo SetGlobalQueryForSoftDeleteMethodInfo = typeof(ApplicationDbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
    .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQueryForSoftDelete");

        public void SetGlobalQueryForSoftDelete<T>(ModelBuilder builder) where T : class, ISoftDelete
        {
            builder.Entity<T>().HasQueryFilter(item => !EF.Property<bool>(item, "IsDeleted"));
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

        public override int SaveChanges()
        {
            ChangeTracker.SetShadowProperties();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.SetShadowProperties();
            return await base.SaveChangesAsync(true, cancellationToken);
        }
    }
}

