using ATONTestTask.Data.Entites;
using Microsoft.EntityFrameworkCore;

namespace ATONTestTask.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var defaultUser = new User() 
            { 
                Id = Guid.NewGuid(),  
                Login = "Admin",
                PasswordHash = "",
                Admin = true,
                Name = "Admin",
            };
            modelBuilder.Entity<User>().HasData(defaultUser);
        }
    }
}
