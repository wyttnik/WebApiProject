using Microsoft.EntityFrameworkCore;

namespace RestProject.Models
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("UserInfo");
            modelBuilder.Entity<User>().HasKey(e => e.User_id);
        }
    }
}
