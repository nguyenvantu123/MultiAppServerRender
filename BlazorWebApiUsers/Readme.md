-- Migration Command

add-migration InitDb -OutputDir  Data/Migrations  -StartupProject BlazorWebApiUsers -Context ApplicationDbContext

-- Update Database

update-database -StartupProject BlazorWebApiUsers  -Context ApplicationDbContext

-- Remove Migration

remove-migration -StartupProject BlazorWebApiUsers 