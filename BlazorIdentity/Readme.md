-- Migration Command

add-migration GenerateApplicationDB3 -OutputDir  Data/Migrations/ApplicationDb  -StartupProject BlazorIdentityUsers -Context ApplicationDbContext

add-migration GenerateApplicationTenantDB1 -OutputDir  Data/Migrations/TenantStoreDb  -StartupProject BlazorIdentityUsers -Context TenantStoreDbContext


-- Update Database

update-database -StartupProject BlazorIdentityUsers  -Context ApplicationDbContext


update-database -StartupProject BlazorIdentityUsers  -Context TenantStoreDbContext



-- Remove Migration
remove-migration  -StartupProject BlazorIdentityUsers -Context ApplicationDbContext
remove-migration  -StartupProject BlazorIdentityUsers -Context TenantStoreDbContext
