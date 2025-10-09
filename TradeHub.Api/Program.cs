using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TradeHub.Api.Middleware;
using TradeHub.Api.Models;
using TradeHub.Api.Repository;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api;

public class Program
{
    public static async Task Main(string[] args)
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
        // Identity
        builder.Services.AddIdentity<User, IdentityRole<long>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<TradeHubContext>()
        .AddDefaultTokenProviders();

                // Jwt Auth
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var key = jwtSettings["SecretKey"]?? throw new InvalidOperationException("JwtSettings.SecretKey is missing in appsettings.json!");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });

        builder.Services.AddAuthorization( options =>
        {
            options.AddPolicy("Users", policy => policy.RequireClaim("User"));
            options.AddPolicy("Admins", policy => policy.RequireClaim("Admin"));
        });

        // add services to container
        builder.Services.AddControllers();

        // Add CORS configuration
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:4173", "http://localhost:5174")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddOpenApi();

        // Register Repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<IOfferRepository, OfferRepository>();
        builder.Services.AddScoped<ITradeRepository, TradeRepository>();

        // Register Services
        builder.Services.AddHttpClient<ILLMService, MultiLLMService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IItemService, ItemService>();
        builder.Services.AddScoped<IOfferService, OfferService>();
        builder.Services.AddScoped<ITradeService, TradeService>();
        builder.Services.AddScoped<SeedDataService>();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger(); // read from appsettings.json
        builder.Host.UseSerilog();

        var app = builder.Build();

        // Check for seed command line argument
        if (args.Contains("--seed"))
        {
            await SeedDatabaseAsync(app);
            return;
        }

        // Auto-seed in development if database is empty
        if (app.Environment.IsDevelopment())
        {
            await TrySeedDatabaseAsync(app);
        }

        app.UseMiddleware<GlobalExceptionHandler>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // Use CORS
        app.UseCors("AllowFrontend");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static async Task SeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seedService = scope.ServiceProvider.GetRequiredService<SeedDataService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("Manual database seeding requested...");
            await seedService.SeedAsync();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during manual database seeding.");
            throw;
        }
    }

    private static async Task TrySeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seedService = scope.ServiceProvider.GetRequiredService<SeedDataService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("Checking if database needs seeding...");
            await seedService.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Auto-seeding failed, but application will continue. Use --seed flag for manual seeding.");
        }
    }
}
