using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradeHub.Api.Models;

public class Item
{
    [Key]
    public long Id { get; set; }

    public string Description { get; set; }

    public string Image { get; set; }

    public decimal Value { get; set; }

    [Required]
    public long Owner { get; set; }

    public string Tags { get; set; }

    [Required]

    public string Condition { get; set; }  // new property

    [Required]
    public string Availability { get; set; }

    protected Item(string description, string image, decimal value, long owner, List<string> tags1, string tags, string availability)
    {
        Description = description;
        Image = image;
        Value = value;
        Owner = owner;
        Tags = tags;
        Availability = availability;
    }
}

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Description).IsRequired();
        builder.Property(i => i.Image).IsRequired();
        builder.Property(i => i.Value).IsRequired();
        builder.Property(i => i.Owner).IsRequired();
        builder.Property(i => i.Tags).IsRequired();
        builder.Property(i => i.Condition).IsRequired();
        builder.Property(i => i.Availability).IsRequired();
    }
}
