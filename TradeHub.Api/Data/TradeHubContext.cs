using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TradeHub.Api.Models;

public partial class TradeHubContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public TradeHubContext() { }

    public TradeHubContext(DbContextOptions<TradeHubContext> options)
        : base(options) { }

    public DbSet<Item> Items { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<Offer> Offers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Trade>()
            .HasOne(t => t.InitiatedUser)
            .WithMany(u => u.InitiatedTrades)
            .HasForeignKey(t => t.InitiatedId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Trade>()
            .HasOne(t => t.ReceivedUser)
            .WithMany(u => u.ReceivedTrades)
            .HasForeignKey(t => t.ReceivedId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Item>()
            .Property(i => i.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Item>()
            .Property(i => i.Value)
            .HasColumnType("decimal(18,2)");
    }

    internal async Task SaveChangesAsync(Trade trade)
    {
        throw new NotImplementedException();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
