using Microsoft.EntityFrameworkCore;

namespace TradeHub.API.Models;

public partial class TradeHubContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // apply any IEntityTypeConfiguration<> implementations found in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TradeHubContext).Assembly);

        // configure value converter for Condition and Availibility enums
        modelBuilder.Entity<Item>().Property(i => i.Condition).HasConversion<string>();
        modelBuilder.Entity<Item>().Property(i => i.Availability).HasConversion<string>();
    }
}
