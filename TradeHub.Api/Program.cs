using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TradeHub.Api.Middleware;
using TradeHub.Api.Models;
using TradeHub.Api.Services;
using TradeHub.Api.Services.Interfaces;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Repository;


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

        // add services to container
        builder.Services.AddControllers();

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddOpenApi();

        // register HttpClient for the LLM service
        builder.Services.AddHttpClient<ILLMService, MultiLLMService>();
        // builder.Services.AddHttpClient();
        // builder.Services.AddScoped<ILLMService, MultiLLMService>();

        // DI for trade layer
        builder.Services.AddScoped<ITradeService, TradeService>();
        builder.Services.AddScoped<ITradeRepository, TradeRepository>();



        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger(); // read from appsettings.json
        builder.Host.UseSerilog();

        var app = builder.Build();

        app.UseMiddleware<GlobalExceptionHandler>();

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
