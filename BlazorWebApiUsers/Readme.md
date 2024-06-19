-- Migration Command

add-migration GenerateApplicationDB1 -OutputDir  Data/Migrations  -StartupProject BlazorWebApiUsers -Context ApplicationDbContext

add-migration GenerateApplicationDB -OutputDir  Data/Migrations  -StartupProject BlazorWebApiUsers -Context TenantStoreDbContext


-- Update Database

update-database -StartupProject BlazorWebApiUsers  -Context ApplicationDbContext


update-database -StartupProject BlazorWebApiUsers  -Context TenantStoreDbContext



-- Remove Migration

remove-migration -StartupProject BlazorWebApiUsers 