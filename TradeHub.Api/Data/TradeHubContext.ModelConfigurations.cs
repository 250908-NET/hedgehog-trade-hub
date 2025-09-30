using Microsoft.EntityFrameworkCore;

namespace TradeHub.Api.Models;

public partial class TradeHubContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // apply any IEntityTypeConfiguration<> implementations found in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TradeHubContext).Assembly);
    }
}
