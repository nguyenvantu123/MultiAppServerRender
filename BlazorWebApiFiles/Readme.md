-- Migration Command

add-migration AddNullableValue -OutputDir  Data/Migrations  -StartupProject BlazorWebApiFiles -Context FileDbContext

add-migration GenerateApplicationTenantDB1 -OutputDir  Data/Migrations/TenantStoreDb  -StartupProject BlazorWebApiUsers -Context TenantStoreDbContext


-- Update Database

update-database -StartupProject BlazorWebApiUsers  -Context ApplicationDbContext


update-database -StartupProject BlazorWebApiUsers  -Context TenantStoreDbContext



-- Remove Migration
remove-migration  -StartupProject BlazorWebApiUsers -Context ApplicationDbContext
remove-migration  -StartupProject BlazorWebApiUsers -Context TenantStoreDbContext
