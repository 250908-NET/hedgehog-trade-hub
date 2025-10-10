using Microsoft.EntityFrameworkCore;

namespace TradeHub.Api.Models;

public partial class TradeHubContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // apply any IEntityTypeConfiguration<> implementations found in this assembly
        // fully automatic! the wonders of modern technology
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TradeHubContext).Assembly);

        // configure value converter for Condition and Availibility enums
        modelBuilder.Entity<Item>().Property(i => i.Condition).HasConversion<string>();
        modelBuilder.Entity<Item>().Property(i => i.Availability).HasConversion<string>();

        // configure value converter for TradeStatus enum
        modelBuilder.Entity<Trade>().Property(t => t.Status).HasConversion<string>();
    }
}
