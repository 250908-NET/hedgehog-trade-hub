using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using TradeHub.Api.Utilities;

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
            var connectionString =
                builder.Configuration.GetValue<string>("DB_CONNECTION_STRING")
                ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'DB_CONNECTION_STRING' not found."
                );

            builder.Services.AddDbContext<TradeHubContext>(options =>
                options.UseSqlServer(connectionString)
            );
        }

        // Identity
        builder
            .Services.AddIdentity<User, IdentityRole<long>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<TradeHubContext>()
            .AddDefaultTokenProviders();

        // Jwt Auth
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var key =
            jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException(
                "JwtSettings.SecretKey is missing in appsettings.json!"
            );

        builder
            .Services.AddAuthentication(options =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                };
            });

        builder
            .Services.AddAuthorizationBuilder()
            .AddPolicy("Users", policy => policy.RequireClaim("User"))
            .AddPolicy("Admins", policy => policy.RequireClaim("Admin"));

        // add services to container
        builder
            .Services.AddControllers()
            .AddJsonOptions(options =>
            {
                // Use the custom FlexibleEnumConverterFactory to allow both string and int for enums
                // Serialization will still output enum names as strings.
                options.JsonSerializerOptions.Converters.Add(new FlexibleEnumConverterFactory());
            });

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<IOfferRepository, OfferRepository>();
        builder.Services.AddScoped<IOfferItemRepository, OfferItemRepository>();
        builder.Services.AddScoped<ITradeRepository, TradeRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddScoped<IItemService, ItemService>();
        builder.Services.AddScoped<IOfferService, OfferService>();
        builder.Services.AddScoped<IOfferItemService, OfferItemService>();
        builder.Services.AddScoped<ITradeService, TradeService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAdminUserService, AdminUserService>();

        // Cookie auth
        builder
            .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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
        builder.Services.AddCors(o =>
            o.AddDefaultPolicy(p =>
                p.WithOrigins("http://localhost:5173", "http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            )
        );

        // ---------- DI registrations ----------
        // Identity password hasher (generic)
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowReactApp",
                policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger(); // read from appsettings.json
        builder.Host.UseSerilog();

        var app = builder.Build();

        // seed admin user
        await SeedAdminUserAsync(app.Services);

        app.UseMiddleware<GlobalExceptionHandler>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowReactApp");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    private static async Task SeedAdminUserAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<
            UserManager<User>
        >();
        RoleManager<IdentityRole<long>> roleManager = scope.ServiceProvider.GetRequiredService<
            RoleManager<IdentityRole<long>>
        >();
        ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // ensure "Admin" role exists
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<long>("Admin"));
        }

        string adminUsername = "admin";

        User? adminUser = await userManager.FindByNameAsync(adminUsername);

        if (adminUser == null)
        {
            User newAdminUser = new()
            {
                UserName = adminUsername,
                Email = "admin@example.com",
                Description = "Administrator",
                EmailConfirmed = true, // not actually but shhhhh
            };

            // if this was a production app, you would be lined up against the wall and shot for doing this
            // TODO: check if password needs to be hashed
            string adminPassword = "ThisIsTheGreatestPassword!!!1!";
            IdentityResult result = await userManager.CreateAsync(newAdminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");
                logger.LogInformation(
                    "Admin user {UserName} ({UserId}) created and added to role {RoleName}.",
                    newAdminUser.UserName,
                    newAdminUser.Id,
                    "Admin"
                );
            }
            else
            {
                Log.Error(
                    "Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }
        }
        else
        {
            logger.LogInformation(
                "Admin user {UserName} already exists. ({UserId})",
                adminUser.UserName,
                adminUser.Id
            );
        }
    }
}
