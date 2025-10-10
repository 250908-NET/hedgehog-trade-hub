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

        if (app.Environment.IsDevelopment())
        {
            // seed demo items
            await SeedDemoItemsAsync(app.Services, true);
        }

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

    /// <summary>
    /// Seed db with demo items owned by the admin.
    /// </summary>
    /// <param name="clear">Whether the Items table should be cleared first</param>
    private static async Task SeedDemoItemsAsync(IServiceProvider services, bool clear = false)
    {
        if (clear)
        {
            const int delay = 5; // seconds
            Console.WriteLine("WARNING: This will delete all existing items!");

            for (int i = delay; i > 0; i--)
            {
                Console.Write($"Operation will continue in {i} seconds...\r");
                Thread.Sleep(1000);
            }

            Console.WriteLine("Continuing...                                     ");
        }

        using var scope = services.CreateScope();

        TradeHubContext context = scope.ServiceProvider.GetRequiredService<TradeHubContext>();
        UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<
            UserManager<User>
        >();
        ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Seeding demo items...");

        // find admin user
        User? adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            logger.LogError("Admin user not found. Cannot seed demo items.");
            return;
        }
        long adminUserId = adminUser.Id;

        if (clear)
        {
            logger.LogWarning("Deleting all existing items to seed demo data.");
            context.Items.RemoveRange(context.Items);
            await context.SaveChangesAsync();
        }

        var demoItems = new List<Item>
        {
            /*
            new(
                name
                description
                image
                quantity
                owner
                location
                condition
                availability
            ),
            */
            new(
                "Test Item 1",
                "test item :)",
                "", // no image
                0,
                adminUserId,
                "",
                Condition.New,
                Availability.Available
            ),
            new(
                "Pinecone",
                "i think it's a pinecone? maybe?",
                "/images/pinecone.jpg",
                100,
                adminUserId,
                "",
                Condition.UsedLikeNew,
                Availability.Available
            ),
            new(
                "This Rock I Found",
                "it's a cool rock i guess",
                "/images/rock.jpg",
                20,
                adminUserId,
                "",
                Condition.UsedGood,
                Availability.Available
            ),
            new(
                "Air",
                "come get your air :)",
                "", // no image
                100,
                adminUserId,
                "air",
                Condition.UsedAcceptable,
                Availability.Available
            ),
            new(
                "TI-84 Plus Calculator",
                "Some assembly required.",
                "/images/ti84.jpg",
                20,
                adminUserId,
                "calculator",
                Condition.UsedBad,
                Availability.Available
            ),
            new(
                "Weird Car",
                "My friend gave me this weird car but I don't have a driver's license so I can't drive it.",
                "/images/ti84.jpg",
                1000000,
                adminUserId,
                "car",
                Condition.Refurbished,
                Availability.Available
            ),
            new(
                "Succulent Hedgehog Garden Statues with Solar Lights Garden Gifts for Women Mom Lawn Ornaments for Patio Yard Decor",
                """ 
                Cute Design:Lovely hedgehogs with succulents and warm white lights can bring a cheerful atmosphere to your home, no matter where you place it.
                Multiple Locations Applications:This statue is suitable for various locations, including the front yard, patio, porch, front step, balcony, flower bed, stairs, poolside, planter box decoration and garden centerpiece.
                Perfect Gift Choice:This hedgehog statue is a perfect gift for Christmas, Easter, Mother's Day, housewarming, birthdays, anniversaries, weddings and more.
                Auto ON/OFF:Before use this solar lights, please turn on the button located at the bottom of the statue and keep the product in the sun to ensure sufficient sunlight. It will automatically charge during the day and light up at night for 8 hours.
                Durable Quality:Made from high-quality materials and are coated with a weather-resistant protective layer to ensure waterproofing and colorfastness. This statue is built to last and will add a touch of magic to your outdoor space for years to come.
                """,
                "/images/statue.jpg",
                20,
                adminUserId,
                "hedgehog,statue,garden",
                Condition.New,
                Availability.Available
            ),
            new(
                "Bow",
                "Lightly used, but still functional. Arrows not included.",
                "/images/bow.jpg",
                300,
                adminUserId,
                "",
                Condition.UsedAcceptable,
                Availability.Available
            ),
            new(
                "Car",
                "Recently tuned, 3.0 liter flat-six engine with turbo. Only 12,000 miles.",
                "/images/car.jpg",
                20000,
                adminUserId,
                "car,good",
                Condition.UsedLikeNew,
                Availability.Unavailable
            ),
        };

        await context.Items.AddRangeAsync(demoItems);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Successfully seeded {ItemCount} demo items for admin user.",
            demoItems.Count
        );
    }
}
