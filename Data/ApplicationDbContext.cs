using Microsoft.EntityFrameworkCore;
using SkyStore.Models;
namespace SkyStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets para las entidades
        public DbSet<User> Users { get; set; }
        public DbSet<StoreFile> Files { get; set; }    
    }
}
