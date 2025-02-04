using BlazorIdentity.Data;
using BlazorIdentityApi.Users.Constants;
using BlazorIdentity.Users.Models;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace BlazorIdentity.Users.Data
{
    public class TenantStoreDbContext : EFCoreStoreDbContext<AppTenantInfo>
    {
        public static readonly AppTenantInfo DefaultTenantValue = new AppTenantInfo() { Id = DefaultTenant.DefaultTenantId, Identifier = DefaultTenant.DefaultTenantId, Name = DefaultTenant.DefaultTenantId };

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
                .HasData(DefaultTenantValue);
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
    }
}