using BlazorIdentity.Users.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace BlazorIdentity.Users.Models
{
    public static class ChangeTrackerExtensions
    {
        public static void SetShadowProperties(this ChangeTracker changeTracker, ClaimsPrincipal user)
        {
            changeTracker.DetectChanges();
            Guid? userId = null;
            string userName = null;
            DateTime timestamp = DateTime.UtcNow;


            //userId = user.GetUserId();

            //userName = user.GetDisplayByName();

            foreach (EntityEntry entry in changeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
            {

                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedOn").CurrentValue = timestamp;

                    if (userId != null)
                        entry.Property("CreatedBy").CurrentValue = userName;
                }

                if (entry.State == EntityState.Modified)
                {
                    if (Convert.ToBoolean(entry.Property("IsDeleted").CurrentValue) == true)
                    {
                        entry.Property("DeletedOn").CurrentValue = timestamp;
                        entry.Property("DeletedBy").CurrentValue = userName;
                    }

                    else
                    {

                        entry.Property("LastModifiedOn").CurrentValue = timestamp;
                        entry.Property("LastModifiedBy").CurrentValue = userName;
                    }


                }
            }
            //if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete)
            //{
            //    entry.State = EntityState.Modified;
            //    entry.Property("IsDeleted").CurrentValue = true;
            //}
        }
    }
}