-- Migration Command

add-migration InitDb -OutputDir  Data/Migrations  -StartupProject BlazorWebApi.Users

-- Update Database

update-database -StartupProject BlazorWebApi.Users 

-- Remove Migration

remove-migration -StartupProject BlazorWebApi.Users 