namespace TradeHub.Api.Services.Interfaces;

public interface ILLMService
{
    Task<decimal> EstimateItemValueAsync(
        string itemName,
        string itemDescription,
        string itemCondition,
        string currency = "USD"
    );
}
