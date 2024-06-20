using BlazorBoilerplate.Infrastructure.Storage.DataInterfaces;
using BlazorBoilerplate.Infrastructure.Storage.DataModels;
using BlazorBoilerplate.Shared.Interfaces;
using BlazorBoilerplate.Storage;
using BlazorBoilerplate.Storage.Configurations;
using BlazorWebApi.Users.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace BlazorWebApi.Users.Data
{

    public class ApplicationDbContext : MultiTenantIdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>,
        ApplicationRoleClaim, IdentityUserToken<Guid>>, IMultiTenantDbContext
    {
        public ApplicationDbContext(ITenantInfo tenantInfo, DbContextOptions<ApplicationDbContext> options, IUserSession userSession)
         : base(tenantInfo, options)
        {
            TenantNotSetMode = TenantNotSetMode.Overwrite;
            TenantMismatchMode = TenantMismatchMode.Overwrite;
            UserSession = userSession;
        }

        public DbSet<ApiLogItem> ApiLogs { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Message> Messages { get; set; }

        private IUserSession UserSession { get; set; }

        public DbSet<TenantSetting> TenantSettings { get; set; }

        public DbSet<AppTenantInfo> MyTenants { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ConfigureMultiTenant();

            //builder.HasDefaultSchema("Identity");

            // foreach (var property in builder.Model.GetEntityTypes()
            //.SelectMany(t => t.GetProperties())
            //.Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            // {
            //     property.SetColumnType("decimal(18,2)");
            // }

            // foreach (var property in builder.Model.GetEntityTypes()
            //     .SelectMany(t => t.GetProperties())
            //     .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
            // {
            //     property.SetColumnType("nvarchar(128)");
            // }



            // builder.Entity<ApplicationUser>(b =>
            // {
            //     // Each User can have many entries in the UserRole join table
            //     builder.Entity<ApplicationUser>(b =>
            //     {
            //         b.HasOne(a => a.Profile)
            //         .WithOne(b => b.ApplicationUser)
            //         .HasForeignKey<UserProfile>(b => b.UserId);

            //         b.HasMany(e => e.UserRoles)
            //         .WithOne(e => e.User)
            //         .HasForeignKey(ur => ur.UserId)
            //         .IsRequired();
            //     });

            // });

            // builder.Entity<ApplicationRole>(b =>
            // {

            //     b.HasMany(e => e.UserRoles)
            //    .WithOne(e => e.Role)
            //    .HasForeignKey(ur => ur.RoleId)
            //    .IsRequired();

            //     b.HasMany(e => e.RoleClaims)
            //         .WithOne(e => e.Role)
            //         .HasForeignKey(ur => ur.RoleId)
            //         .IsRequired();
            // });


            // builder.Entity<ApiLogItem>(b =>
            // {
            //     b.HasOne(e => e.ApplicationUser)
            //         .WithMany(e => e.ApiLogItems)
            //         .HasForeignKey(e => e.ApplicationUserId)
            //         .OnDelete(DeleteBehavior.Cascade);
            // });

            // builder.Entity<Message>().ToTable("Messages");

            // builder.Entity<TenantSetting>().ToTable("TenantSettings").HasKey(i => new { i.TenantId, i.Key }); ;

            //builder.ApplyConfiguration(new MessageConfiguration());

            //SetGlobalQueryFilters(builder);
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

        public override int SaveChanges()
        {
            ChangeTracker.SetShadowProperties(UserSession);
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.SetShadowProperties(UserSession);
            return await base.SaveChangesAsync(true, cancellationToken);
        }
    }
}

