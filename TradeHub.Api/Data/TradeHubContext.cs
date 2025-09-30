using Microsoft.EntityFrameworkCore;

namespace TradeHub.Api.Models;

public partial class TradeHubContext : DbContext
{
    public TradeHubContext() { }

    public TradeHubContext(DbContextOptions<TradeHubContext> options)
        : base(options) { }

    public DbSet<Item> Items { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    internal async Task SaveChangesAsync(Trade trade)
    {
        throw new NotImplementedException();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
