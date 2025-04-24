using ATONTestTask.Abstractions;
using ATONTestTask.Data;
using ATONTestTask.Data.Entites;
using ATONTestTask.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Common
    {
        public static void SeedData(ApplicationContext dbContext, IPasswordHasher passwordHasher)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var random = new Random();

            var admin = new User() { Name = "admin", Admin = true, Login = "admin", PasswordHash = passwordHasher.HashPassword("admin") };
            dbContext.Users.Add(admin);

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
