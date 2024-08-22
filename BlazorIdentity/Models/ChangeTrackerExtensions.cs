using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlazorIdentity.Users.Models
{
    public static class ChangeTrackerExtensions
    {
        public static void SetShadowProperties(this ChangeTracker changeTracker, IUserSession userSession)
        {
            changeTracker.DetectChanges();
            Guid? userId = null;
            string userName = null;
            DateTime timestamp = DateTime.UtcNow;

            if (userSession != null && userSession.UserId != Guid.Empty)
            {
                userId = userSession.UserId;
                userName = userSession.UserName;
            }


            foreach (EntityEntry entry in changeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
            {
                if (entry.Entity is IAuditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreatedOn").CurrentValue = timestamp;

                        if (userId != null)
                            entry.Property("CreatedBy").CurrentValue = userName;
                    }

                    if (entry.State == EntityState.Deleted || entry.State == EntityState.Modified)
                    {
                        entry.Property("LastModifiedOn").CurrentValue = timestamp;

                        if (userId != null)
                            entry.Property("LastModifiedBy").CurrentValue = userName;
                    }
                }

                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete)
                {
                    entry.State = EntityState.Modified;
                    entry.Property("IsDeleted").CurrentValue = true;
                }
            }
        }
    }
}