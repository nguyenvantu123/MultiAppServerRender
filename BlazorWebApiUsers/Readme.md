-- Migration Command

add-migration GenerateApplicationDB -OutputDir  Data/Migrations  -StartupProject BlazorWebApiUsers -Context ApplicationDbContext

-- Update Database

update-database -StartupProject BlazorWebApiUsers  -Context ApplicationDbContext

-- Remove Migration

remove-migration -StartupProject BlazorWebApiUsers 