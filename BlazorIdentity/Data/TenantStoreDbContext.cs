using BlazorIdentity.Users.Constants;
using BlazorIdentity.Users.Models;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;

namespace BlazorIdentity.Users.Data
{
    public class TenantStoreDbContext : EFCoreStoreDbContext<AppTenantInfo>
    {
        public static readonly AppTenantInfo DefaultTenant = new AppTenantInfo() { Id = Constants.DefaultTenant.DefaultTenantId, Identifier = Constants.DefaultTenant.DefaultTenantId, Name = Constants.DefaultTenant.DefaultTenantId };

        public TenantStoreDbContext(DbContextOptions<TenantStoreDbContext> options) : base(options)
        {
        }

        public DbSet<AppTenantInfo> AppTenantInfo { get; set; }

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