using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradeHub.Api.Models;

// pivot table between offers and items
public class OfferItem
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long OfferId { get; set; }
    public Offer Offer { get; set; } = null!; // Navigation property

    [Required]
    public long ItemId { get; set; }
    public Item Item { get; set; } = null!; // Navigation property

    public int Quantity { get; set; } = 1;

    public string? Notes { get; set; }
}

public class OfferItemConfiguration : IEntityTypeConfiguration<OfferItem>
{
    public void Configure(EntityTypeBuilder<OfferItem> builder)
    {
        builder
            .HasOne(oi => oi.Offer)
            .WithMany(o => o.OfferItems) // Offer has many OfferItems
            .HasForeignKey(oi => oi.OfferId)
            .OnDelete(DeleteBehavior.Cascade); // if an offer is deleted, delete all its OfferItems

        builder
            .HasOne(oi => oi.Item)
            .WithMany(i => i.OfferItems) // Item has many OfferItems
            .HasForeignKey(oi => oi.ItemId)
            .OnDelete(DeleteBehavior.Restrict); // prevent deletion of an Item if it's part of an Offer
    }
}
