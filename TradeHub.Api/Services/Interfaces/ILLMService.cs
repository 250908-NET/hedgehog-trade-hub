namespace TradeHub.API.Services.Interfaces;

public interface ILLMService
{
    Task<decimal> EstimateItemValueAsync(
        string itemName,
        string itemDescription,
        string itemCondition,
        string currency = "USD"
    );
}
