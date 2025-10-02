using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradeHub.Api.Models;

public class Item(
    string name,
    string description,
    string image,
    decimal value,
    long ownerId,
    string tags,
    string condition,
    string availability
)
{
    public long Id { get; set; }
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string Image { get; set; } = image;
    public decimal Value { get; set; } = value;
    public long OwnerId { get; set; } = ownerId;
    public User Owner { get; set; } = null!; // navigation
    public string Tags { get; set; } = tags;
    public string Condition { get; set; } = condition;
    public string Availability { get; set; } = availability;
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

        builder.Property(i => i.Condition).IsRequired();

        builder.Property(i => i.Availability).IsRequired();

        builder.Property(o => o.RowVersion).IsRowVersion();
    }
}
