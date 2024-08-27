-- Migration Command

add-migration GenDBForDeviceFlow -OutputDir  Data/Migrations/ApplicationDb  -StartupProject BlazorIdentity -Context ApplicationDbContext

add-migration GenerateApplicationTenantDB1 -OutputDir  Data/Migrations/TenantStoreDb  -StartupProject BlazorIdentity -Context TenantStoreDbContext


-- Update Database

update-database -StartupProject BlazorIdentity  -Context ApplicationDbContext


update-database -StartupProject BlazorIdentity  -Context TenantStoreDbContext



-- Remove Migration
remove-migration  -StartupProject BlazorIdentity -Context ApplicationDbContext
remove-migration  -StartupProject BlazorIdentity -Context TenantStoreDbContext

--Revert Migration
Update-Database <tenmigration> -StartupProject BlazorIdentity  -Context ApplicationDbContext

Update-Database 20240826093257_GenerateApplicationForDuende -StartupProject BlazorIdentity  -Context ApplicationDbContext