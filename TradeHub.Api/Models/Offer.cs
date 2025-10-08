using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;


namespace TradeHub.API.Models;

public class Offer
{
    public int Id { get; set; }

    public long UserId { get; set; } // fk to User
    public User User { get; set; } = null!; // navigation property

    public long TradeId { get; set; } // fk to Trade
    public Trade Trade { get; set; } = null!; // navigation property

    public string? Notes { get; set; }

    public DateTimeOffset Created { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>(); // concurrency

    // when a user adding multiple items
    
    public ICollection<OfferItem> OfferItems { get; set; } = new List<OfferItem>();
}

// Fluent API configuration
public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId).IsRequired();
        builder.HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId);

        builder.Property(o => o.TradeId).IsRequired();
        builder.HasOne(o => o.Trade).WithMany().HasForeignKey(o => o.TradeId);

        builder.Property(o => o.Created).HasDefaultValueSql("SYSDATETIMEOFFSET()");

        builder.Property(o => o.RowVersion).IsRowVersion();
    }
}
