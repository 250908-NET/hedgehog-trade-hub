using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradeHub.API.Models;

namespace TradeHub.API.Models;

public partial class TradeHubContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public TradeHubContext() { }

    public TradeHubContext(DbContextOptions<TradeHubContext> options)
        : base(options) { }

    public DbSet<Item> Items { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<OfferItem> OfferItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>().Property(i => i.RowVersion).IsRowVersion();
        modelBuilder.Entity<Item>().Property(i => i.Value).HasColumnType("decimal(18,2)");

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
        // Trade → Offers
        modelBuilder
            .Entity<Offer>()
            .HasOne(o => o.Trade)
            .WithMany(t => t.Offers)
            .HasForeignKey(o => o.TradeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Offer → User
        modelBuilder.Entity<Offer>().HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId);

        // Optional: configure RowVersion
        modelBuilder.Entity<Offer>().Property(o => o.RowVersion).IsRowVersion();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
