-- Migration Command

add-migration AddFileUrl -OutputDir  Data/Migrations  -StartupProject BlazorWebApiFiles -Context FileDbContext

add-migration GenerateApplicationTenantDB1 -OutputDir  Data/Migrations/TenantStoreDb  -StartupProject BlazorIdentityUsers -Context TenantStoreDbContext


-- Update Database

update-database -StartupProject BlazorWebApiFiles  -Context FileDbContext


update-database -StartupProject BlazorIdentityUsers  -Context TenantStoreDbContext



-- Remove Migration
remove-migration  -StartupProject BlazorIdentityUsers -Context ApplicationDbContext
remove-migration  -StartupProject BlazorIdentityUsers -Context TenantStoreDbContext
