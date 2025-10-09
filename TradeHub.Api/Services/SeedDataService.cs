using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;

namespace TradeHub.Api.Services;

public class SeedDataService
{
    private readonly TradeHubContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(TradeHubContext context, UserManager<User> userManager, ILogger<SeedDataService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            // Check if data already exists
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Database already contains users. Skipping seeding.");
                return;
            }

            await SeedUsersAsync();
            await SeedItemsAsync();
            await SeedTradesAsync();
            await SeedOffersAsync();

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        _logger.LogInformation("Seeding users...");

        var users = new[]
        {
            new User
            {
                UserName = "alice_trader",
                Email = "alice@example.com",
                Description = "Vintage collector and retro gaming enthusiast. Always looking for rare finds!"
            },
            new User
            {
                UserName = "bob_collector",
                Email = "bob@example.com",
                Description = "Electronics hobbyist and gadget lover. I fix and trade tech items."
            },
            new User
            {
                UserName = "charlie_games",
                Email = "charlie@example.com",
                Description = "Board game enthusiast and card collector. Love trading strategy games!"
            },
            new User
            {
                UserName = "diana_books",
                Email = "diana@example.com",
                Description = "Bookworm and literature lover. Trading books is my passion."
            },
            new User
            {
                UserName = "eve_music",
                Email = "eve@example.com",
                Description = "Vinyl collector and music lover. Always seeking rare albums and vintage instruments."
            },
            new User
            {
                UserName = "frank_sports",
                Email = "frank@example.com",
                Description = "Sports equipment trader and outdoor enthusiast. From bikes to hiking gear!"
            },
            new User
            {
                UserName = "grace_art",
                Email = "grace@example.com",
                Description = "Artist and craft supplies trader. Love exchanging art materials and handmade items."
            },
            new User
            {
                UserName = "henry_tools",
                Email = "henry@example.com",
                Description = "DIY enthusiast and tool collector. Trading quality tools and hardware."
            }
        };

