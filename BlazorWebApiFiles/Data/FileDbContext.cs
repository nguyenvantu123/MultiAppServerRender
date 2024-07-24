using Microsoft.EntityFrameworkCore;

namespace BlazorWebApi.Files.Data
{
    public class FileDbContext : DbContext, IUnitOfWork
    {
    }
}
