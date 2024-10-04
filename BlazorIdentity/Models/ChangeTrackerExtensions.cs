using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlazorIdentity.Users.Models
{
    public static class ChangeTrackerExtensions
    {
        public static void SetShadowProperties(this ChangeTracker changeTracker)
        {
            changeTracker.DetectChanges();
            Guid? userId = null;
            string userName = null;
            DateTime timestamp = DateTime.UtcNow;

            //if (SignInManager == null || SignInManager.Context == null)
            //{
            //    return;
            //}

            //userId = Guid.Parse(SignInManager.Context.User.FindFirst("sub").Value);
            //userName = SignInManager.Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

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