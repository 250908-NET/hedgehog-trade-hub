using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradeHub.API.Models;

public enum Condition
{
    New,
    Refurbished,
    UsedLikeNew,
    UsedGood,
    UsedAcceptable,
    UsedBad,
}

public enum Availability
{
    Available,
    Unavailable,
}

public class Item(
    string name,
    string description,
    string image,
    decimal value,
    long ownerId,
    string tags,
    Condition condition,
    Availability availability
)
{
    public long Id { get; set; }
    public string Name { get; set; } = name;
    public string Description { get; set; } = description; // can be empty string
    public string Image { get; set; } = image; // can be empty string (for now)
    public decimal Value { get; set; } = value;
    public bool IsValueEstimated { get; set; } = false;
    public long OwnerId { get; set; } = ownerId;
    public User Owner { get; set; } = null!; // navigation
    public string Tags { get; set; } = tags; // can be empty string (for now)
    public Condition Condition { get; set; } = condition;
    public Availability Availability { get; set; } = availability;
    public byte[] RowVersion { get; set; } = []; // concurrency
}

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name).HasMaxLength(127).IsRequired();

        builder.Property(i => i.Description).IsRequired();

        builder.Property(i => i.Image).IsRequired();

        builder.Property(i => i.Value).HasPrecision(18, 2).IsRequired();

        builder.Property(i => i.OwnerId).IsRequired();
        builder.HasOne(i => i.Owner).WithMany().HasForeignKey(i => i.OwnerId);

        builder.Property(i => i.Tags).IsRequired();

        builder.Property(i => i.Condition).HasConversion<Condition>().IsRequired();

        builder.Property(i => i.Availability).HasConversion<Availability>().IsRequired();

        builder.Property(i => i.RowVersion).IsRowVersion();
    }
}
