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


        modelBuilder.Entity<Trade>()
              .Property(t => t.Status)
                .HasConversion<byte>(); // store enum as byte
            

    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
