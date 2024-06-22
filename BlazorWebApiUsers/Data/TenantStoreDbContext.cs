using BlazorBoilerplate.Constants;
using BlazorWebApi.Users.RoleConst;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Finbuckle.MultiTenant.Stores;
using IdentitySample;
using Microsoft.EntityFrameworkCore;

namespace BlazorBoilerplate.Storage
{
    public class TenantStoreDbContext : EFCoreStoreDbContext<AppTenantInfo>
    {
        public static readonly TenantInfo DefaultTenant = new TenantInfo() { Id = Settings.DefaultTenantId, Identifier = Settings.DefaultTenantId, Name = Settings.DefaultTenantId };

        public TenantStoreDbContext(DbContextOptions<TenantStoreDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("Identity");

            modelBuilder.Entity<AppTenantInfo>()
                .Property(t => t.ConnectionString)
                .IsRequired(false);
            modelBuilder.Entity<AppTenantInfo>()
                .HasData(DefaultTenant);
        }
    }
}