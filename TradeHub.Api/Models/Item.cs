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

    protected Item(string description, string image, decimal value, long owner, string tags)
    {
        Description = description;
        Image = image;
        Value = value;
        Owner = owner;
        Tags = tags;
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
    }
}
