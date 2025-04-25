using ATONTestTask.Abstractions;
using ATONTestTask.Data;
using ATONTestTask.Data.Entites;
using ATONTestTask.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class Common
    {
        public static void SeedData(ApplicationContext dbContext, IPasswordHasher passwordHasher)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var admin = new User()
            {
                Name = "admin",
                Admin = true,
                Login = "admin",
                PasswordHash = passwordHasher.HashPassword("admin"),
                CreatedOn = new DateTime(2002, 09, 29, 0,0,0, DateTimeKind.Utc)
            };
            var revokedUser = new User() 
            { 
                Name = "revoked_user_name", 
                Login = "revoked_user", 
                PasswordHash = passwordHasher.HashPassword("1234"), 
                RevokedOn = DateTime.UtcNow, 
                RevokedBy = "admin",
                CreatedOn = new DateTime(2006, 10, 23, 0, 0, 0, DateTimeKind.Utc)
            };
            dbContext.Users.AddRange(admin, revokedUser);

            var users = new User[10];
            for (var i = 0; i < 10; i++)
            {
                users[i] = new User()
                {
                    Login = "user" + i,
                    Name = "user" + i,
                    Admin = false,
                    PasswordHash = passwordHasher.HashPassword("user" + 1),
                    Gender = (Gender)(i % 2),
                    CreatedOn = DateTime.UtcNow + TimeSpan.FromMinutes(i)
                };
            }
            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
            DetachAllEntities(dbContext);
        }

        private static void DetachAllEntities(ApplicationContext dbContext)
        {
            var undetachedEntriesCopy = dbContext.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in undetachedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
