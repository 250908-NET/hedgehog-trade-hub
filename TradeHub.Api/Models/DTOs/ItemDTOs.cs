namespace TradeHub.API.Models.DTOs;

public class ItemDTO
{
    public long Id { get; set; }
    public string Description { get; set; } = "";
    public string Image { get; set; } = "";
    public decimal Value { get; set; }
    public long Owner { get; set; }
    public string Condition { get; set; } = null!;
    public string Availability { get; set; } = null!;

    public List<string> Tags { get; set; } = [];
}
