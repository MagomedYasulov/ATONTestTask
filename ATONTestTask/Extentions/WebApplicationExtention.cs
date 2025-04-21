using ATONTestTask.Abstractions;
using ATONTestTask.Data;

namespace ATONTestTask.Extentions
{
    public static class WebApplicationExtention
    {
        public static WebApplication SeedData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var passwordhasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            dbContext.Users.Add(new Data.Entites.User() { Login = "admin", PasswordHash = passwordhasher.HashPassword("admin"), Admin = true });
            dbContext.SaveChanges();
            return app;
        }
    }
}
