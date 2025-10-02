using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradeHub.Api.Models;

public class Item
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = "";
    public string Image { get; set; } = ""; // TODO: how 2 store image?
    public decimal Value { get; set; } // decimal(18,2)
    public long OwnerId { get; set; } // fk to user
    public User Owner { get; set; } = null!; // navigation
    public string Tags { get; set; } = ""; // TODO: how to represent tags (json list, csv, etc.)
    public byte[] RowVersion { get; set; } = []; // concurrency
}

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name).HasMaxLength(127).IsRequired();

        // builder.Property(i => i.Description)

        // builder.Property(i => i.Image).IsRequired();

        builder.Property(i => i.Value).HasPrecision(18, 2).IsRequired();

        builder.Property(i => i.OwnerId).IsRequired();
        builder.HasOne(i => i.Owner).WithMany().HasForeignKey(i => i.OwnerId);

        // builder.Property(i => i.Tags)

        builder.Property(o => o.RowVersion).IsRowVersion();
    }
}
