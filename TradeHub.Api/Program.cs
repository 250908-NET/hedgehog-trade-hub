using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TradeHub.Api.Middleware;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services;
using TradeHub.Api.Services.Interfaces;

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

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddOpenApi();

        builder.Services.AddScoped<IUserRepository, UserRepository>();

        // register HttpClient for the LLM service
        builder.Services.AddHttpClient<ILLMService, MultiLLMService>();
        // builder.Services.AddHttpClient();
        // builder.Services.AddScoped<ILLMService, MultiLLMService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger(); // read from appsettings.json
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
