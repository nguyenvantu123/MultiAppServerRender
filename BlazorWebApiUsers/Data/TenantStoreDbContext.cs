using BlazorWebApi.Users.Constants;
using BlazorWebApi.Users.Models;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;

namespace BlazorWebApi.Users.Data
{
    public class TenantStoreDbContext : EFCoreStoreDbContext<AppTenantInfo>
    {
        public static readonly AppTenantInfo DefaultTenant = new AppTenantInfo() { Id = Settings.DefaultTenantId, Identifier = Settings.DefaultTenantId, Name = Settings.DefaultTenantId };

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