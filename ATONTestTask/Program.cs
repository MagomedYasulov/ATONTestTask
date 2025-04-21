
using ATONTestTask.Abstractions;
using ATONTestTask.Data;
using ATONTestTask.Extentions;

namespace ATONTestTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.AddData();
            builder.AddControllers();
            builder.AddAuthentication();
            builder.AddAutoMapper();
            builder.AddAppServices();
            builder.AddExceptionHandler();
            builder.AddSwagger();
            builder.AddFluentValidation();

            var app = builder.Build();

            //Добавление дефолтного админа
            app.SeedData();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
