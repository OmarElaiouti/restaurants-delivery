
using Delivery.Api.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("https://localhost:7166")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            builder.Services.AddDbContext<RestaurantDeliveryDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection"))
                
                );

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Logging.AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
