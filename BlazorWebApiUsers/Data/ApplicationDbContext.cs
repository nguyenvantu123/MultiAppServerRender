using BlazorWebApi.Users.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BlazorWebApi.Users.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>,
        ApplicationRoleClaim, IdentityUserToken<Guid>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

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

            //builder.Entity<ApplicationUser>(entity =>
            //{
            //    entity.ToTable(name: "Users", "Identity");
            //    entity.Property(e => e.Id).ValueGeneratedOnAdd();
            //});

            //builder.Entity<ApplicationRole>(entity =>
            //{
            //    entity.ToTable(name: "Roles", "Identity");
            //});
            //builder.Entity<ApplicationUserRole>(entity =>
            //{
            //    entity.ToTable("UserRoles", "Identity");
            //});

            //builder.Entity<ApplicationUserClaim>(entity =>
            //{
            //    entity.ToTable("UserClaims", "Identity");
            //});

            //builder.Entity<IdentityUserLogin<Guid>>(entity =>
            //{
            //    entity.ToTable("UserLogins", "Identity");
            //    entity.HasNoKey();
            //});

            //builder.Entity<ApplicationRoleClaim>(entity =>
            //{
            //    entity.ToTable(name: "RoleClaims", "Identity");
            //    entity.Property(e => e.Id).ValueGeneratedOnAdd();
            //});

            //builder.Entity<IdentityUserToken<Guid>>(entity =>
            //{
            //    entity.ToTable("UserTokens", "Identity");
            //    entity.HasNoKey();
            //});

            builder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                // Each User can have one entry in the JobTitle join table

                // Each User can have many entries in the RefreshToken join table
                //b.HasMany(e => e.RefreshTokens)
                //    .WithOne(e => e.User)
                //    .HasForeignKey(ur => ur.UserId)
                //    .IsRequired();
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
    }
}

