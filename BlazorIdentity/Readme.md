-- Migration Command

add-migration GenerateApplicationForDuende -OutputDir  Data/Migrations/ApplicationDb  -StartupProject BlazorIdentity -Context ApplicationDbContext

add-migration GenerateApplicationTenantDB1 -OutputDir  Data/Migrations/TenantStoreDb  -StartupProject BlazorIdentity -Context TenantStoreDbContext


-- Update Database

update-database -StartupProject BlazorIdentity  -Context ApplicationDbContext


update-database -StartupProject BlazorIdentityUsers  -Context TenantStoreDbContext



-- Remove Migration
remove-migration  -StartupProject BlazorIdentityUsers -Context ApplicationDbContext
remove-migration  -StartupProject BlazorIdentityUsers -Context TenantStoreDbContext
