using FellesForumAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FellesForumAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(x => x.Phone).IsUnique();
            base.OnModelCreating(modelBuilder);
        }

    }
}
