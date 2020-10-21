using CatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatApi
{
    public class CatDbContext : DbContext
    {
        public CatDbContext(DbContextOptions<CatDbContext> options) 
            : base(options)
        {
        }
        
        public DbSet<Cat> Cats { get; set; }
    }
}