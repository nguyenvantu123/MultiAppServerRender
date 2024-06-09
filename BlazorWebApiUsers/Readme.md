-- Migration Command

add-migration InitDb -OutputDir  Data/Migrations  -StartupProject BlazorWebApiUsers

-- Update Database

update-database -StartupProject BlazorWebApiUsers 

-- Remove Migration

remove-migration -StartupProject BlazorWebApiUsers 