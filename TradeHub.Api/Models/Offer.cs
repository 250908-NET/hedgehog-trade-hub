using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradeHub.API.Models;

public class Offer
{
    public long Id { get; set; }

    public long UserId { get; set; } // fk to User
    public User User { get; set; } = null!; // navigation property

    public long TradeId { get; set; } // fk to Trade
    public Trade Trade { get; set; } = null!; // navigation property

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public byte[] RowVersion { get; set; } = []; // concurrency

    // when a user adding multiple items

    public ICollection<OfferItem> OfferItems { get; set; } = [];
}

// Fluent API configuration
public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.Property(o => o.UserId).IsRequired();
        builder.HasOne(o => o.User).WithMany(u => u.Offers).HasForeignKey(o => o.UserId);

        builder.Property(o => o.TradeId).IsRequired();
        builder
            .HasOne(o => o.Trade)
            .WithMany(t => t.Offers)
            .HasForeignKey(o => o.TradeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()");

        builder.Property(o => o.RowVersion).IsRowVersion();
    }
}
