using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradeHub.API.Models;

public class Trade
{
    [Key]
    public long Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public long InitiatedId { get; set; }

    [ForeignKey("InitiatedId")]
    public User? InitiatedUser { get; set; }

    [Required]
    public long ReceivedId { get; set; }

    [ForeignKey("ReceivedId")]
    public User? ReceivedUser { get; set; }

    public byte Status { get; set; }

    // Trade review details
    [Required]
    public string ItemCondition { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public int OwnerReputation { get; set; }

    // Navigation properties

    public ICollection<Item>? TradeItems { get; set; } = [];
    public ICollection<Offer>? Offers { get; set; } = [];
}

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        // modelBuilder
        //     .Entity<Trade>()
        //     .HasOne(t => t.InitiatedUser)
        //     .WithMany(u => u.InitiatedTrades)
        //     .HasForeignKey(t => t.InitiatedId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // modelBuilder
        //     .Entity<Trade>()
        //     .HasOne(t => t.ReceivedUser)
        //     .WithMany(u => u.ReceivedTrades)
        //     .HasForeignKey(t => t.ReceivedId)
        //     .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()");

        // builder.Property(i => i.InitiatedId).IsRequired();
        builder
            .HasOne(t => t.InitiatedUser)
            .WithMany()
            .HasForeignKey(i => i.InitiatedId)
            .OnDelete(DeleteBehavior.Restrict);

        // builder.Property(i => i.ReceivedId).IsRequired();
        builder
            .HasOne(t => t.ReceivedUser)
            .WithMany()
            .HasForeignKey(i => i.ReceivedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
