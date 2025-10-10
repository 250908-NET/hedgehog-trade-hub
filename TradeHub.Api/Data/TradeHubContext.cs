using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;

namespace TradeHub.Api.Models;

public partial class TradeHubContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public TradeHubContext() { }

    public TradeHubContext(DbContextOptions<TradeHubContext> options)
        : base(options) { }

    public DbSet<Item> Items { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<OfferItem> OfferItems { get; set; }

    // already handled by IdentityDbContext
    // public DbSet<User> Users { get; set; }

    /// <summary>
    /// Provides the configuration for TradeHubContext models.
    /// Any custom configurations should be defined in <see cref="OnModelCreatingPartial"/>.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
