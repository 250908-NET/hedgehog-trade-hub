// Program.cs
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using TradeHub.Api;                 // DbContext
using TradeHub.Api.Models;          // User entity
using TradeHub.Api.Repository;      // <-- add if your concrete repos live here
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services;        // IAdminUserService, AdminUserService

namespace TradeHub.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        Env.Load();
        var builder = WebApplication.CreateBuilder(args);

        // DbContext (skip when Testing)
        if (!builder.Environment.IsEnvironment("Testing"))
        {
            var connectionString = builder.Configuration.GetValue<string>("DB_CONNECTION_STRING")
                                  ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'DB_CONNECTION_STRING' not found.");

            builder.Services.AddDbContext<TradeHubContext>(options => options.UseSqlServer(connectionString));
        }

        // Controllers + AutoMapper + OpenAPI
        builder.Services.AddControllers();
        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddOpenApi();

        // Cookie auth
        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "tradehub_auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/forbidden";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

        // Authorization
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
        });

        // CORS
        builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
            p.WithOrigins("http://localhost:5173", "http://localhost:3000")
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials()));

        // ---------- DI registrations ----------
        // Identity password hasher (generic)
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Repositories
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<ITradeRepository, TradeRepository>();
        builder.Services.AddScoped<IOfferRepository, OfferRepository>();


        // Services
        builder.Services.AddScoped<IAdminUserService, AdminUserService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi(); // /openapi/v1.json
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Optional: apply migrations & seed admin (Identity hasher not needed here)
        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TradeHubContext>();
            await db.Database.MigrateAsync();

            // seed admin if missing (uses BCrypt before; now just keep the stored hash you want)
            if (!await db.Users.AnyAsync(u => u.Username == "admin"))
            {
                // If you want the Identity hasher to create this hash:
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@tradehub.local",
                    Description = "System administrator",
                    Role = 1
                };
                admin.PasswordHash = hasher.HashPassword(admin, "AdminPass123!");
                db.Users.Add(admin);
                await db.SaveChangesAsync();
            }
        }

        app.Run();
    }
}