        foreach (var user in users)
        {
            var result = await _userManager.CreateAsync(user, "Password123!");
            if (!result.Succeeded)
            {
                _logger.LogWarning($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {users.Length} users.");
    }

    private async Task SeedItemsAsync()
    {
        _logger.LogInformation("Seeding items...");

        var users = await _context.Users.ToListAsync();
        var random = new Random();

        var itemTemplates = new[]
        {
            // Electronics
            new { Name = "iPhone 13", Description = "Barely used iPhone 13 in excellent condition", Tags = "electronics,phone,apple", BaseValue = 500m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            new { Name = "MacBook Air M1", Description = "2020 MacBook Air with M1 chip, great for students", Tags = "electronics,laptop,apple,computer", BaseValue = 800m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            new { Name = "Gaming Mouse", Description = "High-DPI gaming mouse with RGB lighting", Tags = "electronics,gaming,computer,mouse", BaseValue = 60m, Conditions = new[] { Condition.New, Condition.UsedLikeNew } },
            new { Name = "Mechanical Keyboard", Description = "Cherry MX Blue switches, satisfying click", Tags = "electronics,keyboard,computer,gaming", BaseValue = 120m, Conditions = new[] { Condition.New, Condition.UsedLikeNew } },
            new { Name = "Bluetooth Headphones", Description = "Noise-cancelling wireless headphones", Tags = "electronics,audio,headphones,bluetooth", BaseValue = 180m, Conditions = new[] { Condition.New, Condition.UsedGood } },
            
            // Gaming
            new { Name = "Nintendo Switch", Description = "Portable gaming console with dock", Tags = "gaming,console,nintendo,portable", BaseValue = 250m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            new { Name = "PlayStation 5 Controller", Description = "DualSense wireless controller", Tags = "gaming,controller,playstation,sony", BaseValue = 70m, Conditions = new[] { Condition.New, Condition.UsedLikeNew } },
            new { Name = "The Legend of Zelda: BOTW", Description = "Complete game with case and manual", Tags = "gaming,nintendo,zelda,adventure", BaseValue = 40m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            new { Name = "Gaming Chair", Description = "Ergonomic gaming chair with lumbar support", Tags = "gaming,furniture,chair,ergonomic", BaseValue = 200m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            
            // Books
            new { Name = "Clean Code", Description = "Robert Martin's guide to writing maintainable code", Tags = "books,programming,software,education", BaseValue = 35m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            new { Name = "Harry Potter Box Set", Description = "Complete 7-book series in hardcover", Tags = "books,fantasy,series,hardcover", BaseValue = 150m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            new { Name = "Design Patterns", Description = "Gang of Four design patterns book", Tags = "books,programming,design,software", BaseValue = 45m, Conditions = new[] { Condition.UsedGood, Condition.UsedAcceptable } },
            
            // Music
            new { Name = "Fender Stratocaster", Description = "Classic electric guitar in sunburst finish", Tags = "music,guitar,electric,fender", BaseValue = 800m, Conditions = new[] { Condition.UsedGood, Condition.UsedAcceptable } },
            new { Name = "Pink Floyd - Dark Side of the Moon", Description = "Original vinyl pressing in great condition", Tags = "music,vinyl,rock,classic", BaseValue = 25m, Conditions = new[] { Condition.UsedGood, Condition.UsedLikeNew } },
            new { Name = "Audio Interface", Description = "2-channel USB audio interface for recording", Tags = "music,audio,recording,usb", BaseValue = 120m, Conditions = new[] { Condition.New, Condition.UsedLikeNew } },
            
            // Sports
            new { Name = "Mountain Bike", Description = "Full suspension mountain bike, 21-speed", Tags = "sports,bike,mountain,outdoor", BaseValue = 400m, Conditions = new[] { Condition.UsedGood, Condition.UsedAcceptable } },
            new { Name = "Tennis Racket", Description = "Professional grade tennis racket with grip", Tags = "sports,tennis,racket,professional", BaseValue = 80m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            new { Name = "Basketball", Description = "Official size basketball in good condition", Tags = "sports,basketball,ball,official", BaseValue = 20m, Conditions = new[] { Condition.UsedGood, Condition.New } },
            
            // Tools
            new { Name = "Cordless Drill", Description = "18V cordless drill with battery and charger", Tags = "tools,drill,cordless,diy", BaseValue = 90m, Conditions = new[] { Condition.UsedGood, Condition.UsedLikeNew } },
            new { Name = "Socket Set", Description = "Complete metric and SAE socket set", Tags = "tools,socket,mechanic,automotive", BaseValue = 65m, Conditions = new[] { Condition.UsedGood, Condition.New } },
            
            // Art Supplies
            new { Name = "Professional Paint Set", Description = "Acrylic paint set with 24 colors", Tags = "art,paint,acrylic,supplies", BaseValue = 45m, Conditions = new[] { Condition.New, Condition.UsedLikeNew } },
            new { Name = "Digital Drawing Tablet", Description = "Pressure sensitive drawing tablet for digital art", Tags = "art,digital,tablet,drawing", BaseValue = 150m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            
            // Board Games
            new { Name = "Settlers of Catan", Description = "Classic strategy board game, complete set", Tags = "games,board,strategy,family", BaseValue = 35m, Conditions = new[] { Condition.UsedLikeNew, Condition.UsedGood } },
            new { Name = "Ticket to Ride", Description = "Railway adventure board game", Tags = "games,board,strategy,trains", BaseValue = 40m, Conditions = new[] { Condition.New, Condition.UsedLikeNew } },
            new { Name = "Dungeons & Dragons Starter Set", Description = "Complete D&D starter set with dice and manual", Tags = "games,rpg,dnd,fantasy", BaseValue = 25m, Conditions = new[] { Condition.New, Condition.UsedLikeNew } }
        };

        var items = new List<Item>();

        // Create 3-5 items per user
        foreach (var user in users)
        {
            var itemCount = random.Next(3, 6);
            var availableTemplates = itemTemplates.OrderBy(x => random.Next()).Take(itemCount).ToList();

            foreach (var template in availableTemplates)
            {
                var condition = template.Conditions[random.Next(template.Conditions.Length)];
                var conditionMultiplier = condition switch
                {
                    Condition.New => 1.0m,
                    Condition.Refurbished => 0.85m,
                    Condition.UsedLikeNew => 0.8m,
                    Condition.UsedGood => 0.65m,
                    Condition.UsedAcceptable => 0.45m,
                    Condition.UsedBad => 0.25m,
                    _ => 0.65m
                };

                var finalValue = Math.Round(template.BaseValue * conditionMultiplier, 2);
                var availability = random.NextDouble() > 0.1 ? Availability.Available : Availability.Unavailable;

                items.Add(new Item(
                    name: template.Name,
                    description: template.Description,
                    image: "", // Empty for now as specified in the model
                    value: finalValue,
                    ownerId: user.Id,
                    tags: template.Tags,
                    condition: condition,
                    availability: availability
                )
                {
                    IsValueEstimated = random.NextDouble() > 0.7 // 30% are estimated values
                });
            }
        }

        await _context.Items.AddRangeAsync(items);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {items.Count} items.");
    }

    private async Task SeedTradesAsync()
    {
        _logger.LogInformation("Seeding trades...");

        var users = await _context.Users.ToListAsync();
        var random = new Random();
        var trades = new List<Trade>();

        // Create 8-12 trades
        for (int i = 0; i < 10; i++)
        {
            var initiatedUser = users[random.Next(users.Count)];
            var receivedUser = users[random.Next(users.Count)];
            
            // Ensure different users
            while (receivedUser.Id == initiatedUser.Id)
            {
                receivedUser = users[random.Next(users.Count)];
            }

            var trade = new Trade
            {
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)), // Random date in last 30 days
                InitiatedId = initiatedUser.Id,
                ReceivedId = receivedUser.Id,
                Status = (byte)random.Next(0, 4) // Random status 0-3
            };

            trades.Add(trade);
        }

        await _context.Trades.AddRangeAsync(trades);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {trades.Count} trades.");
    }

    private async Task SeedOffersAsync()
    {
        _logger.LogInformation("Seeding offers...");

        var users = await _context.Users.ToListAsync();
        var trades = await _context.Trades.ToListAsync();
        var random = new Random();
        var offers = new List<Offer>();

        // Create 1-3 offers per trade
        foreach (var trade in trades)
        {
            var offerCount = random.Next(1, 4);
            var availableUsers = users.Where(u => u.Id != trade.InitiatedId && u.Id != trade.ReceivedId).ToList();

            for (int i = 0; i < Math.Min(offerCount, availableUsers.Count); i++)
            {
                var user = availableUsers[random.Next(availableUsers.Count)];
                availableUsers.Remove(user); // Prevent duplicate offers from same user

                var offer = new Offer
                {
                    UserId = user.Id,
                    TradeId = trade.Id,
                    Created = DateTimeOffset.UtcNow.AddDays(-random.Next(0, 15)) // Random date within last 15 days
                };

                offers.Add(offer);
            }
        }

        await _context.Offers.AddRangeAsync(offers);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {offers.Count} offers.");
    }
}