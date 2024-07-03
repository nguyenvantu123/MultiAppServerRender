-- Migration Command

add-migration GenerateApplicationDB3 -OutputDir  Data/Migrations/ApplicationDb  -StartupProject BlazorWebApiUsers -Context ApplicationDbContext

add-migration GenerateApplicationDB -OutputDir  Data/Migrations/TenantStoreDb  -StartupProject BlazorWebApiUsers -Context TenantStoreDbContext


-- Update Database

update-database -StartupProject BlazorWebApiUsers  -Context ApplicationDbContext


update-database -StartupProject BlazorWebApiUsers  -Context TenantStoreDbContext



-- Remove Migration
remove-migration  -StartupProject BlazorWebApiUsers -Context ApplicationDbContext
remove-migration  -StartupProject BlazorWebApiUsers -Context TenantStoreDbContext
