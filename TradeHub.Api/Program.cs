using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;

namespace TradeHub.Api;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load(); // load .env file

        var builder = WebApplication.CreateBuilder(args);

        // register DbContext
        // this code runs before Moq is able to intercept it in tests, so it needs to be skipped when in testing environment
        if (!builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<TradeHubContext>(options =>
            {
                // load connection string from environment variable
                string? connectionString = builder.Configuration.GetValue<string>(
                    "DB_CONNECTION_STRING"
                );
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "Connection string 'DB_CONNECTION_STRING' not found in configuration."
                    );
                }

                options.UseSqlServer(connectionString);
            });
        }

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
